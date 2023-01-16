
using UnityEngine;
using UnityEngine.SceneManagement;

public class Player : MonoBehaviour {

    //Singleton
    public static Player instance;

    //Changeable speed & jumpforce
    private static float MAXSPEED = 15f, MINSPEED = 1f;
    public static float currentSpeed = (MAXSPEED - MINSPEED) / 2 + MINSPEED;
    private static float MAXJUMP = 15.5f, MINJUMP = 5f;
    private static float currentJump = (MAXJUMP - MINJUMP) / 2 + MINJUMP;
    private static bool isChangingSpeed = false, isChangingJump = false;

    public static KeyCode jumpKey = KeyCode.Space, attackKey = KeyCode.E;

    // Animation & Movement
    private float moveInput;
    private bool facingRight = true;
    private bool isGrounded = false;
    private Rigidbody2D rb;
    private Animator anim;
    private SpriteRenderer[] spriteRenderer;

    [Header("Attributes")]
    private bool isDead = false;
    public float damage;
    public float health;
    public bool isPlayable = true;
    public float stepAudioDelay;
    private float stepAudioCounter;

    [Header("HitBox")]
    public Transform hitbox;
    public float maxHitTime; // time until can be hit again
    private bool isCurrentlyHit = false;
    private float hitTimer = 0f; // Time since last hit

    [Header("Particle")]
    public GameObject stompParticle;
    public GameObject deathParticle;
    public Transform stompParticleSpawnPos;
    private bool hasParticleSpawned = true;

    [Header("GroundCheck")]
    public Transform groundCheck;
    public float checkRadius;
    public LayerMask whatisGround;

    private void Start() {
        rb = GetComponent<Rigidbody2D>();
        instance = this;
        anim = GetComponent<Animator>();
        spriteRenderer = GetComponentsInChildren<SpriteRenderer>();
    }

    private void Update() {
        if (!isDead) {
            if (isGrounded && isPlayable) {
                if (Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyDown(jumpKey)) {
                    rb.velocity = Vector2.up * currentJump;
                    anim.SetTrigger("takeOf");
                } else if (Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyDown(jumpKey)) {
                    rb.velocity = Vector2.up * currentJump;
                    anim.SetTrigger("takeOf");
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
        }
    }

    public void draw(Color color) {
        foreach (SpriteRenderer s in spriteRenderer) {
            s.color = color;
        }
    }

    public void hit(GameObject g) {
        if (!isCurrentlyHit) {
            isCurrentlyHit = true;

            Ghost enemy = g.GetComponent<Ghost>();
            // Do anything if is hit 
            if (enemy != null) {
                health -= enemy.damage;
            }

            Projectile projectile = g.GetComponent<Projectile>();
            // Do anything if is hit 
            if (projectile != null) {
                health -= projectile.damage;
            }
        }
        if (health <= 0) {
            die();
        }
    }

    public void die() {
        if (!isDead) {
            isDead = true;
            deathParticle.transform.position = transform.position;
            Instantiate(deathParticle);
            gameObject.SetActive(false);
            AudioManager.instance.play("DeathSound");

            Invoke("restartLevel", 1.5f);

        }
    }

    private void restartLevel() {
        Debug.Log("Level Restart!");
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    private void FixedUpdate() {
        if (!isDead) {
            isGrounded = Physics2D.OverlapCircle(groundCheck.position, checkRadius, whatisGround);
            if (isGrounded) {
                anim.SetBool("isJumping", false);
                if (!hasParticleSpawned) {
                    spawnParticle();
                    hasParticleSpawned = true;
                }
            } else {
                anim.SetBool("isJumping", true);
                hasParticleSpawned = false;
            }

            if (isPlayable) {
                moveInput = Input.GetAxis("Horizontal"); // right => 1, left => -1
                rb.velocity = new Vector2(moveInput * currentSpeed, rb.velocity.y);
            }
            if (moveInput == 0) {
                anim.SetBool("isRunning", false);
            } else if (isGrounded) {
                anim.SetBool("isRunning", true);
                stepAudioCounter += Time.deltaTime;
                if (stepAudioDelay < stepAudioCounter) {
                    stepAudioCounter = 0;
                    playStep();
                }
            }

            if (!facingRight && moveInput > 0) {
                flip();
            } else if (facingRight && moveInput < 0) {
                flip();
            }
        }
    }

    private void spawnParticle() {
        // spawn stomp-particles
        stompParticle.transform.position = stompParticleSpawnPos.position;
        GameObject go = Instantiate(stompParticle);
        playStep();
    }

    private void playStep() {
        int rand = Random.Range(0, 3);
        switch (rand) {
            case 0:
                AudioManager.instance.play("Step1");
                break;
            case 1:
                AudioManager.instance.play("Step2");
                break;
            case 2:
                AudioManager.instance.play("Step3");
                break;
        }
    }

    void flip() {
        facingRight = !facingRight;

        Vector3 scaler = transform.localScale;
        scaler.x *= -1;
        transform.localScale = scaler;
    }

    public static void changeSpeed(float f) { // 0.0f - 1.0f
        if (isChangingJump || isChangingSpeed)
            return;

        isChangingSpeed = true;
        currentSpeed = (MAXSPEED - MINSPEED) * f + MINSPEED;
        currentJump = (MAXJUMP - MINJUMP) * (1 - f) + MINJUMP;
        UIManager.instance.updateJumpSlider();
        isChangingSpeed = false;
    }

    public static void changeJump(float f) { // 0.0f - 1.0f
        if (isChangingJump || isChangingSpeed)
            return;

        isChangingJump = true;
        currentJump = (MAXJUMP - MINJUMP) * f + MINJUMP;
        currentSpeed = (MAXSPEED - MINSPEED) * (1 - f) + MINSPEED;
        UIManager.instance.updateSpeedSlider();
        isChangingJump = false;
    }

    public void isDamaged(bool isDamaged) {
        if (isDamaged)
            draw(new Color(255, 0, 0));
        else
            draw(new Color(255, 255, 255));
    }
}
