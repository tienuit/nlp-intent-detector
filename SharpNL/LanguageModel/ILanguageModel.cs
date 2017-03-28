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

using SharpNL.Utility;

namespace SharpNL.LanguageModel {

    /// <summary>
    /// A language model can calculate the probability P (between 0 and 1) of a certain <see cref="StringList"/> sequence of tokens, given its underlying vocabulary.
    /// </summary>
    public interface ILanguageModel {

        /// <summary>
        /// Calculate the probability of a series of tokens (e.g. a sentence), given a vocabulary
        /// </summary>
        /// <param name="tokens">Tokens the text tokens to calculate the probability for.</param>
        /// <returns>The probability of the given text tokens in the vocabulary.</returns>
        double CalculateProbability(StringList tokens);

        /// <summary>
        /// Predict the most probable output sequence of tokens, given an input sequence of tokens.
        /// </summary>
        /// <param name="tokens">A sequence of tokens.</param>
        /// <returns>The most probable subsequent token sequence.</returns>
        StringList PredictNextTokens(StringList tokens);

    }
}