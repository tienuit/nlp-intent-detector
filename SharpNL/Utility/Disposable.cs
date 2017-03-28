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

namespace SharpNL.Utility {
    /// <summary>
    /// Represents a disposable object.
    /// </summary>
    public abstract class Disposable : IDisposable {

        /// <summary>
        /// Occurs when the object is disposed.
        /// </summary>
        public event EventHandler<EventArgs> Disposed;

        #region . Finalizer .
        /// <summary>
        /// Releases unmanaged resources and performs other cleanup operations 
        /// before the <see cref="Disposable"/> is reclaimed by garbage collection.
        /// </summary>
        ~Disposable() {
            Dispose(false);
        }
        #endregion

        #region + Properties .

        #region . IsDisposed .
        /// <summary>
        /// Gets a value indicating whether this instance is disposed.
        /// </summary>
        /// <value><c>true</c> if this instance is disposed; otherwise, <c>false</c>.</value>
        protected bool IsDisposed { get; private set; }
        #endregion

        #endregion

        #region . CheckDisposed .
        /// <summary>
        /// Checks the if the object is disposed.
        /// </summary>
        /// <exception cref="System.ObjectDisposedException">The object is disposed.</exception>
        protected void CheckDisposed() {
            if (IsDisposed)
                throw new ObjectDisposedException(GetType().Namespace);
        }
        #endregion

        #region + Dispose .

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose() {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Releases unmanaged and - optionally - managed resources.
        /// </summary>
        /// <param name="disposing"><c>true</c> to release both managed and unmanaged resources; <c>false</c> to release only unmanaged resources.</param>
        protected virtual void Dispose(bool disposing) {
            if (IsDisposed)
                return;

            try {
                if (disposing)
                    DisposeManagedResources();

                DisposeUnmanagedResources();
            } finally {
                IsDisposed = true;

                Disposed?.Invoke(this, EventArgs.Empty);
            }
        }

        #endregion

        #region . DisposeManagedResources .
        /// <summary>
        /// Releases the managed resources.
        /// </summary>
        protected virtual void DisposeManagedResources() {
            
        }
        #endregion

        #region . DisposeUnmanagedResources .
        /// <summary>
        /// Releases the native resources.
        /// </summary>
        protected virtual void DisposeUnmanagedResources() {
            
        }
        #endregion

    }
}