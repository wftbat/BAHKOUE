using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Text.RegularExpressions;
using Utf8Json.Formatters;

namespace MyIA.Trading.Converter
{
    public static class Extensions
    {

        public static List<Trade> Skip(this List<Trade> input, double ratio)
        {
            if (ratio<=0)
            {
                return input;
            }
            var objRandom = new FastRandom((int)DateTime.Now.Ticks);
            var toReturn = new List<Trade>();
            foreach (var objTrade in input)
            {
                if (objRandom.NextDouble() > ratio)
                {
                    toReturn.Add(objTrade);
                }
            }
            return toReturn;
        }

        

        public static List<Trade> GetRange(this List<Trade> input, DateTime start, DateTime end)
        {
            var startUnix = start.ToUnixTime();
            var endUnix = end.ToUnixTime();
            var toReturn = new List<Trade>((int)Math.DivRem(endUnix - startUnix, input.Last().UnixTime - input.First().UnixTime, out _) * input.Count);
            foreach (var objTrade in input)
            {
                if (objTrade.UnixTime > endUnix)
                {
                    break;
                }
                if (objTrade.UnixTime >= startUnix)
                {
                    toReturn.Add(objTrade);
                }
            }
            return toReturn;
        }



       

        public static long ToUnixTime(this DateTime objDate)
        {
            return ((DateTimeOffset)objDate).ToUnixTimeSeconds();

        }

        public static String GetRawExtensionUpper(this string fileName)
        {
            return Path.GetExtension(fileName)?.TrimStart('.').ToUpperInvariant();

        }

        public static List<Tickbar> ToTickbars(this List<Trade> input, TimeSpan period, bool randomPeriodStart)
        {
            var toReturn = new List<Tickbar>();
            if (input.Count == 0)
            {
                return toReturn;
            }

            var firstTradeTime = input.First().Time;

            DateTime startPeriod;

            if (period == TimeSpan.FromDays(1))
            {
                startPeriod = new DateTime(firstTradeTime.Year, firstTradeTime.Month, firstTradeTime.Day);
            }
            else if (period == TimeSpan.FromHours(1))
            {
                startPeriod = new DateTime(firstTradeTime.Year, firstTradeTime.Month, firstTradeTime.Day, firstTradeTime.Hour, 0, 0);
            }
            else if (period == TimeSpan.FromMinutes(1))
            {
                startPeriod = new DateTime(firstTradeTime.Year, firstTradeTime.Month, firstTradeTime.Day, firstTradeTime.Hour, firstTradeTime.Minute, 0);
            }
            else if (period == TimeSpan.FromSeconds(1))
            {
                startPeriod = new DateTime(firstTradeTime.Year, firstTradeTime.Month, firstTradeTime.Day, firstTradeTime.Hour, firstTradeTime.Minute, firstTradeTime.Second);
            }
            else
            {
                // fallback to previous logic or throw exception
                startPeriod = firstTradeTime;
            }

            var startIdx = 0;
            if (randomPeriodStart)
            {
                var objRandom = new Random();
                var randomOffset = TimeSpan.FromTicks((long)(objRandom.NextDouble() * period.Ticks));
                startPeriod = startPeriod + randomOffset;
                startIdx = input.FindIndex(x => x.Time >= startPeriod);
            }
            var currentTarget = startPeriod.Add(period);
            var currentTickTrades = new List<Trade>();
            for (var index = startIdx; index < input.Count; index++)
            {
                var objTrade = input[index];
                if (objTrade.Time > currentTarget)
                {
                    var tickBarDate = currentTarget.Subtract(period);
                    var currentTickBar = (currentTickTrades.ToTickbar(tickBarDate));
                    toReturn.Add(currentTickBar);
                    currentTickTrades.Clear();
                    currentTarget = currentTarget.Add(period);
                }

                currentTickTrades.Add(objTrade);
            }

            return toReturn;
        }

        public static Tickbar ToTickbar(this List<Trade> input, DateTime dateTime)
        {
            if (input.Count == 0)
            {
                return null;
            }

            var toReturn = new Tickbar
            {
                DateTime = dateTime,
                Open = input.First().Price,
                High = decimal.MinValue,
                Low = decimal.MaxValue,
                Close = input.Last().Price
            };
            foreach (var objTrade in input)
            {
                if (toReturn.High < objTrade.Price)
                {
                    toReturn.High = objTrade.Price;
                }
                if (toReturn.Low > objTrade.Price)
                {
                    toReturn.Low = objTrade.Price;
                }
                toReturn.Volume += objTrade.Amount;
            }
            return toReturn;
        }


        private static Regex _InterpolateRegex = new Regex(@"{(.+?)}", RegexOptions.Compiled);

        private static Dictionary<string, Delegate> _CachedIntepolationExpressions = new Dictionary<string, Delegate>();

        public static string Interpolate(this string value, Dictionary<string, Object> context)
        {
            return _InterpolateRegex.Replace(value,
                match =>
                {
                    var matchToken = match.Groups[1].Value;
                    var key = $"{value}/{matchToken}";
                    if (!_CachedIntepolationExpressions.TryGetValue(key, out var tokenDelegate))
                    {
                        var parameters = new List<ParameterExpression>(context.Count);
                        foreach (var contextObject in context)
                        {
                            var p = Expression.Parameter(contextObject.Value.GetType(), contextObject.Key);
                            parameters.Add(p);
                        }
                        var e = System.Linq.Dynamic.Core.DynamicExpressionParser.ParseLambda(parameters.ToArray(), null, matchToken);
                        tokenDelegate = e.Compile();
                        _CachedIntepolationExpressions[key] = tokenDelegate;
                    }
                    return (tokenDelegate.DynamicInvoke(context.Values.ToArray()) ?? "").ToString();
                });
        }

    }


}
