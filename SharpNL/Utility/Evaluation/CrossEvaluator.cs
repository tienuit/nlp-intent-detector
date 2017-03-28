// 
//  Copyright 2015 Gustavo J Knuppe (https://github.com/knuppe)
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


namespace SharpNL.Utility.Evaluation {
    /// <summary>
    /// Provides the base class for cross evaluators.
    /// </summary>
    /// <typeparam name="T">The sample type.</typeparam>
    /// <typeparam name="P">The type of the objects to be evaluated.</typeparam>
    public abstract class CrossEvaluator<T, P> where P : class {

        /// <summary>
        /// Initializes a new instance of the <see cref="CrossEvaluator{T, P}"/> class.
        /// </summary>
        protected CrossEvaluator() {
            FMeasure = new FMeasure<P>();
        }
        
        #region + Properties .

        #region . FMesure .
        /// <summary>
        /// Gets the f-measure.
        /// </summary>
        /// <value>The f-measure.</value>
        public FMeasure<P> FMeasure { get; private set; }
        #endregion

        #endregion

        #region . Evaluate .
        /// <summary>
        /// Evaluates the samples with a given number of partitions.
        /// </summary>
        /// <param name="samples">The samples to train and test.</param>
        /// <param name="partitions">The number of folds.</param>
        public void Evaluate(IObjectStream<T> samples, int partitions) {

            var partitioner = new CrossValidationPartitioner<T>(samples, partitions);
            while (partitioner.HasNext) {
                var ps = partitioner.Next();

                var fm = Process(ps);

                FMeasure.MergeInto(fm);
            }

        }
        #endregion

        #region . Process .
        /// <summary>
        /// Processes the specified sample stream.
        /// </summary>
        /// <param name="sampleStream">The sample stream.</param>
        /// <returns>The computed f-mesure of the sample stream.</returns>
        protected abstract FMeasure<P> Process(CrossValidationPartitioner<T>.TrainingSampleStream sampleStream);
        #endregion

    }
}