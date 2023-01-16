using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class CameraController : MonoBehaviour {

    public Transform target;
    public float maxDist, speed;

    private void FixedUpdate() {
        Vector2 moveDir = target.position - transform.position;
        if (moveDir.magnitude > maxDist) {
            Vector2 camMove = moveDir - moveDir.normalized * maxDist;
            transform.Translate(camMove * speed * Time.deltaTime);
        }
    }

    private void OnDrawGizmosSelected() {
        Gizmos.DrawWireSphere(transform.position, maxDist);
    }
}
