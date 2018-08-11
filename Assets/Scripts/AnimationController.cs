using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationController : MonoBehaviour {

    [SerializeField] private List<AnimationAbstraction> animationAbstractions;
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void StopAllAnimations() {
        foreach(AnimationAbstraction aa in animationAbstractions) {
            aa.Stop();
        }
    }

    public void PlayAnimationAt(float time, int channel) {
        if(channel < animationAbstractions.Count) {
            animationAbstractions[channel].PlayAtTime(time);
        }
    }
}
