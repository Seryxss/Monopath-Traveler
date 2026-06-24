using UnityEngine;
    
public class PlayerRestorePosition : MonoBehaviour{
    private void Start()
    {
        if (SceneTransitionManager.Instance != null && SceneTransitionManager.Instance.isReturningFromBattle)
        {
            // Matikan sementara komponen seperti CharacterController (jika ada) agar tidak memblokir teleportasi
            CharacterController cc = GetComponent<CharacterController>();
            if (cc != null) cc.enabled = false;

            // Teleportasi ke titik terakhir!
            transform.position = SceneTransitionManager.Instance.lastPlayerPosition;

            if (cc != null) cc.enabled = true;

            // Reset penanda agar tidak teleport terus-terusan
            SceneTransitionManager.Instance.isReturningFromBattle = false;
        }
    }
}