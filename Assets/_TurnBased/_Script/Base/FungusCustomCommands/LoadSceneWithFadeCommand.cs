using UnityEngine;
using Fungus;
using UnityEngine.SceneManagement;

[CommandInfo("Scene", 
             "Load Scene With Fade", 
             "Change Scene with Fade in and Out")]
public class LoadSceneWithFadeCommand : Command
{
    [Tooltip("Destionation scene  (Ex: EncounterParty)")]
    [SerializeField] private string targetSceneName;

    public override void OnEnter()
    {
        if (SceneTransitionManager.Instance != null)
        {
            // Menyuruh Manager kita untuk melakukan proses Fade lalu pindah scene
            SceneTransitionManager.Instance.TransitionToScene(targetSceneName, SpawnId.None); 
        }
        else
        {
            Debug.LogWarning("[Fungus] SceneTransitionManager tidak ditemukan, pindah instan!");
            SceneManager.LoadScene(targetSceneName);
        }

        // CATATAN PENTING:
        // Kita TIDAK memanggil Continue() di sini.
        // Karena begitu fungsi ini jalan, layar akan mulai menghitam dan scene akan dihancurkan.
        // Kita biarkan blok Fungus ini berhenti dengan damai di sini.
        Continue();
    }

    public override string GetSummary()
    {
        if (string.IsNullOrEmpty(targetSceneName)) return "Error: Nama scene kosong";
        return "Fade to: " + targetSceneName;
    }

    public override Color GetButtonColor()
    {
        return new Color32(200, 200, 200, 255); // Warna abu-abu yang elegan
    }
}