using UnityEngine;

public class ScenePortal : MonoBehaviour
{
    [Header("Destination Scene")]
    public string sceneToLoad; 
    
    [Tooltip("Destination spawn door")]
    public SpawnId destinationSpawnId; 

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