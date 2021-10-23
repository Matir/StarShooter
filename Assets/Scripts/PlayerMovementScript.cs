using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class PlayerMovementScript : MonoBehaviour
{
    public float speed = 10.0f;
    public float minFireDelay = 1.0f;
    public GameObject projectile;

    private float threshold = 0.01f;
    private Rigidbody2D rigidBody;
    private bool firing = false;
    private float lastFireTime = 0.0f;

    // Start is called before the first frame update
    void Start()
    {
        rigidBody = GetComponent<Rigidbody2D>();
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
        float movement = Input.GetAxis("Horizontal");
        if (Math.Abs(movement) < threshold) {
            movement = 0.0f;
        }
        movement *= speed;
        rigidBody.velocity = new Vector2(movement, 0);
    }

    private void FireCannon() {
        Debug.Log("Firing cannon!");
        GameObject fired = Instantiate(projectile, transform.position, transform.rotation);
        // Todo: per-ammo launch type?
        fired.GetComponent<Rigidbody2D>().AddRelativeForce(new Vector3(0, 5, 0));
    }
}
