using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationAbstraction : MonoBehaviour {
    private Animator animator;
    private AudioSource audioSource;

    // Use this for initialization
    void Start () {
        animator = GetComponent<Animator>();
        audioSource = GetComponent<AudioSource>();
        if (animator == null || audioSource == null) {
            Debug.LogError("Animation abstraction created with null animator or audio source");
        }
    }

    public void PlayAtTime(float time) {
        var info = animator.GetCurrentAnimatorStateInfo(0);
        animator.Play("Channel1", -1, time/ info.length);
        audioSource.Stop();
        if (audioSource.clip.length < time) {
            Debug.LogWarningFormat("time is longer than clip {0} > {1}", time, audioSource.clip.length);
            return;
        }
        audioSource.time = time;
        audioSource.Play();
    }

    public float GetAnimationTime() {
        var info = animator.GetCurrentAnimatorStateInfo(0);
        return info.normalizedTime * info.length;
    }

    public void Stop() {
        audioSource.Stop();
    }

    // Update is called once per frame
    void Update () {
		
	}
}
