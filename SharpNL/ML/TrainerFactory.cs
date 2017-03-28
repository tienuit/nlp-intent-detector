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
using System.Collections.Generic;
using SharpNL.ML.MaxEntropy;
using SharpNL.ML.MaxEntropy.QuasiNewton;
using SharpNL.ML.Model;
using SharpNL.ML.NaiveBayes;
using SharpNL.ML.Perceptron;
using SharpNL.Utility;

namespace SharpNL.ML {
    /// <summary>
    /// Represents the trainer factory.
    /// </summary>
    public class TrainerFactory {
        private static readonly Dictionary<string, Type> builtInTrainers;
        private static readonly Dictionary<string, Type> customTrainers;

        static TrainerFactory() {
            builtInTrainers = new Dictionary<string, Type>();
            customTrainers = new Dictionary<string, Type>();

            builtInTrainers[Parameters.Algorithms.MaxEnt] = typeof(GIS);
            builtInTrainers[Parameters.Algorithms.MaxEntQn] = typeof(QNTrainer);
            builtInTrainers[Parameters.Algorithms.Perceptron] = typeof(PerceptronTrainer);
            builtInTrainers[Parameters.Algorithms.NaiveBayes] = typeof (NaiveBayesTrainer);
        }

        private static T CreateCustomTrainer<T>(string type, Monitor monitor) {
            if (customTrainers.ContainsKey(type)) {
                return (T)Activator.CreateInstance(customTrainers[type], monitor);
            }
            return default(T);
        }
        private static T CreateBuiltinTrainer<T> (string type, Monitor monitor) {
            if (builtInTrainers.ContainsKey(type)) {
                return (T) Activator.CreateInstance(builtInTrainers[type], monitor);
                
            }
            return default(T);
        }

        #region . GetEventTrainer .

        /// <summary>
        /// Gets the event trainer.
        /// </summary>
        /// <param name="parameters">The machine learnable parameters.</param>
        /// <param name="reportMap">The report map.</param>
        /// <param name="monitor">A evaluation monitor that can be used to listen the messages during the training or it can cancel the training operation.
        /// This argument can be a <c>null</c> value.</param>
        /// <returns>The <see cref="IEventTrainer" /> trainer object.</returns>
        /// <exception cref="System.InvalidOperationException">
        /// Unable to retrieve the trainer from the training parameters.
        /// or
        /// The constructor of the trainer must have a standard constructor.
        /// </exception>
        public static IEventTrainer GetEventTrainer(TrainingParameters parameters, Dictionary<string, string> reportMap, Monitor monitor) {

            var algorithm = parameters.Get(Parameters.Algorithm);

            if (algorithm == null) {
                AbstractEventTrainer trainer = new GIS(monitor);
                trainer.Init(parameters, reportMap);
                return trainer;
            }

            var trainerType = GetTrainerType(parameters);
            if (trainerType.HasValue && trainerType.Value == TrainerType.EventModelTrainer) {
                var type = GetTrainer(algorithm);

                if (type == null)
                    throw new InvalidOperationException("Unable to retrieve the trainer from the training parameters.");

                var ctor = type.GetConstructor(new [] {typeof (Monitor)});
                if (ctor == null)
                    throw new InvalidOperationException("The constructor of the trainer must have a standard constructor.");

                var trainer = (IEventTrainer) ctor.Invoke(new object[] {monitor});
                trainer.Init(parameters, reportMap);
                return trainer;
            }

            return null;
        }

        #endregion

        #region . GetEventModelSequenceTrainer .

        /// <summary>
        /// Gets the event model sequence trainer.
        /// </summary>
        /// <param name="parameters">The machine learnable parameters.</param>
        /// <param name="reportMap">The report map.</param>
        /// <param name="monitor">
        /// A evaluation monitor that can be used to listen the messages during the training or it can cancel the training operation.
        /// This argument can be a <c>null</c> value.
        /// </param>
        /// <returns>The <see cref="IEventModelSequenceTrainer"/> trainer object.</returns>
        /// <exception cref="System.InvalidOperationException">Trainer type couldn't be determined!</exception>
        public static IEventModelSequenceTrainer GetEventModelSequenceTrainer(TrainingParameters parameters, Dictionary<string, string> reportMap, Monitor monitor) {

            var trainerType = parameters.Get(Parameters.Algorithm);
            if (!string.IsNullOrEmpty(trainerType)) {
                if (builtInTrainers.ContainsKey(trainerType)) {
                    var trainer = CreateBuiltinTrainer<IEventModelSequenceTrainer>(trainerType, monitor);
                    trainer.Init(parameters, reportMap);
                    return trainer;
                }

                if (customTrainers.ContainsKey(trainerType)) {
                    var type = customTrainers[trainerType];
                    var trainer2 = (IEventModelSequenceTrainer)Activator.CreateInstance(type, monitor);
                    trainer2.Init(parameters, reportMap);
                    return trainer2;
                }
            }

            throw new InvalidOperationException("Trainer type couldn't be determined!");
        }

        #endregion

        #region . GetSequenceModelTrainer .

        /// <summary>
        /// Gets the sequence model trainer.
        /// </summary>
        /// <param name="parameters">The machine learnable parameters.</param>
        /// <param name="reportMap">The report map.</param>
        /// <param name="monitor">
        /// A evaluation monitor that can be used to listen the messages during the training or it can cancel the training operation.
        /// This argument can be a <c>null</c> value.
        /// </param>
        /// <returns>The <see cref="ISequenceTrainer"/> trainer object.</returns>
        /// <exception cref="System.InvalidOperationException">Trainer type couldn't be determined!</exception>
        public static ISequenceTrainer GetSequenceModelTrainer(TrainingParameters parameters, Dictionary<string, string> reportMap, Monitor monitor) {

            var trainerType = parameters.Get(Parameters.Algorithm);

            ISequenceTrainer trainer = null;

            if (trainerType != null) {
                if (builtInTrainers.ContainsKey(trainerType)) {
                    trainer = CreateBuiltinTrainer<ISequenceTrainer>(trainerType, monitor);
                }
                if (customTrainers.ContainsKey(trainerType)) {
                    trainer = CreateCustomTrainer<ISequenceTrainer>(trainerType, monitor);
                }
            } 

            if (trainer == null) {
                throw new InvalidOperationException("Trainer type couldn't be determined!");
            }

            trainer.Init(parameters, reportMap);
            return trainer;
        }
        #endregion

        #region . GetTrainer .
        internal static Type GetTrainer(string algorithm) {
            if (builtInTrainers.ContainsKey(algorithm)) {
                return builtInTrainers[algorithm];
            }
            if (customTrainers.ContainsKey(algorithm)) {
                return customTrainers[algorithm];
            }
            return null;
        }
        #endregion

        #region . GetTrainerType .

        /// <summary>
        /// Gets the type of the trainer from the <see cref="TrainingParameters"/> object.
        /// </summary>
        /// <param name="trainParams">The train parameters.</param>
        /// <returns>A nullable <see cref="TrainerType"/> value.</returns>
        public static TrainerType? GetTrainerType(TrainingParameters trainParams) {

            var algorithm = trainParams.Get(Parameters.Algorithm);

            if (algorithm == null) {
                return TrainerType.EventModelTrainer;
            }

            Type trainerType = null;

            if (builtInTrainers.ContainsKey(algorithm)) {
                trainerType = builtInTrainers[algorithm];
            } else if (customTrainers.ContainsKey(algorithm)) {
                trainerType = customTrainers[algorithm];
            }

            return GetTrainerType(trainerType);
        }

        private static TrainerType? GetTrainerType(Type trainerType) {
            if (trainerType == null) 
                return null;

            if (typeof(IEventTrainer).IsAssignableFrom(trainerType)) {
                return TrainerType.EventModelTrainer;
            }

            if (typeof(IEventModelSequenceTrainer).IsAssignableFrom(trainerType)) {
                return TrainerType.EventModelSequenceTrainer;
            }

            if (typeof(ISequenceTrainer).IsAssignableFrom(trainerType)) {
                return TrainerType.SequenceTrainer;
            }

            return null;
        }


        #endregion

        #region . IsValid .

        /// <summary>
        /// Determines whether the specified train parameters are valid.
        /// </summary>
        /// <param name="trainParams">The train parameters.</param>
        /// <returns><c>true</c> if the specified train parameters are valid; otherwise, <c>false</c>.</returns>
        public static bool IsValid(TrainingParameters trainParams) {

            if (!trainParams.IsValid()) {
                return false;
            }

            var algorithmName = trainParams.Get(Parameters.Algorithm);
            if (!(builtInTrainers.ContainsKey(algorithmName) || GetTrainerType(trainParams) != null)) {
                return false;
            }

            var dataIndexer = trainParams.Get(Parameters.DataIndexer);
            if (dataIndexer != null)
                switch (dataIndexer) {
                    case Parameters.DataIndexers.OnePass:
                    case Parameters.DataIndexers.TwoPass:
                        break;
                    default:
                        return false;
                }

            return true;
        }
        #endregion

        #region . RegisterTrainer .
        /// <summary>
        /// Registers a custom trainer with the given name.
        /// </summary>
        /// <param name="trainerName">Name of the trainer.</param>
        /// <param name="trainerType">The trainer type.</param>
        /// <exception cref="System.ArgumentNullException">trainerName
        /// or
        /// trainerType</exception>
        /// <exception cref="System.ArgumentException">The specified trainer name is an built in trainer.</exception>
        /// <exception cref="System.ArgumentException">The specified trainer name is already registered.</exception>
        /// <exception cref="System.InvalidOperationException">The specified trainer type does not implement an valid interface.</exception>
        public static void RegisterTrainer(string trainerName, Type trainerType) {
            if (string.IsNullOrEmpty(trainerName)) {
                throw new ArgumentNullException(nameof(trainerName));
            }
            if (trainerType == null) {
                throw new ArgumentNullException(nameof(trainerType));
            }

            if (builtInTrainers.ContainsKey(trainerName)) {
                throw new ArgumentException(@"The specified trainer name is an built in trainer.", nameof(trainerName));
            }

            if (customTrainers.ContainsKey(trainerName)) {
                throw new ArgumentException(@"The specified trainer name is already registered.", nameof(trainerName));
            }

            TrainerType? type = GetTrainerType(trainerType);

            if (!type.HasValue) {
                throw new InvalidOperationException("The specified trainer type does not implement an valid interface.");
            }

            customTrainers.Add(trainerName, trainerType);
        }

        #endregion

    }
}