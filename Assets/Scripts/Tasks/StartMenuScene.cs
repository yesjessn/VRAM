using UnityEngine;
using UnityEngine.SceneManagement;

// Start straight into the menu. The Initialization scene exists so that the objects with DoNoDestroy don't
// get recreated when we return to the main menu when a task completes.
public class StartMenuScene : MonoBehaviour {
    void Start() {
        SceneManager.LoadScene ("MenuScene");
    }
}