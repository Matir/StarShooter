using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelController : MonoBehaviour
{
    enum GameState {
        New,
        Playing,
        Over,
    }

    public GameObject HUD;
    public GameObject EndGameScreen;

    public float playerLineY = -4.0f;
    public float enemyLineY = 3.5f;

    public float gameOverScreenDelay = 1.0f;

    private GameObject player;
    private int levelNo = 0;
    private int score = 0;
    private int enemiesKilled = 0;
    private GameState currentState = GameState.New;
    private HUDScript hudScript = null;
    
    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        if (player == null) {
            Debug.Log("LevelController failed to find player!");
        }
        if (HUD != null) {
            hudScript = HUD.GetComponent<HUDScript>();
        } else {
            Debug.Log("Need a HUD Component!");
        }
        NewGame();
    }

    void NewGame() {
        levelNo = 0;
        score = 0;
        enemiesKilled = 0;
        if (player != null && currentState != GameState.New) {
            player.GetComponent<PlayerScript>().ResetPlayer();
        }
        EndGameScreen.SetActive(false);
        currentState = GameState.Playing;
        LaunchLevel();
    }

    // Update is called once per frame
    void Update()
    {
        // Check if we should reset
        if (currentState == GameState.Over && Input.GetKeyDown(KeyCode.R)) {
            NewGame();
        }
    }

    // Called on player death
    public void PlayerDie() {
        currentState = GameState.Over;
        ShowGameOver();
    }

    // Called on enemy death
    public void EnemyDeath(GameObject who, int pointValue) {
        score += pointValue;
        enemiesKilled++;
        hudScript.SetScore(score);
        // Track levels here
    }

    // Show the gameover screen
    void ShowGameOver() {
        Debug.Log("Game over triggered!");
        StartCoroutine(ShowGameOverDelay());
    }

    IEnumerator ShowGameOverDelay() {
        yield return new WaitForSeconds(gameOverScreenDelay);
        ShowGameOverImmediate();
    }

    // Show the gameover screen now
    void ShowGameOverImmediate() {
        Debug.Log("Showing game over.");
        EndGameScreen.SetActive(true);
    }

    // Launch the next level
    void LaunchLevel() {
        levelNo++;
        Debug.Log("Starting level " + levelNo);
        hudScript.SetLevel(levelNo);
    }
}
