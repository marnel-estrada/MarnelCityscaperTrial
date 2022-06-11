using Common;
using Common.Signal;

using TMPro;

using UnityEngine;

namespace Game {
    [RequireComponent(typeof(SwarmItem))]
    public class ContributionEntryRow : MonoBehaviour {
        [SerializeField]
        private TMP_Text titleLabel;
        
        [SerializeField]
        private TMP_Text contentLabel;

        [SerializeField]
        private VotingHandler votingHandler;

        private SwarmItem swarmItem;

        private Option<Contribution> contribution;

        private void Awake() {
            Assertion.NotNull(this.titleLabel);
            Assertion.NotNull(this.contentLabel);
            Assertion.NotNull(this.votingHandler);

            this.swarmItem = this.GetRequiredComponent<SwarmItem>();
        }

        public void Init(Contribution contribution) {
            this.contribution = Option<Contribution>.Some(contribution);
            
            this.titleLabel.text = contribution.Title;
            this.contentLabel.text = contribution.ContributionContent;
            
            this.votingHandler.Init(contribution);
        }

        public void Recycle() {
            this.swarmItem.Recycle();
        }

        public Contribution Contribution {
            get {
                Assertion.IsSome(this.contribution);
                return this.contribution.ValueOrError();
            }
        }

        /// <summary>
        /// Used as a button action
        /// </summary>
        public void ShowDetails() {
            Signal signal = GameSignals.OPEN_CONTRIBUTION_DETAIL;
            signal.ClearParameters();
            signal.AddParameter(Params.CONTRIBUTION, this.Contribution);
            signal.Dispatch();
        }
    }
}