using UnityEngine;
using Unity.Cinemachine; // Gunakan 'using Cinemachine;' jika versi Unity/Cinemachine kamu yang lebih lama

public class CameraManager : MonoBehaviour
{
    [Header("Cinemachine Settings")]
    [SerializeField] private CinemachineBrain cinemachineBrain;

    private void OnEnable()
    {
        // Mulai mendengarkan perubahan status game
        GameManager.OnGameStateChanged += HandleGameStateChanged;
    }

    private void OnDisable()
    {
        // Berhenti mendengarkan saat scene hancur
        GameManager.OnGameStateChanged -= HandleGameStateChanged;
    }

    private void HandleGameStateChanged(GameState newState)
    {
        if (cinemachineBrain == null) return;

        switch (newState)
        {
            case GameState.Exploring:
                cinemachineBrain.enabled = true;
                break;

            case GameState.InDialog:
            case GameState.InBattle:
                cinemachineBrain.enabled = false;
                break;
        }
    }
}
