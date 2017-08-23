using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.UI;

namespace Math {
	public class GradeLoader : MonoBehaviour {
		public static string GradeRoot = "Math";

		public Textures textures;
		public Dropdown menu;

		// Use this for initialization
		void Start () {
		}

		public void LoadGrade(int grade) {
			textures.easy_group = LoadTextureGroup (grade - 2);
			textures.medium_group = LoadTextureGroup (grade);
		}

		private Texture[] LoadTextureGroup(int grade) {
			var path = GradeRoot + "/" + grade;
			return Resources.LoadAll (path, typeof(Texture)).Cast<Texture>().ToArray();
		}
	}
}
