using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[RequireComponent(typeof(CanvasGroup))]
public class ActionMenuUI : MonoBehaviour
{
    public ScriptableHero CurrentHero { get; private set; }
    public static ActionMenuUI Instance; 
    private HeroStatUI currentStatPanel;
    private ScriptableHero currentHeroSettingUp; 

    [Header("Menu Setup")]
    public Transform commandHolder; 
    public GameObject skillButtonPrefab; 
    public Image portraitImageUI;

    [Header("Animation Settings")]
    public Vector2 hiddenPosition = new Vector2(500, 0); 
    public Vector2 visiblePosition = new Vector2(0, 0); 
    public float animationDuration = 0.3f;

    [Header("Boost System")]
    public int maxBoost = 3;
    private int currentBoost = 0;
    public GameObject boostLightVFXPrefab; 
    private GameObject currentLightVFX;
    [SerializeField] private BoostVFXController vfxController;

    [Header("Boost Audio (Universal)")]
    public AudioClip boostLv1Clip;
    public AudioClip boostLv2Clip;
    public AudioClip boostLv3Clip;
    private AudioSource audioSource;

    [Header("UI Navigation")]
    public GameObject firstSelectedButton;
    private RectTransform rectTransform;
    private CanvasGroup canvasGroup;
    private PlayerInputAction _actions;

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
    }

    private void OnEnable()
    {
        _actions.Battle.Boost.performed += OnBoostPerformed;
    }

    private void OnDisable()
    {
        _actions.Battle.Boost.performed -= OnBoostPerformed;
    }

    public void OpenMenuForHero(ScriptableHero hero, HeroStatUI sourcePanel) 
    {
        if (isMenuActive && CurrentHero == hero)
        {
            Hide();
            return;
        }

        CurrentHero = hero;
        currentStatPanel = sourcePanel;
        
        StopAllCoroutines();
        StartCoroutine(SetupAndShowMenuCoroutine());
    }

    private IEnumerator SetupAndShowMenuCoroutine()
    {
        // 1. CABUT DAN HANCURKAN SECARA INSTAN
        for (int i = commandHolder.childCount - 1; i >= 0; i--)
        {
            Transform child = commandHolder.GetChild(i);
            child.SetParent(null); // Lepas dengan aman
            Destroy(child.gameObject);
        }
        
        firstSelectedButton = null;
        GameObject buttonToSelect = null;

        // HAPUS "yield return null;" YANG ADA DI SINI! Kita tidak mau nunggu 1 frame lagi.

        if (CurrentHero != null)
        {
            if (portraitImageUI != null) portraitImageUI.sprite = CurrentHero.MenuSprite;

            string lastIntent = "";
            if (currentStatPanel != null && currentStatPanel.intentText != null)
            {
                lastIntent = currentStatPanel.intentText.text;
            }

            System.Collections.Generic.List<Button> spawnedButtons = new System.Collections.Generic.List<Button>();

            if (CurrentHero.skills != null)
            {
                foreach (ScriptableSkill skill in CurrentHero.skills)
                {
                    GameObject newBtn = Instantiate(skillButtonPrefab, commandHolder);
                    
                    SkillButtonUI ui = newBtn.GetComponent<SkillButtonUI>();
                    if (ui != null) ui.Setup(skill);

                    Button btnComp = newBtn.GetComponent<UnityEngine.UI.Button>();
                    if (btnComp != null)
                    {
                        btnComp.onClick.AddListener(() => OnSkillClicked(skill)); 
                        spawnedButtons.Add(btnComp);
                    }

                    if (firstSelectedButton == null) firstSelectedButton = newBtn;
                    if (skill.skillName == lastIntent) buttonToSelect = newBtn; 
                }
            }

            // Atur Navigasi (Wrap Around)
            for (int i = 0; i < spawnedButtons.Count; i++)
            {
                Navigation customNav = new Navigation();
                customNav.mode = Navigation.Mode.Explicit;
                customNav.selectOnUp = spawnedButtons[i == 0 ? spawnedButtons.Count - 1 : i - 1];
                customNav.selectOnDown = spawnedButtons[i == spawnedButtons.Count - 1 ? 0 : i + 1];
                spawnedButtons[i].navigation = customNav;
            }

            LayoutRebuilder.ForceRebuildLayoutImmediate(commandHolder.GetComponent<RectTransform>());
            
            Show();
            
            EventSystem.current.SetSelectedGameObject(null); // Bersihkan dulu
            
            if (buttonToSelect != null)
            {
                EventSystem.current.SetSelectedGameObject(buttonToSelect);
                buttonToSelect.GetComponent<UnityEngine.UI.Button>().Select();
            }
            else if (firstSelectedButton != null)
            {
                EventSystem.current.SetSelectedGameObject(firstSelectedButton);
                firstSelectedButton.GetComponent<UnityEngine.UI.Button>().Select(); //
            }
            // ...
        }
        yield break; 
    }

    private void OnSkillClicked(ScriptableSkill selectedSkill)
    {
        if (currentStatPanel != null)
        {
            currentStatPanel.SetIntentText(selectedSkill.skillName);
        }

        Hide(); 
    }

    public void Show()
    {
        _actions.Battle.Enable(); 
        isMenuActive = true;
        currentBoost = 0; 

        if (firstSelectedButton != null)
        {
            EventSystem.current.SetSelectedGameObject(firstSelectedButton);
        }
        
        StopAllCoroutines(); 
        StartCoroutine(AnimateMenu(visiblePosition, 1f, true));
    }

    public void Hide()
    {
        _actions.Battle.Disable(); 
        isMenuActive = false;
        ClearBoostVFX();
        
        StopAllCoroutines();
        StartCoroutine(AnimateMenu(hiddenPosition, 0f, false));
    }

    private void OnBoostPerformed(InputAction.CallbackContext ctx)
    {
        if (!isMenuActive) return;

        float boostValue = ctx.ReadValue<float>();

        if (boostValue > 0)
        {
            if (currentBoost < maxBoost)
            {
                currentBoost++;
                Debug.Log($"[BOOST] Naik! Level: {currentBoost}");
                UpdateBoostVFX();
            }
        }
        else if (boostValue < 0)
        {
            if (currentBoost > 0)
            {
                currentBoost--;
                Debug.Log($"[BOOST] Turun. Level: {currentBoost}");
                UpdateBoostVFX();
            }
        }
    }

    private void UpdateBoostVFX()
    {
        ClearBoostVFX(); 

        if (currentBoost > 0)
        {
            // Kirim posisi default, atau posisi karakter aktif
            vfxController.PlayBoostEffect(currentBoost, new Vector3(0, 3.21f, -4f));
        }
        else
        {
            vfxController.StopEffect();
        }
    }

    private void ClearBoostVFX()
    {
        if (currentLightVFX != null) Destroy(currentLightVFX);
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