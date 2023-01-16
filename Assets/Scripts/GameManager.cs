using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour {
    public static GameManager instance;
    private Animator anim;

    public Transform levelCompletedTrigger;
    public bool shouldLoadNextScene = true;
    public bool ShouldLoadNextScene { set { shouldLoadNextScene = value; } }

    private bool hasStartedNextLevel = false;

    void Start() {
        instance = this;
        anim = GameObject.FindGameObjectWithTag("Crossfader").GetComponent<Animator>();
    }

    public void Update() {
        if (shouldLoadNextScene) {
            Player p = Player.instance;
            if (p != null) {
                if (new Bounds(p.hitbox.position, p.hitbox.localScale).Intersects(new Bounds(levelCompletedTrigger.position, levelCompletedTrigger.localScale))) {
                    startLevelTransition();
                }
            }
        }
    }

    public void startLevelTransition() {
        if (!hasStartedNextLevel) {
            hasStartedNextLevel = true;
            AudioManager.instance.play("Spell");
            anim.SetTrigger("Start");
            Invoke("nextLevel", .5f);
        }
    }

    private void nextLevel() {
        if (SceneManager.sceneCountInBuildSettings > SceneManager.GetActiveScene().buildIndex + 1) {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
            hasStartedNextLevel = false;
            Debug.Log("[GameManager] :: starting next level");
        } else {
            Debug.Log("There is no next level \nRedirecting to first Level!");
            SceneManager.LoadScene(0);
            hasStartedNextLevel = false;
        }
    }

    public void gotBook() {
        stopPlayer();
        Invoke("startLevelTransition", 1f);
    }

    public void stopPlayer() {
        Player.instance.isPlayable = false;
        Rigidbody2D rb = Player.instance.GetComponent<Rigidbody2D>();
        rb.velocity = Vector2.zero;
        rb.gravityScale = 0;
    }

}
