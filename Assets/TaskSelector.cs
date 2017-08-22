using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using AXCPT;
using Math;

public class TaskSelector : MonoBehaviour {

    public static TaskSelector instance;
    public GameObject[] tasks;
	public Dropdown taskDropdown;
	public Dropdown categoryDropdown;
	public Dropdown gradeDropdown;
	public GameObject mainMenu;
	public GameObject axcptMenu;
	public GameObject mathMenu;
	public GameObject axcptTask;
	public GameObject axcptPractice;
	public GameObject mathTask;
	public GameObject verbalStroopTask;
    private GameObject _activeTask;

    void Awake()
    {
        if(instance != null)
        {
            Destroy(this.gameObject);
        }
        else
        {
            instance = this;
        }
    }

	public void SelectTask() {
		var selectedTask = taskDropdown.value;
		switch (selectedTask) {
		case 0:
			mainMenu.SetActive (false);
			axcptMenu.SetActive (true);
			break;
		case 1:
			mainMenu.SetActive (false);
			axcptPractice.SetActive (true);
			break;
		case 2:
			mainMenu.SetActive (false);
			mathMenu.SetActive (true);
			break;
		case 3:
			mainMenu.SetActive (false);
			verbalStroopTask.SetActive (true);
			break;
		}
	}

    public void ActivateActiveTask()
    {
        if (_activeTask != null)
            _activeTask.SetActive(true);
        else
            Debug.LogError("There was no task selected");
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
        _activeTask = axcptTask;
        SceneManager.LoadScene("VRClassRoom");
        //axcptTask.SetActive (true);
	}

	public void SelectMathGrade() {
		var selectedGrade = gradeDropdown.value + 3;
		GradeLoader gradeloader = mathTask.GetComponent (typeof(GradeLoader)) as GradeLoader;
		gradeloader.LoadGrade (selectedGrade);
		mathMenu.SetActive (false);
        _activeTask = mathTask;
        SceneManager.LoadScene("VRClassRoom");
        //mathTask.SetActive (true);
    }

}
