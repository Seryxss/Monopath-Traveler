using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInteraction : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private float interactRadius = 2f;
    [SerializeField] private LayerMask interactableLayer;

    private PlayerInputAction _actions;

    private void Awake()
    {
        _actions = new PlayerInputAction();
    }

    private void OnEnable()
    {
        _actions.Enable();
        _actions.Player.Interact.performed += OnInteractPerformed;
    }

    private void OnDisable()
    {
        // Melepas pendaftaran saat script mati/pindah scene
        _actions.Player.Interact.performed -= OnInteractPerformed;
        _actions.Disable();
    }

    private void OnInteractPerformed(InputAction.CallbackContext ctx)
    {
        if (GameManager.Instance.State != GameState.Exploring) return; 

        TryInteract();
    }

    private void TryInteract()
    {
        Collider[] hits = Physics.OverlapSphere(transform.position, interactRadius, interactableLayer);

        foreach(var hit in hits)
        {
            IInteractable interactableObject = hit.GetComponent<IInteractable>();
            
            if (interactableObject != null)
            {
                Debug.Log($"[SUKSES] Berinteraksi dengan: {hit.gameObject.name}");

                interactableObject.Interact();
                
                break;
            }
        }
    }

    // Fungsi bantuan visual untuk melihat seberapa besar radius interaksimu di layar Scene
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, interactRadius);
    }
}