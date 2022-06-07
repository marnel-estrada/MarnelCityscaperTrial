using System.Collections.Generic;

using Common;
using Common.Signal;

using UnityEngine;

namespace Game {
    public class CommentTreePanelHandler : SignalHandlerComponent {
        [SerializeField]
        private PrefabManager pool;

        [SerializeField]
        private RectTransform commentsRoot;

        private readonly List<CommentTreeEntry> entries = new List<CommentTreeEntry>(); 
        
        protected override void Awake() {
            base.Awake();
            
            Assertion.NotNull(this.pool);
            Assertion.NotNull(this.commentsRoot);
            
            AddSignalListener(GameSignals.OPEN_COMMENT_TREE_PANEL, OnOpen);
        }

        private void OnOpen(ISignalParameters parameters) {
            Clear();
            
            Option<Contribution> contribution = parameters.GetParameter<Contribution>(Params.CONTRIBUTION);
            Assertion.IsSome(contribution);
            GenerateEntry(contribution.ValueOrError());

            Option<CommentTreeNode> comment = parameters.GetParameter<CommentTreeNode>(Params.COMMENT_NODE);
            Assertion.IsSome(comment);
            GenerateTreeEntries(comment.ValueOrError());
        }

        private void Clear() {
            for (int i = 0; i < this.entries.Count; ++i) {
                this.entries[i].Recycle();
            }    
            this.entries.Clear();
        }

        private void GenerateEntry(CommentTreeNode node) {
            GameObject go = this.pool.Request("CommentTreeEntry");
            
            CommentTreeEntry entry = go.GetRequiredComponent<CommentTreeEntry>();
            entry.Init(node);
            
            Transform goTransform = go.transform;
            goTransform.SetParent(this.commentsRoot);
            goTransform.localScale = Vector3.one;

            // Manage
            this.entries.Add(entry);
        }

        // Generates the entries of the whole tree
        private void GenerateTreeEntries(CommentTreeNode node) {
            GameObject go = this.pool.Request("CommentTreeEntry");
            
            CommentTreeEntry entry = go.GetRequiredComponent<CommentTreeEntry>();
            entry.Init(node);
            
            Transform goTransform = go.transform;
            goTransform.SetParent(this.commentsRoot);
            goTransform.localScale = Vector3.one;

            // Manage
            this.entries.Add(entry);
            
            IEnumerable<CommentTreeNode> children = node.Children;
            foreach (CommentTreeNode childNode in children) {
                GenerateTreeEntries(childNode);
            }
        }
    }
}