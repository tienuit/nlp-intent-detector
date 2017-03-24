using SharpNL.DocumentCategorizer;
using SharpNL.NameFind;
using SharpNL.Tokenize;
using SharpNL.Utility;
using SharpNL.Utility.FeatureGen;
using System;
using System.Collections.Generic;
using System.IO;

namespace IntentDetector
{
    class Program
    {
        static void Main(string[] args)
        {
            DirectoryInfo trainingDirectory = new DirectoryInfo(@"data");
            string[] slots = new string[] { "city"};
            //if (args.Length > 1)
            //{
            //    slots = args[1].Split(',');
            //}

            
           
            if (!trainingDirectory.Exists)
            {
                throw new System.ArgumentException("TrainingDirectory is not a directory: " + trainingDirectory.FullName);
            }

            List<IObjectStream<DocumentSample>> categoryStreams = new List<IObjectStream<DocumentSample>>();
            foreach (FileInfo trainingFile in trainingDirectory.GetFiles())
            {
                string intent = trainingFile.Name.Replace("[.][^.]+$", "");
                IObjectStream<string> lineStream = new PlainTextByLineStream(trainingFile.OpenRead());
                IObjectStream<DocumentSample> documentSampleStream = new IntentDocumentSampleStream(intent, lineStream);
                categoryStreams.Add(documentSampleStream);
            }
            IObjectStream<DocumentSample> combinedDocumentSampleStream = ObjectStreamUtils.CreateObjectStream(categoryStreams.ToArray());
            var param = new TrainingParameters();
            param.Set(Parameters.Iterations, "70");
            param.Set(Parameters.Cutoff, "1");
            DocumentCategorizerModel doccatModel = DocumentCategorizerME.Train("vi", combinedDocumentSampleStream, param, new DocumentCategorizerFactory());
            combinedDocumentSampleStream.Dispose();

            IList<TokenNameFinderModel> tokenNameFinderModels = new List<TokenNameFinderModel>();

            foreach (string slot in slots)
            {
                List<IObjectStream<NameSample>> nameStreams = new List<IObjectStream<NameSample>>();
                foreach (FileInfo trainingFile in trainingDirectory.GetFiles())
                {
                    IObjectStream<string> lineStream = new PlainTextByLineStream(trainingFile.OpenRead());
                    IObjectStream<NameSample> nameSampleStream = new NameSampleStream(lineStream);
                    nameStreams.Add(nameSampleStream);
                }
                IObjectStream<NameSample> combinedNameSampleStream = ObjectStreamUtils.CreateObjectStream(nameStreams.ToArray());

                TokenNameFinderModel tokenNameFinderModel = NameFinderME.Train("vi", slot, combinedNameSampleStream, TrainingParameters.DefaultParameters(), new TokenNameFinderFactory(null, new Dictionary<string, object>()));
                combinedNameSampleStream.Dispose();
                tokenNameFinderModels.Add(tokenNameFinderModel);
            }


            DocumentCategorizerME categorizer = new DocumentCategorizerME(doccatModel);
            NameFinderME[] nameFinderMEs = new NameFinderME[tokenNameFinderModels.Count];
            for (int i = 0; i < tokenNameFinderModels.Count; i++)
            {
                nameFinderMEs[i] = new NameFinderME(tokenNameFinderModels[i]);
            }

            Console.WriteLine("Training complete. Ready.");
            Console.Write(">");
            string s;
            while (!string.ReferenceEquals((s = System.Console.ReadLine()), null))
            {
                double[] outcome = categorizer.Categorize(s);
                Console.Write("action=" + categorizer.GetBestCategory(outcome) + " args={ ");

                string[] tokens = WhitespaceTokenizer.Instance.Tokenize(s);
                foreach (NameFinderME nameFinderME in nameFinderMEs)
                {
                    Span[] spans = nameFinderME.Find(tokens);
                    string[] names = Span.SpansToStrings(spans, tokens);
                    for (int i = 0; i < spans.Length; i++)
                    {
                        Console.Write(spans[i].Type + "=" + names[i] + " ");
                    }
                }
                Console.WriteLine("}");
                Console.Write(">");

            }

        }
    }
}
