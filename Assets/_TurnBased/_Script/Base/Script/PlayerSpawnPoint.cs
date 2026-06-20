using UnityEngine;

public class PlayerSpawnPoint : MonoBehaviour
{
    [Tooltip("Spawn ID for Transition")]
    public SpawnId spawnId;

    private void Start()
    {
        if (SceneTransitionManager.Instance != null && 
            SceneTransitionManager.Instance.nextSpawnPointId != SpawnId.None && 
            SceneTransitionManager.Instance.nextSpawnPointId == spawnId)
        {
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            
            if (player != null)
            {
                CharacterController cc = player.GetComponent<CharacterController>();
                if (cc != null) cc.enabled = false;
                
                player.transform.position = transform.position;
                player.transform.rotation = transform.rotation;
                
                if (cc != null) cc.enabled = true;
                
                Debug.Log($"Player berhasil di-spawn di titik: {spawnId}!");

                SceneTransitionManager.Instance.nextSpawnPointId = SpawnId.None;
            }
        }
    }
}