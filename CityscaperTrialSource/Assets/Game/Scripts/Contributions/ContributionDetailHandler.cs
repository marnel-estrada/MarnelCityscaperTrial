using System;
using System.Collections.Generic;

using Common;
using Common.Signal;

using TMPro;

using UnityEngine;

namespace Game {
    public class ContributionDetailHandler : SignalHandlerComponent {
        [SerializeField]
        private PrefabManager pool;

        [SerializeField]
        private RectTransform commentsRoot;

        // This will be used to set the index of comment entries
        [SerializeField]
        private RectTransform transformBeforeButtons;
        
        [SerializeField]
        private TMP_Text headerLabel; // For the title
        
        [SerializeField]
        private TMP_Text authorLabel;

        [SerializeField]
        private TMP_Text contentText;

        [SerializeField]
        private TMP_Text commentCountLabel;
        
        [SerializeField]
        private TMP_InputField newCommentTitleInput;

        [SerializeField]
        private TMP_Dropdown newCommentTypeDropdown;
        
        [SerializeField]
        private TMP_InputField newCommentContentInput;

        private Option<Contribution> currentContribution;

        private readonly List<CommentEntry> commentEntries = new List<CommentEntry>();

        protected override void Awake() {
            base.Awake();
            
            Assertion.NotNull(this.pool);
            Assertion.NotNull(this.commentsRoot);
            Assertion.NotNull(this.transformBeforeButtons);
            Assertion.NotNull(this.headerLabel);
            Assertion.NotNull(this.authorLabel);
            Assertion.NotNull(this.contentText);
            Assertion.NotNull(this.commentCountLabel);
            Assertion.NotNull(this.newCommentTitleInput);
            Assertion.NotNull(this.newCommentTypeDropdown);
            Assertion.NotNull(this.newCommentContentInput);
            
            AddSignalListener(GameSignals.OPEN_CONTRIBUTION_DETAIL, OnOpen);
        }

        private void OnOpen(ISignalParameters parameters) {
            this.currentContribution = parameters.GetParameter<Contribution>(Params.CONTRIBUTION);
            Assertion.IsSome(this.currentContribution);
            
            UpdateDisplay(this.currentContribution.ValueOrError());
        }

        private void UpdateDisplay(Contribution contribution) {
            this.headerLabel.text = $"Contribution: {contribution.Title}";
            this.authorLabel.text = contribution.Author;
            this.contentText.text = contribution.ContributionContent;
            this.commentCountLabel.text = $"{TextUtils.AsCommaSeparated(contribution.CommentsNumber)} Comments";

            RegenerateCommentEntries(contribution);
        }

        private void RegenerateCommentEntries(Contribution contribution) {
            // Clear the existing ones first
            for (int i = 0; i < this.commentEntries.Count; ++i) {
                this.commentEntries[i].Recycle();
            }
            this.commentEntries.Clear();

            IEnumerable<CommentTreeNode> comments = contribution.Children;
            foreach (CommentTreeNode comment in comments) {
                GenerateCommentEntry(comment);
            }
        }

        // Used as a button action
        public void AddComment() {
            string commentTitle = this.newCommentTitleInput.text;
            if (string.IsNullOrWhiteSpace(commentTitle)) {
                ModalMessageHandler.Open("New Comment", "Comment title can't be empty.");
                return;
            }

            string commentContent = this.newCommentContentInput.text;
            if (string.IsNullOrWhiteSpace(commentContent)) {
                ModalMessageHandler.Open("New Comment", "Comment content can't be empty");
                return;
            }
            
            Assertion.IsSome(this.currentContribution);
            Contribution contribution = this.currentContribution.ValueOrError();
            
            string commentId = GameQueries.GET_NEW_COMMENT_ID.Execute();
            DateTime timestamp = DateTime.Now;
            Comment newComment = new Comment(contribution, commentId, timestamp);
            newComment.Title = commentTitle;
            newComment.CommentContent = commentContent;
            
            int selectedContributionTypeIndex = this.newCommentTypeDropdown.value;
            ContributionType contributionType = ContributionType.ConvertFromIndex(selectedContributionTypeIndex);
            newComment.ContributionType = contributionType;

            newComment.CommentOnContribution = contribution.ID;
            
            contribution.AddChild(newComment);

            GenerateCommentEntry(newComment);
            
            // Clear the input fields
            this.newCommentTitleInput.text = string.Empty;
            this.newCommentContentInput.text = string.Empty;
        }

        private void GenerateCommentEntry(CommentTreeNode comment) {
            GameObject go = this.pool.Request("CommentEntry");
            Transform goTransform = go.transform;
            goTransform.SetParent(this.commentsRoot);
            goTransform.localScale = Vector3.one;
            goTransform.SetSiblingIndex(this.transformBeforeButtons.GetSiblingIndex() + 1);

            CommentEntry entry = go.GetRequiredComponent<CommentEntry>();
            entry.Init(comment);

            // Manage
            this.commentEntries.Add(entry);
        }
    }
}