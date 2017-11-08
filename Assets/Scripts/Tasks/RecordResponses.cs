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

	public string[] buttons;

	private bool recording;
	private float recordingStartTime;
	private List<Response> response;

	private InputBroker input;

	public bool isRecording { get { return recording; } }
	public bool hasResponse { get { return response != null; } }

	void Start () {
		recording = false;
		response = new List<Response> ();
		input = (InputBroker)FindObjectOfType(typeof(InputBroker));
	}

	public void StartRecording() {
		recording = true;
		recordingStartTime = Time.time;
		response.Clear();
		print ("starting recording");
	}

	public List<Response> StopRecording() {
		print ("stopping recording");
		recording = false;
		return response;

	}

	// Update is called once per frame
	void Update () {
		if (recording) {
			foreach (string button in buttons) {
				if (input.GetButtonDown (button)) {
					var responseTime = Time.time - recordingStartTime;
					response.Add (new Response (button, responseTime));
					break;
				}
			}
		}
	}
}
