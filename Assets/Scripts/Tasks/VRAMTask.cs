using System;
using UnityEngine;
using Distraction;
using UnityEngine.SceneManagement;
using Subject;

public abstract class VRAMTask : MonoBehaviour {
    private static Color DEFAULT_TEXT_COLOR = Color.black;

    [SerializeField]
	protected DistractionController distractionController;

    private ShowText whiteboardText;
    private ShowImage whiteboardImage;
    protected InputBroker input;
    protected SubjectDataHolder subject;

    void Awake() {
        var wb = GameObject.Find("WhiteBoardWithDisplay");
        if (wb != null) {
            whiteboardText = wb.GetComponent<ShowText>();
            whiteboardText.Hide();
            whiteboardImage = wb.GetComponent<ShowImage>();
            whiteboardImage.Hide();
        } else {
            print("ERROR: WhiteBoardWithDisplay not found!");
        }
		input = FindObjectOfType<InputBroker>();
        if (input == null) {
            print("ERROR: Could not find InputBroker!");
        }
        subject = FindObjectOfType<SubjectDataHolder>();
    }

    protected void ShowImage(Texture img) {
        whiteboardImage.SetTexture(img);
        whiteboardText.Hide();
        whiteboardImage.Show();
    }

    protected void ShowText(string txt) {
        ShowColorText(txt, DEFAULT_TEXT_COLOR);
    }

    protected void ShowColorText(string txt, Color color) {
        whiteboardText.SetColor(color);
        whiteboardText.SetText(txt);
        whiteboardImage.Hide();
        whiteboardText.Show();
    }

    public void EndTask() {
        whiteboardImage.Hide();
        whiteboardText.Hide();
        distractionController.gameObject.SetActive (false);
        this.gameObject.SetActive(false);
        subject.AppendSession(new SessionData(DateTime.Now));
        SceneManager.LoadScene ("MenuScene");
    }

    public abstract string GetCurrentState();
}