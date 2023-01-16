using UnityEngine;
using UnityEngine.Events;

public class Key : MonoBehaviour {

    public Transform hitbox;
    public GameObject collectedParticle;
    public UnityEvent PlayerColliding;
    private Animator anim;

    private bool hasCollided = false;

    private void Start() {
        anim = GetComponent<Animator>();
    }

    void Update() {
        Player p = Player.instance;
        if (!hasCollided && p != null) {
            if (new Bounds(p.hitbox.position, p.hitbox.localScale).Intersects(new Bounds(hitbox.position, hitbox.localScale))) {
                Debug.Log("Key Collected");

                collectedParticle.transform.position = new Vector2(transform.position.x, transform.position.y);
                Instantiate(collectedParticle);

                anim.SetTrigger("Collected");
                AudioManager.instance.play("Spell");
                PlayerColliding.Invoke();
                hasCollided = true;
            }
        }
    }
}
