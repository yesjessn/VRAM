using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AXCPT;
using UnityEngine.UI;
using System.Linq;

public class CategoryDropdownLoader : MonoBehaviour {
	public CategoryLoader loader;

	void Start () {
		var dropdown = GetComponent (typeof(Dropdown)) as Dropdown;
		dropdown.AddOptions (loader.categories.ToList());
	}
}
