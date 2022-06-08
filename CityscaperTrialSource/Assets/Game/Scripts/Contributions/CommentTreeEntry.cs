using Common;
using Common.Signal;

using TMPro;

using UnityEngine;
using UnityEngine.UI;

namespace Game {
    [RequireComponent(typeof(SwarmItem))]
    public class CommentTreeEntry : MonoBehaviour {
        [SerializeField]
        private LayoutElement depthLayoutElement;
        
        [SerializeField]
        private TMP_Text timestampLabel;
        
        [SerializeField]
        private TMP_Text titleLabel;
        
        [SerializeField]
        private TMP_Text contentLabel;
        
        [SerializeField]
        private TMP_Text upVoteLabel;
        
        [SerializeField]
        private TMP_Text downVoteLabel;
        
        [SerializeField]
        private Button commentButton;

        private Option<CommentTreeNode> commentNode;

        private SwarmItem swarmItem;

        private void Awake() {
            Assertion.NotNull(this.depthLayoutElement);
            Assertion.NotNull(this.timestampLabel);
            Assertion.NotNull(this.titleLabel);
            Assertion.NotNull(this.contentLabel);
            Assertion.NotNull(this.upVoteLabel);
            Assertion.NotNull(this.downVoteLabel);
            Assertion.NotNull(this.commentButton);

            this.swarmItem = this.GetRequiredComponent<SwarmItem>();
        }

        public void Init(CommentTreeNode commentNode) {
            this.commentNode = Option<CommentTreeNode>.Some(commentNode);
            Assertion.IsSome(this.commentNode);
            UpdateDisplay();
        }

        private const int WIDTH_SPACE_PER_DEPTH = 20;

        private void UpdateDisplay() {
            Assertion.IsSome(this.commentNode);
            CommentTreeNode node = this.commentNode.ValueOrError();
            
            // Update the horizontal space of the comment
            // We minus one here because the root comment (which is the contribution) has a depth of 1
            this.depthLayoutElement.minWidth = (node.Depth - 1) * WIDTH_SPACE_PER_DEPTH;

            this.timestampLabel.text = node.DateCreated;
            this.titleLabel.text = node.Title;
            this.contentLabel.text = node.CommentContent;
            UpdateUpvoteDisplay(node);
            UpdateDownvoteDisplay(node);
        }

        private void UpdateUpvoteDisplay(CommentTreeNode node) {
            this.upVoteLabel.text = TextUtils.AsCommaSeparated(node.VotingPro);
        }

        private void UpdateDownvoteDisplay(CommentTreeNode node) {
            this.downVoteLabel.text = TextUtils.AsCommaSeparated(node.VotingContra);
        }

        // Used as button action
        public void Upvote() {
            Assertion.IsSome(this.commentNode);
            CommentTreeNode node = this.commentNode.ValueOrError();
            ++node.VotingPro;
            UpdateUpvoteDisplay(node);
        }

        // Used as button action
        public void Downvote() {
            Assertion.IsSome(this.commentNode);
            CommentTreeNode node = this.commentNode.ValueOrError();
            ++node.VotingContra;
            UpdateDownvoteDisplay(node);
        }

        // Used as button action
        public void Recycle() {
            // Always show the button on recycle so it's shown when the instance is activated again
            ShowCommentButton();
            
            this.swarmItem.Recycle();
        }

        // Used as button action
        public void OpenNewCommentPanel() {
            Assertion.IsSome(this.commentNode);
            
            Signal signal = GameSignals.OPEN_NEW_COMMENT_PANEL;
            signal.ClearParameters();
            signal.AddParameter(Params.PARENT_COMMENT, this.commentNode.ValueOrError());
            signal.Dispatch();
        }

        public void HideCommentButton() {
            this.commentButton.gameObject.Deactivate();
        }

        public void ShowCommentButton() {
            this.commentButton.gameObject.Activate();
        }
    }
}