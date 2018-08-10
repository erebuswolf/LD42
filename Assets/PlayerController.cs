using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour {
    private InputAbstraction inputAbstraction;

    // Use this for initialization
    void Start () {
        inputAbstraction = GetComponent<InputAbstraction>();
        if (inputAbstraction ==null) {
            Debug.LogError("Player Controller created with no input abstraction");
        }
        inputAbstraction.AddInputToAction(KeyCode.Space, new InputContainer(OnSpaceDown, OnSpaceHeld, OnSpaceUp));
    }
	
	// Update is called once per frame
	void Update () {
		
	}

    public void OnSpaceUp() {
        Debug.LogWarning("space up");
    }

    public void OnSpaceDown() {
        Debug.LogWarning("space down");
    }

    public void OnSpaceHeld() {
        Debug.LogWarning("space held");
    }
}
