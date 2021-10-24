using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShotScript : MonoBehaviour
{
    // Shot forward velocity
    public float shotVelocity = 1.0f;
    // Damage from this shot
    [SerializeField]
    private int damage = 1;
    // Who was the shooter?
    private bool friendly = false;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Fire(bool isFriendly) {
        friendly = isFriendly;
        Rigidbody2D rb = GetComponent<Rigidbody2D>();
        Vector2 v = new Vector2(0, shotVelocity);
        rb.velocity = Quaternion.Euler(0, 0, rb.rotation) * v;
    }

    void OnBecameInvisible() {
        Destroy(gameObject);
    }

    public bool IsFriendly() {
        return friendly;
    }

    public int GetDamage() {
        return damage;
    }

    public static bool IsShot(GameObject obj) {
        // Check if we have a shot script
        ShotScript script = obj.GetComponent<ShotScript>();
        return (script != null);
    }
}
