using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShotScript : MonoBehaviour
{
    // Shot forward velocity
    public float ShotVelocity = 8.0f;
    // Damage from this shot
    public int Damage = 1;
    // name of shot
    public string ShotName = "Shot";
    // Shot tripler?
    public bool TripleShot = false;

    // Who was the shooter?
    private bool friendly = false;
    private float tripleSpacing = 0.4f;

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
        Vector2 v = new Vector2(0, ShotVelocity);
        rb.velocity = Quaternion.Euler(0, 0, rb.rotation) * v;
        if (TripleShot) {
            float[] offsets = new float[]{-tripleSpacing, tripleSpacing};
            foreach (float offset in offsets) {
                Vector3 pos = transform.position;
                pos.x += offset;
                var fired = Instantiate(gameObject, pos, transform.rotation);
                var newScript = fired.GetComponent<ShotScript>();
                newScript.TripleShot = false;
                newScript.Fire(friendly);
            }
        }
    }

    void OnBecameInvisible() {
        Destroy(gameObject);
    }

    public bool IsFriendly() {
        return friendly;
    }

    public int GetDamage() {
        return Damage;
    }

    public static bool IsShot(GameObject obj) {
        // Check if we have a shot script
        ShotScript script = obj.GetComponent<ShotScript>();
        return (script != null);
    }
}
