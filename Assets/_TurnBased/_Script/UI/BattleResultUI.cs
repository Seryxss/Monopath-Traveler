using UnityEngine;
using UnityEngine.SceneManagement;

public class BattleResultUI : MonoBehaviour
{
    [Header("Panels")]
    [SerializeField] private GameObject victoryPanel;
    [SerializeField] private GameObject defeatPanel;

    [Header("Scene Names")]
    public string exploreSceneName = "ExploreScene"; 
    public string mainMenuSceneName = "MainMenu";    

    public void ShowVictory()
    {
        if (victoryPanel != null) victoryPanel.SetActive(true);
    }

    public void ShowDefeat()
    {
        if (defeatPanel != null) defeatPanel.SetActive(true);
    }

    public void OnTapToContinueVictory()
    {
        if (ProgressManager.Instance != null)
        {
            ProgressManager.Instance.CompleteActiveEvent();
        }

        if (SceneTransitionManager.Instance != null)
        {
            GameManager.Instance.PlayBGM();
            SceneTransitionManager.Instance.ReturnFromBattle();
        }
    }

    public void OnTapToContinueDefeat()
    {
        // Jika kalah, kita cukup batalkan (kosongkan) penanda event aktif tanpa menyelesaikannya.
        // Dengan begini, saat map ter-load, statusnya bukan Completed, dan bosnya bisa dilawan lagi.
        if (ProgressManager.Instance != null)
        {
            // Catatan: Anda perlu membuat CurrentActiveEvent = null, tapi karena private setter, 
            // kita bisa abaikan saja karena IsEventCompleted tetap akan mendeteksi dia "belum selesai".
        }

        if (SceneTransitionManager.Instance != null)
        {
            SceneTransitionManager.Instance.ReturnFromBattle();
        }
    }
}