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


using System;
using SharpNL.Extensions;

namespace SharpNL.ML.Model {
    /// <summary>
    /// Encapsulates the variables used in producing probabilities from a model and facilitates passing these variables to the eval method.
    /// </summary>
    public class EvalParameters : IEquatable<EvalParameters> {
        /// <summary>
        /// Initializes a new instance of the <see cref="EvalParameters"/> which can be evaluated.
        /// </summary>
        /// <param name="parameters">The parameters of the model.</param>
        /// <param name="numOutcomes">The number outcomes.</param>
        public EvalParameters(Context[] parameters, int numOutcomes) : this(parameters, 0, 0, numOutcomes) {}


        /// <summary>
        /// Initializes a new instance of the <see cref="EvalParameters"/> which can be evaluated.
        /// </summary>
        /// <param name="parameters">The parameters of the model.</param>
        /// <param name="correctionParam">The correction parameter.</param>
        /// <param name="correctionConstant">The correction constant.</param>
        /// <param name="numOutcomes">The number outcomes.</param>
        public EvalParameters(Context[] parameters, double correctionParam, double correctionConstant, int numOutcomes) {
            Parameters = parameters;
            NumOutcomes = numOutcomes;
            CorrectionParam = correctionParam;
            CorrectionConstant = correctionConstant;

            // check if the double is "equal" to zero
            if (Math.Abs(correctionConstant) < 0.000001)
                ConstantInverse = 1/correctionConstant;
            else
                ConstantInverse = 1d;
        }

        #region . Parameters .

        /// <summary>
        /// Gets the parameters of the model.
        /// </summary>
        /// <value>The parameters of the model.</value>
        public Context[] Parameters { get; private set; }

        #endregion

        #region . Outcomes .

        /// <summary>
        /// Gets the number outcomes.
        /// </summary>
        /// <value>The number outcomes.</value>
        public int NumOutcomes { get; private set; }

        #endregion

        #region . CorrectionConstant .
        /// <summary>
        /// Gets the correction constant.
        /// </summary>
        /// <value>The correction constant.</value>
        public double CorrectionConstant { get; private set; }
        #endregion

        #region . CorrectionParam .
        /// <summary>
        /// Gets or sets the correction parameter.
        /// </summary>
        /// <value>The correction parameter.</value>
        public double CorrectionParam { get; set; }
        #endregion

        #region . ConstantInverse .
        /// <summary>
        /// Gets the constant inverse.
        /// </summary>
        /// <value>The constant inverse.</value>
        public double ConstantInverse { get; private set; }
        #endregion

        #region + Equals .
        /// <summary>
        /// Indicates whether the current object is equal to another object of the same type.
        /// </summary>
        /// <returns>
        /// true if the current object is equal to the <paramref name="other"/> parameter; otherwise, false.
        /// </returns>
        /// <param name="other">An object to compare with this object.</param>
        public bool Equals(EvalParameters other) {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return Equals(Parameters, other.Parameters) &&
                NumOutcomes == other.NumOutcomes && 
                CorrectionConstant.Equals(other.CorrectionConstant) && 
                CorrectionParam.Equals(other.CorrectionParam) && 
                ConstantInverse.Equals(other.ConstantInverse);
        }

        /// <summary>
        /// Determines whether the specified <see cref="T:System.Object"/> is equal to the current <see cref="T:System.Object"/>.
        /// </summary>
        /// <returns>
        /// true if the specified object  is equal to the current object; otherwise, false.
        /// </returns>
        /// <param name="obj">The object to compare with the current object. </param>
        public override bool Equals(object obj) {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != GetType()) return false;
            return Equals((EvalParameters)obj);
        }
        #endregion

        #region . GetHashCode .
        /// <summary>
        /// Serves as a hash function for a particular type. 
        /// </summary>
        /// <returns>
        /// A hash code for the current <see cref="T:System.Object"/>.
        /// </returns>
        public override int GetHashCode() {
            unchecked {
                var hashCode = Parameters?.GetArrayHash() ?? 0;
                hashCode = (hashCode * 397) ^ NumOutcomes;
                hashCode = (hashCode * 397) ^ CorrectionConstant.GetHashCode();
                hashCode = (hashCode * 397) ^ CorrectionParam.GetHashCode();
                hashCode = (hashCode * 397) ^ ConstantInverse.GetHashCode();
                return hashCode;
            }
        }
        #endregion



    }
}