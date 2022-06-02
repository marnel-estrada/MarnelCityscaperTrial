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
            Assertion.NotNull(this.outline);
            
            // Resolve the ID
            Vector3 position = this.transform.position;
            this.id = $"{this.objectName} ({position.x:n2}, {position.y:n2}, {position.z:n2})";
        }

        public void ShowOutline() {
            this.outline.OutlineWidth = 2.0f;
        }

        public void HideOutline() {
            this.outline.OutlineWidth = 0;
        }
        
        public string Id {
            get {
                return this.id;
            }
        }
    }
}