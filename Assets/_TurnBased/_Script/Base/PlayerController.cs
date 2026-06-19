using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private float speed;
    [SerializeField] private float groundDist;
    [SerializeField] private LayerMask terrainLayer;
    [SerializeField] private CharacterController controller;

    private Vector2 _moveInput;

    private void Update()
    {
        //if (GameManager.Instance.State != GameState.Exploring) return;

        HandleInput();
        HandleMovement();
        SnapToTerrain();
    }

    private void HandleInput()
    {
        var keyboard = Keyboard.current;
        _moveInput = Vector2.zero;

        if (keyboard.dKey.isPressed || keyboard.rightArrowKey.isPressed) _moveInput.x = 1f;
        if (keyboard.aKey.isPressed || keyboard.leftArrowKey.isPressed) _moveInput.x = -1f;
        if (keyboard.wKey.isPressed || keyboard.upArrowKey.isPressed) _moveInput.y = 1f;
        if (keyboard.sKey.isPressed || keyboard.downArrowKey.isPressed) _moveInput.y = -1f;
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