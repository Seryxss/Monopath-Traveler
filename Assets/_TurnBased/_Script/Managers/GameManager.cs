using System;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;

public class GameManager : Singleton<GameManager>
{
    public static event Action<GameState> OnPreStateChange;
    public static event Action<GameState> OnPostStateChange;

    public GameState State { get; private set; }

    //Start game with State
    void Start()
    {
        ChangeState(GameState.Starting);
    }

    public void ChangeState(GameState newState)
    {
        if(State == newState) return;
        
        OnPreStateChange?.Invoke(newState);

        State = newState;
        switch (newState)
        {
            case GameState.Starting:
                break;
            case GameState.SpawningHeroes:
                HandlingSpawningHeroes();
                break;
            case GameState.SpawningEnemies:
                HandlingSpawningEnemies();
                break;
            case GameState.Win:
                break;
            case GameState.Lose:
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(newState), newState, null);
        }

        OnPostStateChange?.Invoke(newState);

        Debug.Log($"New State: {newState}");
    }

    public void HandlingSpawningHeroes()
    {
        ChangeState(GameState.SpawningEnemies);
    }

    public void HandlingSpawningEnemies()
    {
        ChangeState(GameState.HeroTurn);
    }
}


[Serializable]
public enum GameState{
    Starting = 0,
    SpawningHeroes = 1,
    SpawningEnemies = 2,
    HeroTurn = 3,
    EnemyTurn = 4,
    Win = 8,
    Lose = 9
}
