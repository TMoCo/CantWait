using LootLocker.Requests;
using LootLocker;
using UnityEngine;
using System.Collections;
using System;
using TMPro;

public class LeaderboardManager : MonoBehaviour
{
    static int llTableID = 496;
    static int llMaxScores = 100;

    private void Start()
    {
	    LootLockerSDKManager.StartSession("Player", (response) =>
	    {
	        if(response.success)
            {
                Debug.Log("Start Session");
            }
            else
	        {
	        	Debug.Log("Fail Start Session");
	        }
	    });
    }

    public static float getTotalScore()
    {
        Debug.Log("Get Total Score");
        float totalScore = 0.0f;
        bool valid = true;
        for(int i = 0; i < 5; i++)
        {
            float thisScore = PlayerPrefs.GetFloat("LevelScore" + (i+1), -1.0f);
            if(thisScore == -1.0f)
            {
                Debug.Log("No Score For");
                Debug.Log(i);
                valid = false;
            }
            totalScore += thisScore;
        }
        if(valid)
            return totalScore;
        else
            return -1.0f;
    }

    public void SubmitScore()
    {
        float totalScore = getTotalScore();
        if(totalScore != -1.0f)
        {
            string userName = PlayerPrefs.GetString("UserName", "No Name");

            LootLockerSDKManager.SubmitScore(userName, (int)(totalScore * 100.0f), llTableID, (response) =>
            {
                if(!response.success)
                {
                    Debug.Log("Fail Submit Score");
                }
                });
            }
        }


    private static string pad(string s, int len)
    {
        string outS = "";
        for(int i = 0; i < Math.Min(s.Length, len); i++)
        {
            outS += s[i];
        }
        for(int i = 0; i < Math.Max(0, len - s.Length); i++)
        {
            outS += " ";
        }
        return outS;
    }

    public static void UpdateScores(TMP_Text textbox)
    {
		string scores = "";

		LootLockerSDKManager.GetScoreList(llTableID, llMaxScores, (response) =>
        {
			for(int i = 0; i < Math.Min(response.pagination.total, 10); i++)
            {
                scores += pad((response.items[i].rank).ToString(), 8) + pad((response.items[i].member_id).ToString(), 25) + "    " + pad(((float)(response.items[i].score) / 100.0f).ToString(), 10) + "\n";
            }
            if(response.pagination.total < 10)
            {
                for(int i = response.pagination.total; i < 10; i++)
                    scores += "---\n";
            }

            // add user score
            scores += "\nYour Score: ";
            float totalScore = getTotalScore();
            if(totalScore == -1.0f)
                scores += "Complete all levels first";
            else
                scores += totalScore;
            textbox.text = scores;
        });
	}
}
