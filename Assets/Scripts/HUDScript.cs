using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class HUDScript : MonoBehaviour
{
    public TMP_Text LevelText;
    public TMP_Text ScoreText;
    public TMP_Text HighScoreText;
    public TMP_Text AmmoText;
    public GameObject PauseObject;

    private bool paused = false;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        // Handle pause menu option here
        if (Input.GetKeyDown(KeyCode.Escape)) {
            paused = !paused;
            ShowPause(paused);
            Time.timeScale = paused ? 0 : 1;
        }
    }

    public void SetLevel(int level) {
        LevelText.text = String.Format("Level: {0}", level);
    }

    public void SetScore(int score) {
        ScoreText.text = String.Format("Score: {0}", score);
    }

    public void SetHighScore(int score) {
        HighScoreText.text = String.Format("High Score: {0}", score);
    }

    public void SetAmmo(string ammo) {
        AmmoText.text = String.Format("Ammo: {0}", ammo);
    }

    public void ShowPause(bool show) {
        PauseObject.SetActive(show);
    }
}
