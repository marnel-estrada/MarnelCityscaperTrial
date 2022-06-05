using Common;
using Common.Signal;

using TMPro;

using UnityEngine;

namespace Game {
    public class ContributionDetailHandler : SignalHandlerComponent {
        [SerializeField]
        private TMP_Text headerLabel; // For the title
        
        [SerializeField]
        private TMP_Text authorLabel;

        [SerializeField]
        private TMP_Text contentText;

        [SerializeField]
        private TMP_Text commentCountLabel;

        private Option<Contribution> currentContribution;

        protected override void Awake() {
            base.Awake();
            
            Assertion.NotNull(this.headerLabel);
            Assertion.NotNull(this.authorLabel);
            Assertion.NotNull(this.contentText);
            Assertion.NotNull(this.commentCountLabel);
            
            AddSignalListener(GameSignals.OPEN_CONTRIBUTION_DETAIL, OnOpen);
        }

        private void OnOpen(ISignalParameters parameters) {
            this.currentContribution = parameters.GetParameter<Contribution>(Params.CONTRIBUTION);
            Assertion.IsSome(this.currentContribution);
            
            UpdateDisplay(this.currentContribution.ValueOrError());
        }

        private void UpdateDisplay(Contribution contribution) {
            this.headerLabel.text = $"Contribution: {contribution.Title}";
            this.authorLabel.text = contribution.Author;
            this.contentText.text = contribution.ContributionContent;
            this.commentCountLabel.text = $"{TextUtils.AsCommaSeparated(contribution.CommentsNumber)} Comments";
        }
    }
}