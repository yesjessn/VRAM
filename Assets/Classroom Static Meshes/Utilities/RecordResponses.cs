using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class RecordResponses : MonoBehaviour {
	public class Response {
		public readonly string buttonPressed;
		public readonly float responseTime;

		public Response(string buttonPressed, float responseTime) {
			this.buttonPressed = buttonPressed;
			this.responseTime = responseTime;
		}

		public override string ToString ()
		{
			return buttonPressed + " after " + responseTime.ToString();
		}
	}
	public static Response EMPTY_RESPONSE = new Response("", 0);

	public string[] buttons;

	private bool recording;
	private float recordingStartTime;
	private Response response;

	public bool isRecording { get { return recording; } }

	void Start () {
		recording = false;
	}

	public void StartRecording() {
		recording = true;
		recordingStartTime = Time.time;
		response = null;
		print ("starting recording");
	}

	public Response StopRecording() {
		print ("stopping recording");
		recording = false;
		if (response == null) {
			return EMPTY_RESPONSE;
		} else {
			return response;
		}
	}

	// Update is called once per frame
	void Update () {
		if (recording && response == null) {
			foreach (string button in buttons) {
				if (Input.GetButtonDown (button)) {
					var responseTime = Time.time - recordingStartTime;
					response = new Response (button, responseTime);
					break;
				}
			}
		}
	}
}
