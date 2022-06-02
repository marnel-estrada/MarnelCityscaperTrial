using Common;

using UnityEngine;
using UnityEngine.EventSystems;

namespace Game {
    public class EventSystemHandler : MonoBehaviour {
        [SerializeField]
        private EventSystem eventSystem;
        
        private void Awake() {
            Assertion.NotNull(this.eventSystem);
            
            GameQueries.IS_ON_UI_OBJECT.RegisterProvider(delegate {
                return this.eventSystem.IsPointerOverGameObject();
            });
        }
    }
}