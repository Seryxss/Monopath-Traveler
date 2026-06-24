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
    private List<Button> _activeSkillButtons = new List<Button>();

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
        if (isMenuActive)
        {
            if (CurrentHero == hero)
            {
                Hide();
                return;
            }
            else
            {
                if (BattleManager.Instance != null)
                {
                    BattleManager.Instance.StopTargetingFromMenu(); 
                }
            }
        }
        CurrentHero = hero;
        if (charNameText != null) charNameText.text = hero.heroName;
        currentStatPanel = sourcePanel;
        CurrentHeroUnit = sourcePanel.physicalHero; 
        
        if (CurrentHeroUnit != null)
        {
            BattleManager.Instance.StartTargetingForHero(CurrentHeroUnit);
        }

        StopAllCoroutines();
        StartCoroutine(SetupAndShowMenuCoroutine());
    }

    private IEnumerator SetupAndShowMenuCoroutine()
    {
        commandHolder.DestroyChilderns();
        yield return new WaitForEndOfFrame();

        firstSelectedButton = null;
        GameObject buttonToSelect = null;
        
        _activeSkillButtons.Clear();

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

                    if (ui != null) ui.Setup(skill, CurrentHeroUnit.currentSp);

                    Button btnComp = newBtn.GetComponent<Button>();
                    if (btnComp != null)
                    {
                        if (CurrentHeroUnit.currentSp < skill.spCost)
                        {
                            btnComp.interactable = false; 
                        }
                        else
                        {
                            btnComp.interactable = true;
                            btnComp.onClick.AddListener(() => OnSkillClicked(skill, btnComp)); 
                            _activeSkillButtons.Add(btnComp); 
                            
                            if (firstSelectedButton == null) firstSelectedButton = newBtn;
                            if (skill.skillName == lastIntent) buttonToSelect = newBtn; 
                        }
                    }
                }
            }

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
                
                HighlightSelectedButton(buttonToSelect.GetComponent<Button>()); 
            }
            else if (firstSelectedButton != null)
            {
                EventSystem.current.SetSelectedGameObject(firstSelectedButton);
                firstSelectedButton.GetComponent<Button>().Select(); 
                
                HighlightSelectedButton(firstSelectedButton.GetComponent<Button>());
                
                if (CurrentHero.skills.Count > 0)
                {
                    ScriptableSkill firstSkill = CurrentHero.skills[0];
                    CurrentHeroUnit.CurrentIntent.ChosenSkill = firstSkill;
                    if (currentStatPanel != null) currentStatPanel.SetIntentText(firstSkill.skillName);
                }
            }
        }
    }

    private IEnumerator PlaySwapAnimation()
    {
        if (_dynamicContentCanvasGroup == null || _dynamicContentRect == null) yield break;

        float elapsed = 0f;
        
        float startX = _originalContentX + _slideOffset;
        _dynamicContentRect.anchoredPosition = new Vector2(startX, _dynamicContentRect.anchoredPosition.y);
        _dynamicContentCanvasGroup.alpha = 0f;

        while (elapsed < _swapAnimDuration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / _swapAnimDuration;
            float easeOutT = t * (2f - t); 

            float currentX = Mathf.Lerp(startX, _originalContentX, easeOutT);
            _dynamicContentRect.anchoredPosition = new Vector2(currentX, _dynamicContentRect.anchoredPosition.y);
            
            _dynamicContentCanvasGroup.alpha = Mathf.Lerp(0f, 1f, easeOutT);
            
            yield return null;
        }

        _dynamicContentRect.anchoredPosition = new Vector2(_originalContentX, _dynamicContentRect.anchoredPosition.y);
        _dynamicContentCanvasGroup.alpha = 1f;
    }

    private void OnSkillClicked(ScriptableSkill selectedSkill, Button clickedButton)
    {
        if (currentStatPanel != null) currentStatPanel.SetIntentText(selectedSkill.skillName);
        if (CurrentHeroUnit != null) CurrentHeroUnit.CurrentIntent.ChosenSkill = selectedSkill;

        HighlightSelectedButton(clickedButton);
    }


    private void HighlightSelectedButton(Button selectedBtn)
    {
        foreach (Button btn in _activeSkillButtons)
        {
            if (btn == null) continue;

            ColorBlock cb = btn.colors; 

            if (btn == selectedBtn)
            {
            
                cb.normalColor = cb.selectedColor; 
            }
            else
            {
                cb.normalColor = Color.white; 
            }
            
            btn.colors = cb;
        }
    }

    public void Show()
    {
        _actions.Battle.Enable(); 
        isMenuActive = true;
        if (CurrentHeroUnit != null) {
            currentBoost = CurrentHeroUnit.AllocatedBoost;
        } else {
            currentBoost = 0;
        }
        
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

        if (BattleManager.Instance != null)
        {
            BattleManager.Instance.StopTargetingFromMenu();
        }

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
            // Jika dua-duanya aman, maka jalankan Boost
            if (currentBoost < BattleManager.MAX_BOOST && CurrentHeroUnit != null && currentBoost < CurrentHeroUnit.CurrentBP)
            {
                currentBoost++;
                CurrentHeroUnit.AllocatedBoost = currentBoost;
                UpdateBoostSystem(prevBoost);
            }
            else
            {
                // INI ADALAH KODE DETEKTIF: Akan muncul tulisan kuning di Console
                if (currentBoost >= BattleManager.MAX_BOOST)
                {
                    Debug.LogWarning($"[DITOLAK] Gagal nambah! Karena settingan 'Max Boost' di Inspector UI adalah: {BattleManager.MAX_BOOST}");
                }
                else if (currentBoost >= CurrentHeroUnit.CurrentBP)
                {
                    Debug.LogWarning($"[DITOLAK] Gagal nambah! Karena BP Hero di ronde ini hanya sisa: {CurrentHeroUnit.CurrentBP}");
                }
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
        // 1. UPDATE TEKS & WARNA UI
        if (boostMultiplierText != null) 
        {
            Color boostColor = Color.white; 
            
            if (currentBoost == 1) boostColor = Color.red; 
            else if (currentBoost == 2) boostColor = Color.yellow; 
            else if (currentBoost >= 3) boostColor = Color.cyan; 

            boostMultiplierText.color = boostColor;
            
            boostMultiplierText.text = $"Boost : x{currentBoost + 1}";
        }

        // 2. KIRIM DATA KE VFX MANAGER
        if (boostVFX != null && CurrentHeroUnit != null)
        {
            boostVFX.PlayBoostEffect(CurrentHeroUnit, currentBoost, prevBoost); 
        }
        
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
