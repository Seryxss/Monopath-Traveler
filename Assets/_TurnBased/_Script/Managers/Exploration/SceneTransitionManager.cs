using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Collections.Generic;

public class SceneTransitionManager : PersistentSingleton<SceneTransitionManager>
{
    [Header("Scene Names")]
    [SerializeField] private string battleSceneName = "BattleScene";
    [SerializeField] private string startSceneName = "StartScene";

    [Header("Transition Data")]
    [SerializeField] private SpawnId nextSpawnPointId = SpawnId.None;

    [Header("Transition UI")]
    [Tooltip("Masukkan CanvasGroup dari layar hitam (Panel UI) di sini")]
    [SerializeField] private CanvasGroup fadePanel;
    [SerializeField] private float fadeDuration = 1f;
    
    public string lastSceneBeforeBattle;
    public Vector3 lastPlayerPosition;
    public bool isReturningFromBattle = false;

    // 1. TAMBAHKAN VARIABEL GEMBOK INI
    public bool isTransitioning { get; private set; } = false;

    public string BattleSceneName => battleSceneName;
    public SpawnId NextSpawnPointId => nextSpawnPointId;
    
    public void SetNextSpawnPointId(SpawnId spawnId) => nextSpawnPointId = spawnId;

    public void TransitionToScene(string sceneName, SpawnId spawnId)
    {
        if (isTransitioning) return; 

        SetNextSpawnPointId(spawnId);
        
        StartCoroutine(TransitionRoutine(sceneName, GameState.Exploring));
    }
    

    public void TransitionToStartScene()
    {
        if (isTransitioning) return;

        isReturningFromBattle = false; 
        if (ProgressManager.Instance != null)
        {
            ProgressManager.Instance.ResetAllProgress();
        }
        
        StartCoroutine(TransitionRoutine(startSceneName, GameState.Exploring));
    }

    public void TransitionToBattle()
    {
        if (isTransitioning) return;

        lastSceneBeforeBattle = SceneManager.GetActiveScene().name;
        
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null) lastPlayerPosition = player.transform.position;

        isReturningFromBattle = true;
        
        GameManager.Instance.ChangeState(GameState.InBattle);

        StartCoroutine(TransitionRoutine(battleSceneName, GameState.InBattle));
    }

    public void ReturnFromBattle()
    {
        if (isTransitioning) return;

        string sceneToLoad = !string.IsNullOrEmpty(lastSceneBeforeBattle) ? lastSceneBeforeBattle : "SampleScene";
        
        StartCoroutine(TransitionRoutine(sceneToLoad, GameState.Exploring));
    }

    // 3. COROUTINE YANG SUDAH DIPERBARUI
    private IEnumerator TransitionRoutine(string targetScene, GameState targetStateAfterFade)
    {
        isTransitioning = true;

        if (fadePanel != null)
        {
            fadePanel.blocksRaycasts = true; 
            float timer = 0;
            while (timer < fadeDuration)
            {
                fadePanel.alpha = Mathf.Lerp(0, 1, timer / fadeDuration);
                timer += Time.deltaTime;
                yield return null;
            }
            fadePanel.alpha = 1;
        }

        yield return SceneManager.LoadSceneAsync(targetScene);

        if (fadePanel != null)
        {
            float timer = 0;
            while (timer < fadeDuration)
            {
                fadePanel.alpha = Mathf.Lerp(1, 0, timer / fadeDuration);
                timer += Time.deltaTime;
                yield return null;
            }
            fadePanel.alpha = 0;
            fadePanel.blocksRaycasts = false;
        }

        GameManager.Instance.ChangeState(targetStateAfterFade);
        isTransitioning = false; 
    }

    
}