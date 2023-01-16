using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonAddon : MonoBehaviour {
    public void playClick() {
        AudioManager.instance.play("Click");
    }
}
