using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShotScript : MonoBehaviour
{
    // Shot forward velocity
    public float shotVelocity = 1.0f;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Fire() {
        GetComponent<Rigidbody2D>().AddRelativeForce(new Vector3(0, shotVelocity, 0));
    }

    void OnBecameInvisible() {
        Debug.Log("Invisible!");
        Destroy(gameObject);
    }
}
