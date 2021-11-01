using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyScript : MonoBehaviour
{
    public int hp = 10;
    public GameObject Projectile;
    public float ShootRange = 0.5f;
    public float MinFireDelay = 1.0f;
    public float MaxForce = 5.0f;
    public GameObject HealthBarPrefab;
    public float HPBarVerticalOffset = 0.8f;
    public Color HealthBarColor = Color.red;
    public int PointValue = 1;
    public string EnemyName;
    public GameObject DeathExplosion;

    private Rigidbody2D playerBody;
    private int i=0;
    private int enemyLevel=1;
    private float lastFireTime = 0.0f;
    protected int currhp;
    private HealthBar hpbar = null;
    private LevelController levelController = null;

    // Start is called before the first frame update
    protected virtual void Start()
    {
        GameObject[] candidates = GameObject.FindGameObjectsWithTag("Player");
        if (candidates.Length != 1) {
            throw new Exception("Expected exactly one player tag.");
        }
        playerBody = candidates[0].GetComponent<Rigidbody2D>();
        currhp = hp;
        if (HealthBarPrefab != null) {
            GameObject canvas = GameObject.FindGameObjectWithTag("Canvas");
            if (canvas == null) {
                Debug.Log("Unable to find canvas!");
            }
            GameObject barobj = Instantiate(
                HealthBarPrefab, new Vector3(0, 0, 0),
                Quaternion.identity,
                canvas.transform);
            barobj.name = "EnemyHealthBar";
            hpbar = barobj.GetComponent<HealthBar>();
            hpbar.BarColor = HealthBarColor;
        }
        GameObject lvlc = GameObject.Find("LevelController");
        if (lvlc != null) {
            levelController = lvlc.GetComponent<LevelController>();
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (hpbar != null) {
            hpbar.Position(
                transform.position.x,
                transform.position.y + HPBarVerticalOffset);
        }
    }

    // FixedUpdate is called on a fixed interval
    void FixedUpdate() {
        MoveTowardsPlayer();
    }

    void MoveTowardsPlayer() {
        if (playerBody == null)
            return;
        if (!playerBody.gameObject.activeInHierarchy)
            return;
        Rigidbody2D rb = GetComponent<Rigidbody2D>();
        float delta = rb.position.x - playerBody.position.x;
        // This overshoots so badly
        float force = delta;
        if (Math.Sign(delta) != Math.Sign(rb.velocity.x)) {
            force = delta*4;
        }
        force = Mathf.Clamp(force, -MaxForce, MaxForce);
        rb.AddRelativeForce(new Vector2(force, 0));
        // If we're close, try shooting
        if (Math.Abs(delta) < ShootRange) {
            Shoot();
        }
        if (i++%100 == 0) {
            Debug.Log("Delta: " + delta + " Velocity: " + rb.velocity.x);
        }
    }

    void Shoot() {
        if (Time.time < lastFireTime + MinFireDelay) {
            return;
        }
        lastFireTime = Time.time;
        GameObject fired = Instantiate(Projectile, transform.position, transform.rotation);
        fired.GetComponent<ShotScript>().Fire(false);
    }

    // Bullets & missiles have trigger enabled
    void OnTriggerEnter2D(Collider2D col) {
        if (ShotScript.IsShot(col.gameObject) && col.gameObject.GetComponent<ShotScript>().IsFriendly()) {
            HitFire(col.gameObject);
        }
    }

    protected virtual void HitFire(GameObject fire) {
        // TODO: trigger any dying effect, etc.
        ShotScript shot = fire.GetComponent<ShotScript>();
        int dmg = shot.GetDamage();
        bool dying = (currhp > 0 && currhp <= dmg);
        currhp -= dmg;
        if (currhp < 0) {
            currhp = 0;
        }
        if (hpbar != null) {
            hpbar.SetHealth((float)(currhp)/(float)(hp));
        }
        Destroy(fire);
        if (dying) {
            // TODO: big show of dying, etc.
            if (levelController != null) {
                levelController.EnemyDeath(gameObject, PointValue);
            }
            if (DeathExplosion != null) {
                Instantiate(
                    DeathExplosion,
                    transform.position,
                    Quaternion.identity);
            }
            Destroy(gameObject);
        }
    }

    void OnDestroy() {
        if (hpbar != null) {
            Destroy(hpbar.gameObject);
        }
    }

    public void SetLevelController(LevelController c) {
        levelController = c;
    }

    public void SetLevel(int level) {
        enemyLevel = level;
        if (level > 4) {
            hp += (level-4)/2;
            currhp += (level-4)/2;
        }
    }
}
