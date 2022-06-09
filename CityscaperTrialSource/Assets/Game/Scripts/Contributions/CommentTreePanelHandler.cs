using System.Collections;
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

        private float commentsRootOriginalWidth;

        private readonly Dictionary<CommentTreeNode, CommentTreeEntry> entryMap = new Dictionary<CommentTreeNode, CommentTreeEntry>(); 
        
        protected override void Awake() {
            base.Awake();
            
            Assertion.NotNull(this.pool);
            Assertion.NotNull(this.commentsRoot);
            
            AddSignalListener(GameSignals.OPEN_COMMENT_TREE_PANEL, OnOpen);
            AddSignalListener(GameSignals.NEW_COMMENT_ADDED, OnNewCommentAdded);

            StartCoroutine(ResolveCommentRootOriginalWidth());
        }

        private void OnOpen(ISignalParameters parameters) {
            Clear();
            
            Option<Contribution> contribution = parameters.GetParameter<Contribution>(Params.CONTRIBUTION);
            Assertion.IsSome(contribution);
            GenerateContributionEntry(contribution.ValueOrError());

            Option<CommentTreeNode> comment = parameters.GetParameter<CommentTreeNode>(Params.COMMENT_NODE);
            Assertion.IsSome(comment);
            GenerateTreeEntries(comment.ValueOrError());
        }

        private void Clear() {
            foreach (KeyValuePair<CommentTreeNode,CommentTreeEntry> mapEntry in this.entryMap) {
                mapEntry.Value.Recycle();
            }    
            this.entryMap.Clear();
        }

        private void GenerateContributionEntry(CommentTreeNode node) {
            GameObject go = this.pool.Request("CommentTreeEntry");
            
            CommentTreeEntry entry = go.GetRequiredComponent<CommentTreeEntry>();
            entry.Init(node);
            
            // We hide the comment button here because direct comments are only added in contribution detail
            entry.HideCommentButton();
            
            Transform goTransform = go.transform;
            goTransform.SetParent(this.commentsRoot);
            goTransform.localScale = Vector3.one;

            // Manage
            this.entryMap.Add(node, entry);
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
            this.entryMap.Add(node, entry);
            
            IEnumerable<CommentTreeNode> children = node.Children;
            foreach (CommentTreeNode childNode in children) {
                GenerateTreeEntries(childNode);
            }
        }

        private const int COMMENT_TREE_ENTRY_WIDTH = 500;

        private void OnNewCommentAdded(ISignalParameters parameters) {
            // Create a CommentTreeEntry for the new comment
            Option<CommentTreeNode> parentCommentOption = parameters.GetParameter<CommentTreeNode>(Params.PARENT_COMMENT);
            Assertion.IsSome(parentCommentOption);

            Option<CommentTreeNode> newCommentOption = parameters.GetParameter<CommentTreeNode>(Params.NEW_COMMENT);
            Assertion.IsSome(newCommentOption);
            
            GameObject go = this.pool.Request("CommentTreeEntry");
            
            CommentTreeEntry entry = go.GetRequiredComponent<CommentTreeEntry>();
            CommentTreeNode newComment = newCommentOption.ValueOrError();
            entry.Init(newComment);
            
            Transform goTransform = go.transform;
            goTransform.SetParent(this.commentsRoot);
            goTransform.localScale = Vector3.one;
            
            // Set the new entry next to the parent
            CommentTreeNode parentComment = parentCommentOption.ValueOrError();
            CommentTreeEntry parentEntry = this.entryMap[parentComment];
            goTransform.SetSiblingIndex(parentEntry.transform.GetSiblingIndex() + parentComment.DescendantCount);

            // Manage
            this.entryMap.Add(newComment, entry);
        }

        private IEnumerator ResolveCommentRootOriginalWidth() {
            yield return null;

            this.commentsRootOriginalWidth = this.commentsRoot.rect.width;
            print($"Original width: {this.commentsRootOriginalWidth}");
        }
    }
}