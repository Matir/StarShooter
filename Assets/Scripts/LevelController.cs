using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

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

    public float PlayerLineY = -4.0f;
    public float EnemyLineY = 3.5f;
    public float EnemySpacing = 1.0f;
    public float PowerupLineY = 2.75f;
    public float PowerUpMinX = -8.0f;
    public float PowerUpMaxX = 8.0f;

    public float GameOverScreenDelay = 1.0f;
    public float LevelIncrementDelay = 1.0f;

    public float PowerUpChancePerTick = 0.08f;

    private GameObject player;
    private PlayerScript playerScript;
    private int levelNo = 0;
    private int score_ = 0;
    private int enemiesKilled = 0;
    private GameState currentState = GameState.New;
    private HUDScript hudScript = null;
    private Dictionary<string, GameObject> enemyPrefabMap;
    private List<GameObject> levelEnemies = new List<GameObject>();
    private Coroutine powerUpCoroutine = null;
    private float powerUpChance = 0.0f;
    private float powerUpTick = 3.0f;
    private bool levelIncrementing = false;

    private int score {
        get {
            return score_;
        }
        set {
            score_ = value;
            if (hudScript != null) {
                hudScript.SetScore(score_);
            }
        }
    }

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
        enemiesKilled = 0;
        levelIncrementing = false;

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
        // Start a power up coroutine
        powerUpCoroutine = StartCoroutine(CreatePowerups());
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
        if (powerUpCoroutine != null) {
            StopCoroutine(powerUpCoroutine);
        }
        ShowGameOver();
    }

    // Called on enemy death
    public void EnemyDeath(GameObject who, int pointValue) {
        score += pointValue;
        enemiesKilled++;
        if (!levelEnemies.Remove(who)) {
            Debug.Log("Unknown enemy died!");
        }
        if (levelEnemies.Count == 0) {
            StartCoroutine(IncrementLevel());
        }
    }

    IEnumerator IncrementLevel() {
        if (levelIncrementing) {
            yield break;
        }
        levelIncrementing = true;
        yield return new WaitForSeconds(LevelIncrementDelay);
        if (currentState != GameState.Playing) {
            levelIncrementing = false;
            yield break;
        }
        LaunchLevel();
        levelIncrementing = false;
    }

    // Show the gameover screen
    void ShowGameOver() {
        Debug.Log("Game over triggered!");
        StartCoroutine(ShowGameOverDelay());
    }

    IEnumerator ShowGameOverDelay() {
        yield return new WaitForSeconds(GameOverScreenDelay);
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
        if ((levelNo % 5) == 0) {
            return new List<string>{"Boss1"};
        }
        if ((levelNo % 2) == 1) {
            return new List<string>{"BasicEnemy"};
        }
        return new List<string>{"BasicEnemy", "HeavyEnemy"};
    }

    // Load enemies based on list
    void LoadEnemies(List<string> enemies) {
        int numEnemies = enemies.Count;
        float startPos = (float)(numEnemies - 1) / 2.0f * EnemySpacing;
        int i = 0;
        foreach(var name in enemies) {
            if (!enemyPrefabMap.ContainsKey(name)) {
                Debug.Log("Could not find enemy: " + name);
                continue;
            }
            float x = startPos + i * EnemySpacing;
            Debug.Log("x="+x);
            GameObject enemy = Instantiate(
                enemyPrefabMap[name],
                new Vector3(x, EnemyLineY, 0),
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

    private IEnumerator CreatePowerups() {
        // Create powerups periodically
        while (true) {
            powerUpChance += PowerUpChancePerTick;
            if (Utils.RandomChance(powerUpChance)) {
                Debug.Log("Creating a powerup.");
                PlacePowerUp();
                powerUpChance = 0.0f;
            }
            yield return new WaitForSeconds(powerUpTick);
        }
    }

    private void OnPowerUpHit(PowerUpScript script) {
        // Do some stuff
        Debug.Log("Power up was hit!");
        score++;
        playerScript.ApplyPowerUp(script.PType);
    }

    private void PlacePowerUp() {
        float x = Random.Range(PowerUpMinX, PowerUpMaxX);
        Vector3 pos = new Vector3(x, PowerupLineY, 0);
        GameObject power = Instantiate(PowerUp, pos, Quaternion.identity);
        PowerUpType pt = Utils.GetRandomElement(PowerUpScript.PowerUpTypes);
        power.GetComponent<PowerUpScript>().PType = pt;
    }
}
