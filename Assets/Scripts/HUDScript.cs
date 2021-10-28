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

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
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
}
