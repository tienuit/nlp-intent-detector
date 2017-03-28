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

using System.Collections.Generic;
using SharpNL.Extensions;
using SharpNL.ML.Model;

namespace SharpNL.ML.MaxEntropy.IO {
    /// <summary>
    /// Generalized Iterative Scaling model writer.
    /// </summary>
    public abstract class GISModelWriter : AbstractModelWriter {
        protected int CorrectionConstant;
        protected double CorrectionParam;
        protected string[] OutcomeLabels;
        protected Context[] Parameters;
        protected string[] PredLabels;

        /// <para>
        ///   <c>index 0</c>: <see cref="T:Context[]"/> containing the model parameters.
        ///   <c>index 1</c>: <see cref="T:IndexHashTable{string}"/> containing the mapping of model predicates to unique integers.
        ///   <c>index 2</c>: <see cref="T:string[]"/> containing the names of the outcomes, stored in the index of the array which represents their unique ids in the model.
        ///   <c>index 3</c>: <see cref="T:double"/> containing the value of the models correction constant.
        ///   <c>index 4</c>: <see cref="T:double"/> containing the value of the models correction parameter.
        /// </para>
        /// 
        protected GISModelWriter(AbstractModel model) {
            var data = model.GetDataStructures();

            Parameters = (Context[]) data[0];
            var map = (IndexHashTable<string>) data[1];
            OutcomeLabels = (string[]) data[2];
            CorrectionConstant = (int) data[3];
            CorrectionParam = (double) data[4];

            PredLabels = map.ToArray();
        }

        #region . CompressOutcomes .

        /// <summary>
        /// Compresses the outcomes.
        /// </summary>
        /// <param name="sorted">The sorted predicates.</param>
        /// <returns>A list of compressed predicates.</returns>
        protected List<List<ComparablePredicate>> CompressOutcomes(ComparablePredicate[] sorted) {
            var outcomePatterns = new List<List<ComparablePredicate>>();

            if (sorted.Length == 0)
                return outcomePatterns;

            var cp = sorted[0];
            var newGroup = new List<ComparablePredicate>();
            foreach (var t in sorted) {
                if (cp.CompareTo(t) == 0) {
                    newGroup.Add(t);
                } else {
                    cp = t;
                    outcomePatterns.Add(newGroup);
                    newGroup = new List<ComparablePredicate> {t};
                }
            }
            outcomePatterns.Add(newGroup);
            return outcomePatterns;
        }

        #endregion


        /// <summary>
        /// Persists this instance.
        /// </summary>
        public override void Persist() {
            // the type of model (GIS)
            Write("GIS");

            // the value of the correction constant
            Write(CorrectionConstant);

            // the value of the correction constant
            Write(CorrectionParam);

            // the mapping from outcomes to their integer indexes
            Write(OutcomeLabels.Length);

            foreach (var label in OutcomeLabels)
                Write(label);

            // the mapping from predicates to the outcomes they contributed to.
            // The sorting is done so that we actually can write this out more
            // compactly than as the entire list.
            var sorted = SortValues();
            var compressed = CompressOutcomes(sorted);

            Write(compressed.Count);

            foreach (var a in compressed) 
                Write(a.Count + a[0].ToString());

            // the mapping from predicate names to their integer indexes
            Write(Parameters.Length);

            foreach (var cp in sorted)
                Write(cp.Name);

            // write out the parameters
            foreach (var cp in sorted)
                foreach (var p in cp.Parameters)
                    Write(p);

            Close();
        }

        #region . SortValues .

        protected ComparablePredicate[] SortValues() {
            var sortPreds = new ComparablePredicate[Parameters.Length];
            for (var pid = 0; pid < Parameters.Length; pid++) {
                sortPreds[pid] = new ComparablePredicate(
                    PredLabels[pid],
                    Parameters[pid].Outcomes,
                    Parameters[pid].Parameters);
            }

            //
            // Knuppe 2015-10-28:
            // - We can not use the Array.Sort because it uses QuickSort algorithm in the older versions of the .net framework.
            // - To get everything working as the java version I had to implement the MergeSort algorithm in order to keep the
            // - same sort behavior.
            //
            // Array.Sort(sortPreds); works on .net45 or newer
            //
            // References:
            // https://msdn.microsoft.com/en-us/library/system.array.sort(v=vs.110).aspx
            // http://docs.oracle.com/javase/7/docs/api/java/util/Arrays.html#sort(T[],%20java.util.Comparator)
            //
            
            sortPreds.MergeSort();

            return sortPreds;
        }

        #endregion

    }
}