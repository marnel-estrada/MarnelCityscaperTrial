using Common;

using TMPro;

using UnityEngine;

namespace Game {
    public class NewContributionHandler : MonoBehaviour {
        [SerializeField]
        private TMP_InputField titleInput;

        [SerializeField]
        private TMP_Dropdown contributionTypeDropdown;

        [SerializeField]
        private TMP_InputField contentInput;

        private void Awake() {
            Assertion.NotNull(this.titleInput);
            Assertion.NotNull(this.contributionTypeDropdown);
            Assertion.NotNull(this.contentInput);
        }

        private const string MODAL_HEADER = "Add New Contribution";

        /// <summary>
        /// Adds the new contribution. Used as a button action.
        /// </summary>
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

            int selectedContributionTypeIndex = this.contributionTypeDropdown.value;
            ContributionType contributionType = ContributionType.ConvertFromIndex(selectedContributionTypeIndex);
            
            GameSignals.ADD_NEW_CONTRIBUTION.Dispatch(new AddNewContribution(title, content, contributionType));
            
            // Clear the fields
            this.titleInput.text = string.Empty;
            this.contentInput.text = string.Empty;
            
            // Close
            GameSignals.CLOSE_NEW_CONTRIBUTIONS_PANEL.Dispatch();
        }
    }
}