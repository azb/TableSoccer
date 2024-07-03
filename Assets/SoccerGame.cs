using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoccerGame : MonoBehaviour
{
    public enum GameState { WaitingForPlayers, Playing, BallOutOfBounds, ThrowingFromBounds, Foul, PenaltyKick, GameOver };
    public GameState gameState = GameState.WaitingForPlayers;
    public static SoccerGame Instance;
    
    void Start()
    {
        Instance = this;
    }

    public static GameState GetState()
    {
        return Instance.gameState;
    }

    public static GameState SetState(GameState gameState)
    {
        return Instance.gameState = gameState;
    }
}
