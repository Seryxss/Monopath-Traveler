using UnityEngine;

public class FungusBattleBridge : MonoBehaviour
{
    [Header("Encounter Data")]
    [SerializeField] private ScriptableEncounter encounterData; 

    public void CallEncounter()
    {
        if (encounterData != null)
        {
            GameEvents.RequestBattle(encounterData);
        }
        else
        {
            Debug.LogError("Data musuh di Bridge masih kosong!");
        }
    }
}
