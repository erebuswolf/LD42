using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TVController : MonoBehaviour {
    const float VHS_LENGTH = 90;

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

    private bool recording;
    private float recStartAnimTime;
    private float recStartTapeTime;

    private bool playing;
    // Time in the animation we started playing at.
    private float playStartTime;
    // Position we started playing the tape at.
    private float playStartPosition;
    private float playHeadPosition;

    private bool WasPlayingDuringSeek;

    private bool FFing;
    private bool RWing;

    private VHSData vhsData;

    private Timestamp activePlayingTimestamp;

    // Use this for initialization
    void Start() {
        startTime = Time.realtimeSinceStartup;
        tVAnimator = GetComponent<Animator>();
        if (tVAnimator == null) {
            Debug.LogError("Null animator in TVController");
        }
        CheckTVPlayState();
        CheckVCRPlayState();
        vhsData = new VHSData();
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
        if (VCRisOn && isOn && !playing && !FFing && !RWing) {
            animationController.StopAllAudio();
            animationController.PlayAnimationAt(GetTimePassed(), 0);
        } else if (VCRisOn && isOn && playing) {
            startAudioAgain();
        } else {
            animationController.StopAllAudio();
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
        if (!VCRisOn) {
            StopLogic();
        }
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
        if (playing) {
            return;
        }
        VCRText.text = "PLAY";
        // Do logic to keep track of play time, update play position and
        // swap channels and animations accordingly.
        StopLogic();
        PlayLogic();
    }

    private void PlayLogic() {
        if (playing) {
            return;
        }
        playing = true;
        playStartTime = GetTimePassed();
        CheckVCRPlayState();
    }

    private bool PlayingTransitionWhiteNoise;
    private float TransitionWhiteNoiseStart;
    const float TRANSITION_DURATION = .2F;

    private void startAudioAgain() {
        if (activePlayingTimestamp != null && vhsData.GetTimestampAtHead(playHeadPosition) == activePlayingTimestamp) {
            float timeIntoAnimation = (playHeadPosition - activePlayingTimestamp.TapeStart);
            animationController.PlayAnimationAt(activePlayingTimestamp.AnimStart + timeIntoAnimation, activePlayingTimestamp.Channel);
        } else {
            Debug.LogWarningFormat("starting audio again {0} {1}", activePlayingTimestamp != null, vhsData.GetTimestampAtHead(playHeadPosition) == activePlayingTimestamp);

            animationController.PlayAnimationAt(0, -1);
        }
    }

    private void PlayingUpdateCall() {
        if (activePlayingTimestamp == null || vhsData.GetTimestampAtHead(playHeadPosition) != activePlayingTimestamp) {
            if (!PlayingTransitionWhiteNoise) {
                PlayingTransitionWhiteNoise = true;
                animationController.StopAllAnimations(true);
                animationController.PlayAnimationAt(0, -1);
                TransitionWhiteNoiseStart = GetTimePassed();
                return;
            } else if (GetTimePassed() - TransitionWhiteNoiseStart > TRANSITION_DURATION) {
                PlayingTransitionWhiteNoise = false;
                animationController.StopAllAnimations(true);
                activePlayingTimestamp = vhsData.GetTimestampAtHead(playHeadPosition);
            } else if (PlayingTransitionWhiteNoise) {
                return;
            }

            if (activePlayingTimestamp == null) {
                //play white noise
                if (!animationController.PlayingWhiteNoise) {
                    animationController.PlayAnimationAt(0, -1);
                }
            } else {
                animationController.PlayAnimationAt(activePlayingTimestamp.AnimStart + playHeadPosition - activePlayingTimestamp.TapeStart, activePlayingTimestamp.Channel);
            }
        }
    }

    public void StopButton() {
        PlayVCRClick();
        if (!VCRisOn) {
            return;
        }
        StopLogic();
        VCRText.text = "STOP";
    }

    public void StopLogic() {
        playing = false;
        FFing = false;
        RWing = false;
        WasPlayingDuringSeek = false;

        // Do logic to keep track of play time
        StopRecording();
        CheckTVPlayState();
        playStartPosition = playHeadPosition;
        activePlayingTimestamp = null;
        vhsData.RemoveClipsSmallerThan(.5f);
    }

    public void SeekCleanup() {
        playing = false;
        FFing = false;
        RWing = false;

        // Do logic to keep track of play time
        StopRecording();
        CheckTVPlayState();
        playStartPosition = playHeadPosition;
        activePlayingTimestamp = null;
        vhsData.RemoveClipsSmallerThan(.5f);
    }

    public void RecButton() {
        PlayVCRClick();
        if (!VCRisOn) {
            return;
        }
        if (recording) {
            return;
        }
        StopLogic();
        VCRText.text = "REC";
        // Do logic to keep track of play time, update play position and
        // swap channels and animations accordingly.
        StartRecording();
    }

    public void StartRecording() {
        recording = true;
        recStartAnimTime = GetTimePassed();
        recStartTapeTime = playHeadPosition;
        playStartTime = GetTimePassed();
    }

    public void StopRecording() {
        FFing = false;
        RWing = false;
        playStartPosition = playHeadPosition;
        if (!recording) {
            return;
        }
        recording = false;
        float recLength = GetTimePassed() - recStartAnimTime;
        vhsData.AddTimestamp(new Timestamp(channel, recStartTapeTime, recStartTapeTime + recLength, recStartAnimTime));
    }

    public void FFButton() {
        PlayVCRClick();
        if (!VCRisOn) {
            return;
        }
        WasPlayingDuringSeek |= playing;
        VCRText.text = "FF";
        SeekCleanup();
        // Do logic to keep track of play time, update play position and
        // swap channels and animations accordingly.
        StopRecording();
        FFing = true;
    }

    public void RWButton() {
        PlayVCRClick();
        if (!VCRisOn) {
            return;
        }
        
        WasPlayingDuringSeek |= playing;
        SeekCleanup();
        VCRText.text = "RW";
        // Do logic to keep track of play time, update play position and
        // swap channels and animations accordingly.
        StopRecording();
        RWing = true;
    }

    public void ChannelUpButton() {
        ChangeChannel(1);
    }

    public void ChannelDownButton() {
        ChangeChannel(-1);
    }

    public void ChangeChannel(int change) {
        PlayVCRClick();
        if (!VCRisOn) {
            return;
        }

        bool handleRecordingLogic = recording;
        if (handleRecordingLogic) {
            StopRecording();
        }

        channel += change;
        if (channel < 0) {
            channel = 3;
        } else if (channel > 3) {
            channel = 0;
        }
        SetChannelDisp();
        // Do logic to keep track of play time, update play position and
        // swap channels and animations accordingly.
        if (handleRecordingLogic) {
            StartRecording();
        }
    }

    public float GetTimePassed() {
        return Time.realtimeSinceStartup - startTime;
    }

    public void UpdateVCRDisp() {
        if (!VCRisOn) {
            return;
        }
        System.TimeSpan time = System.TimeSpan.FromSeconds(playHeadPosition);
        VCRTapeTime.text = string.Format("{0:00}:{1:00}", time.Minutes, time.Seconds);
    }

    public void EndOfTape() {
        PlayVCRClick();
        bool wasPlaying = WasPlayingDuringSeek;
        StopLogic();
        VCRText.text = "STOP";
        if (wasPlaying) {
            VCRText.text = "PLAY";
            PlayLogic();
        }
    }

    private void SeekVisual() {
        var ts = vhsData.GetTimestampAtHead(playHeadPosition);
        if (ts != null) {
            animationController.PlayAnimationAt(ts.AnimStart + playHeadPosition - ts.TapeStart, ts.Channel);
        } else {
            animationController.PlayAnimationAt(0, -1);
        }
    }

    public void SeekLogic() {
        if (FFing) {
            playHeadPosition = playHeadPosition + Time.deltaTime * 8;
            if (playHeadPosition > VHS_LENGTH) {
                playHeadPosition = VHS_LENGTH;
                EndOfTape();
            } else {
                if (WasPlayingDuringSeek) {
                    SeekVisual();
                }
            }
        } else if (RWing) {
            playHeadPosition = playHeadPosition - Time.deltaTime * 8;
            if (playHeadPosition < 0) {
                playHeadPosition = 0;
                EndOfTape();
            } else {
                if (WasPlayingDuringSeek) {
                    SeekVisual();
                }
            }
        }
    }

	// Update is called once per frame
	void Update () {
        if ((playing && !PlayingTransitionWhiteNoise) || recording) {
            playHeadPosition = playStartPosition + (GetTimePassed() - playStartTime);
            if (playHeadPosition > VHS_LENGTH) {
                playHeadPosition = VHS_LENGTH;
                EndOfTape();
            }
        }
        if (isOn && playing) {
            PlayingUpdateCall();
        }
        
        if (FFing || RWing) {
            SeekLogic();
        }

        UpdateVCRDisp();
    }
}
