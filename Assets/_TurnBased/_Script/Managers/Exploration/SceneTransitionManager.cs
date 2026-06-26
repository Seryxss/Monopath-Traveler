using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class SceneTransitionManager : PersistentSingleton<SceneTransitionManager>
{
    [Header("Scene Names")]
    [SerializeField] private string battleSceneName = "BattleScene";
    [SerializeField] private string startSceneName = "StartScene";

    [Header("Transition Data")]
    [SerializeField] private SpawnId nextSpawnPointId = SpawnId.None;

    [Header("Transition UI (Fade)")]
    [Tooltip("Canvas Group")]
    [SerializeField] private CanvasGroup fadePanel;
    [SerializeField] private float fadeDuration = 1f;

    [Header("Loading Screen UI")]
    [Tooltip("Panel Loading Screen ")]
    [SerializeField] private GameObject loadingPanel;
    [SerializeField] private float minLoadingTime = 5.0f;
    
    public string lastSceneBeforeBattle;
    public Vector3 lastPlayerPosition;
    public bool isReturningFromBattle = false;

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
        if (ProgressManager.Instance != null) ProgressManager.Instance.ResetAllProgress();
        
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

        string sceneToLoad = !string.IsNullOrEmpty(lastSceneBeforeBattle) ? lastSceneBeforeBattle : "StartScene";
        StartCoroutine(TransitionRoutine(sceneToLoad, GameState.Exploring));
    }

    private IEnumerator TransitionRoutine(string targetScene, GameState targetStateAfterFade)
    {
        isTransitioning = true;

        // 1. FADE OUT (Layar jadi hitam)
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

        // 2. MUNCULKAN LOADING SCREEN & ANIMASINYA
        if (loadingPanel != null) loadingPanel.SetActive(true);

        // 3. ASYNCHRONOUS LOADING (TANPA KATA "yield return" !!)
        AsyncOperation operation = SceneManager.LoadSceneAsync(targetScene);
        
        // Langsung kunci agar Unity tidak pindah scene otomatis
        operation.allowSceneActivation = false;

        float loadingTimer = 0f;

        // Tahan loop ini sampai waktu minimal (10 detik) habis DAN memori selesai memuat (0.9f)
        while (loadingTimer < minLoadingTime || operation.progress < 0.9f)
        {
            loadingTimer += Time.deltaTime;
            yield return null; 
        }

        // 4. WAKTU HABIS, IZINKAN PINDAH SCENE
        operation.allowSceneActivation = true;

        // Tunggu sepersekian frame sampai Unity benar-benar selesai berpindah
        while (!operation.isDone)
        {
            yield return null;
        }

        // 5. SEMBUNYIKAN LOADING SCREEN
        if (loadingPanel != null) loadingPanel.SetActive(false);

        // 6. FADE IN (Layar terang kembali)
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