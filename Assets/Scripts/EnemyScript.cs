using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyScript : MonoBehaviour
{
    public int hp = 10;
    public GameObject projectile;
    public float shootRange = 0.5f;
    public float minFireDelay = 1.0f;
    public float maxForce = 5.0f;

    private Rigidbody2D playerBody;
    private int i=0;
    private float lastFireTime = 0.0f;

    // Start is called before the first frame update
    void Start()
    {
        GameObject[] candidates = GameObject.FindGameObjectsWithTag("Player");
        if (candidates.Length != 1) {
            throw new Exception("Expected exactly one player tag.");
        }
        playerBody = candidates[0].GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    // FixedUpdate is called on a fixed interval
    void FixedUpdate() {
        MoveTowardsPlayer();
    }

    void MoveTowardsPlayer() {
        if (playerBody == null)
            return;
        Rigidbody2D rb = GetComponent<Rigidbody2D>();
        float delta = rb.position.x - playerBody.position.x;
        // This overshoots so badly
        float force = delta;
        if (Math.Sign(delta) != Math.Sign(rb.velocity.x)) {
            force = delta*4;
        }
        force = Mathf.Clamp(force, -maxForce, maxForce);
        rb.AddRelativeForce(new Vector2(force, 0));
        // If we're close, try shooting
        if (Math.Abs(delta) < shootRange) {
            Shoot();
        }
        if (i++%100 == 0) {
            Debug.Log("Delta: " + delta + " Velocity: " + rb.velocity.x);
        }
    }

    void Shoot() {
        if (Time.time < lastFireTime + minFireDelay) {
            return;
        }
        lastFireTime = Time.time;
        GameObject fired = Instantiate(projectile, transform.position, transform.rotation);
        fired.GetComponent<ShotScript>().Fire(false);
    }

    // Bullets & missiles have trigger enabled
    void OnTriggerEnter2D(Collider2D col) {
        if (col.gameObject.GetComponent<ShotScript>().IsFriendly()) {
            HitFire(col.gameObject);
        }
    }

    void HitFire(GameObject fire) {
        Debug.Log("I'm triggered!");
        // TODO: trigger any dying effect, etc.
        Destroy(fire);
        hp--;
        if (hp <= 0) {
            // TODO: big show of dying, scoring, etc.
            Destroy(gameObject);
        }
    }
}
