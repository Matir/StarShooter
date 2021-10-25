using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelController : MonoBehaviour
{


    public float playerLineY = -4.0f;
    public float enemyLineY = 3.5f;

    private GameObject player;
    private int levelNo = 1;
    
    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        if (player == null) {
            Debug.Log("LevelController failed to find player!");
        }
    }

    void NewGame() {
        levelNo = 1;
        if (player != null) {
            player.GetComponent<PlayerScript>.Reset();
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    // Called on player death
    void PlayerDie() {
        // pass for now
    }
}
