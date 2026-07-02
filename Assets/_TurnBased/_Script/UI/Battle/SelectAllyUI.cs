using UnityEngine;

[RequireComponent(typeof(CanvasGroup))]
public class SelectAllyUI : MonoBehaviour
{
    private CanvasGroup canvasGroup;
    private PlayerInputAction _actions;

    void Awake()
    {
        canvasGroup = GetComponent<CanvasGroup>();
        _actions = new PlayerInputAction();

        SetVisible(false);
    }

    private void OnDisable()
    {
        if (_actions != null)
        {
            _actions.Battle.Disable();
        }
    }

    private void OnDestroy()
    {
        if (_actions != null)
        {
            _actions.Battle.Disable();
            _actions.Dispose();
        }
    }
    public void ShowAllyPanel()
    {
        _actions.Battle.Disable(); 
        SetVisible(true);
    }

    public void HideAllyPanel()
    {
        _actions.Battle.Enable();
        SetVisible(false);
    }

    private void SetVisible(bool visible)
    {
        canvasGroup.alpha          = visible ? 1f : 0f;
        canvasGroup.interactable   = visible;
        canvasGroup.blocksRaycasts = visible;
    }
}