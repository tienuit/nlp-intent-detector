using SharpNL.Utility;

namespace IntentDetector
{

    public class ObjectStreamUtils
    {
        public static IObjectStream<T> CreateObjectStream<T>(params IObjectStream<T>[] streams)
        {
            foreach (IObjectStream<T> stream in streams)
            {
                if (stream == null)
                {
                    throw new System.NullReferenceException("stream cannot be null");
                }
            }

            return new CombiledObjectStream<T>(streams);
        }

        private class CombiledObjectStream<T> : IObjectStream<T>
        {
            private IObjectStream<T>[] streams;

            public CombiledObjectStream(IObjectStream<T>[] streams)
            {
                this.streams = streams;
            }


            private int streamIndex = 0;

            //JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
            //ORIGINAL LINE: public T read() throws IOException
            public T Read()
            {

                T obj = default(T);

                while (streamIndex < streams.Length && obj == null)
                {
                    obj = streams[streamIndex].Read();

                    if (obj == null)
                    {
                        streamIndex++;
                    }
                }

                return obj;
            }

            //JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
            //ORIGINAL LINE: public void reset() throws IOException, UnsupportedOperationException
            public void Reset()
            {
                streamIndex = 0;

                foreach (IObjectStream<T> stream in streams)
                {
                    stream.Reset();
                }
            }

            //JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
            //ORIGINAL LINE: public void close() throws IOException
            public void Dispose()
            {

                foreach (IObjectStream<T> stream in streams)
                {
                    stream.Dispose();
                }
            }
        }


    }

}
