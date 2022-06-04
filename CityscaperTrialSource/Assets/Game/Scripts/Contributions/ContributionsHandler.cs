using Common;
using Common.Signal;

using TMPro;

using UnityEngine;

namespace Game {
    public class ContributionsHandler : SignalHandlerComponent {
        [SerializeField]
        private TMP_Text headerLabel;

        private Option<ContributionSet> currentContributionSet;

        protected override void Awake() {
            base.Awake();
            
            Assertion.NotNull(this.headerLabel);
            
            AddSignalListener(GameSignals.OPEN_CONTRIBUTIONS_PANEL, OnOpen);
            
            GameSignals.ADD_NEW_CONTRIBUTION.AddListener(AddContribution);
        }

        private void OnOpen(ISignalParameters parameters) {
            Option<string> objectId = parameters.GetParameter<string>(Params.OBJECT_ID);
            Assertion.IsSome(objectId);
            objectId.Match(new SetHeaderLabelMatcher(this.headerLabel));

            this.currentContributionSet = parameters.GetParameter<ContributionSet>(Params.CONTRIBUTION_SET);
            Assertion.IsSome(this.currentContributionSet);
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

        private void AddContribution(AddNewContribution param) {
            Assertion.IsSome(this.currentContributionSet);
            Debug.Log($"Add Contribution: {param.title}");
        }
    }
}