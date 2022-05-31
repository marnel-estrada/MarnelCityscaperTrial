using Common;

using UnityEngine;

namespace Game {
    public class SceneSelectionHandler : MonoBehaviour {
        [SerializeField]
        private string referenceCameraName;
        
        private Camera referenceCamera;

        private void Awake() {
            Assertion.NotEmpty(this.referenceCameraName);

            this.referenceCamera = UnityUtils.GetRequiredComponent<Camera>(this.referenceCameraName);
        }

        private const int LEFT_MOUSE_BUTTON = 0;

        private void Update() {
            if (Input.GetMouseButtonDown(LEFT_MOUSE_BUTTON)) {
                Ray ray = this.referenceCamera.ScreenPointToRay(Input.mousePosition);
                if (Physics.Raycast(ray, out RaycastHit hit, 100)) {
                    Outline outline = hit.transform.GetComponent<Outline>();
                    if (outline != null) {
                        outline.OutlineWidth = 3;
                    }
                }
            }
        }
    }
}