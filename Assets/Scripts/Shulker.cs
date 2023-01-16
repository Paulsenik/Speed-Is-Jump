using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Shulker : MonoBehaviour {
    private SpriteRenderer[] spriteRenderer;
    private Animator anim;
    public Transform eye;

    [Header("Attributes")]
    public GameObject deathParticle;
    public Transform attackBox;
    public Transform projectileSpawnpoint;
    public float damage;
    public float health;
    public bool canAttack = true;
    private bool hasPlayerDetected = false;

    [Header("HitBox")]
    public Transform hitbox;
    public float maxHitTime; // time until can be hit again
    protected bool isCurrentlyHit = false;
    private float hitTimer = 0f; // Time since last hit

    [Header("Abilities")]
    public float vision;
    public GameObject projectile;

    [Header("Animations")]
    public float scoutingIntervall;
    private float scoutingTimer = 0f;

    public UnityEvent deadEvent;

    private void Start() {
        anim = GetComponent<Animator>();
        spriteRenderer = GetComponentsInChildren<SpriteRenderer>();
    }

    // Update is called once per frame
    void Update() {

        scoutingTimer += Time.deltaTime;
        if (scoutingIntervall <= scoutingTimer) {
            if (!hasPlayerDetected) {
                anim.SetTrigger("Scouting");
                //Debug.Log("Scouting!");
            }
            scoutingTimer = 0;
        }

        if (Player.instance != null) {
            Vector2 dir = Player.instance.transform.position - transform.position;
            if (dir.magnitude < vision) {
                if (!hasPlayerDetected) { // Detected Player!
                    anim.SetTrigger("PlayerFound");
                    //Debug.Log("detected!");
                }
                hasPlayerDetected = true;
                rotateEyeTowardsPlayer();
                // player detected!
            } else {
                if (hasPlayerDetected) { // lost Player
                    anim.SetTrigger("PlayerOutOfReach");
                    //Debug.Log("ran away!");
                }
                hasPlayerDetected = false;
            }
        }

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

        if (canAttack && attackBox != null) {
            Transform hb = Player.instance.hitbox;
            if (new Bounds(hb.transform.position, hb.transform.localScale).Intersects(new Bounds(attackBox.position, attackBox.localScale))) {
                Player.instance.hit(gameObject);
            }
        }
    }
    private void rotateEyeTowardsPlayer() {
        Vector2 dir = (Player.instance.transform.position - eye.transform.position).normalized;

        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        eye.transform.rotation = Quaternion.AngleAxis(angle-90, Vector3.forward);
    }

    public void makeAttack() {
        Debug.Log("attacking the player!!");
        AudioManager.instance.play("Plop");
        Vector2 dir = (Player.instance.transform.position - transform.position).normalized;
        projectile.transform.position = projectileSpawnpoint.position;

        //projectile.transform.rotation = Quaternion.LookRotation(dir);
        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        projectile.GetComponentInChildren<SpriteRenderer>().gameObject.transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
        projectile.GetComponent<Projectile>().targetDir = dir;

        Instantiate(projectile);
    }

    public void hit(GameObject source) {
        if (!isCurrentlyHit) {
            isCurrentlyHit = true;

            // Do anything if is hit by...
            Player p = source.GetComponent<Player>();
            if (p != null) {
                health -= p.damage;
            }
        }
        if (health <= 0) {
            die();
        }
    }

    private void OnDrawGizmosSelected() {
        Gizmos.DrawWireSphere(transform.position, vision);
    }

    public void die() {
        deathParticle.transform.position = transform.position;
        Instantiate(deathParticle);
        deadEvent.Invoke();
        ScreenShakeController.instance.startShake(0.3f, 0.02f, 0.02f, 10f);
        AudioManager.instance.play("DeathSound");
        Destroy(gameObject);
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
}
