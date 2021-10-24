using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class PlayerScript : MonoBehaviour
{
    public float speed = 10.0f;
    public float minFireDelay = 1.0f;
    public GameObject projectile;
    public int hp = 10;

    private float threshold = 0.01f;
    private Rigidbody2D rigidBody;
    private bool firing = false;
    private float lastFireTime = 0.0f;
    private int currhp;

    // Start is called before the first frame update
    void Start()
    {
        rigidBody = GetComponent<Rigidbody2D>();
        currhp = hp;
    }

    // Update is called once per frame
    void Update()
    {
        firing = Input.GetButton("Fire1");
        if (firing && (Time.time > lastFireTime + minFireDelay)) {
            lastFireTime = Time.time;
            FireCannon();
        }
    }

    // FixedUpdate is called on a Fixed interval
    void FixedUpdate() {
        UpdateHorizontal();
    }

    private void UpdateHorizontal() {
        if (rigidBody == null)
            return;
        float movement = Input.GetAxis("Horizontal");
        if (Math.Abs(movement) < threshold) {
            movement = 0.0f;
        }
        movement *= speed;
        rigidBody.velocity = new Vector2(movement, 0);
    }

    private void FireCannon() {
        //Debug.Log("Firing cannon!");
        GameObject fired = Instantiate(projectile, transform.position, transform.rotation);
        // Todo: per-ammo launch type?
        fired.GetComponent<ShotScript>().Fire(true);
    }

    // Bullets & missiles have trigger enabled
    void OnTriggerEnter2D(Collider2D col) {
        if (!ShotScript.IsShot(col.gameObject)) {
            return;
        }
        if (!col.gameObject.GetComponent<ShotScript>().IsFriendly()) {
            HitFire(col.gameObject);
        }
    }

    void HitFire(GameObject fire) {
        // TODO: Game Over
        ShotScript shot = fire.GetComponent<ShotScript>();
        currhp -= shot.GetDamage();
        Destroy(fire);
        if (currhp <= 0) {
            // Need game over
            rigidBody = null;
            Destroy(gameObject);
        }
    }
}
