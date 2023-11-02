using System;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Threading;
using MyIA.Trading.Converter;
using Utf8Json;
using Utf8Json.Formatters;
using Utf8Json.Resolvers;

namespace MyIA.Trading.Backtester
{
    class Program
    {

        static System.Diagnostics.Stopwatch RunningTime = Stopwatch.StartNew();

        static void Main(string[] args)
        {
            Thread.CurrentThread.Priority = ThreadPriority.Lowest;
            Thread.CurrentThread.CurrentCulture = CultureInfo.InvariantCulture;

            //var objConfig = GetConfig();
            var objConfig = new BackTestingConfig();
            Log($"BackTestingConfig loaded");

            //objConfig.TrainingConfig.SampleConfig.Load(Log);

            var backTest = new BackTesting() { Config = objConfig };
            //var settings = backTest.LoadModels(Log, out var bestBackTesting);


            backTest.Run(Log);
            //backTest.RunSimple(Log);


            Log($"End - Total elapsed: {RunningTime.Elapsed.ToString()}");
            Console.ReadKey();
        }


        static BackTestingConfig GetConfig()
        {

            var configFileName = Path.Combine(Environment.CurrentDirectory, "BacktestingConfig.json");
            CompositeResolver.RegisterAndSetAsDefault(new IJsonFormatter[] { new TimeSpanFormatter() }, new IJsonFormatterResolver[] { StandardResolver.Default });
            if (!File.Exists(configFileName))
            {
                var newConfig = new BackTestingConfig();
                var strNewConfig = JsonSerializer.PrettyPrint(JsonSerializer.ToJsonString(newConfig));
                File.WriteAllText(configFileName, strNewConfig);
            }

            using var configStream = File.OpenRead(configFileName);
            var config = JsonSerializer.Deserialize<BackTestingConfig>(configStream);
            return config;
        }



        private static TimeSpan _currentElpased = TimeSpan.Zero;
        public static void Log(string message)
        {
            var totalElapsed = RunningTime.Elapsed;
            var stepElapsed = totalElapsed - _currentElpased;
            _currentElpased = totalElapsed;
            Console.WriteLine($"{RunningTime.Elapsed}+{stepElapsed.TotalSeconds:##.#####}s> {message}");

        }
    }






}
