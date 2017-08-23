using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System;

namespace AXCPT {
	/**
	 * Grab the list of trials from the file.
	 */
	public class TrialList : MonoBehaviour {
		public TextAsset trialFile;

		public TrialType[] trialTypes;

		void Start () {
			var list = new List<TrialType>();
			using (StringReader reader = new StringReader(trialFile.text)) {
				string line;
				while ((line = reader.ReadLine()) != null) {
					if (line.Length >= 2) {
						var first = (TrialItem)Enum.Parse (typeof(TrialItem), line [0].ToString ());
						var second = (TrialItem)Enum.Parse (typeof(TrialItem), line [1].ToString ());
						list.Add (new TrialType (first, second));
					}
				}
			}
			this.trialTypes = list.ToArray ();
			print ("Loaded " + trialTypes.Length + " trials from " + trialFile.name);
		}

		void Update () {
			
		}
	}
}