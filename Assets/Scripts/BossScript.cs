using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossScript : EnemyScript
{
    public int NumShieldPoints = 2;
    public float ShieldDuration = 5.0f; 

    private GameObject shieldObject = null;
    private bool shields_ = false;
    private List<int> shieldPoints = null;

    private bool shieldsEnabled {
        get {
            return shields_;
        }
        set {
            shields_ = value;
            if (shieldObject != null) {
                shieldObject.SetActive(value);
            }
        }
    }

    protected override void Start() {
        base.Start();
        shieldObject = transform.Find("Shields").gameObject;
        shieldObject.SetActive(false);
        shieldPoints = new List<int>(NumShieldPoints);
        for (int i=NumShieldPoints; i>0; i--) {
            shieldPoints.Add(hp*i/(NumShieldPoints+1));
        }
    }

    protected override void HitFire(GameObject fire) {
        if (!shields_) {
            base.HitFire(fire);
            if (shieldPoints.Count > 0) {
                int thresh = shieldPoints[0];
                if (currhp < thresh) {
                    StartCoroutine(RunShields());
                    shieldPoints.Remove(thresh);
                }
            }
        } else {
            Destroy(fire);
        }
    }

    private IEnumerator RunShields() {
        shieldsEnabled = true;
        yield return new WaitForSeconds(ShieldDuration);
        shieldsEnabled = false;
    }
}
