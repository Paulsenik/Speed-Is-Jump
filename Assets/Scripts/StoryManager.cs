using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class StoryManager : MonoBehaviour {

    public StoryLine[] lines;

    public int currentIndex = 0;

    private float cTime;

    private void Start() {
        lines[currentIndex].fadeIn();
    }

    // Update is called once per frame
    void Update() {
        cTime += Time.deltaTime;

        if (Input.anyKeyDown) {
            setNextLine();
        }
        if (lines[currentIndex].skipAfterDuration)
            if (cTime >= lines[currentIndex].duration) {
                setNextLine();
            }
    }

    private void FixedUpdate() {
    }

    private void setNextLine() {
        cTime = 0;
        lines[currentIndex].fadeOut();

        if (lines.Length > currentIndex + 1) {
            lines[++currentIndex].fadeIn();
        } else {
            GameManager.instance.startLevelTransition();
        }
    }
}
