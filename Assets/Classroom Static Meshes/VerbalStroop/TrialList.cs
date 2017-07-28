using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System;

namespace VerbalStroop {
	/**
	 * Grab the list of trials from the file.
	 */
	public class TrialList : MonoBehaviour {
		public TextAsset trialFile;

		public TrialProperties[] trialProperties;

		void Start () {
			var list = new List<TrialProperties>();
			using (StringReader reader = new StringReader(trialFile.text)) {
				string line;
				while ((line = reader.ReadLine()) != null) {
					var parts = line.Split (new char[] {','});
					if (parts.Length >= 3) {
						var first = (TrialText)Enum.Parse (typeof(TrialText), parts[0]);
						var second = (TrialColor)Enum.Parse (typeof(TrialColor), parts[1]);
						var third = (TrialSound)Enum.Parse (typeof(TrialSound), parts[2]);
						list.Add (new TrialProperties (first, second, third));
					}
				}
			}
			this.trialProperties = list.ToArray ();
			print ("Loaded " + trialProperties.Length + " trials from " + trialFile.name);
		}

		void Update () {
			
		}
	}
}