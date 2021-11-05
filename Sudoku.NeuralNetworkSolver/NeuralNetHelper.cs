using System;
using System.Linq;
using System.Xml.Xsl;
using Keras;
using Keras.Layers;
using Keras.Models;
using Numpy;
using Python.Included;
using Python.Runtime;
using Sudoku.Shared;

namespace Sudoku.NeuralNetworkSolver
{
    public class NeuralNetHelper
    {
        private const string ModuleNameTensorFlow = "tensorflow";
        private const string ModuleNameKeras = "keras";
        private const string ModuleNameTensorFlowKeras = "tensorflow.keras";

        static NeuralNetHelper()
        {

            Installer.SetupPython().Wait();


            //PythonEngine.PythonHome = @"C:\Users\vavav\AppData\Roaming\Microsoft\Windows\Start Menu\Programs\Anaconda3 (64-bit)"; //PYTHON INCLUDED qui embarque la distribution python
            PythonEngine.PythonHome = Python.Included.Installer.EmbeddedPythonHome;
            if (!Installer.IsPipInstalled())
            {
                Installer.TryInstallPip();
            }
            if (!Installer.IsModuleInstalled(ModuleNameTensorFlow))
            {
                Installer.PipInstallModule(ModuleNameTensorFlow);
            }
            if (!Installer.IsModuleInstalled(ModuleNameKeras))
            {
                Installer.PipInstallModule(ModuleNameKeras);
            }
            //if (!Installer.IsModuleInstalled(ModuleNameTensorFlowKeras))
            //{
            //    Installer.PipInstallModule(ModuleNameTensorFlowKeras);
            //}

           
            //var k = Keras.Keras.Instance;
            Setup.UseTfKeras();
        }

        public static BaseModel LoadModel(string strpath)
        {
            return BaseModel.LoadModel(strpath);
        }

        public static NDarray GetFeatures(SudokuGrid objSudoku)
        {
            return Normalize(np.array(objSudoku.Cells.Flatten()).reshape(9, 9)); // on est déjà en 9,9 en entrée on a un réseau de 9/9
        }

        public static SudokuGrid GetSudoku(NDarray features)
        {
            return new SudokuGrid() { Cells = features.flatten().astype(np.int32).GetData<int>().ToJaggedArray(9) };
        }

        public static NDarray Normalize(NDarray features)
        {
            return (features / 9) - 0.5;
        }

        public static NDarray DeNormalize(NDarray features)
        {
            return (features + 0.5) * 9;
        }


        public static BaseModel GenerateModel()
        {
            var model = new Sequential();

            model.Add(new Conv2D(64, kernel_size: (3, 3).ToTuple(), activation: "relu", padding: "same", input_shape: (9, 9, 1)));
            model.Add(new BatchNormalization());
            model.Add(new Conv2D(64, kernel_size: (3, 3).ToTuple(), activation: "relu", padding: "same"));
            model.Add(new BatchNormalization());
            model.Add(new Conv2D(128, kernel_size: (1, 1).ToTuple(), activation: "relu", padding: "same"));

            model.Add(new Flatten());
            model.Add(new Dense(81 * 9));
            model.Add(new Reshape((-1, 9)));
            model.Add(new Activation("softmax"));

            return model;
        }


       

        public static (BaseModel model, double accuracy) TrainAndTest(BaseModel model, string datasetPath)
        {
            // Global parameters
           
            int numSudokus = 1000;

            // ML parameters
            double testPercent = 0.01;
            float learningRate = .001F;
            int batchSize = 32;
            int epochs = 10;

            
            // Initialize dataset
            var sampleSudokus = DataSetHelper.ParseCSV(datasetPath, numSudokus);

            Console.WriteLine($"Parsed {sampleSudokus.Count} sudokus puzzle with solutions from {datasetPath}");

            var (_sTrain, _sTest) = DataSetHelper.SplitDataSet(sampleSudokus, testPercent);

            Console.WriteLine($"Split dataset into {_sTrain.Count} train sudokus and {_sTest.Count} test sudokus");

            // Preprocess data
            var sPuzzzlesTrain = DataSetHelper.PreprocessSudokus(_sTrain.Select(s=>s.Quiz));
            var sSolsTrain = DataSetHelper.PreprocessSudokus(_sTrain.Select(s => s.Solution));

            Console.WriteLine($"Normalised train quizzes and solutions for Keras");

            // Add optimizer
            var adam = new Keras.Optimizers.Adam(learningRate);
            var strLoss = "sparse_categorical_crossentropy";
            model.Compile(loss: strLoss, optimizer: adam);

            Console.WriteLine($"Compiled model with {adam.GetType().Name} optimizer and {strLoss} loss");

            Console.WriteLine($"Start training");
            // Train model
            model.Fit(sPuzzzlesTrain, sSolsTrain, batch_size: batchSize, epochs: epochs);
            
            Console.WriteLine($"End training");


            Console.WriteLine($"Quick test on training set");

            var sample = _sTrain[0];
            Console.WriteLine("Quiz : \n" + sample.Quiz);
            Console.WriteLine("Prediction : \n" + SolveSudoku(sample.Quiz, model));
            Console.WriteLine("Solution : \n" + sample.Solution);


            // Test model
            int correct = 0;

            for (int i = 0; i < _sTest.Count; i++)
            {
                Console.WriteLine("Testing " + i);

                // Predict result
                var prediction = SolveSudoku(_sTest[i].Quiz, model);
                var solution = _sTest[i].Solution;


                // Compare sudoku
                var same = true;
                for (int j = 0; j < 9; j++)
                {
                    for (int k = 0; k < 9; k++)
                    {
                        if (prediction.Cells[j][k] != solution.Cells[j][k])
                        {
                            same = false;
                        }
                    }
                }
                Console.WriteLine("Quiz : \n" + _sTest[i].Quiz);
                Console.WriteLine("Prediction : \n" + prediction);
                Console.WriteLine("Solution : \n" + solution);

                if (same)
                {
                    correct += 1;
                }
            }

            // Calculate accuracy
            var accuracy = (correct / (double) _sTest.Count) * 100;

            // Return
            return (model, accuracy);
        }


        public static SudokuGrid SolveSudoku(SudokuGrid s, BaseModel model)
        {
            var features = GetFeatures(s);
            while (true)
            {
                var output = model.Predict(features.reshape(1, 9, 9, 1));//il fait la prédiction
                output = output.squeeze();
                var prediction = np.argmax(output, axis: 1).reshape(9, 9) + 1; // récupère la meilleure prédiction pour chaque cellule
                var proba = np.around(np.max(output, axis: new[] { 1 }).reshape(9, 9), 2);  // pour chaque cellule ( 9/9) on a un vecteur de proba softmax qui va donner les proba de chaque chiffres

                features = DeNormalize(features);
                var mask = features.@equals(0);// regarde s'il reste des 0
                // Si plus de 0 dans le sudokus on sort
                if (((int)mask.sum()) == 0)
                {
                    break;
                }

                var probNew = proba * mask;
                var ind = (int)np.argmax(probNew); //On ne garde que la cellule de proba maximale
                var (x, y) = ((ind / 9), ind % 9);
                var val = prediction[x][y];
                features[x][y] = val; //On réinjecte la prédiction
                features = Normalize(features);

            }

            return GetSudoku(features); // reprend les caractéristiques du réseau
        }


        


    }
}