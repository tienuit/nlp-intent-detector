//  
//  Copyright 2016 Gustavo J Knuppe (https://github.com/knuppe)
//  
//   Licensed under the Apache License, Version 2.0 (the "License");
//   you may not use this file except in compliance with the License.
//   You may obtain a copy of the License at
//  
//       http://www.apache.org/licenses/LICENSE-2.0
//  
//   Unless required by applicable law or agreed to in writing, software
//   distributed under the License is distributed on an "AS IS" BASIS,
//   WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//   See the License for the specific language governing permissions and
//   limitations under the License.
//  
//   - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - -
//   - May you do good and not evil.                                         -
//   - May you find forgiveness for yourself and forgive others.             -
//   - May you share freely, never taking more than you give.                -
//   - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - -
//   

using SharpNL.ML.Model;
using SharpNL.Utility;
using Sequence = SharpNL.ML.Model.Sequence;

namespace SharpNL.Lemmatizer {
    public class LemmaSampleSequenceStream : ISequenceStream {
        private readonly ILemmatizerContextGenerator contextGenerator;


        private readonly IObjectStream<LemmaSample> samples;

        public LemmaSampleSequenceStream(IObjectStream<LemmaSample> samples, ILemmatizerContextGenerator contextGenerator) {
            this.samples = samples;
            this.contextGenerator = contextGenerator;
        }


        //private readonly IObjectStream<LemmaSa> 

        public void Dispose() {
            samples.Dispose();
        }

        public Sequence Read() {
            var sample = samples.Read();

            if (sample == null)
                return null;

            var events = new Event[sample.Length];

            for (var i = 0; i < sample.Length; i++) {
                // it is safe to pass the tags as previous tags because
                // the context generator does not look for non predicted tags
                var context = contextGenerator.GetContext(i, sample.Tokens, sample.Tags, sample.Lemmas);

                events[i] = new Event(sample.Tags[i], context);
            }
            return new Sequence(events, sample);
        }

        public void Reset() {
            samples.Reset();
        }

        public Event[] UpdateContext(Sequence sequence, AbstractModel model) {
            // TODO: Should be implemented for Perceptron sequence learning ...
            return null;
        }
    }
}