using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class InputContainer {
    public Action OnKeyUp { get; private set; }
    public Action OnKeyDown { get; private set; }
    public Action OnKeyHeld { get; private set; }

    public InputContainer() {

    }

    public InputContainer(Action onKeyDown, Action onKeyHeld, Action onKeyUp) {
        this.OnKeyUp = onKeyUp;
        this.OnKeyDown = onKeyDown;
        this.OnKeyHeld = onKeyHeld;
    }
}
