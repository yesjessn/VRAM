using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using AXCPT;
using Math;
using VerbalStroop;
using Subject;

// Class responsible for turning menu -> running task.
public class TaskSelector : MonoBehaviour {
    public GameObject[] tasks;
	public InputField subjectField;
	public Dropdown taskDropdown;
	public Dropdown categoryDropdown;
	public Dropdown gradeDropdown;
	public GameObject mainMenu;
	public GameObject axcptMenu;
	public GameObject mathMenu;

	private TaskList taskList;
	private SubjectDataHolder subject;
	
    private VRAMTask _activeTask;
	public VRAMTask activeTask {get {return _activeTask;}}

    void Awake() {
		taskList = TaskList.instance;
		subject = FindObjectOfType<SubjectDataHolder>();
    }

	public void SelectTask() {
		var selectedTask = taskDropdown.value;
		switch (selectedTask) {
		case 0:
			if (taskList.axcpt != null) {
				mainMenu.SetActive (false);
				axcptMenu.SetActive (true);
				taskList.axcpt.GetComponent<AXCPTPractice>().enabled = false;
			}
			break;
		case 1:
			if (taskList.axcpt != null) {
				mainMenu.SetActive (false);
				CategoryLoader categoryloader = taskList.axcpt.GetComponent (typeof(CategoryLoader)) as CategoryLoader;
				categoryloader.LoadCategoryByString ("Shapes");
				taskList.axcpt.GetComponent<AXCPTPractice>().enabled = true;
				StartTask(taskList.axcpt);
			}
			break;
		case 2:
			if (taskList.math != null) {
				mainMenu.SetActive (false);
				mathMenu.SetActive (true);
				taskList.math.GetComponent<MathPractice>().enabled = false;
			}
			break;
		case 3: 
			if (taskList.math != null) {
				mainMenu.SetActive (false);
				taskList.math.GetComponent<MathPractice>().enabled = true;
				StartTask(taskList.math);
			}
			break;
		case 4:
			if (taskList.verbalStroop != null) {
				mainMenu.SetActive (false);
				taskList.verbalStroop.GetComponent<VerbalStroopPractice>().enabled = false;
				StartTask(taskList.verbalStroop);
			}
			break;
		case 5:
			if (taskList.verbalStroop != null) {
				taskList.verbalStroop.GetComponent<VerbalStroopPractice>().enabled = true;
				StartTask(taskList.verbalStroop);
			}
			break;
		case 6:
			if (taskList.noTask != null) {
				StartTask(taskList.noTask);
			}
			break;
		}
	}

	private void StartTask(VRAMTask task) {
		taskList.SetActiveTask(task);
		subject.SetSubjectId(subjectField.text);
		SceneManager.LoadScene ("VRClassRoom");
	}

	public void BackToMainMenu() {
		axcptMenu.SetActive (false);
		mathMenu.SetActive (false);
		mainMenu.SetActive (true);
	}

	public void SelectAXCPTCategory() {
		var selectedCategory = categoryDropdown.value;
		CategoryLoader categoryloader = taskList.axcpt.GetComponent (typeof(CategoryLoader)) as CategoryLoader;
		categoryloader.LoadCategory (selectedCategory);
		axcptMenu.SetActive (false);
        StartTask(taskList.axcpt);
	}

	public void SelectMathGrade() {
		var selectedGrade = gradeDropdown.value + 3;
		GradeLoader gradeloader = taskList.math.GetComponent (typeof(GradeLoader)) as GradeLoader;
		gradeloader.LoadGrade (selectedGrade);
		mathMenu.SetActive (false);
        StartTask(taskList.math);
    }
}
