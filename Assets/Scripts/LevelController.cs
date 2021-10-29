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

    private const string highScorePref = "HighScore";

    public GameObject HUD;
    public GameObject EndGameScreen;
    public GameObject PowerUp;
    public List<GameObject> EnemyPrefabs;

    public float playerLineY = -4.0f;
    public float enemyLineY = 3.5f;
    public float enemySpacing = 1.0f;

    public float gameOverScreenDelay = 1.0f;
    public float levelIncrementDelay = 1.0f;

    private GameObject player;
    private PlayerScript playerScript;
    private int levelNo = 0;
    private int score = 0;
    private int enemiesKilled = 0;
    private GameState currentState = GameState.New;
    private HUDScript hudScript = null;
    private Dictionary<string, GameObject> enemyPrefabMap;
    private List<GameObject> levelEnemies = new List<GameObject>();
    
    // Awake is called even before Start
    void Awake() {
        PowerUpScript.OnPowerUpHit += OnPowerUpHit;
    }
    
    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        if (player == null) {
            Debug.Log("LevelController failed to find player!");
        } else {
            playerScript = player.GetComponent<PlayerScript>();
        }
        if (HUD != null) {
            hudScript = HUD.GetComponent<HUDScript>();
        } else {
            Debug.Log("Need a HUD Component!");
        }

        // Convert EnemyPrefabs to dict for performance
        enemyPrefabMap = new Dictionary<string, GameObject>();
        foreach (var enemy in EnemyPrefabs) {
            var enemyScript = enemy.GetComponent<EnemyScript>();
            var name = enemyScript.EnemyName;
            if (enemyPrefabMap.ContainsKey(name)) {
                Debug.Log("Duplicate enemy name: " + name);
            } else {
                enemyPrefabMap.Add(name, enemy);
            }
        }

        NewGame();
    }

    void NewGame() {
        levelNo = 0;
        score = 0;
        hudScript.SetScore(0);
        enemiesKilled = 0;
        if (player != null && currentState != GameState.New) {
            player.GetComponent<PlayerScript>().ResetPlayer();
        }
        EndGameScreen.SetActive(false);
        currentState = GameState.Playing;
        // Kill any leftovers
        foreach(var e in levelEnemies) {
            Destroy(e);
        }
        levelEnemies.Clear();
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
        if (!levelEnemies.Remove(who)) {
            Debug.Log("Unknown enemy died!");
        }
        if (levelEnemies.Count == 0) {
            StartCoroutine(IncrementLevel());
        }
    }

    IEnumerator IncrementLevel() {
        yield return new WaitForSeconds(levelIncrementDelay);
        if (currentState != GameState.Playing) {
            yield break;
        }
        LaunchLevel();
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
        int highScore = MaybeSaveHighScore(score);
        hudScript.SetHighScore(highScore);
        EndGameScreen.SetActive(true);
    }

    // Launch the next level
    void LaunchLevel() {
        if (currentState != GameState.Playing) {
            return;
        }
        levelNo++;
        Debug.Log("Starting level " + levelNo);
        hudScript.SetLevel(levelNo);
        LoadEnemies(GetLevelConfig(levelNo));
    }

    List<string> GetLevelConfig(int levelNo) {
        // TODO: real implementation
        if ((levelNo % 2) == 1) {
            return new List<string>{"BasicEnemy"};
        }
        return new List<string>{"BasicEnemy", "HeavyEnemy"};
    }

    // Load enemies based on list
    void LoadEnemies(List<string> enemies) {
        int numEnemies = enemies.Count;
        float startPos = (float)(numEnemies - 1) / 2.0f * enemySpacing;
        int i = 0;
        foreach(var name in enemies) {
            if (!enemyPrefabMap.ContainsKey(name)) {
                Debug.Log("Could not find enemy: " + name);
                continue;
            }
            float x = startPos + i * enemySpacing;
            Debug.Log("x="+x);
            GameObject enemy = Instantiate(
                enemyPrefabMap[name],
                new Vector3(x, enemyLineY, 0),
                Quaternion.Euler(0, 0, 180)
            );
            enemy.GetComponent<EnemyScript>().SetLevel(levelNo);
            levelEnemies.Add(enemy);
            i++;
        }
    }

    int MaybeSaveHighScore(int newScore) {
        int highScore = PlayerPrefs.GetInt(highScorePref, 0);
        if (newScore > highScore) {
            highScore = newScore;
            PlayerPrefs.SetInt(highScorePref, highScore);
        }
        return highScore;
    }

    private void OnPowerUpHit(PowerUpScript script) {
        // Do some stuff
        Debug.Log("Power up was hit!");
        playerScript.ApplyPowerUp(script.PType);
    }
}
