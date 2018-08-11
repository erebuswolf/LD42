using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TVController : MonoBehaviour {
    private Animator animator;

    // Use this for initialization
    void Start () {
        animator = GetComponent<Animator>();
        if (animator == null) {
            Debug.LogError("Null animator in TVController");
        }
    }
	
    public void ToggleTV() {
        animator.SetTrigger("ToggleOnOff");
        Debug.LogWarning("button pressed");
    }
    
	// Update is called once per frame
	void Update () {
		
	}
}
