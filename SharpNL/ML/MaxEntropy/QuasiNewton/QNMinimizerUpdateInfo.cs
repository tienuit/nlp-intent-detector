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
    public partial class QNMinimizer {
        private class UpdateInfo {
            private readonly int dimension;
            private readonly int m;

            internal readonly double[] rho;
            internal readonly double[][] S;
            internal readonly double[][] Y;
            internal readonly double[] alpha;
            internal int kCounter;

            // Constructor
            public UpdateInfo(int numCorrection, int dimension) {
                this.dimension = dimension;

                m = numCorrection;
                kCounter = 0;

                S = new double[m][];
                Y = new double[m][];
                for (var i = 0; i < m; i++) {
                    S[i] = new double[dimension];
                    Y[i] = new double[dimension];
                }
                    
                rho = new double[m];
                alpha = new double[m];
            }

            public void Update(LineSearchResult lsr) {
                var currPoint = lsr.CurrPoint;
                var gradAtCurr = lsr.GradAtCurr;
                var nextPoint = lsr.NextPoint;
                var gradAtNext = lsr.GradAtNext;

                // Inner product of S_k and Y_k
                var SYk = 0.0;

                // Add new ones.
                if (kCounter < m) {
                    for (var j = 0; j < dimension; j++) {
                        S[kCounter][j] = nextPoint[j] - currPoint[j];
                        Y[kCounter][j] = gradAtNext[j] - gradAtCurr[j];
                        SYk += S[kCounter][j]*Y[kCounter][j];
                    }

                    rho[kCounter] = 1.0/SYk;
                } else {
                    // Discard oldest vectors and add new ones.
                    for (var i = 0; i < m - 1; i++) {
                        S[i] = S[i + 1];
                        Y[i] = Y[i + 1];
                        rho[i] = rho[i + 1];
                    }

                    for (var j = 0; j < dimension; j++) {
                        S[m - 1][j] = nextPoint[j] - currPoint[j];
                        Y[m - 1][j] = gradAtNext[j] - gradAtCurr[j];
                        SYk += S[m - 1][j]*Y[m - 1][j];
                    }

                    rho[m - 1] = 1.0/SYk;
                }

                if (kCounter < m)
                    kCounter++;
            }
        }
    }
}