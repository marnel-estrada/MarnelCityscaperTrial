using System.Collections;

using Common;

using UnityEngine;
using UnityEngine.SceneManagement;

namespace Game {
    /// <summary>
    /// Sets the active scene a frame later
    /// </summary>
    public class ActiveSceneSetter : MonoBehaviour {
        [SerializeField]
        private string sceneName; // The scene to set active

        private void Start() {
            Assertion.NotEmpty(this.sceneName);
            
            StartCoroutine(SetActiveSceneLater());
        }

        private IEnumerator SetActiveSceneLater() {
            yield return null;

            Scene scene = SceneManager.GetSceneByName(this.sceneName);
            SceneManager.SetActiveScene(scene);
        }
    }
}