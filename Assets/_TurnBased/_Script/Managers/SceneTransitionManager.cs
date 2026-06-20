using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneTransitionManager : PersistentSingleton<SceneTransitionManager>
{
    [Header("Scene Names")]
    public string battleSceneName = "BattleScene";

    [Header("Transition Data")]
    public SpawnId nextSpawnPointId = SpawnId.None;

    public void TransitionToScene(string sceneName, SpawnId spawnId)
    {
        nextSpawnPointId = spawnId;
        GameManager.Instance.ChangeState(GameState.Exploring);
        SceneManager.LoadScene(sceneName);
    }

    public void TransitionToBattle()
    {
        GameManager.Instance.ChangeState(GameState.InBattle);
        SceneManager.LoadScene(battleSceneName);
    }
}