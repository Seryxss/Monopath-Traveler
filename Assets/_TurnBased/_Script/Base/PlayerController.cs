using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private float speed = 5f;
    [SerializeField] private float groundDist = 0.35f;
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
    }

    private void OnDisable()
    {
        _actions.Player.Move.performed -= OnMovementPerformed;
        _actions.Player.Move.canceled -= OnMovementCanceled;
        _actions.Disable();
    }

    private void OnMovementPerformed(InputAction.CallbackContext ctx)
    {
        _moveInput = ctx.ReadValue<Vector2>();
    }

    private void OnMovementCanceled(InputAction.CallbackContext ctx)
    {
        _moveInput = Vector2.zero;
    }

    private void Update()
    {
        //if (GameManager.Instance.State != GameState.Exploring) return;
        HandleMovement();
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