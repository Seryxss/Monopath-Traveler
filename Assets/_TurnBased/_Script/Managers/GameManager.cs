using System;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : PersistentSingleton<GameManager> 
{
    public static event Action<GameState> OnGameStateChanged;

    public GameState State { get; private set; }

    [Header("Player Data (Party)")]
    // Sesuai permintaanmu, data tim masih di sini untuk sementara
    public List<HeroType> currentParty = new List<HeroType>(); 

    private void Start()
    {
        ChangeState(GameState.InBattle); 
    }

    public void ChangeState(GameState newState)
    {
        if (State == newState) return;

        State = newState;
        OnGameStateChanged?.Invoke(newState);
        
        Debug.Log($"Game State Berubah: {newState}");
    }
}