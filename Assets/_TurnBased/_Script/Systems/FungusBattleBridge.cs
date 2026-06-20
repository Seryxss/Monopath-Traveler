using UnityEngine;

public class FungusBattleBridge : MonoBehaviour
{
    [Header("Encounter Data")]
    public ScriptableEncounter encounterData; // Pastikan nama tipe datanya sesuai dengan milikmu

    // Fungsi ini yang akan dipanggil oleh perintah Invoke Method di Fungus
    public void CallEncounter()
    {
        if (EncounterManager.Instance != null)
        {
            // Kirim pesan ke Manager gaib kita!
            EncounterManager.Instance.StartBattleEncounter(encounterData);
        }
        else
        {
            Debug.LogError("Gagal: EncounterManager belum di-spawn oleh Bootstrapper!");
        }
    }
}