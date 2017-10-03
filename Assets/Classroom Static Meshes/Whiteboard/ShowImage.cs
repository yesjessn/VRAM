using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShowImage : MonoBehaviour {
	
	public RawImage img;

	// Use this for initialization
	void Start () {
		Hide ();
	}
	
	// Update is called once per frame
	void Update () { }

	public void Hide() {
		img.gameObject.SetActive (false);
	}

	public void Show() {
		img.gameObject.SetActive (true);
	}

	public void SetTexture(Texture texture) {
		img.texture = texture;
	}

	public Texture GetTexture () {
		return img.texture;
	}
}
