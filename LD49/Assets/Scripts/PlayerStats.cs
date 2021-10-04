using UnityEngine;
using TMPro;

public class PlayerStats : MonoBehaviour
{
    // array of stars
    public GameObject[] stars = new GameObject[15];

    void Start()
    {
        for (int i = 0; i < stars.Length; ++i)
        {
            stars[i] = transform.Find("Star" + i).gameObject;
            stars[i].SetActive(false);
        }
        gameObject.SetActive(false);
    }

    public void ShowStats()
    {
        for (int lvl = 0; lvl < 5; ++lvl)
        {
            // show stars for the level
            int rating = PlayerPrefs.GetInt("LevelRating" + (lvl+1), 0);    
            switch(rating)
            {
                case 3:
                    stars[lvl * 3    ].SetActive(true);
                    stars[lvl * 3 + 1].SetActive(true);
                    stars[lvl * 3 + 2].SetActive(true);
                    break;
                case 2:
                    stars[lvl * 3    ].SetActive(true);
                    stars[lvl * 3 + 1].SetActive(true);
                    break;
                case 1:
                    stars[lvl * 3    ].SetActive(true);
                    break;
                default:
                    break;
            }

            UpdateScoreText(lvl + 1);
        }

        gameObject.SetActive(true);
    }

    public void HideStats()
    {
        gameObject.SetActive(false);
    }

    public void UpdateScoreText(int level)
    {
        // show score
        float score = PlayerPrefs.GetFloat("LevelScore" + level, -1.0f);
        transform.Find("Score" + level).gameObject.GetComponent<TextMeshProUGUI>().text = score != -1.0f ? score .ToString("f2") : "";
    }

}
