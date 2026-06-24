using System;
using UnityEngine;
using UnityEngine.InputSystem;
using Mouse = UnityEngine.InputSystem.Mouse;
using System.Collections.Generic;
using UnityEngine.EventSystems;

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

    public void StartTargeting(CharacterBase previousTarget = null)
    {
        _actions.Battle.Enable(); 
        isTargeting = true;
        currentTargetIndex = 0;

        if (previousTarget != null && CharacterManager.Instance.ActiveEnemies.Contains(previousTarget))
        {
            currentTargetIndex = CharacterManager.Instance.ActiveEnemies.IndexOf(previousTarget);
        }

        if (arrowIndicatorPrefab != null && currentArrow == null)
            currentArrow = Instantiate(arrowIndicatorPrefab);
        
        if (currentArrow != null) currentArrow.SetActive(true);

        UpdateHighlight();
    }

    // 2. Tambahkan fungsi baru ini di mana saja di dalam kelas TargetingSystem
    public CharacterBase GetCurrentTarget()
    {
        if (CharacterManager.Instance.ActiveEnemies.Count == 0) return null;
        return CharacterManager.Instance.ActiveEnemies[currentTargetIndex];
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
        // ==========================================
        // 1. CEK APAKAH KLIK MENGENAI UI (CANVAS)
        // ==========================================
        if (EventSystem.current.IsPointerOverGameObject())
        {
            // Tembakkan laser ke UI untuk melihat objek UI apa saja yang tertusuk
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

                // Cek apakah UI yang diklik adalah bagian dari Menu Aksi, Panel Stats, atau Tombol
                if (hitObj.GetComponentInParent<ActionMenuUI>() != null || 
                    hitObj.GetComponentInParent<HeroStatUI>() != null ||
                    hitObj.GetComponentInParent<UnityEngine.UI.Button>() != null)
                {
                    isClickingValidUI = true;
                    break; 
                }
            }

            // Jika klik UI, TAPI bukan di Menu/Stats/Tombol (Alias klik background Canvas kosong)
            if (!isClickingValidUI)
            {
                OnTargetCanceled?.Invoke(); // BATALKAN & TUTUP MENU!
            }
            
            return; // Hentikan kode di sini, jangan lakukan Raycast ke 3D
        }

        // ==========================================
        // 2. CEK APAKAH KLIK MENGENAI AREA 3D (GAME)
        // ==========================================
        Ray ray = mainCam.ScreenPointToRay(Mouse.current.position.ReadValue());
        
        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            CharacterBase clickedEnemy = hit.collider.GetComponentInParent<CharacterBase>();
            
            if (clickedEnemy != null && CharacterManager.Instance.ActiveEnemies.Contains(clickedEnemy))
            {
                // JIKA KLIK MUSUH: Eksekusi targeting!
                int clickedIndex = CharacterManager.Instance.ActiveEnemies.IndexOf(clickedEnemy);

                if (clickedIndex == currentTargetIndex)
                {
                    ConfirmTarget();
                }
                else
                {
                    currentTargetIndex = clickedIndex;
                    UpdateHighlight();
                }
            }
            else
            {
                // JIKA KLIK OBJEK 3D LAIN (Pohon, Tanah, Pahlawan):
                OnTargetCanceled?.Invoke(); // BATALKAN & TUTUP MENU!
            }
        }
        else
        {
            // JIKA KLIK LANGIT KOSONG (Tidak mengenai apa pun di 3D):
            OnTargetCanceled?.Invoke(); // BATALKAN & TUTUP MENU!
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
