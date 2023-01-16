using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerDeathBox : MonoBehaviour {

    public Transform deathArea;

    void Update() {
        Player p = Player.instance;
        if (p != null) {
            if (new Bounds(p.hitbox.position, p.hitbox.localScale).Intersects(new Bounds(deathArea.position, deathArea.localScale))) {
                p.die();
            }
        }
    }
}
