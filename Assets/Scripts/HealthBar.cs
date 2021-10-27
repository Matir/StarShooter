using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    public Color barColor = Color.red;
    public bool hideFull = true;

    private Image fillImage;
    private CanvasGroup group;
    private float fullAmount = (1.0f - 0.01f);
    private float fadeTime = 0.3f;
    private int fadeSteps = 10;
    private bool shown = true;

    // Awake is even before Start
    void Awake()
    {
        group = GetComponent<CanvasGroup>();
        if (hideFull) {
            group.alpha = 0f;
            shown = false;
        }
        fillImage = transform.Find("HealthBarFill").gameObject.GetComponent<Image>();
        if (fillImage == null) {
            Debug.Log("Could not find fill image for health bar!");
        }
    }

    // Start is called before first frame update
    void Start() {
        fillImage.color = barColor;
        //Debug.Log("Color: " + barColor);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SetHideFull(bool hf) {
        hideFull = hf;
    }

    public void SetHealth(float health) {
        health = Mathf.Clamp(health, 0.0f, 1.0f);
        fillImage.fillAmount = health;
        //Debug.Log("Health: " + health + " Full: " + fullAmount);
        if (hideFull && health >= fullAmount) {
            // Hide
            HideBar();
        } else {
            // Show
            ShowBar();
        }
    }

    public void Position(float x, float y) {
        transform.position = new Vector3(x, y, 0);
    }

    private void HideBar() {
        if (!shown) {
            return;
        }
        shown = false;
        StartCoroutine(FadeHealthBar(false));
    }

    private void ShowBar() {
        if (shown) {
            return;
        }
        shown = true;
        StartCoroutine(FadeHealthBar(true));
    }

    private IEnumerator FadeHealthBar(bool up) {
        float stepwise = 1.0f/((float)fadeSteps);
        for(int step=0; step<fadeSteps; step++) {
            if (group == null) {
                break;
            }
            if (up) {
                group.alpha = stepwise * (step+1);
            } else {
                group.alpha = 1 - (stepwise * step);
            }
            yield return new WaitForSeconds(fadeTime/(float)fadeSteps);
        }
    }
}
