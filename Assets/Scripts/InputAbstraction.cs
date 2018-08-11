using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class InputAbstraction : MonoBehaviour{
    private Dictionary<KeyCode, InputContainer> InputToActions = new Dictionary<KeyCode, InputContainer>();

    public void AddInputToAction(KeyCode key, InputContainer inputContiner) {
        InputToActions.Add(key, inputContiner);
    }

    private void Update() {
        foreach (KeyCode key in InputToActions.Keys) {
            if (InputToActions[key] == null) {
                continue;
            }
            if( Input.GetKeyDown(key) && InputToActions[key].OnKeyDown != null) {
                InputToActions[key].OnKeyDown.Invoke();
            } else if (Input.GetKey(key) && InputToActions[key].OnKeyHeld != null) {
                InputToActions[key].OnKeyHeld.Invoke();
            } else if(Input.GetKeyUp(key) && InputToActions[key].OnKeyUp != null) {
                InputToActions[key].OnKeyUp.Invoke();
            }
        }
    }
}

