using System;
using System.Collections.Generic;

using Common;

namespace Game {
    /// <summary>
    /// Base class for a comment tree node.
    /// </summary>
    public class CommentTreeNode {
        private readonly Option<CommentTreeNode> parent; // The parent of the node
        
        private string id;
        private DateTime timestamp;

        private readonly List<CommentTreeNode> children = new List<CommentTreeNode>(0);
        
        private string title;
        private string commentContent;
        private ContributionType contributionType;
        
        private int votingPro;
        private int votingContra;

        public CommentTreeNode(Option<CommentTreeNode> parent) {
            this.parent = parent;
        }

        public CommentTreeNode(Option<CommentTreeNode> parent, string id, DateTime timestamp) : this(parent) {
            this.id = id;
            this.timestamp = timestamp;
        }

        public void AddChild(CommentTreeNode child) {
            Assertion.IsTrue(child.parent.Equals(this)); // The parent must already be set
            Assertion.IsTrue(!this.children.Contains(child)); // Should not contain the new child yet
            this.children.Add(child);
        }

        public int ChildrenCount {
            get {
                return this.children.Count;
            }
        }

        public IEnumerable<CommentTreeNode> Children {
            get {
                return this.children;
            }
        }
        
        [Persist]
        public string ID {
            get {
                return this.id;
            }

            set {
                this.id = value;
            }
        }

        public string DateCreated {
            get {
                return this.timestamp.ToLongDateString() + " " + this.timestamp.ToLongTimeString();
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
        public string CommentContent {
            get {
                return this.commentContent;
            }
            set {
                this.commentContent = value;
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

        public int Depth {
            get {
                Option<CommentTreeNode> current = Option<CommentTreeNode>.Some(this);
                int depth = 0;
                while (current.IsSome) {
                    ++depth;
                    current = current.ValueOrError().parent;
                }

                return depth;
            }
        }

        [Persist]
        public DateTime Timestamp {
            get {
                return this.timestamp;
            }
            set {
                this.timestamp = value;
            }
        }

        [Persist]
        public int VotingPro {
            get {
                return this.votingPro;
            }
            set {
                this.votingPro = value;
            }
        }

        [Persist]
        public int VotingContra {
            get {
                return this.votingContra;
            }
            set {
                this.votingContra = value;
            }
        }
    }
}