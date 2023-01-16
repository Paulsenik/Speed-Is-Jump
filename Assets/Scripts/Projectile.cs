using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour {

    private SpriteRenderer[] spriteRenderer;

    public GameObject trailParticle;
    public float particleDelay;
    private float delayTimer;

    [Header("Attributes")]
    public float lifeTime;
    public float speed;
    private float currentLifeTime = 0;

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

    [HideInInspector]
    public Vector2 targetDir;

    void Start() {
        spriteRenderer = GetComponentsInChildren<SpriteRenderer>();
    }

    // Update is called once per frame
    void Update() {
        currentLifeTime += Time.deltaTime;
        if (currentLifeTime >= lifeTime) { 
            die();
            return;
        }

        transform.Translate(targetDir.normalized * speed * Time.deltaTime);

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
        if (delayTimer >= particleDelay) {
            delayTimer = 0;
            trailParticle.transform.position = transform.position;
            trailParticle.transform.localScale = transform.localScale;
            Instantiate(trailParticle);
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
    public void die() {
        deathParticle.transform.position = transform.position;
        Instantiate(deathParticle);
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
