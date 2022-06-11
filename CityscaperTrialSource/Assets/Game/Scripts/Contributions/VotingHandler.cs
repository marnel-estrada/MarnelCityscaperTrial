using Common;

using TMPro;

using UnityEngine;

namespace Game {
    public class VotingHandler : MonoBehaviour {
        [SerializeField]
        private TMP_Text upVoteCountLabel;
        
        [SerializeField]
        private TMP_Text downVoteCountLabel;

        private Option<CommentTreeNode> commentNode;

        private void Awake() {
            Assertion.NotNull(this.upVoteCountLabel);
            Assertion.NotNull(this.downVoteCountLabel);
        }

        public void Init(CommentTreeNode commentNode) {
            this.commentNode = Option<CommentTreeNode>.Some(commentNode);
            Assertion.IsSome(this.commentNode);

            UpdateUpVoteDisplay();
            UpdateDownVoteDisplay();
        }

        // Used as button action
        public void UpVote() {
            Assertion.IsSome(this.commentNode);
            this.commentNode.ValueOrError().UpVote();
            UpdateUpVoteDisplay();
        }

        private void UpdateUpVoteDisplay() {
            Assertion.IsSome(this.commentNode);
            this.upVoteCountLabel.text = TextUtils.AsCommaSeparated(this.commentNode.ValueOrError().VotingPro);
        }

        // Used as button action
        public void DownVote() {
            Assertion.IsSome(this.commentNode);
            this.commentNode.ValueOrError().DownVote();
            UpdateDownVoteDisplay();
        }

        private void UpdateDownVoteDisplay() {
            Assertion.IsSome(this.commentNode);
            this.downVoteCountLabel.text = TextUtils.AsCommaSeparated(this.commentNode.ValueOrError().VotingContra);
        }
    }
}