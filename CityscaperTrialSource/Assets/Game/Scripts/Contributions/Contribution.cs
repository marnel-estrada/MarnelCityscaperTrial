using System;
using System.Collections.Generic;

namespace Game {
    /// <summary>
    /// The contribution data
    /// </summary>
    public class Contribution {
        private readonly string id;
        private readonly DateTime timestamp; // The actual timestamp so we can sort them
        private readonly string dateCreated;
        private readonly string belongToProject;
        private readonly string author;

        private string link;

        private string title;

        private float latitude;
        private float longitude;

        private ContributionType contributionType;

        private string contributionContent;

        private Status status;

        private string category;
        private string subCategory;

        private int votingPro;
        private int votingContra;

        private string keywordSuggested;
        private string keywordPicked;

        private string sentiment;

        private string customAttribute;

        private readonly List<Comment> commentedBy = new List<Comment>();

        private bool dipasLocated;

        public Contribution(string id, string dateCreated, string belongToProject, string author) {
            this.id = id;
            this.dateCreated = dateCreated;
            this.belongToProject = belongToProject;
            this.author = author;
        }
        
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

        public string BelongToProject {
            get {
                return this.belongToProject;
            }
        }

        public string Author {
            get {
                return this.author;
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

        public ContributionType ContributionType {
            get {
                return this.contributionType;
            }
            set {
                this.contributionType = value;
            }
        }

        public string ContributionContent {
            get {
                return this.contributionContent;
            }
            set {
                this.contributionContent = value;
            }
        }

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
                return this.commentedBy.Count;
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
        
        public void AddComment(Comment comment) {
            this.commentedBy.Add(comment);
        }
    }
}