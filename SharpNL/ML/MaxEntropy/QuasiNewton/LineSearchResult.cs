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

namespace SharpNL.ML.MaxEntropy.QuasiNewton {
    /// <summary>
    /// Represents a line search result.
    /// </summary>
    public class LineSearchResult {
        /// <summary>
        /// Initializes a new instance of the <see cref="LineSearchResult"/> class.
        /// </summary>
        /// <param name="stepSize">Size of the step.</param>
        /// <param name="valueAtCurr">The value at curr.</param>
        /// <param name="valueAtNext">The value at next.</param>
        /// <param name="gradAtCurr">The grad at curr.</param>
        /// <param name="gradAtNext">The grad at next.</param>
        /// <param name="currPoint">The curr point.</param>
        /// <param name="nextPoint">The next point.</param>
        /// <param name="fctEvalCount">The FCT eval count.</param>
        public LineSearchResult(
            double stepSize,
            double valueAtCurr,
            double valueAtNext,
            double[] gradAtCurr,
            double[] gradAtNext,
            double[] currPoint,
            double[] nextPoint,
            int fctEvalCount) {

            SetAll(stepSize, valueAtCurr, valueAtNext, gradAtCurr, gradAtNext, currPoint, nextPoint, fctEvalCount);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="LineSearchResult"/> class using the given sign vector.
        /// </summary>
        /// <param name="stepSize">Size of the step.</param>
        /// <param name="valueAtCurr">The value at curr.</param>
        /// <param name="valueAtNext">The value at next.</param>
        /// <param name="gradAtCurr">The grad at curr.</param>
        /// <param name="gradAtNext">The grad at next.</param>
        /// <param name="pseudoGradAtNext">The pseudo grad at next.</param>
        /// <param name="currPoint">The curr point.</param>
        /// <param name="nextPoint">The next point.</param>
        /// <param name="signVector">The sign vector.</param>
        /// <param name="fctEvalCount">The FCT eval count.</param>
        public LineSearchResult(
            double stepSize,
            double valueAtCurr,
            double valueAtNext,
            double[] gradAtCurr,
            double[] gradAtNext,
            double[] pseudoGradAtNext,
            double[] currPoint,
            double[] nextPoint,
            double[] signVector,
            int fctEvalCount) {
            SetAll(stepSize, valueAtCurr, valueAtNext, gradAtCurr, gradAtNext, pseudoGradAtNext, currPoint, nextPoint, signVector, fctEvalCount);
        }

        /// <summary>
        /// Gets the function change rate.
        /// </summary>
        /// <value>The function change rate.</value>
        public double FuncChangeRate => (ValueAtCurr - ValueAtNext)/ValueAtCurr;

        /// <summary>
        /// Gets or sets the size of the step.
        /// </summary>
        /// <value>The size of the step.</value>
        public double StepSize { get; set; }
        /// <summary>
        /// Gets or sets the value at curr.
        /// </summary>
        /// <value>The value at curr.</value>
        public double ValueAtCurr { get; set; }
        /// <summary>
        /// Gets or sets the value at next.
        /// </summary>
        /// <value>The value at next.</value>
        public double ValueAtNext { get; set; }
        /// <summary>
        /// Gets or sets the grad at curr.
        /// </summary>
        /// <value>The grad at curr.</value>
        public double[] GradAtCurr { get; set; }
        /// <summary>
        /// Gets or sets the grad at next.
        /// </summary>
        /// <value>The grad at next.</value>
        public double[] GradAtNext { get; set; }
        /// <summary>
        /// Gets or sets the pseudo grad at next.
        /// </summary>
        /// <value>The pseudo grad at next.</value>
        public double[] PseudoGradAtNext { get; set; }
        /// <summary>
        /// Gets or sets the curr point.
        /// </summary>
        /// <value>The curr point.</value>
        public double[] CurrPoint { get; set; }

        /// <summary>
        /// Gets or sets the next point.
        /// </summary>
        /// <value>The next point.</value>
        public double[] NextPoint { get; set; }
        /// <summary>
        /// Gets or sets the sign vector.
        /// </summary>
        /// <value>The sign vector.</value>
        public double[] SignVector { get; set; }
        /// <summary>
        /// Gets or sets the FCT eval count.
        /// </summary>
        /// <value>The FCT eval count.</value>
        public int FctEvalCount { get; set; }

        /// <summary>
        /// Update line search elements.
        /// </summary>
        /// <param name="stepSize">Size of the step.</param>
        /// <param name="valueAtCurr">The value at curr.</param>
        /// <param name="valueAtNext">The value at next.</param>
        /// <param name="gradAtCurr">The grad at curr.</param>
        /// <param name="gradAtNext">The grad at next.</param>
        /// <param name="currPoint">The curr point.</param>
        /// <param name="nextPoint">The next point.</param>
        /// <param name="fctEvalCount">The FCT eval count.</param>
        public void SetAll(
            double stepSize,
            double valueAtCurr,
            double valueAtNext,
            double[] gradAtCurr,
            double[] gradAtNext,
            double[] currPoint,
            double[] nextPoint,
            int fctEvalCount) {
            SetAll(stepSize, valueAtCurr, valueAtNext, gradAtCurr, gradAtNext,
                null, currPoint, nextPoint, null, fctEvalCount);
        }


        /// <summary>
        /// Update line search elements.
        /// </summary>
        /// <param name="stepSize">Size of the step.</param>
        /// <param name="valueAtCurr">The value at curr.</param>
        /// <param name="valueAtNext">The value at next.</param>
        /// <param name="gradAtCurr">The grad at curr.</param>
        /// <param name="gradAtNext">The grad at next.</param>
        /// <param name="pseudoGradAtNext">The pseudo grad at next.</param>
        /// <param name="currPoint">The curr point.</param>
        /// <param name="nextPoint">The next point.</param>
        /// <param name="signVector">The sign vector.</param>
        /// <param name="fctEvalCount">The FCT eval count.</param>
        public void SetAll(
            double stepSize,
            double valueAtCurr,
            double valueAtNext,
            double[] gradAtCurr,
            double[] gradAtNext,
            double[] pseudoGradAtNext,
            double[] currPoint,
            double[] nextPoint,
            double[] signVector,
            int fctEvalCount) {

            StepSize = stepSize;
            ValueAtCurr = valueAtCurr;
            ValueAtNext = valueAtNext;
            GradAtCurr = gradAtCurr;
            GradAtNext = gradAtNext;
            PseudoGradAtNext = pseudoGradAtNext;
            CurrPoint = currPoint;
            NextPoint = nextPoint;
            SignVector = signVector;
            FctEvalCount = fctEvalCount;
        }

        /// <summary>
        /// Gets the initial linear search object object.
        /// </summary>
        public static LineSearchResult GetInitialObject(
            double valueAtX,
            double[] gradAtX,
            double[] x) {
            return GetInitialObject(valueAtX, gradAtX, null, x, null, 0);
        }

        /// <summary>
        /// Gets the initial linear search object for L1-regularization
        /// </summary>
        public static LineSearchResult GetInitialObjectForL1(
            double valueAtX,
            double[] gradAtX,
            double[] pseudoGradAtX,
            double[] x) {
            return GetInitialObject(valueAtX, gradAtX, pseudoGradAtX, x, new double[x.Length], 0);
        }

        public static LineSearchResult GetInitialObject(
            double valueAtX,
            double[] gradAtX,
            double[] pseudoGradAtX,
            double[] x,
            double[] signX,
            int fctEvalCount) {
            return new LineSearchResult(0.0, 0.0, valueAtX, new double[x.Length], gradAtX,
                pseudoGradAtX, new double[x.Length], x, signX, fctEvalCount);
        }
    }
}