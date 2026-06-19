using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private float speed;
    [SerializeField] private float groundDist;
    [SerializeField] private LayerMask terrainLayer;
    [SerializeField] private CharacterController controller;

    private Vector2 _moveInput;
    private PlayerInputAction _actions;

    private void Awake()
    {
        _actions = new PlayerInputAction();
    }

    private void OnEnable()
    {
        _actions.Enable();
        _actions.Player.Move.performed += OnMovementPerformed;
        _actions.Player.Move.canceled += OnMovementCanceled;
        
        _actions.Player.Interact.performed += OnInteractPerformed;
    }

    private void OnDisable()
    {
        _actions.Player.Move.performed -= OnMovementPerformed;
        _actions.Player.Move.canceled -= OnMovementCanceled;

        _actions.Player.Interact.performed -= OnInteractPerformed;
        
        _actions.Disable();
    }

    private void OnMovementPerformed(InputAction.CallbackContext ctx) => _moveInput = ctx.ReadValue<Vector2>();
    private void OnMovementCanceled(InputAction.CallbackContext ctx) => _moveInput = Vector2.zero;

    private void OnInteractPerformed(InputAction.CallbackContext ctx)
    {
        if (GameManager.Instance.State != GameState.Exploring) return; 

        // 2. Tembakkan radar (OverlapSphere)
        Collider[] hits = Physics.OverlapSphere(transform.position, 2f);
        Debug.Log($"Radar mendeteksi {hits.Length} objek di sekitar player.");

        bool isNPCFound = false;

        // 3. Cek satu per satu objek yang kena radar
        foreach(var hit in hits)
        {
            NPCBase npc = hit.GetComponent<NPCBase>();
            if (npc != null)
            {
                isNPCFound = true;
                Debug.Log($"[SUKSES] Menemukan NPC: {npc.gameObject.name}. Memanggil blok Fungus: {npc.Data.fungusBlockName}");
                
                DialogManager.Instance.PlayDialog(npc.Data.fungusBlockName);
                break; // Ketemu 1 NPC, langsung stop pencarian
            }
        }

        if (!isNPCFound)
        {
            Debug.Log("Tidak ada komponen NPCBase di sekitar player.");
        }
    }

    private void Update()
    {   
        if (GameManager.Instance.State != GameState.Exploring) return;
        
        HandleMovement();
    }

    private void LateUpdate()
    {
        SnapToTerrain();
    }

    private void HandleMovement()
    {
        Vector3 moveDir = new Vector3(_moveInput.x, 0f, _moveInput.y).normalized;
        controller.Move(moveDir * speed * Time.deltaTime);
    }

    private void SnapToTerrain()
    {
        Vector3 castPos = transform.position;
        castPos.y += 1f;

        if (Physics.Raycast(castPos, Vector3.down, out RaycastHit hit, Mathf.Infinity, terrainLayer))
        {
            Vector3 targetPos = transform.position;
            targetPos.y = hit.point.y + groundDist;
            transform.position = targetPos;
        }
    }
}