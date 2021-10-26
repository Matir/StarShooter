using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class HUDScript : MonoBehaviour
{
    public TMP_Text LevelText;
    public TMP_Text ScoreText;

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
}
