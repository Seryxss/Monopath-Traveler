using UnityEngine;

public class BattleResultUI : MonoBehaviour
{
    [Header("Panels")]
    [SerializeField] private CanvasGroup victoryPanel;
    [SerializeField] private CanvasGroup defeatPanel;

    [Header("Scene Names")]
    public string exploreSceneName = "ExploreScene"; 
    public string mainMenuSceneName = "MainMenu";

    private void Awake()
    {
        // Pastikan kedua panel tersembunyi saat start
        SetPanelVisible(victoryPanel, false);
        SetPanelVisible(defeatPanel, false);
    }

    public void ShowVictory()
    {
        SetPanelVisible(victoryPanel, true);
    }

    public void ShowDefeat()
    {
        SetPanelVisible(defeatPanel, true);
    }

    public void OnTapToContinueVictory()
    {
        if (ProgressManager.Instance != null)
            ProgressManager.Instance.CompleteActiveEvent();

        if (SceneTransitionManager.Instance != null)
        {
            SceneTransitionManager.Instance.ReturnFromBattle();
        }
    }

    public void OnTapToContinueDefeat()
    {
        // Saat kalah, event tidak diselesaikan sehingga bos bisa dilawan lagi.
        if (SceneTransitionManager.Instance != null)
            SceneTransitionManager.Instance.ReturnFromBattle();
    }

    // ─── Helper ──────────────────────────────────────────────────────────────

    private static void SetPanelVisible(CanvasGroup cg, bool visible)
    {
        if (cg == null) return;
        cg.alpha = visible ? 1f : 0f;
        cg.interactable = visible;
        cg.blocksRaycasts = visible;
    }
}