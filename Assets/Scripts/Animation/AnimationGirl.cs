using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationGirl : MonoBehaviour {
	
	Animator childAnimator;

	void Start () {
		childAnimator = gameObject.GetComponent<Animator>();
	}

	void Update () {
		if (!childAnimator.IsInTransition(0) && childAnimator.GetCurrentAnimatorStateInfo (0).normalizedTime >= 2.6) { //Average animation is 11.2s -> every 2.6 loops change for about 30s
			var trigger = GetRandomTrigger ();
			childAnimator.SetTrigger (trigger);
			//print ("Trigger: " + trigger);
		}
		//print ("Normalized time: " + childAnimator.GetCurrentAnimatorStateInfo (0).normalizedTime);
	}

	private string GetRandomTrigger() {
		var sample = Random.value;
		if (sample < 0.2) {
			return "Loop1";
		}
		if (sample < 0.4) {
			return "Loop2";
		}
		if (sample < 0.6) {
			return "Loop3";
		}
		if (sample < 0.8) {
			return "Loop4";
		}
		return "Loop5";
	}

}
