using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelController : MonoBehaviour
{
    enum GameState {
        Playing,
        Over,
    }

    public GameObject HUD;
    public GameObject EndGameScreen;

    public float playerLineY = -4.0f;
    public float enemyLineY = 3.5f;

    private GameObject player;
    private int levelNo = 0;
    private int score = 0;
    private int enemiesKilled = 0;
    private GameState currentState = GameState.Over;
    
    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        if (player == null) {
            Debug.Log("LevelController failed to find player!");
        }
        NewGame();
    }

    void NewGame() {
        levelNo = 0;
        score = 0;
        enemiesKilled = 0;
        if (player != null) {
            player.GetComponent<PlayerScript>().Reset();
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    // Called on player death
    public void PlayerDie() {
        ShowGameOver();
    }

    // Called on enemy death
    public void EnemyDeath(GameObject who) {

    }

    // Show the gameover screen
    void ShowGameOver() {
        // pass
    }

    // Show the gameover screen now
    void ShowGameOverImmediate() {
        // pass
    }

    // Launch the next level
    void LaunchLevel() {
        levelNo++;
        Debug.Log("Starting level " + levelNo);
    }
}
