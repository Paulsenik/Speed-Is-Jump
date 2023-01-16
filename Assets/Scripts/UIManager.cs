using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour {

    public Image attackCooldown;
    public static UIManager instance; // Singleton
    public Text health;
    public Slider speedSlider, jumpSlider;

    private void Start() {
        instance = this;
    }

    void Update() {
        if (Player.instance.health > 0)
            health.text = Player.instance.health.ToString();
        else
            health.text = "0";

        float cooldown = PlayerAttack.instance.attackTimer / PlayerAttack.instance.cooldown;
        cooldown = 1f - (cooldown > 1f ? 1f : cooldown);
        attackCooldown.fillAmount = cooldown;
    }

    public void updateSpeedSlider() {
        speedSlider.value = 1 - jumpSlider.value;
    }
    public void updateJumpSlider() {
        jumpSlider.value = 1 - speedSlider.value;
    }

    public static void updateSpeed(float f) {
        Player.changeSpeed(f);
    }
    public static void updateJump(float f) {
        Player.changeJump(f);
    }
}
