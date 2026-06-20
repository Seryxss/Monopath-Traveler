using UnityEngine;

public class EncounterTrigger : MonoBehaviour
{
    [Header("EnemyData")]
    public ScriptableEncounter encounterData; 

    // Jika player menabrak GameObject ini
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            
            // Lempar datanya ke GameManager yang tadi kita buat
            EncounterManager.Instance.StartBattleEncounter(encounterData);
            
            // Hancurkan musuh di map agar tidak bisa ditabrak 2x
            Destroy(gameObject); 
        }
    }
}