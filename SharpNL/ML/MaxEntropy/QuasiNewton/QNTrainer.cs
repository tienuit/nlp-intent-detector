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
using System.Collections.Generic;
using SharpNL.ML.Model;
using SharpNL.Utility;

namespace SharpNL.ML.MaxEntropy.QuasiNewton {
    /// <summary>
    /// Represents a maxent model trainer using L-BFGS algorithm.
    /// </summary>
    /// <seealso href="https://en.wikipedia.org/wiki/Limited-memory_BFGS"/>
    public class QNTrainer : AbstractEventTrainer {

        #region + Defaults .

        /// <summary>
        /// The default number of threads.
        /// </summary>
        public const int DefaultThreads = 1;

        /// <summary>
        /// The default L1-regularization cost value.
        /// </summary>
        public const double DefaultL1Cost = 0.1;

        /// <summary>
        /// The default L2-regularization cost value.
        /// </summary>
        public const double DefaultL2Cost = 0.1;

        /// <summary>
        /// The default number of updates.
        /// </summary>
        public const int DefaultUpdates = 15;

        /// <summary>
        /// The default maximum number of function evaluations.
        /// </summary>
        public const int DefaultMaxFctEval = 30000;

        #endregion

        #region + Fields .

        /// <summary>
        /// The L1-regularization cost
        /// </summary>
        private double l1Cost;

        /// <summary>
        /// The L2-regularization cost
        /// </summary>
        private double l2Cost;

        /// <summary>
        /// The number of updates.
        /// </summary>
        private int updates;

        /// <summary>
        /// The maximum number of function evaluations.
        /// </summary>
        private int maxFctEval;

        /// <summary>
        /// The number of threads
        /// </summary>
        private int threads;


        #endregion

        #region + Constructors .

        /// <summary>
        /// Initializes a new instance of the <see cref="QNTrainer"/> class using the default parameters.
        /// </summary>
        public QNTrainer() 
            : this(DefaultUpdates, DefaultMaxFctEval) {
           
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="AbstractTrainer" /> class with the given evaluation monitor.
        /// </summary>
        /// <param name="monitor">The evaluation monitor. This value can be null.</param>
        public QNTrainer(Monitor monitor)
            : this(DefaultUpdates, DefaultMaxFctEval, DefaultL1Cost, DefaultL2Cost, DefaultThreads, monitor) {
            
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="QNTrainer"/> class using the specified 
        /// number of hessian updates.
        /// </summary>
        /// <param name="updates">The number of hessian updates to store.</param>
        /// <exception cref="ArgumentOutOfRangeException">The number of Hessian updates must be larger than zero.</exception>
        public QNTrainer(int updates) : this(updates, DefaultMaxFctEval) {
            
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="QNTrainer"/> class using the specified 
        /// number of hessian updates and the maximum number of function evaluations.
        /// </summary>
        /// <param name="updates">The number of hessian updates to store.</param>
        /// <param name="maxFctEval">The maximum number of function evaluations.</param>
        /// <exception cref="ArgumentOutOfRangeException">
        /// The number of Hessian updates must be larger than zero.
        /// or
        /// The maximum number of function evaluations must be larger than zero.
        /// </exception>
        public QNTrainer(int updates, int maxFctEval) : this(updates, maxFctEval, DefaultL1Cost, DefaultL2Cost, DefaultThreads, null) {
               
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="AbstractEventTrainer" /> class.
        /// </summary>
        /// <param name="updates">The number of Hessian updates to store.</param>
        /// <param name="maxFctEval">The maximum number of function evaluations.</param>
        /// <param name="l1Cost">The L1-regularization cost.</param>
        /// <param name="l2Cost">The L2-regularization cost.</param>
        /// <param name="threads">The number of threads.</param>
        /// <param name="monitor">The evaluation monitor.</param>
        /// <exception cref="System.ArgumentOutOfRangeException">
        /// The L1-cost must not be less than zero.
        /// or
        /// The L2-cost must not be less than zero.
        /// or
        /// Number of Hessian updates must be larger than zero.
        /// or
        /// The maximum number of function evaluations must be larger than zero.
        /// or
        /// The number of threads must be larger than zero.
        /// </exception>
        public QNTrainer(int updates, int maxFctEval, double l1Cost, double l2Cost, int threads, Monitor monitor)
            : base(monitor, true) {

            if (l1Cost < 0)
                throw new ArgumentOutOfRangeException(nameof(l1Cost), "L1-cost must not be less than zero");

            if (l2Cost < 0)
                throw new ArgumentOutOfRangeException(nameof(l2Cost), "L2-cost must not be less than zero");

            if (updates <= 0)
                throw new ArgumentOutOfRangeException(nameof(updates), "Number of Hessian updates must be larger than zero");

            if (maxFctEval <= 0)
                throw new ArgumentOutOfRangeException(nameof(maxFctEval),
                    "The maximum number of function evaluations must be larger than zero");

            if (threads <= 0)
                throw new ArgumentOutOfRangeException(nameof(threads), "The number of threads must be larger than zero");

            this.threads = threads;

            this.updates = updates;
            this.maxFctEval = maxFctEval;
            this.l1Cost = l1Cost;
            this.l2Cost = l2Cost;
        }

        #endregion

        #region . IsValid .
        /// <summary>
        /// Determines whether the parameters of this trainer are valid.
        /// </summary>
        /// <returns>
        /// <c>true</c> if the parameters of this trainer are valid; otherwise, <c>false</c>.
        /// </returns>
        protected override bool IsValid() {
            if (!base.IsValid())
                return false;

            if (Algorithm != Parameters.Algorithms.MaxEntQn)
                return false;

            // ReSharper disable once JoinDeclarationAndInitializer
            int iv;

            // Number of Hessian updates to remember
            iv = GetIntParam(Parameters.NumOfUpdates, DefaultUpdates);
            if (iv < 0)
                return false;

            updates = iv;

            // Maximum number of function evaluations
            iv = GetIntParam(Parameters.MaxFctEval, DefaultMaxFctEval);
            if (iv < 0)
                return false;

            maxFctEval = iv;

            // Number of threads must be >= 1
            iv = GetIntParam(Parameters.Threads, DefaultThreads);
            if (iv < 1)
                return false;                

            threads = iv;

            // ReSharper disable once JoinDeclarationAndInitializer
            double dv;

            // Regularization costs must be >= 0
            dv = GetDoubleParam(Parameters.L1Cost, DefaultL1Cost);
            if (dv < 0)
                return false;

            l1Cost = dv;

            dv = GetDoubleParam(Parameters.L2Cost, DefaultL2Cost);
            if (dv < 0)
                return false;

            l2Cost = dv;

            return true;
        }

        #endregion

        /// <summary>
        /// Execute the training operation.
        /// </summary>
        /// <param name="indexer">The data indexer.</param>
        /// <returns>The trained <see cref="IMaxentModel"/> model.</returns>
        protected override IMaxentModel DoTrain(IDataIndexer indexer) {
            return TrainModel(Iterations, indexer);
        }

        /// <summary>
        /// Execute the training operation.
        /// </summary>
        /// <param name="iterations">The number of iterations.</param>
        /// <param name="indexer">The data indexer.</param>
        /// <returns>The trained <see cref="IMaxentModel" /> model.</returns>
        /// <exception cref="System.ArgumentOutOfRangeException">iterations</exception>
        /// <exception cref="System.ArgumentNullException">indexer</exception>
        /// <exception cref="System.InvalidOperationException">The number of threads is invalid.</exception>
        public QNModel TrainModel(int iterations, IDataIndexer indexer) {
            if (iterations < 0)
                throw new ArgumentOutOfRangeException(nameof(iterations));

            if (indexer == null)
                throw new ArgumentNullException(nameof(indexer));

            IFunction function;
            if (threads == 1) {
                Display("Computing model parameters ...");
                function = new NegLogLikelihood(indexer);
            } else if (threads > 1) {
                Display("Computing model parameters in " + threads + " threads ...");
                function = new ParallelNegLogLikelihood(indexer, threads);
            } else {
                throw new InvalidOperationException("The number of threads is invalid.");
            }

            if (!indexer.Completed)
                indexer.Execute();

            var minimizer = new QNMinimizer(l1Cost, l2Cost, iterations, updates, maxFctEval, Monitor) {
                Evaluator = new QNModelEvaluator(indexer)
            };

            // minimized parameters
            var mp = minimizer.Minimize(function);

            // construct model with trained parameters

            var predLabels = indexer.GetPredLabels();
            var nPredLabels = predLabels.Length;

            var outcomeNames = indexer.GetOutcomeLabels();
            var nOutcomes = outcomeNames.Length;

            var parameters = new Context[nPredLabels];
            for (var ci = 0; ci < parameters.Length; ci++) {
                var outcomePattern = new List<int>(nOutcomes);
                var alpha = new List<double>(nOutcomes);
                for (var oi = 0; oi < nOutcomes; oi++) {
                    var val = mp[oi*nPredLabels + ci];
                    outcomePattern.Add(oi);
                    alpha.Add(val);
                }
                parameters[ci] = new Context(outcomePattern.ToArray(), alpha.ToArray());
            }
            return new QNModel(parameters, predLabels, outcomeNames);
        }
    }
}