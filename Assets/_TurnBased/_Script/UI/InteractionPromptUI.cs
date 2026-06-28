using UnityEngine;

public class InteractionPromptUI : MonoBehaviour
{
    [SerializeField] private CanvasGroup canvasGroup;
    [SerializeField] private float fadeSpeed = 8f;

    // private bool _isVisible = false;
    private float _targetAlpha = 0f;

    private void Awake()
    {
        // Langsung set transparan saat mulai
        canvasGroup.alpha = 0f;
        _targetAlpha = 0f;
    }

    private void Update()
    {
        if (Mathf.Approximately(canvasGroup.alpha, _targetAlpha)) return;
        canvasGroup.alpha = Mathf.MoveTowards(canvasGroup.alpha, _targetAlpha, fadeSpeed * Time.deltaTime);
    }

    public void ShowPrompt()
    {
        // _isVisible = true;
        _targetAlpha = 1f;
    }

    public void HidePrompt()
    {
        // _isVisible = false;
        _targetAlpha = 0f;
    }
}