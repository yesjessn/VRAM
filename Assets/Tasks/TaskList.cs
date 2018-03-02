using UnityEngine;

using AXCPT;
using Math;
using VerbalStroop;

// The "master" task list. This gets a "DoNotDestroy" and will persist through scenes and only loaded onces
// (along with the tasks themselves). Use `instance` to get this singleton and the tasks as well as the active task.
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