using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StoryLine : MonoBehaviour{

    public bool skipAfterDuration = false;
    public float duration;

    private Animator anim;

    private void Awake() {
        anim = GetComponent<Animator>();
    }

    public void fadeIn() {
        anim.SetTrigger("In");
    }

    public void fadeOut() {
        anim.SetTrigger("Out");
    }

}
