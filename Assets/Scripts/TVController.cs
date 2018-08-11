using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TVController : MonoBehaviour {
    private Animator tVAnimator;

    [SerializeField]
    private Animator VCRAnimator;

    [SerializeField]
    private AnimationController animationController;

    private bool isOn;
    private bool VCRisOn;

    float startTime = 0;
    
    // Use this for initialization
    void Start () {
        startTime = Time.realtimeSinceStartup;
        tVAnimator = GetComponent<Animator>();
        if (tVAnimator == null) {
            Debug.LogError("Null animator in TVController");
        }
    }
	
    public void ToggleTV() {
        isOn = !isOn;
        tVAnimator.SetBool("TV", isOn);
        Debug.LogWarning("button pressed");
        CheckTVPlayState();
    }
    
    public void CheckTVPlayState() {
        if (VCRisOn && isOn) {
            animationController.PlayAnimationAt(GetTimePassed(), 0);
        } else {
            animationController.StopAllAnimations();
        }
    }

    public void TurnOnVCR() {
        VCRisOn = !VCRisOn;
        CheckTVPlayState();
        VCRAnimator.SetBool("VCR", VCRisOn);
    }

    public float GetTimePassed() {
        Debug.LogWarningFormat("time given {0}", Time.realtimeSinceStartup - startTime);
        return Time.realtimeSinceStartup - startTime;
    }
    
	// Update is called once per frame
	void Update () {

    }
}
