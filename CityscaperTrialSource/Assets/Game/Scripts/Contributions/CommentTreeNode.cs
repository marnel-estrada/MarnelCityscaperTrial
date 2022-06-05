using System;
using System.Collections.Generic;

using Common;

namespace Game {
    /// <summary>
    /// Base class for a comment tree node.
    /// </summary>
    public class CommentTreeNode {
        private readonly Option<CommentTreeNode> parent; // The parent of the node
        
        private readonly string id;
        private readonly DateTime timestamp;

        private readonly List<CommentTreeNode> children = new List<CommentTreeNode>(0);
        
        private string title;
        private string commentContent;
        private ContributionType contributionType;

        public CommentTreeNode(Option<CommentTreeNode> parent, string id, DateTime timestamp) {
            this.parent = parent;
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
        
        public string ID {
            get {
                return this.id;
            }
        }

        public string DateCreated {
            get {
                return this.timestamp.ToLongTimeString();
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

        public string CommentContent {
            get {
                return this.commentContent;
            }
            set {
                this.commentContent = value;
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
    }
}