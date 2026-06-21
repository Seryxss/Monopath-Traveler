using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.EventSystems;

[RequireComponent(typeof(CanvasGroup))]
public class ActionMenuUI : MonoBehaviour
{
    [Header("Animation Settings")]
    public Vector2 hiddenPosition = new Vector2(500, 0); 
    public Vector2 visiblePosition = new Vector2(0, 0); 
    public float animationDuration = 0.3f;

    [Header("Boost System")]
    public int maxBoost = 3;
    private int currentBoost = 0;
    public GameObject boostLightVFXPrefab; 
    private GameObject currentLightVFX;

    [Header("UI Navigation")]
    [Tooltip("Masukkan Tombol 'Attack' ke sini agar otomatis tersorot saat menu muncul")]
    public GameObject firstSelectedButton;

    private RectTransform rectTransform;
    private CanvasGroup canvasGroup;
    private PlayerInputAction _actions;
    private bool isMenuActive = false;

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        canvasGroup = GetComponent<CanvasGroup>();
        _actions = new PlayerInputAction();
        
        // Sembunyikan saat game baru mulai
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

    public void Show()
    {
        _actions.Battle.Enable(); 
        isMenuActive = true;
        currentBoost = 0; 
        
        // Paksa EventSystem menyorot tombol pertama agar bisa digeser dengan W/S bawaan Unity UI
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

    // --- LOGIKA BOOST (1D AXIS: Q & E) ---
    private void OnBoostPerformed(InputAction.CallbackContext ctx)
    {
        if (!isMenuActive) return;

        // Baca nilai (E = 1, Q = -1)
        float boostValue = ctx.ReadValue<float>();

        if (boostValue > 0) // Tekan E (Tambah)
        {
            if (currentBoost < maxBoost)
            {
                currentBoost++;
                Debug.Log($"[BOOST] Naik! Level: {currentBoost}");
                UpdateBoostVFX();
            }
        }
        else if (boostValue < 0) // Tekan Q (Kurang)
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

        if (currentBoost > 0 && boostLightVFXPrefab != null)
        {
            currentLightVFX = Instantiate(boostLightVFXPrefab, Vector3.zero, Quaternion.identity);
            Light vfxLight = currentLightVFX.GetComponent<Light>();
            if (vfxLight != null)
            {
                if (currentBoost == 1) vfxLight.color = Color.red;       
                else if (currentBoost == 2) vfxLight.color = Color.blue; 
                else if (currentBoost == 3) vfxLight.color = Color.yellow; 
            }
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

        if (!interactable) 
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

        if (interactable)
        {
            canvasGroup.interactable = true;
            canvasGroup.blocksRaycasts = true;
        }
    }
}