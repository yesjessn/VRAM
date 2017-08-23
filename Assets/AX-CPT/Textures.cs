using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AXCPT {
	public class Textures : MonoBehaviour {

		public Texture[] a_group, b_group, x_group, y_group;
		public Texture isi;
		public Texture iti;

		public Texture Get(TrialItem type) {
			switch (type) {
			case TrialItem.A:
				return GetA ();
			case TrialItem.B:
				return GetB ();
			case TrialItem.X:
				return GetX ();
			case TrialItem.Y:
				return GetY ();
			}
			return null;
		}

		public Texture GetA() {
			return randomTex(a_group);
		}

		public Texture GetB() {
			return randomTex(b_group);
		}

		public Texture GetX() {
			return randomTex(x_group);
		}

		public Texture GetY() {
			return randomTex(y_group);
		}

		private Texture randomTex(Texture[] g) {
			return g[Random.Range(0, g.Length)];
		}
	}
}