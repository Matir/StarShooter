using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyScript : MonoBehaviour
{
    public int hp = 10;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    // Bullets & missiles have trigger enabled
    void OnTriggerEnter2D(Collider2D col) {
        Debug.Log("I'm triggered!");
        // TODO: trigger any dying effect, etc.
        Destroy(col.gameObject);
        hp--;
        if (hp <= 0) {
            // TODO: big show of dying, scoring, etc.
            Destroy(gameObject);
        }
    }
}
