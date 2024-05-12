using Photon.Pun;
using TMPro;
using UnityEngine;

public class Scoreboard : MonoBehaviour
{
    public TextMeshProUGUI[] Team1ScoreLabel;
    public TextMeshProUGUI[] Team2ScoreLabel;

    public int Team1Score;
    public int Team2Score;

    PhotonView photonView;

    private void Start()
    {
        photonView = GetComponent<PhotonView>();
    }

    public void IncrementTeam1Score()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            photonView.RPC("SetTeam1Score", RpcTarget.All, Team1Score + 1);
        }
    }

    public void IncrementTeam2Score()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            photonView.RPC("SetTeam2Score", RpcTarget.All, Team2Score + 1);
        }
    }

    [PunRPC]
    void SetTeam1Score(int newScore)
    {
        Team1Score = newScore;
        for (int i = 0; i < Team1ScoreLabel.Length; i++)
        {
            Team1ScoreLabel[i].text = "" + newScore;
        }
    }

    [PunRPC]
    void SetTeam2Score(int newScore)
    {
        Team2Score = newScore;
        for (int i = 0; i < Team2ScoreLabel.Length; i++)
        {
            Team2ScoreLabel[i].text = "" + newScore;
        }
    }
}
