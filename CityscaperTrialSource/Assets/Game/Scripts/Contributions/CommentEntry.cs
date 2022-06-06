using Common;
using Common.Signal;

using TMPro;

using UnityEngine;

namespace Game {
    [RequireComponent(typeof(SwarmItem))]
    public class CommentEntry : MonoBehaviour {
        [SerializeField]
        private TMP_Text timestampLabel;
        
        [SerializeField]
        private TMP_Text titleLabel;
        
        [SerializeField]
        private TMP_Text contentLabel;
        
        [SerializeField]
        private TMP_Text seeMoreCommentsButtonLabel;

        private SwarmItem swarmItem;

        private Option<Contribution> contribution;
        private Option<CommentTreeNode> commentNode;

        private void Awake() {
            Assertion.NotNull(this.timestampLabel);
            Assertion.NotNull(this.titleLabel);
            Assertion.NotNull(this.contentLabel);
            Assertion.NotNull(this.seeMoreCommentsButtonLabel);

            this.swarmItem = this.GetRequiredComponent<SwarmItem>();
        }

        public void Init(Contribution contribution, CommentTreeNode comment) {
            this.contribution = Option<Contribution>.Some(contribution);
            this.commentNode = Option<CommentTreeNode>.Some(comment);
            
            // Update display
            this.timestampLabel.text = comment.DateCreated;
            this.titleLabel.text = comment.Title;
            this.contentLabel.text = comment.CommentContent;

            string commentCountText = TextUtils.AsCommaSeparated(comment.ChildrenCount);
            this.seeMoreCommentsButtonLabel.text = $"See more comments ({commentCountText})...";
        }

        public void Recycle() {
            this.swarmItem.Recycle();
            this.commentNode = Option<CommentTreeNode>.NONE;
        }

        // Used as a button action
        public void OpenCommentTree() {
            Assertion.IsSome(this.contribution);
            Assertion.IsSome(this.commentNode);
            
            Signal signal = GameSignals.OPEN_COMMENT_TREE_PANEL;
            signal.ClearParameters();
            signal.AddParameter(Params.CONTRIBUTION, this.contribution.ValueOrError());
            signal.AddParameter(Params.COMMENT_NODE, this.commentNode.ValueOrError());
            signal.Dispatch();
        }
    }
}