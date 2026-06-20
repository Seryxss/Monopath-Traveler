using System;
using UnityEngine;

public class BattleManager : Singleton<BattleManager>
{
    public static event Action<BattleState> OnPreStateChange;
    public static event Action<BattleState> OnPostStateChange;

    public BattleState State { get; private set; }

    void Start()
    {
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
                Debug.Log("Hero Turn");
                break;
            case BattleState.EnemyTurn:
                break;
            case BattleState.Win:
                break;
            case BattleState.Lose:
                break;
        }

        OnPostStateChange?.Invoke(newState);
        Debug.Log($"Battle State: {newState}");
    }

    private void HandleSpawningHeroes()
    {
        Debug.Log("Spawning Hero");
        CharacterManager.Instance.SpawnHeroes(); 
    
        ChangeState(BattleState.SpawningEnemies);
    }

    private void HandleSpawningEnemies()
    {
        Debug.Log("Spawning Enemy");
        CharacterManager.Instance.SpawnEnemies(); 
        
        ChangeState(BattleState.HeroTurn);
    }
}