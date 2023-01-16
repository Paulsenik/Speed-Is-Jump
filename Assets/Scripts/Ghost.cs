using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ghost : MonoBehaviour {

    private SpriteRenderer[] spriteRenderer;
    public GameObject TrailParticle;
    public float particleDelay;
    private float delayTimer;

    [Header("Attributes")]
    public GameObject deathParticle;
    public Transform attackBox;
    public float damage;
    public float health;
    public bool canAttack = true;

    [Header("HitBox")]
    public Transform hitbox;
    public float maxHitTime; // time until can be hit again
    protected bool isCurrentlyHit = false;
    private float hitTimer = 0f; // Time since last hit

    [Header("Abilities")]
    public float vision;
    public float speed;
    private bool isMoving;

    private void Start() {
        spriteRenderer = GetComponentsInChildren<SpriteRenderer>();
    }

    // Update is called once per frame
    void Update() {

        if (Player.instance != null) {
            Vector2 dir = Player.instance.transform.position - transform.position;
            if (dir.magnitude < vision) {
                //follow player
                transform.Translate(dir.normalized * speed * Time.deltaTime);
                isMoving = true;
            } else {
                isMoving = false;
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
        delayTimer += Time.deltaTime;
        if (delayTimer >= particleDelay && isMoving) {
            delayTimer = 0;
            TrailParticle.transform.position = transform.position;
            Instantiate(TrailParticle);
        }
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
