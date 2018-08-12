using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TVController : MonoBehaviour {
    const float VHS_LENGTH = 15;

    bool VCRONFIRSTTIME;

    [SerializeField]
    private GoalsSheet goalSheet;
    
    [SerializeField]
    private Instructions instructions;

    private Animator tVAnimator;

    [SerializeField]
    private List<AudioSource> VCRClicks;

    [SerializeField]
    private Animator VCRAnimator;

    [SerializeField]
    private AnimationController animationController;

    [SerializeField]
    private bool TVIsOn;

    [SerializeField]
    private bool VCRIsOn;

    [SerializeField]
    float startTime = 0;

    int channel = 0;

    [SerializeField]
    private Text VCRChannelText;

    [SerializeField]
    private Text VCRText;

    [SerializeField]
    private Text VCRTapeTime;

    [SerializeField]
    private bool recording;

    [SerializeField]
    private float recStartAnimTime;

    [SerializeField]
    private float recStartTapeTime;

    [SerializeField]
    private bool playing;
    // Time in the animation we started playing at.

    [SerializeField]
    private float playStartTime;
    // Position we started playing the tape at.

    [SerializeField]
    private float playStartPosition;

    [SerializeField]
    private float playHeadPosition;


    [SerializeField]
    private bool WasPlayingDuringSeek;

    [SerializeField]
    private Button Save;

    [SerializeField]
    private InputField SaveOutput;
    
    [SerializeField]
    private InputField LoadInput;
    
    [SerializeField]
    private Button Load;

    [SerializeField]
    private Button Wipe;

    [SerializeField]
    private bool FFing;

    [SerializeField]
    private bool RWing;

    private VHSData vhsData;

    private Timestamp activePlayingTimestamp;

    [SerializeField]
    private bool PlayingTransitionWhiteNoise;

    [SerializeField]
    private float TransitionWhiteNoiseStart;
    
    const float TRANSITION_DURATION = .2F;

    // Use this for initialization
    void Start() {
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
        TVIsOn = !TVIsOn;
        tVAnimator.SetBool("TV", TVIsOn);
        CheckTVPlayState();
    }

    //Called when tv is turned on or off or vcr is turned on or off to check tv state.
    public void CheckTVPlayState() {
        if (VCRIsOn && TVIsOn && !playing && !((FFing || RWing)&&WasPlayingDuringSeek )) {
            animationController.StopAllAudio();
            animationController.PlayAnimationAt(GetTimePassed(), 0);
        } else if (VCRIsOn && TVIsOn && playing) {
            startAudioAgain();
        } else {
            animationController.StopAllAudio();
        }
    }

    public void CheckVCRPlayState() {
        if (!VCRIsOn) {
            VCRText.text = "";
            VCRTapeTime.text = "";
            VCRChannelText.text = "";
        } else {
            if (!playing) {
                VCRText.text = "STOP";
            }
            UpdateVCRDisp();
            SetChannelDisp();
        }
    }

    public void SetChannelDisp() {
        VCRChannelText.text = "CH" + GetHumanChannel().ToString();
    }

    public void TurnOnVCR() {
        VCRIsOn = !VCRIsOn;

        if (!VCRONFIRSTTIME) {
            VCRONFIRSTTIME = true;
            startTime = Time.realtimeSinceStartup;
        }

        CheckTVPlayState();
        CheckVCRPlayState();
        if (!VCRIsOn) {
            StopLogic();
        }
        VCRAnimator.SetBool("VCR", VCRIsOn);
        PlayVCRClick();
    }

    public void PlayVCRClick() {
        VCRClicks[Random.Range(0, VCRClicks.Count)].Play();
    }

    public void PlayButton() {
        PlayVCRClick();
        if (!VCRIsOn) {
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
        if (!VCRIsOn) {
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
        PlayingTransitionWhiteNoise = false;
        // Do logic to keep track of play time
        StopRecording();
        CheckTVPlayState();
        playStartPosition = playHeadPosition;
        activePlayingTimestamp = null;
        vhsData.RemoveClipsSmallerThan(.5f);
        CheckIfSolution();
    }

    public void CheckIfSolution() {
        int solution = vhsData.IsSolution();
        if (solution != -1) {
            goalSheet.SetGoal(solution);
        }
    }

    public void PrintVHS() {
        vhsData.Print();
    }

    public void SeekCleanup() {
        playing = false;
        FFing = false;
        RWing = false;

        // Do logic to keep track of play time
        StopRecording();
        playStartPosition = playHeadPosition;
        activePlayingTimestamp = null;
        vhsData.RemoveClipsSmallerThan(.5f);
    }

    public void RecButton() {
        PlayVCRClick();
        if (!VCRIsOn) {
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
        if (!VCRIsOn) {
            return;
        }
        WasPlayingDuringSeek |= playing;
        VCRText.text = "FF";
        SeekCleanup();
        // Do logic to keep track of play time, update play position and
        // swap channels and animations accordingly.
        FFing = true;
    }

    public void RWButton() {
        PlayVCRClick();
        if (!VCRIsOn) {
            return;
        }
        
        WasPlayingDuringSeek |= playing;
        SeekCleanup();
        VCRText.text = "RW";
        // Do logic to keep track of play time, update play position and
        // swap channels and animations accordingly.
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
        return;
        if (!VCRIsOn) {
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
        if (!VCRIsOn) {
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
        if (GetTimePassed() > 89 && !playing) {
            if (recording) {
                StopRecording();
                while(GetTimePassed() > 89) {
                    startTime += 89;
                }
                StopLogic();
                StartRecording();
            } else {
                while (GetTimePassed() > 89) {
                    startTime += 89;
                }
            }
            CheckTVPlayState();
        }

        if ((playing && !PlayingTransitionWhiteNoise) || recording) {
            playHeadPosition = playStartPosition + (GetTimePassed() - playStartTime);
            if (playHeadPosition > VHS_LENGTH) {
                playHeadPosition = VHS_LENGTH;
                EndOfTape();
            }
        }
        if (TVIsOn && playing) {
            PlayingUpdateCall();
        }
        
        if (FFing || RWing) {
            SeekLogic();
        }

        UpdateVCRDisp();
    }

    public void WipeHandler() {
        vhsData.Wipe();
    }

    public void SaveHandler() {
        SaveOutput.text = vhsData.SerializeToString();
    }

    public void LoadHandler() {
        vhsData.Load(LoadInput.text);
        CheckIfSolution();
    }

    public void GoalHander() {
        goalSheet.ShowSheet();
    }

    public void InsructionHandler() {
        instructions.ShowSheet();
    }

    public void IHaveTheSugaredWheats() {
        string solution1 = "63.5, 68 -" +
            "57.5, 61";
        vhsData.FastLoad(solution1);
    }

    public void SnakeMassacre() {
        string solution1 = "11, 16.8 -" +
            "0, 6";
        vhsData.FastLoad(solution1);
    }

    public void DefeatMumitorWithLook() {
        string solution1 = "27.8, 30.1 -" +
            "78.8, 81.2";
        vhsData.FastLoad(solution1);
    }

    public void NewFriendTeachesManners() {
        string solution1 = "38.1, 40.84 -" +
            "21.2, 22.5 -" +
             "50.77, 52.5";
        vhsData.FastLoad(solution1);
    }

    public void LionManTeachesManners() {
        string solution1 = "38.1, 40.84 -"+
            "70.0, 71.35 -"+
             "76.4, 78.8 -"+
             "50.77, 52.5";
        vhsData.FastLoad(solution1);
    }

    public void SugaredWheatsSaveAll() {
        string solution1 = "31, 35.2 -" +
            "46.07, 48.2 -" +
            "79, 84 -" +
            "56, 59";
        vhsData.FastLoad(solution1);
    }
}
