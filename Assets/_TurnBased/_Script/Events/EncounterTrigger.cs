using UnityEngine;

public class EncounterTrigger : MonoBehaviour
{
    [Header("EnemyData")]
    public ScriptableEncounter encounterData; 

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            GameEvents.RequestBattle(encounterData);
            
            Destroy(gameObject); 
        }
    }
}