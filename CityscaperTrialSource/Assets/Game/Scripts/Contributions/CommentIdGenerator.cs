using Common;

using UnityEngine;

namespace Game {
    public class CommentIdGenerator : MonoBehaviour {
        public readonly IdGenerator idGenerator = new IdGenerator();

        private void Awake() {
            GameQueries.GET_NEW_COMMENT_ID.RegisterProvider(delegate {
                return this.idGenerator.Generate().ToString();
            });
        }
    }
}