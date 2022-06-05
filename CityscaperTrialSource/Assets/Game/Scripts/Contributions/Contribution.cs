using System;

using Common;

namespace Game {
    /// <summary>
    /// The contribution data
    /// </summary>
    public class Contribution : CommentTreeNode {
        private string belongToProject;
        private string author;

        private string link;

        private string title;

        private float latitude;
        private float longitude;

        private ContributionType contributionType;

        private string contributionContent;

        private Status status;

        private string category;
        private string subCategory;

        private string keywordSuggested;
        private string keywordPicked;

        private string sentiment;

        private string customAttribute;

        private bool dipasLocated;

        public Contribution(string id, DateTime timestamp, string belongToProject, string author) : base(Option<CommentTreeNode>.NONE, id, timestamp) {
            this.belongToProject = belongToProject;
            this.author = author;
        }

        [Persist]
        public string BelongToProject {
            get {
                return this.belongToProject;
            }

            set {
                this.belongToProject = value;
            }
        }

        [Persist]
        public string Author {
            get {
                return this.author;
            }

            set {
                this.author = value;
            }
        }

        public string Link {
            get {
                return this.link;
            }
            set {
                this.link = value;
            }
        }

        [Persist]
        public string Title {
            get {
                return this.title;
            }
            set {
                this.title = value;
            }
        }

        [Persist]
        public float Latitude {
            get {
                return this.latitude;
            }
            set {
                this.latitude = value;
            }
        }

        [Persist]
        public float Longitude {
            get {
                return this.longitude;
            }
            set {
                this.longitude = value;
            }
        }

        [Persist]
        public ContributionType ContributionType {
            get {
                return this.contributionType;
            }
            set {
                this.contributionType = value;
            }
        }

        [Persist]
        public string ContributionContent {
            get {
                return this.contributionContent;
            }
            set {
                this.contributionContent = value;
            }
        }

        [Persist]
        public Status Status {
            get {
                return this.status;
            }
            set {
                this.status = value;
            }
        }

        public int CommentsNumber {
            get {
                return this.ChildrenCount;
            }
        }

        public string Category {
            get {
                return this.category;
            }
            set {
                this.category = value;
            }
        }

        public string SubCategory {
            get {
                return this.subCategory;
            }
            set {
                this.subCategory = value;
            }
        }

        public string KeywordSuggested {
            get {
                return this.keywordSuggested;
            }
            set {
                this.keywordSuggested = value;
            }
        }

        public string KeywordPicked {
            get {
                return this.keywordPicked;
            }
            set {
                this.keywordPicked = value;
            }
        }

        public string Sentiment {
            get {
                return this.sentiment;
            }
            set {
                this.sentiment = value;
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

        public bool DipasLocated {
            get {
                return this.dipasLocated;
            }
            set {
                this.dipasLocated = value;
            }
        }
    }
}