using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationController : MonoBehaviour {

    [SerializeField] private List<AnimationAbstraction> animationAbstractions;

    [SerializeField] private AnimationAbstraction whiteNoise;

    public bool PlayingWhiteNoise { get; private set; }

    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void StopAllAudio() {
        foreach (AnimationAbstraction aa in animationAbstractions) {
            aa.Stop();
        }
        whiteNoise.Stop();
        PlayingWhiteNoise = false;
    }

    public void StopAllAnimations(bool keepWhiteNoise) {
        foreach(AnimationAbstraction aa in animationAbstractions) {
            aa.Stop();
            aa.SetVis(false);
        }
        if(!keepWhiteNoise) {
            whiteNoise.Stop();
            whiteNoise.SetVis(false);
            PlayingWhiteNoise = false;
        }
    }

    public void PlayAnimationAt(float time, int channel) {
        StopAllAudio();
        if (channel < animationAbstractions.Count && channel >=0 ) {
            whiteNoise.SetVis(false);
            animationAbstractions[channel].SetVis(true);
            animationAbstractions[channel].PlayAtTime(time);
            PlayingWhiteNoise = false;
        } else {
            if (PlayingWhiteNoise) {
                return;
            }
            whiteNoise.PlayAtTime(0);
            whiteNoise.SetVis(true);
            PlayingWhiteNoise = true;
        }
    }
}
