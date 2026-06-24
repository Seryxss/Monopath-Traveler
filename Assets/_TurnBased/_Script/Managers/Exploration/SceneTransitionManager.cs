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

    public string BattleSceneName => battleSceneName;
    public SpawnId NextSpawnPointId => nextSpawnPointId;
    
    public void SetNextSpawnPointId(SpawnId spawnId) => nextSpawnPointId = spawnId;

    // REVISI FUNGSI INI DI SCENETRANSITIONMANAGER.CS
    public void TransitionToScene(string sceneName, SpawnId spawnId)
    {
        SetNextSpawnPointId(spawnId);
        GameManager.Instance.ChangeState(GameState.Exploring);
        
        StartCoroutine(TransitionRoutine(sceneName));
    }

    public void TransitionToStartScene()
    {
        isReturningFromBattle = false; 
        if (ProgressManager.Instance != null)
        {
            ProgressManager.Instance.ResetAllProgress();
        }
        SceneManager.LoadScene(startSceneName);
    }

    public void TransitionToBattle()
    {
        lastSceneBeforeBattle = SceneManager.GetActiveScene().name;
        
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null) lastPlayerPosition = player.transform.position;

        isReturningFromBattle = true;
        GameManager.Instance.ChangeState(GameState.InBattle);

        StartCoroutine(TransitionRoutine(battleSceneName));
    }

    // Fungsi transisi pulang juga memanggil Coroutine
    public void ReturnFromBattle()
    {
        GameManager.Instance.ChangeState(GameState.Exploring);
        string sceneToLoad = !string.IsNullOrEmpty(lastSceneBeforeBattle) ? lastSceneBeforeBattle : "SampleScene";
        StartCoroutine(TransitionRoutine(sceneToLoad));
    }

    // COROUTINE AJAIB UNTUK FADE YANG MULUS
    private IEnumerator TransitionRoutine(string targetScene)
    {
        // 1. Fade Out (Layar Menjadi Hitam)
        if (fadePanel != null)
        {
            fadePanel.blocksRaycasts = true; // Cegah pemain klik apapun
            float timer = 0;
            while (timer < fadeDuration)
            {
                fadePanel.alpha = Mathf.Lerp(0, 1, timer / fadeDuration);
                timer += Time.deltaTime;
                yield return null;
            }
            fadePanel.alpha = 1;
        }

        // 2. Load Scene (Penonton tidak akan melihat proses ini karena layar hitam)
        yield return SceneManager.LoadSceneAsync(targetScene);

        // 3. Fade In (Layar Kembali Terang di scene yang baru)
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
    }
}