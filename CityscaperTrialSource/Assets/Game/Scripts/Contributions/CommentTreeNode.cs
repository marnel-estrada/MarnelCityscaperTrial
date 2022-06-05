using System.Collections.Generic;

using Common;

namespace Game {
    /// <summary>
    /// Base class for a comment tree node.
    /// </summary>
    public class CommentTreeNode {
        private readonly Option<CommentTreeNode> parent; // The parent of the node

        private readonly List<CommentTreeNode> children = new List<CommentTreeNode>(0);

        public CommentTreeNode(Option<CommentTreeNode> parent) {
            this.parent = parent;
        }

        public void AddChild(CommentTreeNode child) {
            Assertion.IsTrue(child.parent.Equals(this)); // The parent must already be set
            Assertion.IsTrue(this.children.Contains(child)); // Should not contain the new child yet
            this.children.Add(child);
        }

        public int ChildrenCount {
            get {
                return this.children.Count;
            }
        }
    }
}