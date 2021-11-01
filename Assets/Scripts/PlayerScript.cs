using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class PlayerScript : MonoBehaviour
{
    public float speed = 10.0f;
    public float minFireDelay = 1.0f;
    public GameObject[] projectiles;
    public GameObject healthBarPrefab;
    public int hp = 10;
    public float hpbarVerticalOffset = -0.8f;
    public Color healthBarColor = Color.green;
    public float shieldLife = 5.0f;
    public GameObject HUD;
    public LevelController levelController;

    private GameObject shieldObject;
    private GameObject projectile;
    private int unlockedProjectiles = 1;
    private float threshold = 0.01f;
    private Rigidbody2D rigidBody;
    private bool firing = false;
    private float lastFireTime = 0.0f;
    private int currhp;
    private HealthBar hpbar = null;
    private Vector3 startPos;
    private bool shields_ = false;
    private Coroutine shieldDisableCoroutine = null;
    private HUDScript hudScript = null;

    public bool shieldsEnabled {
        get {
            return shields_;
        }
        set {
            shields_ = value;
            if (shieldDisableCoroutine != null) {
                    StopCoroutine(shieldDisableCoroutine);
                    shieldDisableCoroutine = null;
            }
            if (shields_) {
                // set future timeout
                shieldObject.SetActive(true);
                shieldDisableCoroutine = StartCoroutine(DisableShieldTimer());
            } else {
                // Remove
                shieldObject.SetActive(false);
            }
        }
    }

    // Awake is called before Start
    void Awake() {
        rigidBody = GetComponent<Rigidbody2D>();
        shieldObject = transform.Find("Shields").gameObject;
        hudScript = HUD.GetComponent<HUDScript>();
    }
    
    // Start is called before the first frame update
    void Start()
    {
        startPos = transform.position;
        Debug.Log("Initial position: " + startPos);
        if (healthBarPrefab != null) {
            GameObject canvas = GameObject.FindGameObjectWithTag("Canvas");
            if (canvas == null) {
                Debug.Log("Unable to find canvas!");
            }
            GameObject barobj = Instantiate(
                healthBarPrefab, new Vector3(0, 0, 0),
                Quaternion.identity,
                canvas.transform);
            barobj.name = "PlayerHealthBar";
            hpbar = barobj.GetComponent<HealthBar>();
            hpbar.BarColor = healthBarColor;
        }
        ResetPlayer();
    }

    public void ResetPlayer() {
        if (hpbar != null) {
            hpbar.SetHealth(1.0f);
            hpbar.gameObject.SetActive(true);
        }
        currhp = hp;
        transform.position = startPos;
        gameObject.SetActive(true);
        projectile = projectiles[0];
        unlockedProjectiles = 1;
        shieldsEnabled = false;
    }

    // Update is called once per frame
    void Update()
    {
        // Check for change ammo
        if (Input.GetKeyDown(KeyCode.Tab)) {
            int curAmmo = Array.IndexOf<GameObject>(projectiles, projectile);
            curAmmo = (curAmmo + 1) % unlockedProjectiles;
            projectile = projectiles[curAmmo];
            string shotName = projectile.GetComponent<ShotScript>().shotName;
            Debug.Log("Ammo is now " + projectile + " " + shotName);
            hudScript.SetAmmo(shotName);
        }

        // Check for firing
        firing = Input.GetButton("Fire1");
        if (firing && (Time.time > lastFireTime + minFireDelay)) {
            lastFireTime = Time.time;
            FireCannon();
        }

        if (hpbar != null) {
            hpbar.Position(
                transform.position.x,
                transform.position.y + hpbarVerticalOffset);
        }
    }

    // FixedUpdate is called on a Fixed interval
    void FixedUpdate() {
        UpdateHorizontal();
    }

    private void UpdateHorizontal() {
        if (rigidBody == null)
            return;
        float movement = Input.GetAxis("Horizontal");
        if (Math.Abs(movement) < threshold) {
            movement = 0.0f;
        }
        movement *= speed;
        rigidBody.velocity = new Vector2(movement, 0);
    }

    private void FireCannon() {
        //Debug.Log("Firing cannon!");
        GameObject fired = Instantiate(projectile, transform.position, transform.rotation);
        // Todo: per-ammo launch type?
        fired.GetComponent<ShotScript>().Fire(true);
    }

    // Bullets & missiles have trigger enabled
    void OnTriggerEnter2D(Collider2D col) {
        if (!ShotScript.IsShot(col.gameObject)) {
            return;
        }
        if (!col.gameObject.GetComponent<ShotScript>().IsFriendly()) {
            HitFire(col.gameObject);
        }
    }

    void HitFire(GameObject fire) {
        ShotScript shot = fire.GetComponent<ShotScript>();
        if (!shieldsEnabled) {
            currhp -= shot.GetDamage();
        }
        if (hpbar != null) {
            hpbar.SetHealth((float)(currhp)/(float)(hp));
        }
        Destroy(fire);
        if (currhp <= 0) {
            if (levelController != null) {
                levelController.PlayerDie();
            }
            gameObject.SetActive(false);
            hpbar.gameObject.SetActive(false);
        }
    }

    void OnDestroy() {
        if (hpbar != null) {
            Destroy(hpbar.gameObject);
        }
    }

    public void ApplyPowerUp(PowerUpType power) {
        // Apply a power up
        switch (power) {
            case PowerUpType.Shield:
                shieldsEnabled = true;
                break;
            case PowerUpType.MissileUpgrade:
                if (unlockedProjectiles < projectiles.Length) {
                    unlockedProjectiles++;
                }
                Debug.Log("Unlocked projectiles: " + unlockedProjectiles);
                break;
            case PowerUpType.Health:
                currhp = hp;
                hpbar.SetHealth(1.0f);
                break;
            default:
                Debug.Log("Received unknown power up type: " + power);
                break;
        }
    }

    private IEnumerator DisableShieldTimer() {
        yield return new WaitForSeconds(shieldLife);
        shieldsEnabled = false;
        shieldDisableCoroutine = null;
    }
}
