using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class StatisticsPanel : MonoBehaviour
{
    [SerializeField] TMP_Text totalTime;
    [SerializeField] TMP_Text averageTime;
    [SerializeField] TMP_Text playthroughCounts;
    [SerializeField] TMP_Text totalWins;
    [SerializeField] TMP_Text totalLoses;

    private void Awake()
    {
        PlayerData playerData = PlayerDataManager.Instance.PlayerData;

        SetTimeInText(totalTime, playerData.totalSeconds);
        SetTimeInText(averageTime, playerData.averageSeconds);
        playthroughCounts.text = playerData.playthrougsCount.ToString();
        totalWins.text = playerData.totalWinCount.ToString();
        totalLoses.text = playerData.totalLoseCount.ToString();
    }


    private void SetTimeInText(TMP_Text text, int seconds)
    {
        if (seconds < 60)
            if (seconds < 10)
                text.text = "0:" + "0" + seconds;
            else
                text.text = "0:" + seconds;
        else
        {
            int minutes = seconds / 60;
            int secondsLeft = seconds - minutes * 60;
            if (secondsLeft < 10)
                text.text = minutes + ":" + "0" + secondsLeft;
            else
                text.text = minutes + ":" + secondsLeft;
        }
    }
}
