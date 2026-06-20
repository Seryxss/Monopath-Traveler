using UnityEngine;

public class EncounterManager : PersistentSingleton<EncounterManager>
{
    public ScriptableEncounter CurrentEncounter { get; set; }

    public void StartBattleEncounter(ScriptableEncounter encounter)
    {
        CurrentEncounter = encounter;
        
        SceneTransitionManager.Instance.TransitionToBattle();
    }
}