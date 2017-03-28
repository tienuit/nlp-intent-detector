using SharpNL.DocumentCategorizer;
using SharpNL.NameFind;
using SharpNL.Utility;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IntentDetector
{
    public class Trainer
    {
        public static void Train()
        {
            DirectoryInfo trainingDirectory = new DirectoryInfo(@"data\samples");
            string[] slots = new string[] { "city" };
            
            if (!trainingDirectory.Exists)
            {
                trainingDirectory.Create();
            }

            //trainning document categorizer
            List<IObjectStream<DocumentSample>> categoryStreams = new List<IObjectStream<DocumentSample>>();
            foreach (FileInfo trainingFile in trainingDirectory.GetFiles())
            {
                string intent = Path.GetFileNameWithoutExtension(trainingFile.Name).Replace("[.][^.]+$", "");
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

            using (var fileStream = new FileStream("data\\intent-train-model.bin", FileMode.Create))
            {
                doccatModel.Serialize(fileStream);
            }

            //training namefinder
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
                using (var fileStream = new FileStream("data\\tokennames\\"+slot+".bin", FileMode.Create))
                {
                    tokenNameFinderModel.Serialize(fileStream);
                }
                tokenNameFinderModels.Add(tokenNameFinderModel);
            }
        }
    }
}
