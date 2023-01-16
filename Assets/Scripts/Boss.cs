using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boss : MonoBehaviour {

    Animator anim;

    private SpriteRenderer[] spriteRenderer;

    public GameObject drops; // gameobject will be dropped when dead
    public GameObject projectile;
    public GameObject deathParticle;

    [Header("Attributes")]
    public int state = 0; // 0 => 1. Stage 1 => 2. Stage
    public float sqrVision;

    public bool isPlayerInVision = false;

    [Header("HitBox")]
    public float health;
    public float currentHealth;
    private bool isTransitioning = false;
    public Transform hitbox;
    public float maxHitTime; // time until can be hit again
    private bool isCurrentlyHit = false;
    private float hitTimer = 0f; // Time since last hit

    void Start() {
        spriteRenderer = GetComponentsInChildren<SpriteRenderer>();
        anim = GetComponent<Animator>();
        currentHealth = health;
    }

    // Update is called once per frame
    void Update() {
        if (isCurrentlyHit) {
            hitTimer += Time.deltaTime;
            if (hitTimer >= maxHitTime) {
                isCurrentlyHit = false;
                hitTimer = 0;
            }
            //drawred
            isDamaged(true);
        } else {
            //drawwhite
            isDamaged(false);
        }

        if (Player.instance != null && (Player.instance.transform.position - transform.position).sqrMagnitude <= sqrVision && health > 0) {
            anim.SetBool("IsPlayerInRange", true);
            if (!isPlayerInVision)
                AudioManager.instance.play("BossDeath");
            isPlayerInVision = true;
        } else {
            anim.SetBool("IsPlayerInRange", false);
            isPlayerInVision = false;
        }
    }

    public void shoot() {
        if (isPlayerInVision) {
            AudioManager.instance.play("BossDeath");
            Vector2 dir = (Player.instance.transform.position - transform.position).normalized;
            projectile.transform.position = transform.position;

            float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
            projectile.GetComponentInChildren<SpriteRenderer>().gameObject.transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
            projectile.GetComponent<Projectile>().targetDir = dir;

            Instantiate(projectile);
        }
    }

    public void makeStateTransition() {
        isTransitioning = true;
        ScreenShakeController.instance.startShake(2f, .5f, 20f);
        AudioManager.instance.play("DeathSound");
        anim.SetTrigger("NextStageTransition");
        draw(Color.blue);
    }

    public void setNextStage() {
        draw(Color.white);
        anim.SetTrigger("SecondStage");
        isTransitioning = false;
    }

    public void hit(GameObject g) {
        if (!isCurrentlyHit && !isTransitioning) {
            isCurrentlyHit = true;

            Player enemy = g.GetComponent<Player>();
            // Do anything if is hit 
            if (enemy != null) {
                currentHealth -= enemy.damage;
            }
        }
        if (currentHealth <= 0) {
            if (state == 0) {
                currentHealth = health;
                
                state = 1;
                makeStateTransition();
            } else if (state == 1) {
                die();
            }
        }
    }

    public void die() {
        deathParticle.transform.position = transform.position;
        Instantiate(deathParticle);
        drops.transform.position = transform.position;
        ScreenShakeController.instance.startShake(1f, 0.1f, 0.1f, 15);
        AudioManager.instance.play("BossDeath");
        Destroy(gameObject, 1f);
    }

    public void isDamaged(bool isDamaged) {
        if (isDamaged)
            draw(new Color(255, 0, 0));
        else
            draw(new Color(255, 255, 255));
    }
    public void draw(Color color) {
        foreach (SpriteRenderer s in spriteRenderer) {
            s.color = color;
        }
    }

    private void OnDrawGizmosSelected() {
        Gizmos.DrawWireSphere(transform.position, (float)Math.Sqrt(sqrVision));
    }
}
