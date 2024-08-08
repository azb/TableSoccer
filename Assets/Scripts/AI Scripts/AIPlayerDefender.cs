using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static SoccerGame;

public class AIPlayerDefender : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    public static void UpdateWithoutBall(PlayerControls player)
    {
        //If soccer ball is on team 1's side and this player is on team 1 and a defender
        if (SoccerGame.Instance.soccerBallPosition == SoccerGame.SoccerBallPosition.Team1Side
            && player.teamID == TeamID.Team1)
        {
            player.targetZ = SoccerGame.Instance.soccerBall.transform.position.z;
        }

        //If soccer ball is on team 2's side and this player is on team 2 and a defender
        if (SoccerGame.Instance.soccerBallPosition == SoccerGame.SoccerBallPosition.Team2Side
            && player.teamID == TeamID.Team2)
        {
            player.targetZ = SoccerGame.Instance.soccerBall.transform.position.z;
        }
    }

    // Update is called once per frame
    public static void UpdateWithBall(PlayerControls player)
    {
        //If soccer ball is on team 1's side and this player is on team 1 and a defender
        if (SoccerGame.Instance.soccerBallPosition == SoccerGame.SoccerBallPosition.Team1Side
            && player.teamID == TeamID.Team1)
        {
            float centerZ = SoccerGame.Instance.transform.position.z;
            player.targetZ = centerZ;
            if (Mathf.Abs(player.targetZ - centerZ) < .02f)
            {
                //kick the ball
                if (player.possessionTime > player.AIKickDelay)
                {
                    player.DoKick();
                }
            }
        }

        //If soccer ball is on team 2's side and this player is on team 2 and a defender
        if (SoccerGame.Instance.soccerBallPosition == SoccerGame.SoccerBallPosition.Team2Side
            && player.teamID == TeamID.Team2)
        {
            Debug.Log("A player 2 defensive player has the ball");

            float centerZ = SoccerGame.Instance.transform.position.z;
            player.targetZ = centerZ;
            if (Mathf.Abs(player.targetZ - centerZ) < .02f)
            {
                Debug.Log("Should be kicking the ball");
                //kick the ball
                if (player.possessionTime > player.AIKickDelay)
                {
                    player.DoKick();
                }
            }
            else
            {
                Debug.Log("Should be running to the center");
            }
        }
    }
}
