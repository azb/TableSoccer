using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static SoccerGame;

public class AIPlayerAttacker : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }
    
    public static void UpdateWithoutBall(PlayerControls player)
    {
        //If soccer ball is on team 2's side or midfield and this player is on team 1 and an attacker
        if ((SoccerGame.Instance.soccerBallPosition == SoccerGame.SoccerBallPosition.Team2Side
            || SoccerGame.Instance.soccerBallPosition == SoccerGame.SoccerBallPosition.Midfield)
            && player.teamID == TeamID.Team1)
        {
            player.targetZ = SoccerGame.Instance.soccerBall.transform.position.z;
        }

        //If soccer ball is on team 1's side or midfield and this player is on team 2 and an attacker
        if ((SoccerGame.Instance.soccerBallPosition == SoccerGame.SoccerBallPosition.Team1Side
            || SoccerGame.Instance.soccerBallPosition == SoccerGame.SoccerBallPosition.Midfield)
            && player.teamID == TeamID.Team2)
        {
            player.targetZ = SoccerGame.Instance.soccerBall.transform.position.z;
        }
    }

    public static void UpdateWithBall(PlayerControls player)
    {
        //If this player is on team 1 and an attacker
        if (player.teamID == TeamID.Team1)
        {
            Debug.Log("A player 1 attacking player has the ball, should be running to team1Goal");

            player.targetX = SoccerGame.Instance.team2Goal.position.x;
            player.targetZ = SoccerGame.Instance.team2Goal.position.z;

            if (Vector3.Distance(player.transform.position, 
                new Vector3(player.targetX, player.transform.position.y, player.targetZ)) < .25f)
            {
                Debug.Log("In range so should be kicking!");
                if (player.possessionTime > player.AIKickDelay)
                {
                    player.DoKick();
                }
            }
        }

        //If this player is on team 2 and an attacker
        if (player.teamID == TeamID.Team2)
        {
            Debug.Log("A player 2 attacking player has the ball, should be running to team2Goal");

            player.targetX = SoccerGame.Instance.team1Goal.position.x;
            player.targetZ = SoccerGame.Instance.team1Goal.position.z;

            if (Vector3.Distance(player.transform.position, 
                new Vector3(player.targetX, player.transform.position.y, player.targetZ)) < .25f)
            {
                Debug.Log("In range so should be kicking!");
                if (player.possessionTime > player.AIKickDelay)
                {
                    player.DoKick();
                }
            }
        }
    }
}
