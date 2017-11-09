using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Text;
using System.Collections.Generic;
using System;
using System.IO;
using CrazyMinnow.SALSA;

public class AvatarTalkController : MonoBehaviour {

	public Animator myAnimator;
	public AudioSource aSource;
	public Salsa3D salsaRef = null;
    public AudioClip overrideAudioClip;

	[SerializeField] string triggerName = "";

	private Queue<Action> _actionQueue = new Queue<Action>();
	private Action _tempSavedAction = null;

	[SerializeField] float audioTimer = 0.0f, animationTimer = 0.0f;
	[SerializeField] bool runAudioTimer = false, runAnimationTimer = false;

	// Use this for initialization
	void Start () 
	{
		//myAnimator = gameObject.GetComponent<Animator>();
		//aSource = gameObject.GetComponent<AudioSource>();
		//salsaRef = gameObject.GetComponent<Salsa3D>();
	}

	public void PlayClipWithAnimation(AudioClip clipToPlay, string animTriggerName, Action actionToDo)
	{
		triggerName = animTriggerName;
        if (actionToDo != null)
            _tempSavedAction = actionToDo;
        else
        {
            _tempSavedAction = null;
        }
		if(salsaRef != null)
		{
			//----New system with lipsync - where scriptRef is the Salsa ref and Play is the stat function----//
			salsaRef.SetAudioClip(clipToPlay);
			salsaRef.Play();
			if(triggerName != null)
            {
                myAnimator.SetTrigger(animTriggerName);
            }
				//myAnimator.SetInteger(animTriggerName, triggerInt);
			//------------------------------------------------------------------------------------------------//
		}
		else
		{
			//----Old system - no lipsync----//
			aSource.clip = clipToPlay;
			aSource.Play();
            if (triggerName != null)
            {
                myAnimator.SetTrigger(animTriggerName);
            }
            //if (triggerName != null)
				//myAnimator.SetInteger(animTriggerName, triggerInt);
			//-------------------------------//
		}
		audioTimer = clipToPlay.length;
		runAudioTimer = true;
		//StartCoroutine(DelayStartTimer());
		
	}

	IEnumerator DelayStartTimer()
	{
		yield return new WaitForSeconds(0.0f);
		AnimatorTransitionInfo stateinfo = myAnimator.GetAnimatorTransitionInfo(0);

		//animationTimer = (stateinfo.length);
		if(animationTimer > 0.0f)
			runAnimationTimer = true;
		else
			ResetAnimator();
	}

	public void PlayClipWithoutAnimation(AudioClip clipToPlay, Action actionToDo)
	{
		_tempSavedAction = actionToDo;
		aSource.clip = clipToPlay;
		aSource.Play();
		audioTimer = clipToPlay.length;
		runAudioTimer = true;
	}

	public void ResetAnimator()
	{
		myAnimator.SetInteger(triggerName, 0);
		triggerName = "";
	}

	public void FinishedAudioCallback()
	{
        if(_tempSavedAction != null)
		    _actionQueue.Enqueue(_tempSavedAction);
	}
	
	// Update is called once per frame
	void Update () 
	{
		if(_actionQueue.Count > 0) 
		{
			Action a = _actionQueue.Dequeue();
			a();
		}

		if(runAudioTimer)
		{
			audioTimer -= Time.deltaTime;
			if(audioTimer <= 0.0f)
			{
				runAudioTimer = false;
				runAnimationTimer = false;

				if(triggerName != "")
				{
					ResetAnimator();
				}

				FinishedAudioCallback();
			}
		}

//        if (Input.GetKeyDown(UnityEngine.KeyCode.R))
//        {
//            PlayClipWithAnimation(overrideAudioClip,"Red",null);
//        }

        /*if(runAnimationTimer)
		{
			animationTimer -= Time.deltaTime;
			if(animationTimer <= 0.0f)
			{
				runAnimationTimer = false;
				ResetAnimator();
			}
		}*/
    }
}
