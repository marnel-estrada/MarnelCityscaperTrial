using Common;

using UnityEngine;

namespace Game {
    public static class GameQueries {
        public static readonly Query<object, bool> IS_ON_UI_OBJECT = new Query<object, bool>();
        
        private static readonly StaticFieldsInvoker CLEAR_PROVIDERS = 
            new StaticFieldsInvoker(typeof(GameQueries), "ClearProvider");

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        private static void ResetStatic() {
            CLEAR_PROVIDERS.Execute();
        }
    }
}