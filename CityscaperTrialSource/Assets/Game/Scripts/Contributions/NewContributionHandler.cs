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

        /// <summary>
        /// Adds the new contribution. Used as a button action.
        /// </summary>
        public void Add() {
            string title = this.titleInput.text;
            // TODO Check that the title is not empty

            string content = this.contentInput.text;
            // TODO Check that the content is not empty

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