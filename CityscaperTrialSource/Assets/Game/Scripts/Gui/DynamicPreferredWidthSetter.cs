using UnityEngine;
using UnityEngine.UI;

namespace Game {
    [ExecuteInEditMode]
    public class DynamicPreferredWidthSetter : MonoBehaviour {
        [SerializeField]
        private LayoutElement layoutElement;

        [SerializeField]
        private RectTransform targetRect;

        private void Update() {
            if (this.layoutElement == null) {
                // Not specified
                return;
            }

            if (this.targetRect == null) {
                // Not specified
                return;
            }

            this.layoutElement.preferredWidth = this.targetRect.rect.width;
        }
    }
}