using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Xml.Serialization;
using Accord.MachineLearning;
using Accord.MachineLearning.VectorMachines;
using Accord.MachineLearning.VectorMachines.Learning;
using Accord.Statistics.Analysis;
using Accord.Statistics.Kernels;
using DotNetNuke.Services.Log.EventLog;
using Newtonsoft.Json;

namespace MyIA.Trading.Backtester
{
    public enum KnownKernel
    {
        InverseMultiquadric,
        TStudent2,
        NormalizedPolynomial3,
        Polynomial3
    }

    public class ClassifierTradingModel : ITradingModel
    {

        [XmlIgnore]
        [JsonIgnore]
        public IClassifier<double[], int> Classifier { get; set; }


        //public int Predict(double[] input)
        //{
        //    return Classifier.Decide(input);
        //}

        //public int[] Predict(double[][] input)
        //{
        //    return Classifier.Decide(input);
        //}
        public IList<TradingTrainingSample> Predict(IList<TradingTrainingSample> inputs)
        {
            var inputsMatrix = inputs.GetInputMatrix();
            var output = Classifier.Decide(inputsMatrix);

            var toReturn = new List<TradingTrainingSample>(inputs.Count);
            for (var index = 0; index < inputs.Count; index++)
            {
                var sample = inputs[index];
                var newSample = new TradingTrainingSample() {Inputs = sample.Inputs, Sample = sample.Sample};

                newSample.Output = output[index];
                toReturn.Add(newSample);
            }

            return toReturn;
       
        }
        
    }


    public class SvmTradingModel : ClassifierTradingModel
    {

        public MulticlassSupportVectorMachine<IKernel> Svm
        {
            get => (MulticlassSupportVectorMachine<IKernel>) Classifier;
            set => Classifier = value;
        }
    }

    public class TradingSvmModelConfig: TradingModelConfig
    {

        public override string GetModelName(TradingTrainingDataConfig dataConfig)
        {
            var toREturn = dataConfig.GetSampleTrainName();
            var modelName = $"{toREturn.Substring(0, toREturn.Length - Path.GetExtension(toREturn).Length)}-kernel-{Kernel}-complexity-{Complexity}-Model.bin";
            return modelName;
        }

        public override ITradingModel TrainModel(Action<string> logger, TradingTrainingDataConfig dataConfig, ref double testError)
        {
            var objSvm = TrainModelInternal(logger, dataConfig, ref testError);
            if (objSvm!=null)
            {
                return new SvmTradingModel() { Svm = objSvm };
            }

            return null;
        }

        public double Complexity { get; set; } = -1;

        public KnownKernel Kernel { get; set; } = KnownKernel.InverseMultiquadric;

        //sampleFileName.Replace(Path.GetExtension(sampleFileName), $".train-{tradingConfig.OutputPrediction.TotalHours}hours-thresold{tradingConfig.OutputThresold}.json")


        //public string GetSampleName(BitcoinSampleConfig sampleConfig)
        //{
        //    var toREturn = sampleConfig.GetSampleName();
        //    return toREturn.Replace(Path.GetExtension(toREturn), $".predict-{OutputPrediction.TotalHours}hours-thresold-{OutputThresold}.json");
        //}



        public IKernel GetKernel(KnownKernel objKnownKernel)
        {
            switch (objKnownKernel)
            {
                case KnownKernel.InverseMultiquadric:
                    return new InverseMultiquadric();
                case KnownKernel.NormalizedPolynomial3:
                    return new NormalizedPolynomial(3);
                case KnownKernel.Polynomial3:
                    return new Polynomial(3);
                case KnownKernel.TStudent2:
                    return new TStudent(2);
            }

            throw new ApplicationException($"Kernel {objKnownKernel} not accounted for");
        }

        private static Dictionary<string, MulticlassSupportVectorMachine<IKernel>> _CachedModels = new Dictionary<string, MulticlassSupportVectorMachine<IKernel>>();

        private MulticlassSupportVectorMachine<IKernel> TrainModelInternal(Action<string> logger, TradingTrainingDataConfig dataConfig, ref double testingError)
        {


            var modelName = GetModelName(dataConfig);


            MulticlassSupportVectorMachine<IKernel> toReturn = null;
            var exceptionFileName = modelName + "Fail.txt";
            if (File.Exists(exceptionFileName))
            {
                logger($"Skipping previously failed Model: {exceptionFileName}");
                return null;
            }


            if (File.Exists(modelName))
            {
                logger($"Loading Saved Model: {modelName}");
                if (!_CachedModels.TryGetValue(modelName, out toReturn))
                {
                    toReturn = Accord.IO.Serializer.Load<MulticlassSupportVectorMachine<IKernel>>(modelName);
                    _CachedModels[modelName] = toReturn;
                }
                
            }
            TradingTrainTestData data = null;
            if (toReturn == null)
            {
                Debugger.Break();
                logger($"Training new Model: {modelName}");
                data = dataConfig.GetTrainingSets(logger);
                var xTrain = data.Training.GetInputMatrix();
                var yTrain = data.Training.GetOutputClasses();
                var xTest = data.Test.GetInputMatrix();
                var yTest = data.Test.GetOutputClasses();

                var objKernel = GetKernel(Kernel);
                if (Complexity < 0)
                {
                    Complexity = CalibrateComplexity(data, objKernel);
                    logger($"SVM complexity calibrated: {Complexity}");
                }
                else
                {
                    logger($"SVM complexity defined : {Complexity}");
                }

                var teacher = new MulticlassSupportVectorLearning<IKernel>()
                {
                    Learner = (p) => new SequentialMinimalOptimization<IKernel>()
                    {
                        //UseComplexityHeuristic = true,
                        Complexity = Complexity,
                        UseKernelEstimation = true,
                        Kernel = objKernel

                    }
                };


                var startTime = DateTime.Now;
                bool completed = ExecuteWithTimeLimit(TrainingTimeout, () =>
                {

                    try
                    {
                        logger($"Learn start");
                        toReturn = teacher.Learn(xTrain, yTrain);
                    }
                    catch (Exception e)
                    {
                        string exceptionMessage = e.ToString();
                        WriteExceptionFile(logger, exceptionFileName, exceptionMessage);
                        toReturn = null;
                    }
                });
                if (!completed)
                {
                    var exceptionMessage = $"Training timed out: {DateTime.Now.Subtract(startTime).TotalSeconds}s";
                    WriteExceptionFile(logger, exceptionFileName, exceptionMessage);
                    toReturn = null;
                }

                if (toReturn == null)
                {
                    return toReturn;
                }



                double trainError = GeneralConfusionMatrix.Estimate(toReturn, xTrain, yTrain).Error; // 0.084
                double testError = GeneralConfusionMatrix.Estimate(toReturn, xTest, yTest).Error; // 0.0849

                // Create the calibration algorithm using the training data
                var ml = new MulticlassSupportVectorLearning<IKernel>()
                {
                    Model = toReturn,

                    // Configure the calibration algorithm
                    Learner = (p) => new ProbabilisticOutputCalibration<IKernel>()
                    {
                        Model = p.Model
                    }
                };
                ml.Learn(xTrain, yTrain);

            




                if (!File.Exists(modelName))
                {
                    (new FileInfo(modelName)).Directory.Create();
                    Accord.IO.Serializer.Save<MulticlassSupportVectorMachine<IKernel>>(toReturn, modelName);
                }

                logger("SVM Training finished");
            }
           
            if (dataConfig.EnsureModelTested && testingError < 0)
            {
                if (data == null)
                {
                    data = dataConfig.GetTrainingSets(logger);
                }
                testingError = TestModel(data, new ClassifierTradingModel() { Classifier = toReturn });
            }



            return toReturn;

        }




        public void SvmBenchmark(TradingTrainingDataConfig dataConfig, Action<string> logger)
        {

            var bitcoinTrain = dataConfig.GetTrainingSets(logger);

            logger("Entering SVM Benchmark");
            double testingError = double.MinValue;
            var machine = TrainModelInternal(logger, dataConfig, ref testingError);

            var xTrain = bitcoinTrain.Training.GetInputMatrix();
            var yTrain = bitcoinTrain.Training.GetOutputClasses();
            var xTest = bitcoinTrain.Test.GetInputMatrix();
            var yTest = bitcoinTrain.Test.GetOutputClasses();


            double trainError = GeneralConfusionMatrix.Estimate(machine, xTrain, yTrain).Error; // 0.084
            double testError = GeneralConfusionMatrix.Estimate(machine, xTest, yTest).Error; // 0.0849

            // Create the calibration algorithm using the training data
            var ml = new MulticlassSupportVectorLearning<IKernel>()
            {
                Model = machine,

                // Configure the calibration algorithm
                Learner = (p) => new ProbabilisticOutputCalibration<IKernel>()
                {
                    Model = p.Model
                }
            };
            ml.Learn(xTrain, yTrain);


            logger("SVM Training finished");

            var decisions = machine.Decide(xTrain);
            var predictionProbas = machine.Probabilities(xTrain);
            int j = 1, k = 1, l = 1, m = 1;
            for (int i = 0; i < decisions.Length; i++)
            {
                if (decisions[i] != yTrain[i])
                {
                    Console.WriteLine($"Bad Prediction {i}:  {JsonConvert.SerializeObject(decisions[i])} vs y: {yTrain[i].ToString()} ({JsonConvert.SerializeObject(predictionProbas[i])})");
                    j++;
                }

            }
            logger($"SVM Training Error: {trainError}");

            logger($"SVM test Error: {testError}");

            decisions = machine.Decide(xTest);
            predictionProbas = machine.Probabilities(xTest);
            j = 1;
            for (int i = 0; i < decisions.Length; i++)
            {
                if (decisions[i] != yTest[i])
                {
                    if (yTest[i] != 0)
                    {
                        if (decisions[i] == 0)
                        {
                            Console.WriteLine($"False Negative {i}:  {JsonConvert.SerializeObject(decisions[i])} vs y: {yTest[i].ToString()} ({JsonConvert.SerializeObject(predictionProbas[i])})");
                            j++;
                        }
                        else
                        {
                            Console.WriteLine($"Bad Decision {i}:  {JsonConvert.SerializeObject(decisions[i])} vs y: {yTest[i].ToString()} ({JsonConvert.SerializeObject(predictionProbas[i])})");
                            m++;
                        }

                    }
                    else
                    {
                        Console.WriteLine($"False Positive {i}:  {JsonConvert.SerializeObject(decisions[i])} vs y: {yTest[i].ToString()} ({JsonConvert.SerializeObject(predictionProbas[i])})");
                        k++;
                    }

                }
                else
                {
                    if (decisions[i] != 0)
                    {
                        Console.WriteLine($"Good decision {i}:  {JsonConvert.SerializeObject(decisions[i])} vs y: {yTest[i].ToString()} ({JsonConvert.SerializeObject(predictionProbas[i])})");
                        l++;
                    }
                }

            }
            Console.WriteLine($"Good decisions {l} vs {j + k + m} = {m} Bad + {k} False Positive + {j} False Negative");



        }


        public double CalibrateComplexity(TradingTrainTestData objTrainingData, IKernel objKernel)
        {
            var xTrain = objTrainingData.Training.GetInputMatrix();
            var yTrain = objTrainingData.Training.GetOutputClasses();




            //var maxComplexity = double.MaxValue;

            double currentComplexity = 0.0001;
            double maxComplexity = 1000D;
            double minComplexity = currentComplexity;
            double bestComplexity = currentComplexity;

            double currentResult = double.MaxValue;
            Boolean maxedOut = false;
            double testError = double.MaxValue;
            var coef = 100D;

            for (int idx = 0; idx < 25; idx++)
            {
                var teacher = new MulticlassSupportVectorLearning<IKernel>();
                teacher.Learner = (p) => new SequentialMinimalOptimization<IKernel>()
                {
                    //UseComplexityHeuristic = true,
                    Complexity = currentComplexity,
                    UseKernelEstimation = true,
                    Kernel = objKernel
                };
                MulticlassSupportVectorMachine<IKernel> machine = null;
                try
                {
                    bool completed = ExecuteWithTimeLimit(TrainingTimeout,
                        () =>
                        {
                            try
                            {
                                teacher = new MulticlassSupportVectorLearning<IKernel>();
                                //maxedOut = true;
                            }
                            catch (Exception e)
                            {

                            }
                        });
                    if (!completed)
                    {
                        break;
                    }
                    var ml = new MulticlassSupportVectorLearning<IKernel>()
                    {
                        Model = machine,

                        // Configure the calibration algorithm
                        Learner = (p) => new ProbabilisticOutputCalibration<IKernel>()
                        {
                            Model = p.Model
                        }
                    };
                    ml.Learn(xTrain, yTrain);


                    var trainDecisions = machine.Decide(xTrain);
                    int j = 0;
                    for (int i = 0; i < trainDecisions.Length; i++)
                    {
                        if (trainDecisions[i] != yTrain[i])
                        {
                            j++;
                        }
                    }
                    if (j == 0)
                    {
                        maxedOut = true;
                    }




                    testError = TestModel(objTrainingData, new ClassifierTradingModel() { Classifier = machine });

                }
                catch (Exception e)
                {
                    teacher = new MulticlassSupportVectorLearning<IKernel>();
                    //Console.WriteLine(e);
                    maxedOut = true;

                }

                if (testError <= currentResult)
                {
                    if (currentComplexity >= bestComplexity)
                    {
                        if (testError < currentResult)
                        {
                            minComplexity = Math.Max(minComplexity, bestComplexity);
                            bestComplexity = currentComplexity;
                            currentResult = testError;
                        }
                    }
                    else
                    {
                        maxComplexity = Math.Min(maxComplexity, bestComplexity);
                        bestComplexity = currentComplexity;
                        currentResult = testError;
                    }

                }
                else
                {

                    if (currentComplexity < bestComplexity)
                    {
                        minComplexity = Math.Max(minComplexity, currentComplexity);
                    }
                    else
                    {
                        if (maxedOut)
                        {
                            if (currentComplexity > bestComplexity)
                            {
                                maxComplexity = Math.Min(maxComplexity, currentComplexity);
                            }
                        }
                    }

                }

                //if (currentComplexity >= maxComplexity)
                //{
                //    maxComplexity = Math.Min(maxComplexity, currentComplexity);
                //    maxComplexity = Math.Min(maxComplexity, bestComplexity * coef);
                //    currentComplexity = Math.Max(bestComplexity / coef, minComplexity);
                //    coef = Math.Pow(coef, 0.4);
                //}
                //else
                //{
                //    currentComplexity *= coef;
                //}


                if (!maxedOut)
                {
                    currentComplexity *= coef;
                }
                else
                {
                    if (currentComplexity > bestComplexity)
                    {
                        currentComplexity = (bestComplexity + minComplexity) / 2;
                    }
                    else
                    {
                        currentComplexity = (bestComplexity + maxComplexity) / 2;
                    }
                }


            }

            return bestComplexity;
        }


      





    }
}
