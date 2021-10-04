using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using TMPro;
using UnityEngine.UI;

public class ShowLeaderboard : MonoBehaviour
{
    public TMP_Text displayText;
    void Awake()
    {
        LeaderboardManager.UpdateScores(displayText);
    }

    void OnEnable()
    {
        LeaderboardManager.UpdateScores(displayText);
    }
}
