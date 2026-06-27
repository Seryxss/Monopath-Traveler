using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;

[RequireComponent(typeof(Button))]
public class SkillButtonUI : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, ISelectHandler, IDeselectHandler
{
    [Header("Skill Info")]
    [SerializeField] private TextMeshProUGUI typeText;
    [SerializeField] private TextMeshProUGUI nameText;
    [SerializeField] private TextMeshProUGUI spText;
    [SerializeField] private Image iconSprite;

    [Header("Description Box")]
    [SerializeField] private CanvasGroup descriptionBoxGroup;
    [SerializeField] private TextMeshProUGUI descriptionText;

    [Header("SP Container")]
    [SerializeField] private CanvasGroup spContainerGroup;

    [Header("UI Sounds")]
    [SerializeField] private AudioClip clickSound;

    private Button myButton;
    private ScriptableSkill mySkill;

    private void Awake()
    {
        myButton = GetComponent<Button>();
        if (myButton != null)
            myButton.onClick.AddListener(OnButtonClicked);

        SetDescriptionVisible(false);
    }

    public void Setup(ScriptableSkill skill, int currentSp)
    {
        mySkill = skill;
        SetDescriptionVisible(false);
        if (skill == null) return;

        if (typeText != null) typeText.text= $"{skill.targetScope} / {skill.skillCategory}";
        if (nameText != null) nameText.text = skill.skillName;
        if (descriptionText != null) descriptionText.text = skill.description;

        if (iconSprite != null)
        {
            iconSprite.enabled = skill.skillElement != null;
            if (skill.skillElement != null)
                iconSprite.sprite = skill.skillElement.elementIcon;
        }

        bool hasSPCost = skill.spCost > 0;
        SetGroupVisible(spContainerGroup, hasSPCost);

        bool canAfford = currentSp >= skill.spCost;

        if (hasSPCost && spText != null)
        {
            spText.text  = $"SP {skill.spCost}";
            spText.color = canAfford ? Color.white : Color.red; 
        }

        if (myButton != null)
            myButton.interactable = canAfford;
    }

    private void OnButtonClicked()
    {
        if (AudioSystem.Instance != null && clickSound != null)
            AudioSystem.Instance.PlayUISound(clickSound);
    }

    private void OnDestroy()
    {
        if (myButton != null)
            myButton.onClick.RemoveListener(OnButtonClicked);
    }

    // ─── Hover & Select ───────────────────────────────────────────────────────

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (mySkill != null) SetDescriptionVisible(true);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        SetDescriptionVisible(false);
    }

    public void OnSelect(BaseEventData eventData)
    {
        // Aktifkan untuk gamepad support jika diperlukan
        // if (mySkill != null) SetDescriptionVisible(true);
    }

    public void OnDeselect(BaseEventData eventData)
    {
        SetDescriptionVisible(false);
    }

    // ─── Helpers ──────────────────────────────────────────────────────────────

    private void SetDescriptionVisible(bool visible)
        => SetGroupVisible(descriptionBoxGroup, visible);

    private static void SetGroupVisible(CanvasGroup cg, bool visible)
    {
        if (cg == null) return;
        cg.alpha = visible ? 1f : 0f;
        cg.interactable = visible;
        cg.blocksRaycasts = visible;
    }
}