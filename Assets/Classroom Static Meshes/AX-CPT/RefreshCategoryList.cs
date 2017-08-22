using UnityEngine;
using System.Collections;
using System;
using System.IO;
using System.Collections.Generic;
#if (UNITY_EDITOR) 
using UnityEditor;

namespace AXCPT {
	[CustomEditor(typeof(CategoryLoader))]
	[CanEditMultipleObjects]
	public class RefreshCharacterList : Editor {
		SerializedProperty textures;
		SerializedProperty categories;

		public void OnEnable () {
			textures = serializedObject.FindProperty ("textures");
			categories = serializedObject.FindProperty ("categories");
		}

		public override void OnInspectorGUI () {
			serializedObject.Update ();

			EditorGUILayout.ObjectField (textures);

			EditorGUILayout.Space ();
			if (GUILayout.Button ("Auto Populate List")) {
				Populate ();
			}

			EditorGUILayout.Space ();
			ArrayGUI (serializedObject, "categories");
			serializedObject.ApplyModifiedProperties ();
		}

		public void Populate () {
			DirectoryInfo root = new DirectoryInfo ("Assets/Resources/" + CategoryLoader.CategoryRoot);
			DirectoryInfo[] catDirs = root.GetDirectories ();
			string[] categoryStrings = new string[catDirs.Length];
			for (int i = 0; i < catDirs.Length; i++) {
				categoryStrings [i] = catDirs [i].Name;
			}
			categories.arraySize = categoryStrings.Length;
			for (int i = 0; i < categoryStrings.Length; i++) {
				serializedObject.FindProperty ("categories.Array.data[" + (i.ToString ()) + "]").stringValue = categoryStrings [i];
			}
			serializedObject.ApplyModifiedProperties ();
		}

		void ArrayGUI (SerializedObject obj, string name) {
			int size = obj.FindProperty (name + ".Array.size").intValue;
			int newSize = EditorGUILayout.IntField (name + " Size", size);
			if (newSize != size) {
				obj.FindProperty (name + ".Array.size").intValue = newSize;
			}
			EditorGUI.indentLevel = 3;
			for (int i = 0; i<newSize; i++) {
				SerializedProperty prop = obj.FindProperty (string.Format ("{0}.Array.data[{1}]", name, i));
				EditorGUILayout.PropertyField (prop);
			}
		}
	}
}
#endif
