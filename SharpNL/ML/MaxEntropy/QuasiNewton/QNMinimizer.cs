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
using System.Threading;

namespace SharpNL.ML.MaxEntropy.QuasiNewton {
    /// <summary>
    /// Implementation of L-BFGS which supports L1-, L2-regularization and Elastic Net for solving convex optimization problems.
    /// </summary>
    public partial class QNMinimizer {
        /// <summary>
        /// Function change rate tolerance
        /// </summary>
        public static readonly double ConvergeTolerance = 1e-4;

        /// <summary>
        /// Relative gradient norm tolerance
        /// </summary>
        public static readonly double RelGradNormTol = 1e-4;

        /// <summary>
        /// The initial step size
        /// </summary>
        public static readonly double InitialStepSize = 1.0;

        /// <summary>
        /// The minimum step size
        /// </summary>
        public static readonly double MinStepSize = 1e-10;

        /// <summary>
        /// The default L1-cost
        /// </summary>
        public static readonly double L1CostDefault = 0;

        /// <summary>
        /// The default L2-cost
        /// </summary>
        public static readonly double L2CostDefault = 0;

        /// <summary>
        /// The default number of iterations
        /// </summary>
        public static readonly int NumIterationsDefault = 100;

        /// <summary>
        /// The default number of Hessian updates to store.
        /// </summary>
        public static readonly int MDefault = 15;

        /// <summary>
        /// The default maximum number of function evaluations
        /// </summary>
        public static readonly int MaxFctEvalDefault = 30000;

        /// <summary>
        /// The maximum number of iterations
        /// </summary>
        private readonly int iterations;

        /// <summary>
        /// The L1-regularization cost
        /// </summary>
        private readonly double l1Cost;

        /// <summary>
        /// The L2-regularization cost
        /// </summary>
        private readonly double l2Cost;

        /// <summary>
        /// The number of Hessian updates to store
        /// </summary>
        private readonly int updates;

        /// <summary>
        /// The maximum number of function evaluations
        /// </summary>
        private readonly int maxFctEval;

        /// <summary>
        /// The evaluation monitor.
        /// </summary>
        private readonly Monitor monitor;

        /// <summary>
        /// The objective function's dimension
        /// </summary>
        private int dimension;

        // Hessian updates
        private UpdateInfo updateInfo;

        #region + Constructors .

        /// <summary>
        /// Initializes a new instance of the <see cref="QNMinimizer"/> class using the default parameters.
        /// </summary>
        public QNMinimizer() : this(L1CostDefault, L2CostDefault) {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="QNMinimizer"/> class.
        /// </summary>
        /// <param name="l1Cost">The L1-regularization cost.</param>
        /// <param name="l2Cost">The L2-regularization cost.</param>
        /// <exception cref="System.ArgumentOutOfRangeException">
        /// The L1-cost must not be less than zero.
        /// or
        /// The L2-cost must not be less than zero.
        /// or
        /// Number of iterations must be larger than zero.
        /// or
        /// Number of Hessian updates must be larger than zero.
        /// </exception>
        public QNMinimizer(double l1Cost, double l2Cost) : this(l1Cost, l2Cost, NumIterationsDefault) {
        }


        /// <summary>
        /// Initializes a new instance of the <see cref="QNMinimizer"/> class, using the <see cref="MaxFctEvalDefault"/> as the maximum number of function evaluations.
        /// </summary>
        /// <param name="l1Cost">The L1-regularization cost.</param>
        /// <param name="l2Cost">The L2-regularization cost.</param>
        /// <param name="iterations">The maximum number of iterations.</param>
        /// <exception cref="System.ArgumentOutOfRangeException">
        /// The L1-cost must not be less than zero.
        /// or
        /// The L2-cost must not be less than zero.
        /// or
        /// Number of iterations must be larger than zero.
        /// or
        /// Number of Hessian updates must be larger than zero.
        /// </exception>
        public QNMinimizer(double l1Cost, double l2Cost, int iterations)
            : this(l1Cost, l2Cost, iterations, MDefault, MaxFctEvalDefault) {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="QNMinimizer"/> class.
        /// </summary>
        /// <param name="l1Cost">The L1-regularization cost.</param>
        /// <param name="l2Cost">The L2-regularization cost.</param>
        /// <param name="iterations">The maximum number of iterations.</param>
        /// <param name="updates">The number of Hessian updates to store.</param>
        /// <param name="maxFctEval">The maximum number of function evaluations.</param>
        /// <exception cref="System.ArgumentOutOfRangeException">
        /// The L1-cost must not be less than zero.
        /// or
        /// The L2-cost must not be less than zero.
        /// or
        /// Number of iterations must be larger than zero.
        /// or
        /// Number of Hessian updates must be larger than zero.
        /// or
        /// Maximum number of function evaluations must be larger than zero.
        /// </exception>
        public QNMinimizer(double l1Cost, double l2Cost, int iterations, int updates, int maxFctEval)
            : this(l1Cost, l2Cost, iterations, updates, maxFctEval, null) {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="QNMinimizer"/> class.
        /// </summary>
        /// <param name="l1Cost">The L1-regularization cost.</param>
        /// <param name="l2Cost">The L2-regularization cost.</param>
        /// <param name="iterations">The maximum number of iterations.</param>
        /// <param name="updates">The number of Hessian updates to store.</param>
        /// <param name="maxFctEval">The maximum number of function evaluations.</param>
        /// <param name="monitor">The evaluation monitor. This argument can be a <c>null</c> value.</param>
        /// <exception cref="System.ArgumentOutOfRangeException">
        /// The L1-cost must not be less than zero.
        /// or
        /// The L2-cost must not be less than zero.
        /// or
        /// Number of iterations must be larger than zero.
        /// or
        /// Number of Hessian updates must be larger than zero.
        /// or
        /// Maximum number of function evaluations must be larger than zero.
        /// </exception>
        public QNMinimizer(double l1Cost, double l2Cost, int iterations, int updates, int maxFctEval, Monitor monitor) {
            if (l1Cost < 0)
                throw new ArgumentOutOfRangeException(nameof(l1Cost), "L1-cost must not be less than zero");

            if (l2Cost < 0)
                throw new ArgumentOutOfRangeException(nameof(l2Cost), "L2-cost must not be less than zero");

            if (iterations <= 0)
                throw new ArgumentOutOfRangeException(nameof(iterations), "Number of iterations must be larger than zero");

            if (updates <= 0)
                throw new ArgumentOutOfRangeException(nameof(updates), "Number of Hessian updates must be larger than zero");

            if (maxFctEval <= 0)
                throw new ArgumentOutOfRangeException(nameof(maxFctEval),
                    "Maximum number of function evaluations must be larger than zero");

            this.l1Cost = l1Cost;
            this.l2Cost = l2Cost;
            this.iterations = iterations;
            this.updates = updates;
            this.maxFctEval = maxFctEval;
            this.monitor = monitor;
        }

        #endregion

        #region . Evaluator .
        /// <summary>
        /// Gets or sets the evaluator.
        /// </summary>
        /// <value>The evaluator.</value>
        public IEvaluator Evaluator { get; set; }
        #endregion

        #region . Minimize .

        /// <summary>
        /// Find the parameters that minimize the objective function.
        /// </summary>
        /// <param name="function">The objective function.</param>
        /// <returns>The minimizing parameters.</returns>
        /// <exception cref="OperationCanceledException">Occurs when the evaluation monitor cancels the operation.</exception>
        public double[] Minimize(IFunction function) {
            var l2RegFunction = new L2RegFunction(function, l2Cost);
            dimension = l2RegFunction.Dimension;
            updateInfo = new UpdateInfo(updates, dimension);

            // Current point is at the origin
            var currPoint = new double[dimension];
            var currValue = l2RegFunction.ValueAt(currPoint);

            // Gradient at the current point
            var currGrad = new double[dimension];

            Array.Copy(l2RegFunction.GradientAt(currPoint), 0, currGrad, 0, dimension);

            // Pseudo-gradient - only use when L1-regularization is enabled
            double[] pseudoGrad = null;
            if (l1Cost > 0) {
                currValue += l1Cost*ArrayMath.L1Norm(currPoint);
                pseudoGrad = new double[dimension];
                ComputePseudoGrad(currPoint, currGrad, pseudoGrad);
            }

            var lsr = l1Cost > 0
                ? LineSearchResult.GetInitialObjectForL1(currValue, currGrad, pseudoGrad, currPoint)
                : LineSearchResult.GetInitialObject(currValue, currGrad, currPoint);

            if (monitor != null) {
                Display("\nSolving convex optimization problem.");
                Display("\nObjective function has " + dimension + " variable(s).");
                Display("\n\nPerforming " + iterations + " iterations with " + "L1Cost=" + l1Cost + " and L2Cost=" + l2Cost + "\n");
            }
            
            var direction = new double[dimension];
            var startTime = DateTime.Now;
            var token = monitor != null ? monitor.Token : CancellationToken.None;


            // Initial step size for the 1st iteration
            var initialStepSize = l1Cost > 0
                ? ArrayMath.InvL2Norm(lsr.PseudoGradAtNext)
                : ArrayMath.InvL2Norm(lsr.GradAtNext);

            for (var iteration = 1; iteration <= iterations; iteration++) {

                // cancel if requested 
                token.ThrowIfCancellationRequested();

                // Find direction
                Array.Copy(l1Cost > 0
                    ? lsr.PseudoGradAtNext
                    : lsr.GradAtNext, 0, direction, 0, direction.Length);

                ComputeDirection(direction);

                // Line search
                if (l1Cost > 0) {
                    // Constrain the search direction
                    pseudoGrad = lsr.PseudoGradAtNext;

                    for (var i = 0; i < dimension; i++)
                        if (direction[i]*pseudoGrad[i] >= 0) direction[i] = 0;

                    LineSearch.DoConstrainedLineSearch(l2RegFunction, direction, lsr, l1Cost, initialStepSize);

                    ComputePseudoGrad(lsr.NextPoint, lsr.GradAtNext, pseudoGrad);

                    lsr.PseudoGradAtNext = pseudoGrad;
                } else LineSearch.DoLineSearch(l2RegFunction, direction, lsr, initialStepSize);

                // Save Hessian updates
                updateInfo.Update(lsr);

                if (monitor != null) {
                    if (iteration < 10)
                        Display("  " + iteration + ":  ");
                    else if (iteration < 100)
                        Display(" " + iteration + ":  ");
                    else
                        Display(iteration + ":  ");

                    if (Evaluator != null) {
                        Display("\t" + lsr.ValueAtNext
                               + "\t" + lsr.FuncChangeRate
                               + "\t" + Evaluator.Evaluate(lsr.NextPoint) + "\n");
                    } else {
                        Display("\t " + lsr.ValueAtNext +
                                "\t" + lsr.FuncChangeRate + "\n");
                    }
                }

                if (IsConverged(lsr))
                    break;

                initialStepSize = InitialStepSize;
            }

            // Undo L2-shrinkage if Elastic Net is used (since in that case, the shrinkage is done twice)
            //
            // Knuppe: The original code makes no sense, so I change the NextPoint value!
            // 
            // if (l1Cost > 0 && l2Cost > 0) {
            //     double[] x = lsr.getNextPoint();
            //     for (int i = 0; i < dimension; i++) {
            //         x[i] = Math.sqrt(1 + l2Cost) * x[i];
            //     }
            // }

            if (l1Cost > 0 && l2Cost > 0)
                for (var i = 0; i < dimension; i++)
                    lsr.NextPoint[i] = Math.Sqrt(1 + l2Cost)*lsr.NextPoint[i];

            if (monitor != null) {
                var endTime = DateTime.Now;
                var duration = endTime - startTime;

                Display("Running time: " + duration.TotalSeconds + "s\n");                
            }


            // Release memory
            updateInfo = null;

            // Avoid returning the reference to LineSearchResult's member so that GC can
            // collect memory occupied by lsr after this function completes (is it necessary?)
            // double[] parameters = new double[dimension];
            // System.arraycopy(lsr.getNextPoint(), 0, parameters, 0, dimension);

            return lsr.NextPoint;
        }

        #endregion

        #region . ComputePseudoGrad .
        /// <summary>
        /// Pseudo-gradient for L1-regularization (see equation 4 in the paper "Scalable Training of L1-Regularized Log-Linear Models", Andrew et al. 2007)
        /// </summary>
        /// <param name="x">The current point.</param>
        /// <param name="g">The gradient at x.</param>
        /// <param name="pg">The pseudo-gradient at x which is to be computed.</param>
        private void ComputePseudoGrad(double[] x, double[] g, double[] pg) {
            for (var i = 0; i < dimension; i++) {
                if (x[i] < 0) {
                    pg[i] = g[i] - l1Cost;
                } else if (x[i] > 0) {
                    pg[i] = g[i] + l1Cost;
                } else {
                    if (g[i] < -l1Cost) {
                        // right partial derivative
                        pg[i] = g[i] + l1Cost;
                    } else if (g[i] > l1Cost) {
                        // left partial derivative
                        pg[i] = g[i] - l1Cost;
                    } else pg[i] = 0;
                }
            }
        }
        #endregion

        #region . ComputeDirection .
        /// <summary>
        /// L-BFGS two-loop recursion (see Nocedal & Wright 2006, Numerical Optimization, p. 178)
        /// </summary>
        /// <param name="direction">The direction.</param>
        private void ComputeDirection(double[] direction) {
            // Implemented two-loop Hessian update method.
            var k = updateInfo.kCounter;
            var rho = updateInfo.rho;
            var alpha = updateInfo.alpha; // just to avoid recreating alpha
            var S = updateInfo.S;
            var Y = updateInfo.Y;

            // First loop
            for (var i = k - 1; i >= 0; i--) {
                alpha[i] = rho[i]*ArrayMath.InnerProduct(S[i], direction);
                for (var j = 0; j < dimension; j++)
                    direction[j] = direction[j] - alpha[i]*Y[i][j];
            }

            // Second loop
            for (var i = 0; i < k; i++) {
                var beta = rho[i]*ArrayMath.InnerProduct(Y[i], direction);
                for (var j = 0; j < dimension; j++)
                    direction[j] = direction[j] + S[i][j]*(alpha[i] - beta);
                
            }

            for (var i = 0; i < dimension; i++)
                direction[i] = -direction[i];
            
        }
        #endregion

        #region . IsConverged .
        private bool IsConverged(LineSearchResult lsr) {
            // Check function's change rate
            if (lsr.FuncChangeRate < ConvergeTolerance) {
                if (monitor != null)
                    Display("Function change rate is smaller than the threshold " + ConvergeTolerance + ".\nTraining will stop.\n\n");

                return true;
            }

            // Check gradient's norm using the criteria: ||g(x)|| / max(1, ||x||) < threshold
            var xNorm = Math.Max(1, ArrayMath.L2Norm(lsr.NextPoint));
            var gradNorm = l1Cost > 0 ? ArrayMath.L2Norm(lsr.PseudoGradAtNext) : ArrayMath.L2Norm(lsr.GradAtNext);

            if (gradNorm / xNorm < RelGradNormTol) {
                if (monitor != null) {
                    Display("Relative L2-norm of the gradient is smaller than the threshold "
                            + RelGradNormTol + ".\nTraining will stop.\n\n");
                }
                return true;
            }

            // Check step size
            if (lsr.StepSize < MinStepSize) {
                if (monitor != null) {
                    Display("Step size is smaller than the minimum step size "
                            + MinStepSize + ".\nTraining will stop.\n\n");
                }
                return true;
            }

            // Check number of function evaluations
            if (lsr.FctEvalCount > maxFctEval) {
                if (monitor != null) {
                    Display("Maximum number of function evaluations has exceeded the threshold "
                            + maxFctEval + ".\nTraining will stop.\n\n");
                }
                return true;
            }

            return false;
        }
        #endregion

        #region . Display .
        /// <summary>
        /// Displays the given message to the listeners.
        /// </summary>
        /// <param name="s">The message.</param>
        private void Display(string s) {

#if DEBUG
            System.Diagnostics.Debug.WriteLine(s);
#endif

            if (monitor != null)
                monitor.OnMessage(s);
        }
        #endregion

    }
}