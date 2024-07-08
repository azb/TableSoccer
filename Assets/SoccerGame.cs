using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoccerGame : MonoBehaviour
{
    public enum GameState { WaitingForPlayers, Playing, Goal, BallOutOfBounds, ThrowingFromBounds, Foul, PenaltyKick, GameOver };

    public GameState gameState = GameState.Playing;

    public enum TeamID { Team1 = 0, Team2 = 1, None = 2 };

    public TeamID _possessingTeam = TeamID.None;

    public static TeamID PossessingTeam
    {
        get
        {
            return Instance._possessingTeam;
        }
    }

    public PlayerControls _possessingPlayer = null;

    public static PlayerControls PossessingPlayer
    {
        get
        {
            return Instance._possessingPlayer;
        }
    }

    public static SoccerGame Instance;

    public PlayerControls[] soccerPlayers;
    public SoccerBall soccerBall;

    public PlayerControls closestTeam1Player;
    public PlayerControls closestTeam2Player;

    public Transform team1Goal;
    public Transform team2Goal;

    public enum SoccerBallPosition { Team1Side, Midfield, Team2Side }
    public SoccerBallPosition soccerBallPosition = SoccerBallPosition.Midfield;

    void Start()
    {
        Instance = this;
        soccerPlayers = FindObjectsOfType<PlayerControls>();
        Invoke("UpdateClosestPlayer", .2f);
    }

    private void Update()
    {
        //Update which team possesses the ball
        PhotonView PossessingPlayerPhotonView = this.soccerBall.PossessingPlayer;
        if (PossessingPlayerPhotonView != null)
        {
            PlayerControls possessingPlayerControls = PossessingPlayerPhotonView.GetComponent<PlayerControls>();
            _possessingPlayer = possessingPlayerControls;
            if (possessingPlayerControls != null)
            {
                _possessingTeam = possessingPlayerControls.teamID;
            }
        }
        else
        {
            _possessingPlayer = null;
            _possessingTeam = TeamID.None;
        }

        float ballDistanceToCenter = Vector3.Distance(soccerBall.transform.position, transform.position);
        float ballDistanceToTeam1Goal = Vector3.Distance(soccerBall.transform.position, team1Goal.position);
        float ballDistanceToTeam2Goal = Vector3.Distance(soccerBall.transform.position, team2Goal.position);

        if (ballDistanceToCenter < ballDistanceToTeam1Goal)
        {
            if (ballDistanceToCenter < ballDistanceToTeam2Goal)
            {
                //Ball is mid field
                soccerBallPosition = SoccerBallPosition.Midfield;
            }
            else
            {
                //Ball is near team 2 goal
                soccerBallPosition = SoccerBallPosition.Team2Side;
            }
        }
        else
        {
            //Ball is near team 1 goal
            soccerBallPosition = SoccerBallPosition.Team1Side;
        }
    }

    public static GameState GetState()
    {
        return Instance.gameState;
    }

    public static GameState SetState(GameState gameState)
    {
        return Instance.gameState = gameState;
    }

    private void UpdateClosestPlayer()
    {
        float closestTeam1Distance = float.MaxValue;
        float closestTeam2Distance = float.MaxValue;
        int closestTeam1Index = 0;
        int closestTeam2Index = 0;

        for (int i = 0; i < soccerPlayers.Length; i++)
        {
            float distance = Vector3.Distance(soccerPlayers[i].transform.position, soccerBall.transform.position);

            if (soccerPlayers[i].teamID == TeamID.Team1)
            {
                //if team 1
                if (distance < closestTeam1Distance)
                {
                    closestTeam1Distance = distance;
                    closestTeam1Index = i;
                }
            }
            else
            {
                //if team 2
                if (distance < closestTeam2Distance)
                {
                    closestTeam2Distance = distance;
                    closestTeam2Index = i;
                }
            }
        }

        closestTeam1Player = soccerPlayers[closestTeam1Index];
        closestTeam2Player = soccerPlayers[closestTeam2Index];

        Invoke("UpdateClosestPlayer", .2f);
    }
}
