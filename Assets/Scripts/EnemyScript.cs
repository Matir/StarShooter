using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyScript : MonoBehaviour
{
    public int hp = 10;
    public GameObject projectile;
    public float shootRange = 0.5f;
    public float minFireDelay = 1.0f;
    public float maxForce = 5.0f;
    public GameObject healthBarPrefab;
    public float hpbarVerticalOffset = 0.8f;
    public Color healthBarColor = Color.red;
    public int pointValue = 1;

    private Rigidbody2D playerBody;
    private int i=0;
    private float lastFireTime = 0.0f;
    private int currhp;
    private HealthBar hpbar = null;
    private LevelController levelController = null;

    // Start is called before the first frame update
    void Start()
    {
        GameObject[] candidates = GameObject.FindGameObjectsWithTag("Player");
        if (candidates.Length != 1) {
            throw new Exception("Expected exactly one player tag.");
        }
        playerBody = candidates[0].GetComponent<Rigidbody2D>();
        currhp = hp;
        if (healthBarPrefab != null) {
            GameObject canvas = GameObject.FindGameObjectWithTag("Canvas");
            if (canvas == null) {
                Debug.Log("Unable to find canvas!");
            }
            GameObject barobj = Instantiate(
                healthBarPrefab, new Vector3(0, 0, 0),
                Quaternion.identity,
                canvas.transform);
            barobj.name = "EnemyHealthBar";
            hpbar = barobj.GetComponent<HealthBar>();
            hpbar.barColor = healthBarColor;
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
                transform.position.y + hpbarVerticalOffset);
        }
    }

    // FixedUpdate is called on a fixed interval
    void FixedUpdate() {
        MoveTowardsPlayer();
    }

    void MoveTowardsPlayer() {
        if (playerBody == null)
            return;
        Rigidbody2D rb = GetComponent<Rigidbody2D>();
        float delta = rb.position.x - playerBody.position.x;
        // This overshoots so badly
        float force = delta;
        if (Math.Sign(delta) != Math.Sign(rb.velocity.x)) {
            force = delta*4;
        }
        force = Mathf.Clamp(force, -maxForce, maxForce);
        rb.AddRelativeForce(new Vector2(force, 0));
        // If we're close, try shooting
        if (Math.Abs(delta) < shootRange) {
            Shoot();
        }
        if (i++%100 == 0) {
            Debug.Log("Delta: " + delta + " Velocity: " + rb.velocity.x);
        }
    }

    void Shoot() {
        if (Time.time < lastFireTime + minFireDelay) {
            return;
        }
        lastFireTime = Time.time;
        GameObject fired = Instantiate(projectile, transform.position, transform.rotation);
        fired.GetComponent<ShotScript>().Fire(false);
    }

    // Bullets & missiles have trigger enabled
    void OnTriggerEnter2D(Collider2D col) {
        if (col.gameObject.GetComponent<ShotScript>().IsFriendly()) {
            HitFire(col.gameObject);
        }
    }

    void HitFire(GameObject fire) {
        Debug.Log("I'm triggered!");
        // TODO: trigger any dying effect, etc.
        ShotScript shot = fire.GetComponent<ShotScript>();
        currhp -= shot.GetDamage();
        if (hpbar != null) {
            hpbar.SetHealth((float)(currhp)/(float)(hp));
        }
        Destroy(fire);
        if (currhp <= 0) {
            // TODO: big show of dying, scoring, etc.
            if (levelController != null) {
                levelController.EnemyDeath(gameObject);
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
}
