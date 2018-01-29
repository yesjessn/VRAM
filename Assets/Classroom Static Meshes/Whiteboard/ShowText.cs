using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShowText : MonoBehaviour {

	public Text whiteboardText;

	// Use this for initialization
	void Start () { }
	
	public void Hide () {
		whiteboardText.gameObject.SetActive (false);
	}

	public void Show() {
		whiteboardText.gameObject.SetActive (true);
	}

	public void SetText(string contents) {
		whiteboardText.text = contents; 
	}

	public void SetColor (Color color) {
		whiteboardText.color = color;
	}
}
