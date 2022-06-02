using UnityEngine;

using Common;
using Common.Signal;

using System.Collections.Generic;

#nullable enable

namespace Game {
	/**
	 * Base class for panel handlers
	 */
	[RequireComponent(typeof(SignalIntegrityHandler))]
	public class GuiPanelHandler : MonoBehaviour {
		[SerializeField]
		private GameObject? rootView;

        // may be empty to signify that there's no input layer to push/pop
		[SerializeField]
		private string? inputLayerName;

		[SerializeField]
		private bool hiddenByDefault = true;

		// handles removal of signal listeners upon destruction
		private SignalIntegrityHandler? signalIntegrity;

		public virtual void Awake() {
			Assertion.NotNull(this.rootView);
            
			// cache
			this.signalIntegrity = this.GetRequiredComponent<SignalIntegrityHandler>();
		}

        private void Start() {
			if(this.hiddenByDefault) {
                // we hide one frame later to give chance for RectTransforms to finalize their positions
                // it seems that RectTransforms can't resolve their positions right away
                StartCoroutine(HideLater());
			} else {
                // We need to invoke Open() so that the opened flag would be set to true
                // Closing the panel will check this flag and might not close it if it remains closed (opened is false)
                StartCoroutine(OpenLater());
            }
        }

        private IEnumerator<float> HideLater() {
            yield return 0;

	        if (this.rootView != null) {
		        this.rootView.Deactivate();
	        }
        }

        private IEnumerator<float> OpenLater() {
            yield return 0;
            Open();
        }

		/**
		 * Sets the signal for opening the panel
		 */
		protected void SetOpenSignal(Signal signal) {
			AddSignalListener(signal, Open);
		}

		/**
		 * Sets the signal for closing the panel
		 */
		protected void SetCloseSignal(Signal signal) {
			AddSignalListener(signal, Close);
		}

		/**
		 * The safe way to add a listener to the signal
		 * Maintains itself
		 */
		public void AddSignalListener(Signal signal, Signal.SignalListener listener) {
			if (this.signalIntegrity != null) {
				this.signalIntegrity.Add(signal, listener);
			}
		}

        /// <summary>
        /// Another version of Open() that could be attached to a signal
        /// </summary>
        /// <param name="parameters"></param>
		public virtual void Open(ISignalParameters parameters) {
            Open();
		}

        /// <summary>
        /// Another version of Close() that could be attached to a signal
        /// </summary>
        /// <param name="parameters"></param>
        public virtual void Close(ISignalParameters parameters) {
			Close();
		}

        private bool opened;

        public bool Opened {
            get {
                return this.opened;
            }
        }

        /// <summary>
        /// Opens the panel
        /// </summary>
        public virtual void Open() {
	        if (this.rootView == null) {
		        // The root view has already been destroyed
		        return;
	        }
	        
            if(this.opened) {
                // already opened
                return;
            }

			this.rootView.Activate();

            if (!string.IsNullOrEmpty(this.inputLayerName)) {
                InputLayerManager.PushInputLayer(this.inputLayerName!);
            }

            this.opened = true;
        }

        /// <summary>
        /// Closes the panel
        /// </summary>
		public virtual void Close() {
            if(!this.opened) {
                // already closed
                return;
            }

            if (!string.IsNullOrEmpty(this.inputLayerName)) {
	            InputLayerManager.PopInputLayer(this.inputLayerName!);
            }

			this.rootView.Deactivate();
            this.opened = false;
		}
	}
}
