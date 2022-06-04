using Common;
using Common.Signal;

using TMPro;

using UnityEngine;

namespace Game {
    public class ModalMessageHandler : MonoBehaviour {
        [SerializeField]
        private TMP_Text headerLabel;
        
        [SerializeField]
        private TMP_Text bodyLabel;

        private void Awake() {
            Assertion.NotNull(this.headerLabel);
            Assertion.NotNull(this.bodyLabel);
            
            GameSignals.OPEN_MODAL_MESSAGE.AddListener(OnOpen);
        }

        private void OnOpen(ISignalParameters parameters) {
            Option<string> header = parameters.GetParameter<string>(Params.HEADER);
            Assertion.IsSome(header);
            this.headerLabel.text = header.ValueOrError();
            
            Option<string> body = parameters.GetParameter<string>(Params.BODY);
            Assertion.IsSome(body);
            this.bodyLabel.text = body.ValueOrError();
        }

        /// <summary>
        /// A utility method that opens the modal message
        /// </summary>
        /// <param name="header"></param>
        /// <param name="body"></param>
        public static void Open(string header, string body) {
            Signal signal = GameSignals.OPEN_MODAL_MESSAGE;
            signal.ClearParameters();
            signal.AddParameter(Params.HEADER, header);
            signal.AddParameter(Params.BODY, body);
            signal.Dispatch();
        }
    }
}