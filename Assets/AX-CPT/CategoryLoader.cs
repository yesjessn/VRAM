using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.UI;

namespace AXCPT {
	public class CategoryLoader : MonoBehaviour {
		public static string CategoryRoot = "AX-CPT/Categories";

		public string[] categories;
		public Textures textures;
		public Dropdown menu;

		// Use this for initialization
		void Start () {
			print ("Loaded categories: [" + string.Join (", ", categories) + "]");
		}

		public void LoadCategory(int index) {
			var category = categories[index];
			LoadCategoryByString (category);
		}

		public void LoadCategoryByString (string category) {
			textures.a_group = LoadTextureGroup (category, TrialItem.A);
			textures.b_group = LoadTextureGroup (category, TrialItem.B);
			textures.x_group = LoadTextureGroup (category, TrialItem.X);
			textures.y_group = LoadTextureGroup (category, TrialItem.Y);
		
		}

		private Texture[] LoadTextureGroup(string category, TrialItem type) {
			var path = CategoryRoot + "/" + category + "/" + type.ToString ();
			return Resources.LoadAll (path, typeof(Texture)).Cast<Texture>().ToArray();
		}
	}
}
