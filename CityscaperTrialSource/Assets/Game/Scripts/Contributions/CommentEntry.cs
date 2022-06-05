using Common;

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

        private Option<CommentTreeNode> commentNode;

        private void Awake() {
            Assertion.NotNull(this.timestampLabel);
            Assertion.NotNull(this.titleLabel);
            Assertion.NotNull(this.contentLabel);
            Assertion.NotNull(this.seeMoreCommentsButtonLabel);

            this.swarmItem = this.GetRequiredComponent<SwarmItem>();
        }

        public void Init(CommentTreeNode comment) {
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
    }
}