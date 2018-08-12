using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationAbstraction : MonoBehaviour {
    private Animator animator;
    private AudioSource audioSource;
    [SerializeField] string AnimationToPlay;
    private SpriteRenderer spriteRenderer;

    private bool setup;
    private void init() {
        if (setup) {
            return;
        }
        setup = true;
        animator = GetComponent<Animator>();
        audioSource = GetComponent<AudioSource>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        if (animator == null || audioSource == null || spriteRenderer == null) {
            Debug.LogError("Animation abstraction created with null animator or audio source");
        }
    }
    // Use this for initialization
    void Start () {
        init();
        spriteRenderer.enabled = false;
    }

    public void SetVis(bool vis) {
        spriteRenderer.enabled = vis;
    }

    public void PlayAtTime(float time) {
        init();
        var info = animator.GetCurrentAnimatorStateInfo(0);
        animator.Play(AnimationToPlay, -1, time/ info.length);
        audioSource.Stop();
        if(audioSource.clip == null) {
            return;
        }
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
        if (audioSource != null) {
            audioSource.Stop();
        }
    }

    // Update is called once per frame
    void Update () {
		
	}
}
