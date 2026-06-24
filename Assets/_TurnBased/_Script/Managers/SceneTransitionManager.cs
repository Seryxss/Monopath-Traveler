using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneTransitionManager : PersistentSingleton<SceneTransitionManager>
{
    [Header("Scene Names")]
    [SerializeField] private string battleSceneName = "BattleScene";

    [Header("Transition Data")]
    [SerializeField] private SpawnId nextSpawnPointId = SpawnId.None;
    
    public string BattleSceneName => battleSceneName;
    public SpawnId NextSpawnPointId => nextSpawnPointId;
    
    public void SetNextSpawnPointId(SpawnId spawnId) => nextSpawnPointId = spawnId;

    public void TransitionToScene(string sceneName, SpawnId spawnId)
    {
        SetNextSpawnPointId(spawnId);
        GameManager.Instance.ChangeState(GameState.Exploring);
        SceneManager.LoadScene(sceneName);
    }

    public void TransitionToBattle()
    {
        GameManager.Instance.ChangeState(GameState.InBattle);
        SceneManager.LoadScene(battleSceneName);
    }
}