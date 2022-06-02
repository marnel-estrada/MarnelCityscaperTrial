using Common;
using Common.Signal;

using TMPro;

using UnityEngine;

namespace Game {
    public class ContributionsHandler : SignalHandlerComponent {
        [SerializeField]
        private TMP_Text headerLabel;

        protected override void Awake() {
            base.Awake();
            
            Assertion.NotNull(this.headerLabel);
            
            AddSignalListener(GameSignals.OPEN_CONTRIBUTIONS_PANEL, OnOpen);
        }

        private void OnOpen(ISignalParameters parameters) {
            Option<string> objectId = parameters.GetParameter<string>(Params.OBJECT_ID);
            Assertion.IsSome(objectId);
            objectId.Match(new SetHeaderLabelMatcher(this.headerLabel));
        }
        
        private readonly struct SetHeaderLabelMatcher : IOptionMatcher<string> {
            private readonly TMP_Text headerLabel;

            public SetHeaderLabelMatcher(TMP_Text headerLabel) {
                this.headerLabel = headerLabel;
            }

            public void OnSome(string objectId) {
                this.headerLabel.text = $"Contributions: {objectId}";
            }

            public void OnNone() {
            }
        }
    }
}