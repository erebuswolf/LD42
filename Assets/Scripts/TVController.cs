using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TVController : MonoBehaviour {
    private Animator tVAnimator;


    [SerializeField]
    private List<AudioSource> VCRClicks;

    [SerializeField]
    private Animator VCRAnimator;

    [SerializeField]
    private AnimationController animationController;

    private bool isOn;
    private bool VCRisOn;

    float startTime = 0;

    int channel = 0;

    [SerializeField]
    private Text VCRChannelText; 

    [SerializeField]
    private Text VCRText;

    [SerializeField]
    private Text VCRTapeTime;


    private bool playing;
    private float playStart;
    

    // Use this for initialization
    void Start () {
        startTime = Time.realtimeSinceStartup;
        tVAnimator = GetComponent<Animator>();
        if (tVAnimator == null) {
            Debug.LogError("Null animator in TVController");
        }
        CheckTVPlayState();
        CheckVCRPlayState();
    }

    public int GetHumanChannel() {
        return channel + 6;
    }
	
    public void ToggleTV() {
        PlayVCRClick();
        isOn = !isOn;
        tVAnimator.SetBool("TV", isOn);
        CheckTVPlayState();
    }
    
    public void CheckTVPlayState() {
        if (VCRisOn && isOn) {
            animationController.PlayAnimationAt(GetTimePassed(), 0);
        } else {
            animationController.StopAllAnimations();
        }
    }
    public void CheckVCRPlayState() {
        if (!VCRisOn) {
            VCRText.text = "";
            VCRTapeTime.text = "";
            VCRChannelText.text = "";
        } else {
            VCRText.text = "STOP";
            VCRTapeTime.text = "00:00";
            SetChannelDisp();
        }
    }

    public void SetChannelDisp() {
        VCRChannelText.text = "CH" + GetHumanChannel().ToString();
    }

    public void TurnOnVCR() {
        VCRisOn = !VCRisOn;
        CheckTVPlayState();
        CheckVCRPlayState();
        VCRAnimator.SetBool("VCR", VCRisOn);
        PlayVCRClick();
    }

    public void PlayVCRClick() {
        VCRClicks[Random.Range(0, VCRClicks.Count)].Play();
    }

    public void PlayButton() {
        PlayVCRClick();
        if (!VCRisOn) {
            return;
        }
        VCRText.text = "PLAY";
        // Do logic to keep track of play time, update play position and
        // swap channels and animations accordingly.
    }

    private void PlayLogic() {
        if (playing) {
            return;    
        }
        playing = true;
        playStart = Time.realtimeSinceStartup;
    }

    public void StopButton() {
        PlayVCRClick();
        if (!VCRisOn) {
            return;
        }
        VCRText.text = "STOP";

        playing = false;
        // Do logic to keep track of play time
    }
    
    public void RecButton() {
        PlayVCRClick();
        if (!VCRisOn) {
            return;
        }
        VCRText.text = "REC";
        // Do logic to keep track of play time, update play position and
        // swap channels and animations accordingly.
    }

    public void FFButton() {
        PlayVCRClick();
        if (!VCRisOn) {
            return;
        }
        VCRText.text = "FF";
        // Do logic to keep track of play time, update play position and
        // swap channels and animations accordingly.
    }

    public void RWButton() {
        PlayVCRClick();
        if (!VCRisOn) {
            return;
        }
        VCRText.text = "RW";
        // Do logic to keep track of play time, update play position and
        // swap channels and animations accordingly.
    }

    public void ChannelUpButton() {
        PlayVCRClick();
        if (!VCRisOn) {
            return;
        }
        channel++;
        if (channel > 3) {
            channel = 0;
        }
        SetChannelDisp();
        // Do logic to keep track of play time, update play position and
        // swap channels and animations accordingly.
    }

    public void ChannelDownButton() {
        PlayVCRClick();
        if (!VCRisOn) {
            return;
        }
        channel--;

        if (channel < 0) {
            channel = 3;
        }
        SetChannelDisp();
        // Do logic to keep track of play time, update play position and
        // swap channels and animations accordingly.
    }


    public float GetTimePassed() {
        return Time.realtimeSinceStartup - startTime;
    }
    
	// Update is called once per frame
	void Update () {

    }
}
