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
            Debug.Log($"Pindah ke {sceneToLoad}, target spawn: {destinationSpawnId}");
            
            // Sekarang tipe datanya sudah sama-sama Enum SpawnId
            SceneTransitionManager.Instance.TransitionToScene(sceneToLoad, destinationSpawnId);
        }
    }
}
