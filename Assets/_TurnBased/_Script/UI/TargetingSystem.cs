using System;
using UnityEngine;
using UnityEngine.InputSystem;
using Mouse = UnityEngine.InputSystem.Mouse;

public class TargetingSystem : MonoBehaviour
{
    public event Action<CharacterBase> OnTargetConfirmed;
    public event Action OnTargetCanceled;

    [Header("Visuals")]
    [Tooltip("Masukkan Prefab Panah / ArrowTargeting ke sini")]
    [SerializeField] private GameObject arrowIndicatorPrefab; 
    private GameObject currentArrow;

    private PlayerInputAction _actions;
    private int currentTargetIndex = 0;
    private bool isTargeting = false;
    private Camera mainCam;

    private void Awake()
    {
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

    public void StartTargeting()
    {
        _actions.Battle.Enable(); 
        isTargeting = true;
        currentTargetIndex = 0;

        // Munculkan panah jika ada prefab-nya
        if (arrowIndicatorPrefab != null && currentArrow == null)
        {
            currentArrow = Instantiate(arrowIndicatorPrefab);
        }
        
        if (currentArrow != null) currentArrow.SetActive(true);

        UpdateHighlight();
    }

    public void StopTargeting()
    {
        isTargeting = false;
        ClearAllHighlights();
        
        // Sembunyikan panah saat batal/selesai memilih
        if (currentArrow != null) currentArrow.SetActive(false);

        _actions.Battle.Disable(); 
    }

    private void Update()
    {
        if (isTargeting && Mouse.current.leftButton.wasPressedThisFrame)
        {
            HandleMouseClick();
        }
    }

    private void HandleMouseClick()
    {
        Ray ray = mainCam.ScreenPointToRay(Mouse.current.position.ReadValue());
        
        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            CharacterBase clickedEnemy = hit.collider.GetComponentInParent<CharacterBase>();
            
            if (clickedEnemy != null && CharacterManager.Instance.ActiveEnemies.Contains(clickedEnemy))
            {
                int clickedIndex = CharacterManager.Instance.ActiveEnemies.IndexOf(clickedEnemy);

                // LOGIKA BARU:
                if (clickedIndex == currentTargetIndex)
                {
                    ConfirmTarget();
                }
                else
                {
                    currentTargetIndex = clickedIndex;
                    UpdateHighlight();
                    Debug.Log("Target terpilih: " + clickedEnemy.name + ". Klik lagi untuk menyerang.");
                }
            }
        }
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
}
