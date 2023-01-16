using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAttack : MonoBehaviour {

    public static PlayerAttack instance;

    public GameObject attackParticle;
    private Animator anim;
    private bool hasAnimationTriggered = false;

    [Header("Attributes")]
    public float cooldown;
    public float attackDuration;
    public Transform attackBox;
    [HideInInspector]
    public float attackTimer = float.MaxValue, currentAttackDuration = 0;// time since last attack & duration of current attack
    private bool isAttack = false;

    private void Start() {
        instance = this;
        anim = GetComponent<Animator>();
    }

    void Update() {

        if (isAttack) {
            currentAttackDuration += Time.deltaTime;
            if (currentAttackDuration >= attackDuration)
                isAttack = false;
        }
        // Input
        if (Input.GetKeyDown(Player.attackKey) && !isAttack && attackTimer >= cooldown) {
            isAttack = true;
            attackTimer = 0;
            currentAttackDuration = 0;
        }
        attackTimer += Time.deltaTime;

        if (isAttack) {
            anim.SetBool("isCurrentlyAttacking", true);
            AudioManager.instance.play("Spell");
            if (!hasAnimationTriggered) {
                anim.SetTrigger("isAttacking");
                hasAnimationTriggered = true;
                attackParticle.transform.position = attackBox.transform.position;

                ParticleSystem ps = attackParticle.GetComponent<ParticleSystem>();
                var sh = ps.shape;
                sh.scale = attackBox.transform.localScale;

                Instantiate(attackParticle);
            }
            GameObject[] enemys = GameObject.FindGameObjectsWithTag("Enemy");

            foreach (GameObject e in enemys) {

                // Add new damagable enemys here
                Ghost ghost = e.GetComponent<Ghost>();
                Shulker shulker = e.GetComponent<Shulker>();
                Projectile pro = e.GetComponent<Projectile>();
                Boss boss = e.GetComponent<Boss>();

                if (ghost != null) {
                    if (new Bounds(ghost.hitbox.position, ghost.hitbox.localScale).Intersects(new Bounds(attackBox.transform.position, attackBox.transform.localScale))) {
                        ghost.hit(gameObject);
                    }
                } else if (shulker != null) {
                    if (new Bounds(shulker.hitbox.position, shulker.hitbox.localScale).Intersects(new Bounds(attackBox.transform.position, attackBox.transform.localScale))) {
                        shulker.hit(gameObject);
                    }
                } else if (pro != null) {
                    if (new Bounds(pro.hitbox.position, pro.hitbox.localScale).Intersects(new Bounds(attackBox.transform.position, attackBox.transform.localScale))) {
                        pro.hit(gameObject);
                    }
                } else if (boss != null) {
                    if (new Bounds(boss.hitbox.position, boss.hitbox.localScale).Intersects(new Bounds(attackBox.transform.position, attackBox.transform.localScale))) {
                        boss.hit(gameObject);
                    }
                }
            }
        } else {
            anim.SetBool("isCurrentlyAttacking", false);
            hasAnimationTriggered = false;
        }
    }

    private void OnDrawGizmosSelected() {
        Gizmos.DrawWireCube(attackBox.position, attackBox.localScale);
    }
}
