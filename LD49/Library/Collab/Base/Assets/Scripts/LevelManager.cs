using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using StarterAssets;

[RequireComponent(typeof(BoxCollider))]
public class LevelManager : MonoBehaviour
{
    public GameObject playerObject;
    public StopWatch stopWatch;
    public AIManager aIManager;
    public InGameUIManager uIManager;

    public AudioSource winSource;
    public AudioSource loseSource;
    public AudioSource[] plateSounds;
    public AudioSource[] plantSounds;
    public AudioSource[] cutlerySounds;
    public AudioSource[] glassSounds;
    public AudioSource[] woodSounds;
    public AudioSource[] peopleSounds;
    private int plateIndex;
    private int plantIndex;
    private int cutleryIndex;
    private int glassIndex;
    private int woodIndex;
    private int peopleIndex;

    public float score = 0.0f;
    public int objectsDestroyed = 0;

    public float[][] levelRatings = new float[5][]
    {
        new float [3] // level 0
        {
            10.0f, 15.0f, 20.0f
        },
        new float [3] // level 1
        {
            20.0f, 30.0f, 40.0f
        },
        new float [3] // level 2
        {
            10.0f, 20.0f, 30.0f
        },
        new float [3] // level 3
        {
            10.0f, 20.0f, 30.0f
        },
        new float [3] // level 4
        {
            10.0f, 20.0f, 30.0f
        }
    };

    void Awake()
    {
        if (uIManager) stopWatch.onTick = uIManager.TextShowTimeElapsed;
    }

    // level start
    void Start()
    {
        if (stopWatch)
        {
            stopWatch.ResetCount();
            stopWatch.StartCount();
        }
        Physics.autoSimulation = true;
    }

    public void ResetLevel()
    {
        GameScenesManager.ReLoadScene();
        ResumeLevel();
    }

    public void PauseLevel()
    {
        stopWatch.StopCount();
        playerObject.GetComponent<ThirdPersonController>().enabled = false;
        playerObject.GetComponent<Animator>().speed = 0.0f;
        aIManager.PauseAIControllers(true);
        Physics.autoSimulation = false;
    }

    public void ResumeLevel()
    {
        Physics.autoSimulation = true;
        aIManager.PauseAIControllers(false);
        playerObject.GetComponent<Animator>().speed = 1.0f;
        playerObject.GetComponent<ThirdPersonController>().enabled = true;
        stopWatch.StartCount();
    }

    public void ShowOrHideUI()
    {
        if (!uIManager.loseMenu.activeInHierarchy)
        {
            if (uIManager.pauseMenu.activeInHierarchy)
            {
                // hide
                uIManager.pauseMenu.SetActive(false);
                ResumeLevel();
            }
            else
            {
                // show
                PauseLevel();
                uIManager.pauseMenu.SetActive(true);
            }
        }
    }

    public void PlayerLost()
    {
        PauseLevel();
        uIManager.pauseMenu.SetActive(false);
        uIManager.loseMenu.SetActive(true);
        loseSource.Play();
    }

    public void PlayerWon()
    {
        if (!playerObject.GetComponent<ThirdPersonController>()._LostGame)
        {
            PauseLevel();
            uIManager.pauseMenu.SetActive(false);
            uIManager.winMenu.SetActive(true);
            int rating = GetRating(score);
            uIManager.ShowStats(rating, score);
            // save level rating
            int level = GameScenesManager.GetCurrentLevelIndex();

            if (rating > PlayerPrefs.GetInt("LevelRating" + level))
                PlayerPrefs.SetInt("LevelRating" + level, rating); // higher rating is better

            float currentLevelScore = PlayerPrefs.GetFloat("LevelScore" + level, -1.0f);
            if ((score < currentLevelScore) || (currentLevelScore == -1.0f)) // lower score is better
            {
                Debug.Log("Saving Level Score");
                Debug.Log(level);
                Debug.Log(score);
                PlayerPrefs.SetFloat("LevelScore" + level, score);
            }
            winSource.Play();
            gameObject.GetComponent<LeaderboardManager>().SubmitScore();
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            stopWatch.StopCount();
            score = stopWatch.count;
            score += Mathf.Min(Mathf.Pow(objectsDestroyed, 1.1f) - 1.0f * objectsDestroyed, 100.0f); // clamps within range [0, 100]
            PlayerWon();
        }
    }

    int GetRating(float score)
    {
        int rating = 0;
        if (score < 20.0f) // 3 stars: less than 20 seconds
        {
            rating++;
        }
        if (score < 30.0f) // 2 stars: less than 30 seconds
        {
            rating++;
        }
        if (score < 40.0f) // 1 star: less than 40 seconds
        {
            rating++;
        }
        return rating;
    }

    public void Destroyed(Vector3 position, string objectName)
    {
        objectsDestroyed++;
        switch(objectName)
        {
            case("plate"):
                plateSounds[plateIndex].transform.position = position;
                plateSounds[plateIndex].Play();
                plateIndex += 1;
                plateIndex = plateIndex % plateSounds.Length;
                break;
            case("plant"):
                plantSounds[plantIndex].transform.position = position;
                plantSounds[plantIndex].Play();
                plantIndex += 1;
                plantIndex = plantIndex % plantSounds.Length;
                break;
            case("cutlery"):
                cutlerySounds[cutleryIndex].transform.position = position;
                cutlerySounds[cutleryIndex].Play();
                cutleryIndex += 1;
                cutleryIndex = cutleryIndex % cutlerySounds.Length;
                break;
            case("glass"):
                glassSounds[glassIndex].transform.position = position;
                glassSounds[glassIndex].Play();
                glassIndex += 1;
                glassIndex = glassIndex % glassSounds.Length;
                break;
            case("wood"):
                woodSounds[woodIndex].transform.position = position;
                woodSounds[woodIndex].Play();
                woodIndex += 1;
                woodIndex = woodIndex % woodSounds.Length;
                break;
            case("person"):
                peopleSounds[peopleIndex].transform.position = position;
                peopleSounds[peopleIndex].Play();
                peopleIndex += 1;
                peopleIndex = peopleIndex % peopleSounds.Length;
                break;
            default:
                break;
        }
    }
}
