using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;

public class InGameUIManager : MonoBehaviour
{
    [SerializeField] LevelManager lvlManager;
    public GameObject loseMenu;
    public GameObject pauseMenu;
    public GameObject winMenu;
    public TextMeshProUGUI stopWatchText;

    void Awake()
    {
        pauseMenu = transform.Find("PauseMenu").gameObject;
        loseMenu = transform.Find("LoseMenu").gameObject;
        winMenu = transform.Find("WinMenu").gameObject;
        // by default, pause is active
        pauseMenu.SetActive(false);
        loseMenu.SetActive(false);
        winMenu.SetActive(false);
    }

    // Button methods
    public void ButtonResetLevel()
    {
        lvlManager.ResetLevel();
    }

    public void ButtonResume()
    {
        pauseMenu.SetActive(false);
        lvlManager.ResumeLevel();
    }

    public void ButtonViewLeaderBoard()
    {

    }

    public void ButtonNextLevel()
    {
        GameScenesManager.LoadNextScene();
        lvlManager.ResumeLevel();
    }

    public void ButtonExitToMainMenu()
    {
        GameScenesManager.LoadScene(GameScenesManager.Scenes.MainMenu);
    }

    public void TextShowTimeElapsed(float elapsed)
    {
        stopWatchText.text = string.Format("Elapsed: {0}", elapsed);
    }


    public void ShowStats(int rating, float score)
    {
        if (rating >= 1)
            winMenu.transform.Find("Star0").gameObject.SetActive(true);
        if (rating >= 2)
            winMenu.transform.Find("Star1").gameObject.SetActive(true);
        if (rating >= 3)
            winMenu.transform.Find("Star2").gameObject.SetActive(true);
        winMenu.transform.Find("Text").gameObject.GetComponent<TextMeshProUGUI>().text = "Score: " + score.ToString("f2");
    }
}
