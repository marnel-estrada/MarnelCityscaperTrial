using System.Collections.Generic;
using Common;
using Common.Signal;
using UnityEngine;

namespace Game {
    public static class GameSignals {
        public static readonly Signal OPEN_CONTRIBUTIONS_PANEL = new Signal("OpenContributionsPanel");
        public static readonly Signal CLOSE_CONTRIBUTIONS_PANEL = new Signal("CloseContributionsPanel");

        // Only add those signals that needs string mapping
        private static void PopulateStringToSignalMapping() {
            AddStringMapping(OPEN_CONTRIBUTIONS_PANEL);
            AddStringMapping(CLOSE_CONTRIBUTIONS_PANEL);
        }

        private static void AddStringMapping(Signal signal) {
            Assertion.IsTrue(!STRING_TO_SIGNAL_MAPPING.ContainsKey(signal.Name), signal.Name); // signal should not be in the mapping yet
            STRING_TO_SIGNAL_MAPPING[signal.Name] = signal;
        }

        private static readonly Dictionary<string, Signal> STRING_TO_SIGNAL_MAPPING = new Dictionary<string, Signal>();

        // always use this function to get the instance to the mapping
        private static Dictionary<string, Signal> StringToSignalMapping {
            get {
                if (STRING_TO_SIGNAL_MAPPING.Count == 0) {
                    PopulateStringToSignalMapping();
                }

                return STRING_TO_SIGNAL_MAPPING;
            }
        }

        /// <summary>
        /// Dispatches a signal by name
        /// </summary>
        /// <param name="signalName"></param>
        public static void Dispatch(string signalName) {
            Assertion.IsTrue(StringToSignalMapping.ContainsKey(signalName), "String mapping of signal can't be found: " + signalName); // there should be signal mapping; add mapping in PopulateStringToSignalMapping()
            StringToSignalMapping[signalName].Dispatch();
        }

        /// <summary>
        /// Returns whether or not there's a mapping of the specified signal name.
        /// </summary>
        /// <param name="signalName"></param>
        /// <returns></returns>
        public static bool HasSignal(string? signalName) {
            if (string.IsNullOrEmpty(signalName)) {
                throw new CantBeNullException(nameof(signalName));
            }

            return StringToSignalMapping.ContainsKey(signalName);
        }

        /**
		 * Returns the signal with the specified name
		 */
        public static Signal GetSignal(string? signalName) {
            if (string.IsNullOrEmpty(signalName)) {
                throw new CantBeNullException(nameof(signalName));
            }

            // This will throw an error if the key is not found
            return StringToSignalMapping[signalName];
        }

        private static readonly StaticFieldsInvoker CLEAR_LISTENERS = new StaticFieldsInvoker(typeof(GameSignals), "ClearListeners");

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        private static void ResetStatic() {
            STRING_TO_SIGNAL_MAPPING.Clear();
            CLEAR_LISTENERS.Execute();
        }
    }
}