using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameScenesManager : MonoBehaviour
{
    public enum Scenes : int
    {
        MainMenu = 0,
        Level1   = 1,
        Level2   = 2,
        Level3   = 3,
        Level4   = 4,
        Level5   = 5,
        Credits  = 6
    }

    public static int GetCurrentLevelIndex()
    {
        return SceneManager.GetActiveScene().buildIndex;
    }

    public static void ReLoadScene()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex, LoadSceneMode.Single);
    }

    public static void LoadCreditsScene()
    {
        SceneManager.LoadScene((int)Scenes.Credits, LoadSceneMode.Single);
    }

    public static void LoadMainMenuScene()
    {
        SceneManager.LoadScene((int)Scenes.MainMenu, LoadSceneMode.Single);
    }

    public static void LoadScene(Scenes scene)
    {
        SceneManager.LoadScene((int)scene, LoadSceneMode.Single);
    }

    public static void LoadNextScene()
    {
        int index = SceneManager.GetActiveScene().buildIndex;
        SceneManager.LoadScene(index < SceneManager.sceneCountInBuildSettings ? index += 1 : index, LoadSceneMode.Single);
    }

    public static void QuitApplication()
    {
        Application.Quit();
    }
}
