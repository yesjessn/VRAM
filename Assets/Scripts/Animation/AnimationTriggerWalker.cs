using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationTriggerWalker : MonoBehaviour {

    public Animator animator;
	// Use this for initialization
	void Start () {
		
	}

    public void TriggerWalk()
    {
        animator.SetTrigger("Walk");
    }
	
	// Update is called once per frame
	void Update () {
		
	}
}
