using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class EndGameManager : MonoBehaviour
{
    [Header("End Game UI")]
    [SerializeField] private GameObject endGameCanvas;
    [SerializeField] private Button backToMainMenuButton;

    [Header("Trigger Settings")]
    [SerializeField] private bool triggerOnce = true;

    private bool _hasTriggered;

    private void Awake()
    {
        SetEndGameCanvasVisible(false);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;
        if (triggerOnce && _hasTriggered) return;

        _hasTriggered = true;
        ShowEndGameUI();
    }

    private void ShowEndGameUI()
    {
        SetEndGameCanvasVisible(true);

        if (backToMainMenuButton != null)
        {
            backToMainMenuButton.onClick.RemoveAllListeners();
            backToMainMenuButton.onClick.AddListener(OnBackToMainMenuClicked);
        }
    }

    private void OnBackToMainMenuClicked()
    {
        if (SceneTransitionManager.Instance != null)
        {
            SceneTransitionManager.Instance.TransitionToMainMenu();
        }
        else
        {
            SceneManager.LoadScene("StartScene");
        }
    }

    private void SetEndGameCanvasVisible(bool visible)
    {
        if (endGameCanvas != null)
            endGameCanvas.SetActive(visible);
    }
}
