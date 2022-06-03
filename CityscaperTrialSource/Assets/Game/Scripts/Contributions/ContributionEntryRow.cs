using Common;

using TMPro;

using UnityEngine;

namespace Game {
    [RequireComponent(typeof(SwarmItem))]
    public class ContributionEntryRow : MonoBehaviour {
        [SerializeField]
        private TMP_Text titleLabel;
        
        [SerializeField]
        private TMP_Text contentLabel;

        private SwarmItem swarmItem;

        private void Awake() {
            Assertion.NotNull(this.titleLabel);
            Assertion.NotNull(this.contentLabel);

            this.swarmItem = this.GetRequiredComponent<SwarmItem>();
        }

        public void Init(string title, string content) {
            this.titleLabel.text = title;
            this.contentLabel.text = content;
        }

        public void Recycle() {
            this.swarmItem.Recycle();
        }
    }
}