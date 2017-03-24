using SharpNL.DocumentCategorizer;
using SharpNL.Tokenize;
using SharpNL.Utility;
using System;
using System.Collections.Generic;
using System.IO;

namespace IntentDetector
{

    public class IntentDocumentSampleStream : IObjectStream<DocumentSample>
    {

        internal string category;
        internal IObjectStream<string> stream;


        public IntentDocumentSampleStream(string category, IObjectStream<string> stream)
        {
            this.category = category;
            this.stream = stream;
        }

        public DocumentSample Read()
        {
            string sampleString = stream.Read();

            if (!string.ReferenceEquals(sampleString, null))
            {

                // Whitespace tokenize entire string
                string[] tokens = WhitespaceTokenizer.Instance.Tokenize(sampleString);

                //remove entities
                List<string> vector = new List<string>(tokens.Length);
                bool skip = false;
                foreach (string token in tokens)
                {
                    if (token.StartsWith("<", StringComparison.Ordinal))
                    {
                        skip = !skip;
                    }
                    else if (!skip)
                    {
                        Console.Write(token + " ");
                        vector.Add(token);
                    }
                }
                Console.WriteLine();

                tokens = new string[vector.Count];
                vector.CopyTo(tokens);

                DocumentSample sample;

                if (tokens.Length > 0)
                {
                    sample = new DocumentSample(category, tokens);
                }
                else
                {
                    throw new IOException("Empty lines are not allowed!");
                }

                return sample;
            }
            else
            {
                return null;
            }
        }

        public void Reset()
        {
            stream.Reset();
        }



        public void Dispose()
        {
            stream.Dispose();
        }
    }

}
