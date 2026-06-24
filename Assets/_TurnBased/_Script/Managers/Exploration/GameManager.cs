using System;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : PersistentSingleton<GameManager> 
{
    public static event Action<GameState> OnGameStateChanged;

    [Header("Audio")]
    [SerializeField] private AudioClip exploringBGM;

    public GameState State { get; private set; }

    [Header("Player Data (Party)")]
    [SerializeField] private List<HeroType> currentParty = new List<HeroType>();
    
    public List<HeroType> CurrentParty => currentParty; 

    private void Start()
    {
        PlayBGM();
        ChangeState(GameState.Exploring); 
    }

    public void PlayBGM()
    {
        AudioSystem.Instance.PlayMusic(exploringBGM);
    }

    public void ChangeState(GameState newState)
    {
        if (State == newState) return;

        State = newState;
        OnGameStateChanged?.Invoke(newState);
        
        Debug.Log($"Game State Berubah: {newState}");
    }

    public void AddPartyMember(HeroType newHero)
    {
        // Pastikan hero belum ada di dalam party agar tidak dobel
        if (!currentParty.Contains(newHero))
        {
            currentParty.Add(newHero);
            Debug.Log($"[GameManager] {newHero} telah bergabung ke dalam Party!");
        }
    }
}