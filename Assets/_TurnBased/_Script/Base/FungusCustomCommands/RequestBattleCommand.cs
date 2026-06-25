using UnityEngine;
using Fungus.DentedPixel;

namespace Fungus
{
    [CommandInfo("RPG Systems", 
                "Request Battle", 
                "Request Battle with ScriptableEncounter Data to EncounterManager.")]
    public class RequestBattleCommand : Command
    {
        [Tooltip("Data musuh/encounter yang akan dilawan")]
        [SerializeField] private ScriptableEncounter encounterData;
        public override void OnEnter()
        {
            if (encounterData != null)
            {
                GameEvents.RequestBattle(encounterData);
            }
            else
            {
                Debug.LogError("[Fungus Command] Encounter Data kosong! Pertarungan gagal dimulai.");
            }

            Continue();
        }

        // (Opsional) Mengubah teks yang muncul di kotak Flowchart agar lebih rapi
        public override string GetSummary()
        {
            if (encounterData == null) return "Error: No Encounter Data";
            return "Battle vs: " + encounterData.name;
        }

        public override Color GetButtonColor()
        {
            return new Color32(235, 64, 52, 255); // Warna merah ala battle
        }
    }
}