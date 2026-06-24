using UnityEngine;

public class ScenePortal : MonoBehaviour
{
    [Header("Destination Scene")]
    [SerializeField] private string sceneToLoad; 
    
    [Tooltip("Destination spawn door")]
    [SerializeField] private SpawnId destinationSpawnId; 

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {   
            SceneTransitionManager.Instance.TransitionToScene(sceneToLoad, destinationSpawnId);
        }
    }
}
