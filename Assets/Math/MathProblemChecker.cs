using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace Math {
	public class MathProblemChecker : MonoBehaviour {
		public TextAsset solutionsFile;

		protected Dictionary<string, string> solutions;

		void Start() {
			this.solutions = new Dictionary<string, string> ();

			using (StringReader reader = new StringReader(solutionsFile.text)) {
				string line;
				var lineno = 0;
				while ((line = reader.ReadLine()) != null) {
					var parts = line.Split (',');
					if (parts.Length != 2) {
						print ("Line number " + lineno + " has " + parts.Length + " parts!\n\t" + line);
					} else {
						this.solutions.Add (parts [0], parts [1]);
					}
				}
			}
		}

		public bool Check(string problemName, string button) {
			if (solutions.ContainsKey(problemName)) {
				return button.Equals(solutions[problemName]);
			}
			print ("Problem name: '" + problemName + "' does not have a solution defined!");
			return false;
		}
	}
}

