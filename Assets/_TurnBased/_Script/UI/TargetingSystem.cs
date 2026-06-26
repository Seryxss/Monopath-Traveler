using System;
using UnityEngine;
using UnityEngine.InputSystem;
using Mouse = UnityEngine.InputSystem.Mouse;
using System.Collections.Generic;
using UnityEngine.EventSystems;
using System.Collections; // Wajib untuk IEnumerator

public class TargetingSystem : MonoBehaviour
{
    public event Action<CharacterBase> OnTargetConfirmed;
    public event Action OnTargetCanceled;

    [Header("Visuals (Action Menu Mode)")]
    [Tooltip("Masukkan Prefab Panah / ArrowTargeting ke sini")]
    [SerializeField] private GameObject arrowIndicatorPrefab; 
    private GameObject currentArrow;
    
    [Header("Target Data")]
    public GameObject currentTarget;  
    
    [Header("Settings")]
    public float autoHideDelay = 3.0f;

    private PlayerInputAction _actions;
    private int currentTargetIndex = 0;
    private bool isTargeting = false;
    private Camera mainCam;
    private Coroutine hideTimerCoroutine;

    private void Awake()
    {
        if (arrowIndicatorPrefab != null) arrowIndicatorPrefab.SetActive(false);

        _actions = new PlayerInputAction();
        mainCam = Camera.main; 
    }

    private void OnEnable()
    {
        _actions.Battle.Targeting.performed += OnTargetingPerformed;
        _actions.Battle.Submit.performed += OnSubmitPerformed;
        _actions.Battle.Cancel.performed += OnCancelPerformed;
    }

    private void OnDisable()
    {
        _actions.Battle.Targeting.performed -= OnTargetingPerformed;
        _actions.Battle.Submit.performed -= OnSubmitPerformed;
        _actions.Battle.Cancel.performed -= OnCancelPerformed;
    }

    private void Update()
    {
        if (Mouse.current.leftButton.wasPressedThisFrame)
        {
            HandleMouseClick();
        }
    }

    private void HandleMouseClick()
    {
        if (HandleUIClick())
        {
            return; 
        }


        HandleWorldClick();
    }

    private bool HandleUIClick()
    {
        if (EventSystem.current.IsPointerOverGameObject())
        {
            PointerEventData pointerData = new PointerEventData(EventSystem.current)
            {
                position = Mouse.current.position.ReadValue()
            };
            
            List<RaycastResult> results = new List<RaycastResult>();
            EventSystem.current.RaycastAll(pointerData, results);

            bool isClickingValidUI = false;

            foreach (RaycastResult result in results)
            {
                GameObject hitObj = result.gameObject;
                if (hitObj.GetComponentInParent<ActionMenuUI>() != null || 
                    hitObj.GetComponentInParent<HeroStatUI>() != null ||
                    hitObj.GetComponentInParent<UnityEngine.UI.Button>() != null)
                {
                    isClickingValidUI = true;
                    break; 
                }
            }
            
            if (isTargeting && !isClickingValidUI)
            {
                OnTargetCanceled?.Invoke(); 
            }
            
            return true; 
        }
        
        return false; 
    }

    private void HandleWorldClick()
    {
        Ray ray = mainCam.ScreenPointToRay(Mouse.current.position.ReadValue());
        
        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            CharacterBase clickedEnemy = hit.collider.GetComponentInParent<CharacterBase>();
            
            if (clickedEnemy != null && CharacterManager.Instance.ActiveEnemies.Contains(clickedEnemy))
            {
                if (isTargeting)
                {
                    int clickedIndex = CharacterManager.Instance.ActiveEnemies.IndexOf(clickedEnemy);

                    if (clickedIndex == currentTargetIndex) ConfirmTarget();
                    else
                    {
                        currentTargetIndex = clickedIndex;
                        UpdateHighlight();
                    }
                }
                else
                {
                    if (GameManager.Instance.State == GameState.InBattle)
                    {
                        SelectTarget(clickedEnemy.gameObject);
                    }
                }
            }
            else
            {
                if (isTargeting) OnTargetCanceled?.Invoke();
            }
        }
        else
        {
            if (isTargeting) OnTargetCanceled?.Invoke();
        }
    }

    public void SelectTarget(GameObject enemy)
    {
        if (currentTarget != null)
        {
            TargetHighlight prevHighlight = currentTarget.GetComponent<TargetHighlight>();
            if (prevHighlight != null) prevHighlight.SetHighlight(false);
        }

        currentTarget = enemy;

        TargetHighlight currentHighlight = enemy.GetComponent<TargetHighlight>();
        if (currentHighlight != null) currentHighlight.SetHighlight(true);

        if (arrowIndicatorPrefab != null && currentArrow == null)
        {
            currentArrow = Instantiate(arrowIndicatorPrefab);
        }

        if (currentArrow != null)
        {
            currentArrow.SetActive(true);
            currentArrow.transform.SetParent(enemy.transform, false);

            TargetIndicator indicator = currentArrow.GetComponent<TargetIndicator>();
            if (indicator != null)
            {
                indicator.SetPivot(new Vector3(0, 1.3f, 0)); 
            }
            else
            {
                currentArrow.transform.localPosition = new Vector3(0, 1.3f, 0);
            }
        }

        if (hideTimerCoroutine != null) StopCoroutine(hideTimerCoroutine);
        hideTimerCoroutine = StartCoroutine(HideCursorRoutine());
    }

    private IEnumerator HideCursorRoutine()
    {
        yield return new WaitForSeconds(autoHideDelay);
        
        if (currentArrow != null) 
        {
            currentArrow.SetActive(false);
        }

        if (currentTarget != null)
        {
            TargetHighlight highlight = currentTarget.GetComponent<TargetHighlight>();
            if (highlight != null) highlight.SetHighlight(false);
        }
    }
    public void StartTargeting(CharacterBase previousTarget = null)
    {
        _actions.Battle.Enable(); 
        isTargeting = true;

        if (hideTimerCoroutine != null) StopCoroutine(hideTimerCoroutine);
        if (arrowIndicatorPrefab != null) arrowIndicatorPrefab.SetActive(false);

        CharacterBase targetToUse = null;

        if (currentTarget != null)
        {
            targetToUse = currentTarget.GetComponent<CharacterBase>();
        }
        else if (previousTarget != null)
        {
            targetToUse = previousTarget;
        }

        currentTargetIndex = 0; 
        
        if (targetToUse != null && CharacterManager.Instance.ActiveEnemies.Contains(targetToUse))
        {
            currentTargetIndex = CharacterManager.Instance.ActiveEnemies.IndexOf(targetToUse);
        }

        // Munculkan panah tangan
        if (arrowIndicatorPrefab != null && currentArrow == null)
            currentArrow = Instantiate(arrowIndicatorPrefab);
        
        if (currentArrow != null) currentArrow.SetActive(true);

        UpdateHighlight();
    }

    public void StopTargeting()
    {
        isTargeting = false;
        ClearAllHighlights();
        
        if (currentArrow != null) currentArrow.SetActive(false);

        _actions.Battle.Disable(); 
    }

    public CharacterBase GetCurrentTarget()
    {
        if (CharacterManager.Instance.ActiveEnemies.Count == 0) return null;
        return CharacterManager.Instance.ActiveEnemies[currentTargetIndex];
    }

    private void OnTargetingPerformed(InputAction.CallbackContext ctx)
    {
        if (!isTargeting) return;
        float direction = ctx.ReadValue<float>();
        if (direction > 0) ChangeTarget(1); 
        else if (direction < 0) ChangeTarget(-1); 
    }

    private void OnSubmitPerformed(InputAction.CallbackContext ctx)
    {
        if (!isTargeting) return;
        ConfirmTarget();
    }

    private void OnCancelPerformed(InputAction.CallbackContext ctx)
    {
        if (!isTargeting) return;
        OnTargetCanceled?.Invoke();
    }

    private void ConfirmTarget()
    {
        CharacterBase selectedEnemy = CharacterManager.Instance.ActiveEnemies[currentTargetIndex];
        OnTargetConfirmed?.Invoke(selectedEnemy); 
    }

    private void ChangeTarget(int direction)
    {
        SetEnemyHighlight(currentTargetIndex, false);
        currentTargetIndex += direction;

        int enemyCount = CharacterManager.Instance.ActiveEnemies.Count;
        if (currentTargetIndex >= enemyCount) currentTargetIndex = 0;
        if (currentTargetIndex < 0) currentTargetIndex = enemyCount - 1;

        SetEnemyHighlight(currentTargetIndex, true);
    }

    private void UpdateHighlight()
    {
        ClearAllHighlights();
        SetEnemyHighlight(currentTargetIndex, true);
    }

    private void ClearAllHighlights()
    {
        for (int i = 0; i < CharacterManager.Instance.ActiveEnemies.Count; i++)
        {
            SetEnemyHighlight(i, false);
        }
    }

    private void SetEnemyHighlight(int index, bool isHighlighted)
    {
        if (index >= 0 && index < CharacterManager.Instance.ActiveEnemies.Count)
        {
            CharacterBase enemy = CharacterManager.Instance.ActiveEnemies[index];
            
            TargetHighlight highlight = enemy.GetComponent<TargetHighlight>();
            if (highlight != null) highlight.SetHighlight(isHighlighted);

            if (isHighlighted && currentArrow != null)
            {
                currentArrow.transform.SetParent(enemy.transform, false);

                TargetIndicator indicator = currentArrow.GetComponent<TargetIndicator>();
                if (indicator != null)
                {
                    indicator.SetPivot(new Vector3(0, 1.3f, 0)); 
                }
                else
                {
                    currentArrow.transform.localPosition = new Vector3(0, 1.3f, 0);
                }
            }
        }
    }

    private string GetTransformPath(Transform transform)
    {
        string path = transform.name;
        while (transform.parent != null)
        {
            transform = transform.parent;
            path = transform.name + "/" + path;
        }
        return path;
    }
}