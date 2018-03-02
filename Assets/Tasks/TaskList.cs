using UnityEngine;

using AXCPT;
using Math;
using VerbalStroop;

public class TaskList : MonoBehaviour {
    public static TaskList instance;

    public AXCPT.AXCPT axcpt;
    public Math.Math math;
    public VerbalStroop.VerbalStroop verbalStroop;
    public NoTask noTask;

    private VRAMTask activeTask;


    void Awake() {
        if(instance != null) {
            Destroy(this.gameObject);
        } else {
            instance = this;
        }
    }

    public void SetActiveTask(VRAMTask task) {
        this.activeTask = task;
    }

    public void ClearActiveTask() {
        this.activeTask = null;
    }

    public VRAMTask GetActiveTask() {
        return this.activeTask;
    }
}