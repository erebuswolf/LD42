using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TVController : MonoBehaviour {
    private Animator tVAnimator;

    [SerializeField]
    private Animator VCRAnimator;

    private bool isOn;

    private bool VCRisOn;
    
    // Use this for initialization
    void Start () {
        tVAnimator = GetComponent<Animator>();
        if (tVAnimator == null) {
            Debug.LogError("Null animator in TVController");
        }
    }
	
    public void ToggleTV() {
        isOn = !isOn;
        tVAnimator.SetBool("TV", isOn);
        Debug.LogWarning("button pressed");
    }
    
    public void TurnOnVCR() {
        VCRisOn = !VCRisOn;
        VCRAnimator.SetBool("VCR", VCRisOn);
    }

    public float GetAnimationTime() {
        var info = VCRAnimator.GetCurrentAnimatorStateInfo(0);
        return info.normalizedTime * info.length;
    }

	// Update is called once per frame
	void Update () {

    }
}
