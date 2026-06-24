using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;

[RequireComponent(typeof(CanvasGroup))]
public class ActionMenuUI : MonoBehaviour
{
    public ScriptableHero CurrentHero { get; private set; }
    public HeroCharBase CurrentHeroUnit { get; private set; }
    public static ActionMenuUI Instance;
    private HeroStatUI currentStatPanel;

    [Header("Menu Setup")]
    [SerializeField] private Transform commandHolder; 
    [SerializeField] private GameObject skillButtonPrefab; 
    [SerializeField] private Image portraitImageUI;
    [SerializeField] private TextMeshProUGUI charNameText;

    [Header("Animation Settings")]
    [SerializeField] private Vector2 hiddenPosition = new Vector2(500, 0); 
    [SerializeField] private Vector2 visiblePosition = new Vector2(0, 0); 
    [SerializeField] private float animationDuration = 0.3f;

    [Header("Boost System")]
    [SerializeField] private int maxBoost = 3;
    private int currentBoost = 0;
    [SerializeField] private TextMeshProUGUI boostMultiplierText;
    [SerializeField] private BoostVFXManager boostVFX;

    [Header("UI Navigation")]
    private GameObject firstSelectedButton;
    private RectTransform rectTransform;
    private CanvasGroup canvasGroup;
    private PlayerInputAction _actions;

    [Header("Content Animation")]
    [SerializeField] private RectTransform _dynamicContentRect;
    [SerializeField] private CanvasGroup _dynamicContentCanvasGroup; 
    [SerializeField] private float _slideOffset = 30f;
    [SerializeField] private float _swapAnimDuration = 0.15f;

    private float _originalContentX;

    private bool isMenuActive = false;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);

        rectTransform = GetComponent<RectTransform>();
        canvasGroup = GetComponent<CanvasGroup>();
        _actions = new PlayerInputAction();
        
        rectTransform.anchoredPosition = hiddenPosition;
        canvasGroup.alpha = 0f;
        canvasGroup.interactable = false;
        canvasGroup.blocksRaycasts = false;

        if (_dynamicContentRect != null)
        {
            _originalContentX = _dynamicContentRect.anchoredPosition.x;
        }
    }

    private void OnEnable() => _actions.Battle.Boost.performed += OnBoostPerformed;
    private void OnDisable() => _actions.Battle.Boost.performed -= OnBoostPerformed;

    public void OpenMenuForHero(ScriptableHero hero, HeroStatUI sourcePanel) 
    {
        if (isMenuActive && CurrentHero == hero)
        {
            Hide();
            return;
        }

        CurrentHero = hero;
        if (charNameText != null) charNameText.text = hero.heroName;
        currentStatPanel = sourcePanel;
        CurrentHeroUnit = sourcePanel.physicalHero; 
        
        StopAllCoroutines();
        StartCoroutine(SetupAndShowMenuCoroutine());
    }

    private IEnumerator SetupAndShowMenuCoroutine()
    {
        commandHolder.DestroyChilderns();
        
        yield return new WaitForEndOfFrame();

        firstSelectedButton = null;
        GameObject buttonToSelect = null;

        if (CurrentHero != null)
        {
            if (portraitImageUI != null) portraitImageUI.sprite = CurrentHero.MenuSprite;

            string lastIntent = currentStatPanel != null ? currentStatPanel.IntentTextValue : "";

            List<Button> spawnedButtons = new List<Button>();

            if (CurrentHero.skills != null)
            {
                foreach (ScriptableSkill skill in CurrentHero.skills)
                {
                    GameObject newBtn = Instantiate(skillButtonPrefab, commandHolder);
                    
                    SkillButtonUI ui = newBtn.GetComponent<SkillButtonUI>();
                    if (ui != null) ui.Setup(skill);

                    Button btnComp = newBtn.GetComponent<Button>();
                    if (btnComp != null)
                    {
                        btnComp.onClick.AddListener(() => OnSkillClicked(skill)); 
                        spawnedButtons.Add(btnComp);
                    }

                    if (firstSelectedButton == null) firstSelectedButton = newBtn;
                    if (skill.skillName == lastIntent) buttonToSelect = newBtn; 
                }
            }

            // Setup Custom Navigation
            for (int i = 0; i < spawnedButtons.Count; i++)
            {
                Navigation customNav = new Navigation();
                customNav.mode = Navigation.Mode.Explicit;
                customNav.selectOnUp = spawnedButtons[i == 0 ? spawnedButtons.Count - 1 : i - 1];
                customNav.selectOnDown = spawnedButtons[i == spawnedButtons.Count - 1 ? 0 : i + 1];
                spawnedButtons[i].navigation = customNav;
            }

            if (_dynamicContentRect != null) 
            {
                LayoutRebuilder.ForceRebuildLayoutImmediate(_dynamicContentRect);
            }
            
            if (!isMenuActive)
            {
                Show();
            }
            else
            {
                StartCoroutine(PlaySwapAnimation());
                
                currentBoost = CurrentHeroUnit != null ? CurrentHeroUnit.AllocatedBoost : 0;
                if (boostMultiplierText != null) boostMultiplierText.text = $"Boost : x{currentBoost + 1}";
            }
            
            EventSystem.current.SetSelectedGameObject(null); 
            
            if (buttonToSelect != null)
            {
                EventSystem.current.SetSelectedGameObject(buttonToSelect);
                buttonToSelect.GetComponent<Button>().Select();
            }
            else if (firstSelectedButton != null)
            {
                EventSystem.current.SetSelectedGameObject(firstSelectedButton);
                firstSelectedButton.GetComponent<Button>().Select(); 
            }
        }
    }

    private IEnumerator PlaySwapAnimation()
    {
        if (_dynamicContentCanvasGroup == null || _dynamicContentRect == null) yield break;

        float elapsed = 0f;
        
        // Mundurkan X dan set transparan
        float startX = _originalContentX + _slideOffset;
        _dynamicContentRect.anchoredPosition = new Vector2(startX, _dynamicContentRect.anchoredPosition.y);
        _dynamicContentCanvasGroup.alpha = 0f;

        while (elapsed < _swapAnimDuration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / _swapAnimDuration;
            float easeOutT = t * (2f - t); 

            // Geser X wadah keseluruhan
            float currentX = Mathf.Lerp(startX, _originalContentX, easeOutT);
            _dynamicContentRect.anchoredPosition = new Vector2(currentX, _dynamicContentRect.anchoredPosition.y);
            
            // Fade In wadah keseluruhan
            _dynamicContentCanvasGroup.alpha = Mathf.Lerp(0f, 1f, easeOutT);
            
            yield return null;
        }

        // Kunci di posisi akhir
        _dynamicContentRect.anchoredPosition = new Vector2(_originalContentX, _dynamicContentRect.anchoredPosition.y);
        _dynamicContentCanvasGroup.alpha = 1f;
    }

    private void OnSkillClicked(ScriptableSkill selectedSkill)
    {
        if (currentStatPanel != null) currentStatPanel.SetIntentText(selectedSkill.skillName);
        BattleManager.Instance.StartTargetingForHero(CurrentHeroUnit);
        Hide();
    }

    public void Show()
    {
        _actions.Battle.Enable(); 
        isMenuActive = true;

        // BACA MEMORI KARAKTER YANG SEDANG DIPILIH
        if (CurrentHeroUnit != null) {
            currentBoost = CurrentHeroUnit.AllocatedBoost;
        } else {
            currentBoost = 0;
        }
        
        // Tampilkan teks x1, x2, dst sesuai memori
        if (boostMultiplierText != null) 
            boostMultiplierText.text = $"Boost : x{currentBoost + 1}";

        if (firstSelectedButton != null) EventSystem.current.SetSelectedGameObject(firstSelectedButton);
        
        StopAllCoroutines(); 
        StartCoroutine(AnimateMenu(visiblePosition, 1f, true));
    }

    public void Hide()
    {
        _actions.Battle.Disable(); 
        isMenuActive = false;
        
        StopAllCoroutines();
        StartCoroutine(AnimateMenu(hiddenPosition, 0f, false));
    }

    private void OnBoostPerformed(InputAction.CallbackContext ctx)
    {
        if (!isMenuActive) return;

        float boostValue = ctx.ReadValue<float>();
        int prevBoost = currentBoost;

        if (boostValue > 0)
        {
            if (currentBoost < maxBoost && currentStatPanel != null && currentBoost < currentStatPanel.currentBP)
            {
                currentBoost++;
                CurrentHeroUnit.AllocatedBoost = currentBoost;
                UpdateBoostSystem(prevBoost);
            }
        }
        else if (boostValue < 0)
        {
            if (currentBoost > 0)
            {
                currentBoost--;
                CurrentHeroUnit.AllocatedBoost = currentBoost;
                UpdateBoostSystem(prevBoost);
            }
        }
    }

    private void UpdateBoostSystem(int prevBoost)
    {
        if (boostVFX != null && CurrentHeroUnit != null)
        {
            boostVFX.PlayBoostEffect(CurrentHeroUnit, currentBoost, prevBoost); 
        }

        if (boostMultiplierText != null) boostMultiplierText.text = $"Boost : x{currentBoost + 1}";
        if (currentStatPanel != null) currentStatPanel.UpdateBoostVisual();
    }

    private IEnumerator AnimateMenu(Vector2 targetPos, float targetAlpha, bool interactable)
    {
        Vector2 startPos = rectTransform.anchoredPosition;
        float startAlpha = canvasGroup.alpha;
        float elapsed = 0f;

        if (interactable)
        {
            canvasGroup.interactable = true;
            canvasGroup.blocksRaycasts = true;
        }
        else
        {
            canvasGroup.interactable = false;
            canvasGroup.blocksRaycasts = false;
        }

        while (elapsed < animationDuration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.SmoothStep(0, 1, elapsed / animationDuration);
            rectTransform.anchoredPosition = Vector2.Lerp(startPos, targetPos, t);
            canvasGroup.alpha = Mathf.Lerp(startAlpha, targetAlpha, t);
            yield return null;
        }

        rectTransform.anchoredPosition = targetPos;
        canvasGroup.alpha = targetAlpha;
    }
}
