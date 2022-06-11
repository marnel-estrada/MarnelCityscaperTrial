using cakeslice;

using Common;

using UnityEngine;

namespace Game {
    public class SelectableObject : MonoBehaviour {
        [SerializeField]
        private string objectName;
        
        [SerializeField]
        private Outline outline;

        private string id;

        private void Awake() {
            // if (this.outline == null) {
            //     // Automatically get from own object if it was not specified
            //     this.outline = this.GetRequiredComponent<Outline>();
            // }
            // Assertion.NotNull(this.outline);
            
            // Resolve the ID
            Vector3 position = this.transform.position;
            this.id = $"{this.objectName} ({position.x:n2}, {position.y:n2}, {position.z:n2})";
            
            HideOutline();
        }

        public void MarkAsSelected() {
            if (this.outline == null) {
                return;
            }
            
            this.outline.color = 1;
            this.outline.enabled = true;
        }

        public void MarkAsHovered() {
            if (this.outline == null) {
                return;
            }
            
            this.outline.color = 0;
            this.outline.enabled = true;
        }

        public void HideOutline() {
            if (this.outline == null) {
                return;
            }
            
            this.outline.enabled = false;
        }
        
        public string Id {
            get {
                return this.id;
            }
        }
    }
}