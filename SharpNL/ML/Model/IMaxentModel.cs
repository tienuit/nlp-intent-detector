// 
//  Copyright 2014 Gustavo J Knuppe (https://github.com/knuppe)
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
namespace SharpNL.ML.Model {
    /// <summary>
    /// Interface for maximum entropy models.
    /// </summary>
    public interface IMaxentModel {
        /// <summary>
        /// Evaluates a context.
        /// </summary>
        /// <param name="context">A list of String names of the contextual predicates which are to be evaluated together..</param>
        /// <returns>An array of the probabilities for each of the different outcomes, all of which sum to 1.</returns>
        double[] Eval(string[] context);

        /// <summary>
        /// Evaluates a context.
        /// </summary>
        /// <param name="context">A list of string names of the contextual predicates which are to be evaluated together..</param>
        /// <param name="probs">
        /// An array which is populated with the probabilities for each of the different outcomes, all of which
        /// sum to 1.
        /// </param>
        /// <returns>An array of the probabilities for each of the different outcomes, all of which sum to 1.</returns>
        double[] Eval(string[] context, double[] probs);

        /// <summary>
        /// Evaluates a contexts with the specified context values.
        /// </summary>
        /// <param name="context">A list of String names of the contextual predicates which are to be evaluated together.</param>
        /// <param name="probs">The values associated with each context.</param>
        /// <returns>An array of the probabilities for each of the different outcomes, all of which sum to 1.</returns>
        double[] Eval(string[] context, float[] probs);


        /// <summary>
        /// Simple function to return the outcome associated with the index containing the highest probability in the double[].
        /// </summary>
        /// <param name="outcomes"></param>
        /// <returns>The string name of the best outcome.</returns>
        string GetBestOutcome(double[] outcomes);

        /// <summary>
        /// Gets the string name of the outcome associated with the index,
        /// </summary>
        /// <param name="index">The index for which the name of the associated outcome is desired.</param>
        /// <returns>The string name of the outcome.</returns>
        string GetOutcome(int index);

        /// <summary>
        /// Gets an array of the outcome names.
        /// </summary>
        /// <returns>An array of the current outcome names.</returns>
        string[] GetOutcomes();

        /// <summary>
        /// Gets the index associated with the String name of the given outcome.
        /// </summary>
        /// <param name="outcome">The string name of the outcome for which the index is desired.</param>
        /// <returns>The index if the given outcome label exists for this model, -1 if it does not.</returns>
        int GetIndex(string outcome);

        /// <summary>
        /// Gets the data structures relevant to storing the model.
        /// </summary>
        /// <returns>The data structures relevant to storing the model.</returns>
        int GetNumOutcomes();
    }
}