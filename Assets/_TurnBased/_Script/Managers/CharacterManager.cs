using UnityEngine;

//Specific Scene Manager grabbing resources from resource system
public class CharacterManager : Singleton<CharacterManager>
{
    public void SpawnHeroes()
    {
        SpawnUnit(HeroType.Warrior, new Vector3(1, 0, 0));
    }

    void SpawnUnit(HeroType t, Vector3 pos) 
    {
        var baseCharScriptable = ResourceSystem.Instance.GetHero(t);
        
        var spawned = Instantiate(baseCharScriptable.Prefab, pos, Quaternion.identity, transform);

        //Possible Stats Modification
        var stats = baseCharScriptable.BaseStats;
        stats.Health += 20; 

        spawned.SetStats(stats);
    }
}
