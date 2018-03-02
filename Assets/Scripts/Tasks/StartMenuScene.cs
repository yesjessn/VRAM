using UnityEngine;
using UnityEngine.SceneManagement;

public class StartMenuScene : MonoBehaviour {
    void Start() {
        SceneManager.LoadScene ("MenuScene");
    }
}