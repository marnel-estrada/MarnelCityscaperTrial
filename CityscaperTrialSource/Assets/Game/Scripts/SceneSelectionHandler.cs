using Common;

using UnityEngine;

namespace Game {
    public class SceneSelectionHandler : MonoBehaviour {
        [SerializeField]
        private string referenceCameraName;
        
        private Camera referenceCamera;

        private Option<SelectableObject> currentSelectedObject = Option<SelectableObject>.NONE;

        private void Awake() {
            Assertion.NotEmpty(this.referenceCameraName);

            this.referenceCamera = UnityUtils.GetRequiredComponent<Camera>(this.referenceCameraName);
        }

        private const int LEFT_MOUSE_BUTTON = 0;

        private void Update() {
            if (Input.GetMouseButtonDown(LEFT_MOUSE_BUTTON)) {
                Ray ray = this.referenceCamera.ScreenPointToRay(Input.mousePosition);
                if (!Physics.Raycast(ray, out RaycastHit hit, 100)) {
                    // Didn't hit anything
                    return;
                }

                SelectableObject selectableObject = hit.transform.GetComponent<SelectableObject>();
                if (selectableObject == null) {
                    // Currently selected object is not a selectable object.
                    // Clear current selection and move on.
                    this.currentSelectedObject.Match(new DeselectMatcher());
                    this.currentSelectedObject = Option<SelectableObject>.NONE;
                    return;
                }
                    
                // At this point, there's a selected object
                if (this.currentSelectedObject.Equals(selectableObject)) {
                    // Selection is the same
                    return;
                }
                    
                // A new object is selected. We deselect the current one first
                this.currentSelectedObject.Match(new DeselectMatcher());
                    
                // Set new current selected object
                selectableObject.Select();
                this.currentSelectedObject = Option<SelectableObject>.Some(selectableObject);
            }
        }
        
        private readonly struct DeselectMatcher : IOptionMatcher<SelectableObject> {
            public void OnSome(SelectableObject selectableObject) {
                selectableObject.Deselect();
            }

            public void OnNone() {
                // Do nothing
            }
        }
    }
}