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

using System;
using System.Collections.Generic;
using SharpNL.ML.Model;
using SharpNL.Utility;

namespace SharpNL.Lemmatizer {
    /// <summary>
    /// Represents an event stream out of data files for training a probabilistic lemmatizer.
    /// </summary>
    public class LemmaSampleEventStream : AbstractEventStream<LemmaSample> {
        protected readonly ILemmatizerContextGenerator ContextGenerator;

        /// <summary>
        /// Creates a new event stream based on the specified data stream using the specified context generator.
        /// </summary>
        /// <param name="samples">The data stream for this event stream.</param>
        /// <param name="cg">The context generator which should be used in the creation of events for this event stream.</param>
        public LemmaSampleEventStream(IObjectStream<LemmaSample> samples, ILemmatizerContextGenerator cg) : base(samples) {
            if (cg == null)
                throw new ArgumentNullException(nameof(cg));

            ContextGenerator = cg;
        }

        protected override IEnumerator<Event> CreateEvents(LemmaSample sample) {
            if (sample == null)
                yield break;

            for (var i = 0; i < sample.Length; i++)
                yield return new Event(sample.Lemmas[i], ContextGenerator.GetContext(i, sample.Tokens, sample.Tags, sample.Lemmas));
        }
    }
}