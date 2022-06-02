using System.Collections.Generic;

namespace Game {
    /// <summary>
    /// The comment data
    /// </summary>
    public class Comment {
        private readonly string id;
        private readonly string dateCreated;

        private string link;
        private string title;
        
        private float latitude;
        private float longitude;

        private string attachment;

        private ContributionType contributionType;

        private string commentContent;
        
        private int votingPro;
        private int votingContra;

        private string commentOnContribution;
        private string commentOnComment;

        private readonly List<Comment> commentedBy = new List<Comment>();

        private string customAttribute;

        public string ID {
            get {
                return this.id;
            }
        }

        public string DateCreated {
            get {
                return this.dateCreated;
            }
        }

        public string Title {
            get {
                return this.title;
            }
            set {
                this.title = value;
            }
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

        public ContributionType ContributionType {
            get {
                return this.contributionType;
            }
            set {
                this.contributionType = value;
            }
        }

        public string CommentContent {
            get {
                return this.commentContent;
            }
            set {
                this.commentContent = value;
            }
        }

        public int VotingPro {
            get {
                return this.votingPro;
            }
            set {
                this.votingPro = value;
            }
        }

        public int VotingContra {
            get {
                return this.votingContra;
            }
            set {
                this.votingContra = value;
            }
        }

        public string CommentOnContribution {
            get {
                return this.commentOnContribution;
            }
            set {
                this.commentOnContribution = value;
            }
        }

        public string CommentOnComment {
            get {
                return this.commentOnComment;
            }
            set {
                this.commentOnComment = value;
            }
        }

        public void AddComment(Comment comment) {
            this.commentedBy.Add(comment);
        }

        public string CustomAttribute {
            get {
                return this.customAttribute;
            }
            set {
                this.customAttribute = value;
            }
        }

        public Comment(string id, string dateCreated) {
            this.id = id;
            this.dateCreated = dateCreated;
        }

        public string Link {
            get {
                return this.link;
            }
        }
    }
}