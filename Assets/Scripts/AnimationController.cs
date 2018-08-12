using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationController : MonoBehaviour {

    [SerializeField] private List<AnimationAbstraction> animationAbstractions;

    [SerializeField] private AnimationAbstraction whiteNoise;

    private bool PlayingWhiteNoise;

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
        
    }

    public void StopAllAnimations(bool keepWhiteNoise) {
        foreach(AnimationAbstraction aa in animationAbstractions) {
            aa.Stop();
            aa.gameObject.SetActive(false);
        }
        if(!keepWhiteNoise) {
            whiteNoise.Stop();
            whiteNoise.gameObject.SetActive(false);
            PlayingWhiteNoise = false;
        }
    }

    public void PlayAnimationAt(float time, int channel) {
        if(channel < animationAbstractions.Count && channel >=0 ) {
            whiteNoise.gameObject.SetActive(false);
            animationAbstractions[channel].gameObject.SetActive(true);
            animationAbstractions[channel].PlayAtTime(time);
            PlayingWhiteNoise = false;
        } else {
            if (PlayingWhiteNoise) {
                return;
            }
            whiteNoise.gameObject.SetActive(true);
            whiteNoise.PlayAtTime(0);
            PlayingWhiteNoise = true;
        }
    }
}
