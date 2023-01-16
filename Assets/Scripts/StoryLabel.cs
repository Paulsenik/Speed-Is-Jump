using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StoryLabel : MonoBehaviour
{

    public Transform trigger;
    public bool hasBeenActivated = false;
    Animator anim;

    // Start is called before the first frame update
    void Start()
    {
        anim = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        if (!hasBeenActivated) {
            Transform hb = Player.instance.hitbox;
            if (new Bounds(hb.transform.position, hb.transform.localScale).Intersects(new Bounds(trigger.position, trigger.localScale))) {
                anim.SetBool("Shown",true);
                hasBeenActivated = true;
            }
        }
    }
}
