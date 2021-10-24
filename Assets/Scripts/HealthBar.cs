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

    // Start is called before the first frame update
    void Start()
    {
        group = GetComponent<CanvasGroup>();
        if (hideFull) {
            group.alpha = 0f;
        }
        fillImage = transform.Find("HealthBarFill").gameObject.GetComponent<Image>();
        if (fillImage == null) {
            Debug.Log("Could not find fill image for health bar!");
        }
        fillImage.color = barColor;
        Debug.Log("Color: " + barColor);
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
        Debug.Log("Health: " + health + " Full: " + fullAmount);
        if (hideFull && health >= fullAmount) {
            // Hide
            group.alpha = 0f;
            Debug.Log("Full");
        } else {
            // Show
            group.alpha = 1f;
        }
    }

    public void Position(float x, float y) {
        transform.position = new Vector3(x, y, 0);
    }
}
