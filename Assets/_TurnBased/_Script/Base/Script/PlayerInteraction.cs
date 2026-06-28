using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInteraction : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private float interactRadius = 2f;
    [SerializeField] private LayerMask interactableLayer;

    private PlayerInputAction _actions;
    private IInteractable _currentInteractable; // Menyimpan NPC yang sedang didekati

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
        _actions.Player.Interact.performed -= OnInteractPerformed;
        _actions.Disable();

        if (_currentInteractable != null) _currentInteractable.HidePrompt();
    }

    private void Update()
    {
        if (GameManager.Instance.State != GameState.Exploring)
        {
            if (_currentInteractable != null)
            {
                _currentInteractable.HidePrompt();
                _currentInteractable = null;
            }
            return;
        }

        // Cari NPC terdekat
        IInteractable nearbyInteractable = GetNearbyInteractable();

        // Jika NPC yang didekati berubah (baru masuk area, atau pindah NPC)
        if (nearbyInteractable != _currentInteractable)
        {
            // Matikan UI NPC yang lama
            if (_currentInteractable != null) _currentInteractable.HidePrompt();
            
            _currentInteractable = nearbyInteractable;

            // Nyalakan UI NPC yang baru
            if (_currentInteractable != null) _currentInteractable.ShowPrompt();
        }
    }

    private IInteractable GetNearbyInteractable()
    {
        Collider[] hits = Physics.OverlapSphere(transform.position, interactRadius, interactableLayer);
        
        IInteractable closestInteractable = null;
        float closestDistance = Mathf.Infinity; 

        foreach (var hit in hits)
        {
            IInteractable interactable = hit.GetComponent<IInteractable>();
            
            if (interactable != null)
            {
                float distanceToTarget = Vector3.Distance(transform.position, hit.transform.position);

                if (distanceToTarget < closestDistance)
                {
                    closestDistance = distanceToTarget;  
                    closestInteractable = interactable;  
                }
            }
        }

        return closestInteractable;
    }

    private void OnInteractPerformed(InputAction.CallbackContext ctx)
    {
        if (GameManager.Instance.State != GameState.Exploring) return; 
        if (_currentInteractable != null)
        {
            _currentInteractable.Interact();
            _currentInteractable.HidePrompt(); // Sembunyikan setelah ditekan
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, interactRadius);
    }
}