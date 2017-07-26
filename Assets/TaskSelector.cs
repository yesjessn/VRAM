using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using AXCPT;
using Math;

public class TaskSelector : MonoBehaviour {

	public GameObject[] tasks;
	public Dropdown taskDropdown;
	public Dropdown categoryDropdown;
	public Dropdown gradeDropdown;
	public GameObject mainMenu;
	public GameObject axcptMenu;
	public GameObject mathMenu;
	public GameObject axcptTask;
	public GameObject mathTask;


	public void SelectTask() {
		var selectedTask = taskDropdown.value;
		switch (selectedTask) {
		case 0:
			mainMenu.SetActive (false);
			axcptMenu.SetActive (true);
			break;
		case 1:
			mainMenu.SetActive (false);
			mathMenu.SetActive (true);
			break;
		case 2:
			mainMenu.SetActive (false);
			break;
		}
	}

	public void BackToMainMenu() {
		axcptMenu.SetActive (false);
		mathMenu.SetActive (false);
		mainMenu.SetActive (true);
	}

	public void SelectAXCPTCategory() {
		var selectedCategory = categoryDropdown.value;
		CategoryLoader categoryloader = axcptTask.GetComponent (typeof(CategoryLoader)) as CategoryLoader;
		categoryloader.LoadCategory (selectedCategory);
		axcptMenu.SetActive (false);
		axcptTask.SetActive (true);
	}

	public void SelectMathGrade() {
		var selectedGrade = gradeDropdown.value + 1;
		GradeLoader gradeloader = mathTask.GetComponent (typeof(GradeLoader)) as GradeLoader;
		gradeloader.LoadGrade (selectedGrade);
		mathMenu.SetActive (false);
		mathTask.SetActive (true);
	}

}
