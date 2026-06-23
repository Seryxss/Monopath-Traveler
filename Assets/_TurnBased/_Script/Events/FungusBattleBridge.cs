using UnityEngine;

public class FungusBattleBridge : MonoBehaviour
{
    [Header("Encounter Data")]
    public ScriptableEncounter encounterData; 

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