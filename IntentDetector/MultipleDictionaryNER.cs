using SharpNL.Dictionary;
using SharpNL.NameFind;
using SharpNL.Tokenize;
using SharpNL.Utility;
using System;
using System.Collections.Generic;
using System.IO;

public class MultipleDictionaryNER
{
    private static string dictionaryPath = "data\\dictionaries";
    private static IList<DictionaryNameFinder> finders;

    public MultipleDictionaryNER()
    {
        SimpleTokenizer tokenizer = SimpleTokenizer.Instance;

        // Get the directory with the dictionary files
        //Path directory = Paths.get(currentDir, DICTIONARIES);
        var directory = new DirectoryInfo(dictionaryPath);
        if (!directory.Exists)
        {
            throw new Exception("Directory '" + dictionaryPath + "' not found.");
        }

        finders = new List<DictionaryNameFinder>();
        FileInfo[] files = directory.GetFiles();
        foreach (FileInfo file in files)
        {
            try
            {
                using (StreamReader br =  file.OpenText())
                {
                    // Create a list with a dictionary for each file
                    Dictionary dictionary = new Dictionary();
                    for (string line; (line = br.ReadLine()) != null;)
                    {
                        dictionary.Add(new StringList(tokenizer.Tokenize(line)));
                    }
                    
                    string type = Path.GetFileNameWithoutExtension(file.Name);
                    // Use the file name to tag tokens
                    finders.Add(new DictionaryNameFinder(dictionary, type));
                }
            }
            catch (FileNotFoundException e)
            {
                Console.WriteLine(e.ToString());
                Console.Write(e.StackTrace);
            }
            catch (IOException e)
            {
                Console.WriteLine(e.ToString());
                Console.Write(e.StackTrace);
            }
        }
    }

    public Span[] Find(string[] tokens)
    {
        IList<Annotation> annotations = new List<Annotation>();
        List<Span> foundSpans = new List<Span>();

        foreach (DictionaryNameFinder finder in finders)
        {
            Span[] spans = finder.Find(tokens);
            foreach (Span span in spans)
            {
                foundSpans.Add(span);
            }
        }

        foundSpans.Sort(delegate (Span o1, Span o2) {
            return o1.CompareTo(o2);
        });

        return foundSpans.ToArray();
        //foreach (Span span in foundSpans)
        //{
        //    int start = span.Start;
        //    int end = span.End;
        //    string type = span.Type;
        //    string[] foundTokens = new string[end - start];
        //    Array.Copy(tokens, start, foundTokens,0, end - start);
        //    annotations.Add(new Annotation(foundTokens, span));
        //}

        //return annotations;
    }
    public class Annotation
    {
        private string[] tokens;
        private Span span;

        public Annotation(string[] tokens, Span span)
        {
            this.tokens = tokens;
            this.span = span;
        }

        public virtual string[] Tokens
        {
            get
            {
                return tokens;
            }
        }

        public virtual Span Span
        {
            get
            {
                return span;
            }
        }
    }

}
