using System;
using UnityEngine;

public class BattleManager : Singleton<BattleManager>
{
    public static event Action<BattleState> OnPreStateChange;
    public static event Action<BattleState> OnPostStateChange;

    public BattleState State { get; private set; }

    void Start()
    {
        // The battle starts the moment this scene is loaded!
        ChangeState(BattleState.SpawningHeroes);
    }

    public void ChangeState(BattleState newState)
    {
        if(State == newState) return;
        
        OnPreStateChange?.Invoke(newState);
        State = newState;

        switch (newState)
        {
            case BattleState.SpawningHeroes:
                HandleSpawningHeroes();
                break;
            case BattleState.SpawningEnemies:
                HandleSpawningEnemies();
                break;
            case BattleState.HeroTurn:
                // Wait for player input on their CharacterBase scripts
                break;
            case BattleState.EnemyTurn:
                // Trigger Enemy AI scripts
                break;
            case BattleState.Win:
                // Show victory screen, then tell GameManager to load overworld
                break;
            case BattleState.Lose:
                // Show game over screen
                break;
        }

        OnPostStateChange?.Invoke(newState);
        Debug.Log($"Battle State: {newState}");
    }

    private void HandleSpawningHeroes()
    {
        CharacterManager.Instance.SpawnHeroes(); 
    
        ChangeState(BattleState.SpawningEnemies);
    }

    private void HandleSpawningEnemies()
    {
        // CharacterManager.Instance.SpawnEnemies(); 
        ChangeState(BattleState.HeroTurn);
    }
}