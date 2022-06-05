using System;

using Common;

namespace Game {
    /// <summary>
    /// The comment data
    /// </summary>
    public class Comment : CommentTreeNode {
        private string link;
        
        private float latitude;
        private float longitude;

        private string attachment;

        private string commentOnContribution;
        private string commentOnComment;

        private string customAttribute;

        /// <summary>
        /// Constructor with specified parent
        /// Every comment has a parent node whether a contribution or another comment.
        /// </summary>
        /// <param name="parent"></param>
        public Comment(CommentTreeNode parent, string id, DateTime timestamp) : base(Option<CommentTreeNode>.Some(parent), id, timestamp) {
        }

        public float Latitude {
            get {
                return this.latitude;
            }
            set {
                this.latitude = value;
            }
        }

        public float Longitude {
            get {
                return this.longitude;
            }
            set {
                this.longitude = value;
            }
        }

        public string Attachment {
            get {
                return this.attachment;
            }
            set {
                this.attachment = value;
            }
        }

        [Persist]
        public string CommentOnContribution {
            get {
                return this.commentOnContribution;
            }
            set {
                this.commentOnContribution = value;
            }
        }

        [Persist]
        public string CommentOnComment {
            get {
                return this.commentOnComment;
            }
            set {
                this.commentOnComment = value;
            }
        }

        public string CustomAttribute {
            get {
                return this.customAttribute;
            }
            set {
                this.customAttribute = value;
            }
        }

        public string Link {
            get {
                return this.link;
            }
        }
    }
}