using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum PowerUpType {
    Shield,
    MissileUpgrade,
    Health,
}

public class PowerUpScript : MonoBehaviour {

    public Sprite ShieldSprite;
    public Sprite MissileSprite;
    public Sprite HealthSprite;
    public float Lifetime = 1.5f;
    public float FadeTime = 0.5f;

    public delegate void NotifyPowerUp(PowerUpScript script);
    public static event NotifyPowerUp OnPowerUpHit;
    public static List<PowerUpType> PowerUpTypes = Utils.EnumToList<PowerUpType>();

    private PowerUpType _type = PowerUpType.MissileUpgrade;
    private SpriteRenderer sRenderer = null;
    private int fadeSteps = 10;
    private bool isHit = false;


    public PowerUpType PType {
        get {
            return _type;
        }
        set {
            _type = value;
            SetSpriteImage();
        }
    }

    // Awake is called first thing
    void Awake() {
        sRenderer = GetComponent<SpriteRenderer>();
    }

    // Start is called before the first frame update
    void Start()
    {
        SetAlpha(0.0f);
        SetSpriteImage();
        StartCoroutine(LifeCycle());
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void SetSpriteImage() {
        if (sRenderer == null) {
            return;
        }
        switch (_type) {
            case PowerUpType.Shield:
                sRenderer.sprite = ShieldSprite;
                break;
            case PowerUpType.MissileUpgrade:
                sRenderer.sprite = MissileSprite;
                break;
            case PowerUpType.Health:
                sRenderer.sprite = HealthSprite;
                break;
            default:
                Debug.Log("Could not find power up type!");
                break;
        }
    }

    void SetAlpha(float alpha) {
        if (sRenderer == null) {
            Debug.Log("Attempt to set alpha with null renderer!");
        }
        Color curColor = sRenderer.color;
        curColor.a = alpha;
        sRenderer.color = curColor;
    }

    IEnumerator LifeCycle() {
        for(int i=1; i<fadeSteps; i++) {
            SetAlpha((float)i/(float)fadeSteps);
            yield return new WaitForSeconds(FadeTime/(float)fadeSteps);
        }
        SetAlpha(1.0f);
        yield return new WaitForSeconds(Lifetime);
        if (isHit) {
            // This will be handled separately
            yield break;
        }
        yield return StartCoroutine(FadeOutAndDestroy());
    }

    IEnumerator FadeOutAndDestroy() {
        for(int i=fadeSteps-1; i>0; i--) {
            SetAlpha((float)i/(float)fadeSteps);
            yield return new WaitForSeconds(FadeTime/(float)fadeSteps);
        }
        Destroy(gameObject);
    }

    private void OnTriggerEnter2D(Collider2D other) {
        GameObject hit = other.gameObject;
        if (!ShotScript.IsShot(hit)) {
            // Ignore non-shot hits
            return;
        }
        ShotScript shot = hit.GetComponent<ShotScript>();
        if (!shot.IsFriendly()) {
            // Ignore enemy hits
            return;
        }
        if (isHit) {
            // Already hit
            return;
        }
        isHit = true;
        OnPowerUpHit(this);
        StartCoroutine(FadeOutAndDestroy());
    }
}
