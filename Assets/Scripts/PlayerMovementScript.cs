using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class PlayerMovementScript : MonoBehaviour
{
    public float speed = 10.0f;
    private float threshold = 0.01f;
    private Rigidbody2D rigidBody;

    // Start is called before the first frame update
    void Start()
    {
        rigidBody = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
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
}
