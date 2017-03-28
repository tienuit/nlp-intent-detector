using SharpNL.Dictionary;
using SharpNL.DocumentCategorizer;
using SharpNL.NameFind;
using SharpNL.Tokenize;
using SharpNL.Utility;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IntentDetector
{
    public class IntentRecognizer
    {
        public static void Recognize()
        {
            //IList<TokenNameFinderModel> tokenNameFinderModels = new List<TokenNameFinderModel>();

            //foreach (string slot in slots)
            //{
            //    List<IObjectStream<NameSample>> nameStreams = new List<IObjectStream<NameSample>>();
            //    foreach (FileInfo trainingFile in trainingDirectory.GetFiles())
            //    {
            //        IObjectStream<string> lineStream = new PlainTextByLineStream(trainingFile.OpenRead());
            //        IObjectStream<NameSample> nameSampleStream = new NameSampleStream(lineStream);
            //        nameStreams.Add(nameSampleStream);
            //    }
            //    IObjectStream<NameSample> combinedNameSampleStream = ObjectStreamUtils.CreateObjectStream(nameStreams.ToArray());

            //    TokenNameFinderModel tokenNameFinderModel = NameFinderME.Train("vi", slot, combinedNameSampleStream, TrainingParameters.DefaultParameters(), new TokenNameFinderFactory(null, new Dictionary<string, object>()));
            //    combinedNameSampleStream.Dispose();
            //    tokenNameFinderModels.Add(tokenNameFinderModel);
            //}

            //DocumentCategorizerModel intentRecogModel = null;

            //try
            //{
            //    using (var modelFile = new FileStream(@"data\\intent-train-model.bin", FileMode.Open))
            //        intentRecogModel = new DocumentCategorizerModel(modelFile);
            //}
            //catch (Exception e)
            //{
            //    throw new Exception(e.Message);
            //    // handle the error
            //}

            //DocumentCategorizerME categorizer = new DocumentCategorizerME(intentRecogModel);
            //NameFinderME[] nameFinderMEs = new NameFinderME[tokenNameFinderModels.Count];
            //for (int i = 0; i < tokenNameFinderModels.Count; i++)
            //{
            //    nameFinderMEs[i] = new NameFinderME(tokenNameFinderModels[i]);
            //}



            //string s;
            //while (!ReferenceEquals((s = Console.ReadLine()), null))
            //{
            //    double[] outcome = categorizer.Categorize(s);
            //    Console.Write("action=" + categorizer.GetBestCategory(outcome) + " args={ ");

            //    string[] tokens = WhitespaceTokenizer.Instance.Tokenize(s);
            //    foreach (NameFinderME nameFinderME in nameFinderMEs)
            //    {
            //        Span[] spans = nameFinderME.Find(tokens);
            //        string[] names = Span.SpansToStrings(spans, tokens);
            //        for (int i = 0; i < spans.Length; i++)
            //        {
            //            Console.Write(spans[i].Type + "=" + names[i] + " ");
            //        }
            //    }

            //    Dictionary dictionary = new Dictionary();

            //    dictionary.Add(new StringList("Ha", "Noi"));
            //    dictionary.Add(new StringList("Ho", "Chi", "Minh"));
            //    dictionary.Add(new StringList("Yen", "Bai"));

            //    DictionaryNameFinder dictionaryNER = new DictionaryNameFinder(dictionary);

            //    Span[] dspans = dictionaryNER.Find(tokens);
            //    string[] dnames = Span.SpansToStrings(dspans, tokens);
            //    for (int i = 0; i < dspans.Length; i++)
            //    {
            //        Console.Write(dspans[i].Type + "=" + dnames[i] + " ");
            //    }

            //    Console.WriteLine("}");
            //    Console.Write(">");

            //}
        }
    }
}
