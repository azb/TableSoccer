using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Scoreboard : MonoBehaviour
{
    public TextMeshProUGUI Team1ScoreLabel;
    public TextMeshProUGUI Team2ScoreLabel;

    public int Team1Score;
    public int Team2Score;

    public void IncrementTeam1Score()
    {
        Team1Score++;
        Team1ScoreLabel.text = "" + Team1Score;
    }

    public void IncrementTeam2Score()
    {
        Team2Score++;
        Team2ScoreLabel.text = "" + Team2Score;
    }
}
