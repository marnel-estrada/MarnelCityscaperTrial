using Common.Signal;

using UnityEngine;

namespace Game {
    /// <summary>
    /// Used in conjunction with UI such that functions in this component could be hooked to events.
    /// </summary>
    public class SignalDispatcher : MonoBehaviour {
        [SerializeField]
        public string? signalToDispatch;

        private Signal? signal;

        private void Awake() {
            if (!string.IsNullOrEmpty(this.signalToDispatch)) {
                this.signal = GameSignals.GetSignal(this.signalToDispatch);
            }
        }

        /**
		 * Dispatches the specified signal
		 */
        public void DispatchSignal() {
            this.signal?.Dispatch();
        }

        /**
         * Dispatches a named signal
         * We provided this function so we could specify in UI to use this function instead of assigning new SignalDispatcher components
         */
        public void DispatchNamedSignal(string signalName) {
            GameSignals.GetSignal(signalName).Dispatch();
        }
    }
}