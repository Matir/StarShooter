using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartScript : MonoBehaviour
{
    public GameObject LevelController;
    public GameObject HUD;
    public GameObject Player;
    public GameObject StartText;
    public GameObject HelpScreen;

    // Start is called before the first frame update
    void Start()
    {
        HUD.SetActive(false);
        LevelController.SetActive(false);
        Player.SetActive(false);
        DisplayMain();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Question) || Input.GetKeyDown(KeyCode.H)) {
            // Display help
            DisplayHelp();
        }
        if (Input.GetKeyDown(KeyCode.Exclaim)) {
            DisplayMain();
        }
        if (Input.GetKeyDown(KeyCode.Space)) {
            StartCoroutine(StartGame());
        }
    }

    // Display main
    void DisplayMain() {
        HelpScreen.SetActive(false);
        StartText.SetActive(true);
    }

    // Display help
    void DisplayHelp() {
        StartText.SetActive(false);
        HelpScreen.SetActive(true);
    }

    // Start the game
    IEnumerator StartGame() {
        HUD.SetActive(true);
        StartText.SetActive(false);
        HelpScreen.SetActive(false);
        Player.SetActive(true);
        yield return new WaitForSeconds(2.0f);
        LevelController.SetActive(true);
        // Disable self as final step
        gameObject.SetActive(false);
    }
}
