using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationKeith : MonoBehaviour {

	Animator childAnimator;

	void Start () {
		childAnimator = gameObject.GetComponent<Animator>();
	}

	void Update () {
		if (!childAnimator.IsInTransition(0) && childAnimator.GetCurrentAnimatorStateInfo (0).normalizedTime >= 1.3) { //Average animation is 21.5s -> every 1.3 loops change for about 30s
			var trigger = GetRandomTrigger ();
			childAnimator.SetTrigger (trigger);
			//print ("Trigger: " + trigger);
		}
		//print ("Normalized time: " + childAnimator.GetCurrentAnimatorStateInfo (0).normalizedTime);
	}

	private string GetRandomTrigger() {
		var sample = Random.value;
		if (sample < 0.5) {
			return "Loop1";
		}
		return "Loop2";
	}

}

