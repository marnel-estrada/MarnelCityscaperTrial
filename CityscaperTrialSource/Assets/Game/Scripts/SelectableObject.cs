using Common;

using UnityEngine;

namespace Game {
    public class SelectableObject : MonoBehaviour {
        [SerializeField]
        private Outline outline;

        private void Awake() {
            Assertion.NotNull(this.outline);
        }

        public void Select() {
            this.outline.OutlineWidth = 2.0f;
        }

        public void Deselect() {
            this.outline.OutlineWidth = 0;
        }
    }
}