using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using WordProcessingFunctions;

namespace Lab5
{
    class Program
    {
        static void Main()
        {
            var corpus = new List<List<string>>();
            foreach (var filePath in Directory.GetFiles("news"))
            {
                var document = File
                    .ReadAllLines(filePath)
                    .SelectMany(paragraph => paragraph
                        .Split(new[] { ". " }, StringSplitOptions.RemoveEmptyEntries)
                        .Select(sent => sent.Trim()))
                    .ToList();
                corpus.Add(document);
            }

            WordProcessing.RunTwice(() =>
                    corpus = corpus
                        .Select(document => document
                            .Select(sentence => sentence
                                .PreProcess())
                            .ToList())
                        .ToList(),
                () => corpus = corpus
                    .Select(doc => doc
                        .Select(sent => sent.StemIt())
                        .ToList())
                    .ToList(),
                index =>
                {
                    var uniGram = corpus.SelectMany(doc => doc.SelectMany(sent => sent.GetNGrams(1))).ToList();
                    var biGram = corpus.SelectMany(doc => doc.SelectMany(sent => sent.GetNGrams(2))).ToList();

                    var uniGramFrequency = uniGram.GetAnalysisFrequency().ToList();
                    var biGramFrequency = biGram.GetAnalysisFrequency().ToList();

                    var gramFrequency = uniGramFrequency.Concat(biGramFrequency).ToList();
                    gramFrequency.BuildCloud($"uniCloud {index + 1}", 100);

                    var gramTfIdf = gramFrequency.ConvertToTfIdf(corpus.Select(doc => string.Join(" ", doc))).ToList();
                    gramTfIdf.BuildGraphic($"TfIdf {index + 1}", 100);

                    Console.WriteLine();
                    Console.WriteLine(uniGram.Take(5).AsString());
                    Console.WriteLine(biGram.Take(5).AsString());
                    Console.WriteLine();
                    Console.WriteLine(gramFrequency.GetTopRelevant(10).AsString());
                    Console.WriteLine();
                    Console.WriteLine(gramTfIdf.GetTopRelevant(10).AsString());
                    Console.WriteLine(corpus.Select(doc => string.Join(" ", doc))
                        .GetSimilarityCrossMatrix()
                        .CrossMatrixAsString());

                });
            Console.ReadKey();
        }
    }
}
