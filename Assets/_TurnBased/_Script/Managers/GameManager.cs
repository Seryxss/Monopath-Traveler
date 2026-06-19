using System;
using UnityEngine;
using UnityEngine.SceneManagement; // Required for loading scenes

public class GameManager : PersistentSingleton<GameManager> 
{
    public static event Action<GameState> OnGameStateChanged;

    public GameState State { get; private set; }

    // Store the name of your scenes in the Inspector
    [Header("Scene Names")]
    public string overworldSceneName = "Overworld";
    public string battleSceneName = "BattleScene";

    private void Start()
    {
        ChangeState(GameState.Exploring); 
    }

    public void ChangeState(GameState newState)
    {
        if (State == newState) return;

        State = newState;
        OnGameStateChanged?.Invoke(newState);

        switch (newState)
        {
            case GameState.MainMenu:
                break;
            case GameState.Exploring:
                if (SceneManager.GetActiveScene().name != overworldSceneName)
                {
                    SceneManager.LoadScene(overworldSceneName);
                }
                break;

            case GameState.InDialog:
                break;

            case GameState.InBattle:
                SceneManager.LoadScene(battleSceneName);
                break;
        }

        Debug.Log($"Game State: {newState}");
    }
}