using SharpNL.Dictionary;
using SharpNL.DocumentCategorizer;
using SharpNL.NameFind;
using SharpNL.Tokenize;
using SharpNL.Utility;
using SharpNL.Utility.FeatureGen;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using static MultipleDictionaryNER;

namespace IntentDetector
{
    class Program
    {
        static void Main(string[] args)
        {

            //MultipleDictionaryNER ner = new MultipleDictionaryNER();
            //string text = "Thành phố Ho Chi Minh";
            //SimpleTokenizer tokenizer = SimpleTokenizer.Instance;
            //IList<Annotation> annotations = ner.Find(tokenizer.Tokenize(text));
            //foreach (Annotation annotation in annotations)
            //{
            //    foreach (string token in annotation.Tokens)
            //    {
            //        Console.Write("{0} ", token);
            //    }
            //    Span span = annotation.Span;
            //    Console.Write("[{0:D}..{1:D}) {2}\n", span.Start, span.End, span.Type);
            //}
            //return;

            //Trainer.Train();

            //Console.WriteLine("Training complete. Ready.");
            //Console.Write(">");

            DocumentCategorizerModel doccatModel;
            using (var fileStream = new FileStream("data\\intent-train-model.bin", FileMode.Open))
            {
                doccatModel = new DocumentCategorizerModel(fileStream);
            }
           
            DocumentCategorizerME categorizer = new DocumentCategorizerME(doccatModel);

            DirectoryInfo tokenNamesDirecroty = new DirectoryInfo("data\\tokennames");
            List<TokenNameFinderModel> tokenNameFinderModels = new List<TokenNameFinderModel>();
            foreach (FileInfo tokenNameFile in tokenNamesDirecroty.GetFiles())
            {
                using (var fileStream = new FileStream(tokenNameFile.FullName, FileMode.Open))
                {
                    tokenNameFinderModels.Add(new TokenNameFinderModel(fileStream));
                    fileStream.Close();
                }
            }
            NameFinderME[] nameFinderMEs = new NameFinderME[tokenNameFinderModels.Count];
            for (int i = 0; i < tokenNameFinderModels.Count; i++)
            {
                nameFinderMEs[i] = new NameFinderME(tokenNameFinderModels[i]);
            }
           
           

            string s;
            while (!ReferenceEquals((s = Console.ReadLine()), null))
            {
                double[] outcome = categorizer.Categorize(s);
                var max = outcome.Max();
                Console.Write("action=" + categorizer.GetBestCategory(outcome) + " - " +max+" args={ ");

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

                MultipleDictionaryNER multiDictionaryNER = new MultipleDictionaryNER();
                multiDictionaryNER.Find(tokens);

                //Dictionary dictionary = new Dictionary();

                //dictionary.Add(new StringList("Ha", "Noi"));
                //dictionary.Add(new StringList("Ho", "Chi", "Minh"));
                //dictionary.Add(new StringList("Yen", "Bai"));

                //DictionaryNameFinder dictionaryNER = new DictionaryNameFinder(dictionary);
                
                Span[] dspans = multiDictionaryNER.Find(tokens);
                string[] dnames = Span.SpansToStrings(dspans, tokens);
                for (int i = 0; i < dspans.Length; i++)
                {
                    Console.Write(dspans[i].Type + "=" + dnames[i] + " ");
                }

                Console.WriteLine("}");
                Console.Write(">");

            }

        }

       
    }


}
