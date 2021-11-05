using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.IO.Compression;
using System.Linq;
using CsvHelper;
using CsvHelper.Configuration;
using Numpy;
using Sudoku.Shared;

namespace Sudoku.NeuralNetworkSolver
{
    public class DataSetHelper // pour l'entraînement pour pouvoir lire le fichier 
    {

        public static List<SudokuSample> ParseCSV(string path, int numSudokus)
        {
            var records = new List<SudokuSample>();
            using (var compressedStream = File.OpenRead(path))
            using (var decompressedStream = new GZipStream(compressedStream, CompressionMode.Decompress))
            using (var reader = new StreamReader(decompressedStream))
            using (var csv = new CsvReader(reader, new CsvConfiguration(CultureInfo.InvariantCulture){Delimiter = ","}))
            {
                csv.Read();
                csv.ReadHeader();
                var currentNb = 0;
                while (csv.Read() && currentNb < numSudokus)
                {
                    var record = new SudokuSample
                    {
                        Quiz = SudokuGrid.Parse(csv.GetField<string>("quizzes")),
                        Solution = SudokuGrid.Parse(csv.GetField<string>("solutions"))
                    };
                    records.Add(record);
                    currentNb++;
                }
            }
            return records;
        }

        public static NDarray PreprocessSudokus(IEnumerable<SudokuGrid> sudokus)
        {
            List<NDarray> rawRes = new List<NDarray>();
            foreach (SudokuGrid sudoku in sudokus)
            {
                rawRes.Add(np.array(sudoku.Cells.Flatten().ToArray()).reshape(9, 9, 1));
            }

            NDarray res = np.array(rawRes.ToArray());
            res = NeuralNetHelper.Normalize(res);

            return res;
        }

        public static (List<SudokuSample> train, List<SudokuSample> test) SplitDataSet(ICollection<SudokuSample> sudokus, double testPercent)
        {
            int listSize = sudokus.Count;
            int trainSize = (int)((1 - testPercent) * listSize);

            List<SudokuSample> train = sudokus.Take(trainSize).ToList();
            List<SudokuSample> test = sudokus.Skip(trainSize).Take(listSize - trainSize).ToList();

            return (train, test);
        }
    }
}