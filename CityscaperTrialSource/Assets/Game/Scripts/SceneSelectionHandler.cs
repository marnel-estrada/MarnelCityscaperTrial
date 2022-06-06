using Common;
using Common.Signal;

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
            
            GameSignals.DESELECT_CURRENT_SELECTION.AddListener(DeselectCurrentSelection);
        }

        private const int LEFT_MOUSE_BUTTON = 0;

        private void Update() {
            if (Input.GetMouseButtonDown(LEFT_MOUSE_BUTTON)) {
                if (GameQueries.IS_ON_UI_OBJECT.Execute()) {
                    // It's on UI. Do not proceed.
                    return;
                }
                
                Ray ray = this.referenceCamera.ScreenPointToRay(Input.mousePosition);
                if (!Physics.Raycast(ray, out RaycastHit hit, 100)) {
                    // Didn't hit anything
                    return;
                }

                SelectableObject selectableObject = hit.transform.GetComponent<SelectableObject>();
                if (selectableObject == null) {
                    // Currently selected object is not a selectable object.
                    // Clear current selection and move on.
                    ClearSelection();
                    return;
                }
                    
                // At this point, there's a selected object
                if (this.currentSelectedObject.Equals(selectableObject)) {
                    // Selection is the same
                    return;
                }
                    
                // A new object is selected. We deselect the current one first
                this.currentSelectedObject.Match(new HideOutlineMatcher());
                    
                // Set new current selected object and select it
                selectableObject.ShowOutline();
                this.currentSelectedObject = Option<SelectableObject>.Some(selectableObject);
                
                // Dispatch the signal that opens the contributions panel
                Signal signal = GameSignals.OPEN_CONTRIBUTIONS_PANEL;
                signal.ClearParameters();
                signal.AddParameter(Params.OBJECT_ID, selectableObject.Id);
                signal.AddParameter(Params.CONTRIBUTION_SET, selectableObject.GetRequiredComponent<ContributionSet>());
                signal.Dispatch();
            }
        }
        
        private readonly struct HideOutlineMatcher : IOptionMatcher<SelectableObject> {
            public void OnSome(SelectableObject selectableObject) {
                selectableObject.HideOutline();
            }

            public void OnNone() {
                // Do nothing
            }
        }

        private void DeselectCurrentSelection(ISignalParameters parameters) {
            ClearSelection();
        }

        private void ClearSelection() {
            this.currentSelectedObject.Match(new HideOutlineMatcher());
            this.currentSelectedObject = Option<SelectableObject>.NONE;

            GameSignals.CLOSE_CONTRIBUTIONS_PANEL.Dispatch();
            GameSignals.CLOSE_CONTRIBUTION_DETAIL.Dispatch();
        }
    }
}