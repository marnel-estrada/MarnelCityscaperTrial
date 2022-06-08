using System;

using Common;
using Common.Signal;

using TMPro;

using UnityEngine;

namespace Game {
    public class NewCommentHandler : SignalHandlerComponent {
        [SerializeField]
        private TMP_Text parentCommentLabel;
        
        [SerializeField]
        private TMP_InputField titleInput;

        [SerializeField]
        private TMP_Dropdown contributionTypeDropdown;

        [SerializeField]
        private TMP_InputField contentInput;

        private Option<CommentTreeNode> currentParentComment;
        
        protected override void Awake() {
            base.Awake();
            
            Assertion.NotNull(this.parentCommentLabel);
            Assertion.NotNull(this.titleInput);
            Assertion.NotNull(this.contributionTypeDropdown);
            Assertion.NotNull(this.contentInput);
            
            AddSignalListener(GameSignals.OPEN_NEW_COMMENT_PANEL, OnOpen);
        }

        private void OnOpen(ISignalParameters parameters) {
            this.currentParentComment = parameters.GetParameter<CommentTreeNode>(Params.PARENT_COMMENT);
            Assertion.IsSome(this.currentParentComment);
            
            // Clear the fields
            this.titleInput.text = string.Empty;
            this.contentInput.text = string.Empty;
        }
        
        private const string MODAL_HEADER = "New Comment";

        // Adds the comment. Used as a button action
        public void Add() {
            string title = this.titleInput.text;
            if (string.IsNullOrWhiteSpace(title)) {
                ModalMessageHandler.Open(MODAL_HEADER, "The contribution title can't be empty.");
                return;
            }

            string content = this.contentInput.text;
            if (string.IsNullOrWhiteSpace(content)) {
                ModalMessageHandler.Open(MODAL_HEADER, "The contribution content can't be empty.");
                return;
            }
            
            // Prepare the Comment
            Assertion.IsSome(this.currentParentComment);
            CommentTreeNode parentComment = this.currentParentComment.ValueOrError();
            
            string commentId = GameQueries.GET_NEW_COMMENT_ID.Execute();
            DateTime timestamp = DateTime.Now;
            Comment newComment = new Comment(parentComment, commentId, timestamp);
            newComment.Title = title;
            newComment.CommentContent = content;
            
            int selectedContributionTypeIndex = this.contributionTypeDropdown.value;
            ContributionType contributionType = ContributionType.ConvertFromIndex(selectedContributionTypeIndex);
            newComment.ContributionType = contributionType;

            newComment.CommentOnComment = parentComment.ID;
            
            parentComment.AddChild(newComment);
            
            // We dispatch this signal so that comment tree panel can respond by creating a CommentTreeEntry
            Signal signal = GameSignals.NEW_COMMENT_ADDED;
            signal.ClearParameters();
            signal.AddParameter(Params.PARENT_COMMENT, parentComment);
            signal.AddParameter(Params.NEW_COMMENT, newComment);
            signal.Dispatch();
            
            // Clear the fields
            this.titleInput.text = string.Empty;
            this.contentInput.text = string.Empty;
            
            // Close
            GameSignals.CLOSE_NEW_COMMENT_PANEL.Dispatch();
        }
    }
}