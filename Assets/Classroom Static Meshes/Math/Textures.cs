using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Math {
	public class Textures : MonoBehaviour {

		public Texture[] easy_group, medium_group;
		public Texture iti;

		public Texture Get(BlockType type) {
			switch (type) {
			case BlockType.Easy:
				return GetEasy ();
			case BlockType.Medium:
				return GetMedium ();
			}
			return null;
		}

		public Texture GetEasy() {
			return randomTex(easy_group);
		}

		public Texture GetMedium() {
			return randomTex(medium_group);
		}

		private Texture randomTex(Texture[] g) {
			return g[Random.Range(0, g.Length)];
		}
	}
}