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

using System;
using SharpNL.Utility;
using SharpNL.Utility.Evaluation;

namespace SharpNL.Parser {
    /// <summary>
    /// Represents a cross validator for <see cref="ParserModel"/> models.
    /// </summary>
    public class ParserCrossEvaluator : CrossEvaluator<Parse, Span> {

        private readonly string languageCode;
        private readonly TrainingParameters parameters;
        private readonly AbstractHeadRules headRules;
        private readonly ParserType parserType;
        private readonly IEvaluationMonitor<Parse>[] monitors;

        /// <summary>
        /// Initializes a new instance of the <see cref="ParserCrossEvaluator"/> class.
        /// </summary>
        /// <param name="languageCode">The language of the training data.</param>
        /// <param name="parameters">The machine learning train parameters.</param>
        /// <param name="parserType">The parser model type.</param>
        /// <param name="monitors">The training monitors.</param>
        /// <param name="headRules">The headrules.</param>
        public ParserCrossEvaluator(string languageCode, TrainingParameters parameters, AbstractHeadRules headRules, ParserType parserType, params IEvaluationMonitor<Parse>[] monitors) {

            this.languageCode = languageCode;
            this.parameters = parameters;
            this.headRules = headRules;
            this.parserType = parserType;
            this.monitors = monitors;

        }

        /// <summary>
        /// Processes the specified sample stream.
        /// </summary>
        /// <param name="sampleStream">The sample stream.</param>
        /// <returns>The computed f-measure of the sample stream.</returns>
        protected override FMeasure<Span> Process(CrossValidationPartitioner<Parse>.TrainingSampleStream sampleStream) {

            ParserModel model;

            switch (parserType) {
                case ParserType.Chunking:
                    model = Chunking.Parser.Train(languageCode, sampleStream, headRules, parameters);
                    break;
                case ParserType.TreeInsert:
                    model = TreeInsert.Parser.Train(languageCode, sampleStream, headRules, parameters);
                    break;
                default:
                    throw new InvalidOperationException("Unexpected parser type.");
            }


            var evaluator = new ParserEvaluator(ParserFactory.Create(model), monitors);

            evaluator.Evaluate(sampleStream);

            return evaluator.FMeasure;
        }
    }
}