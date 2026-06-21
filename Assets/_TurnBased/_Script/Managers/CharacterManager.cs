using System.Collections.Generic;
using UnityEngine;

public class CharacterManager : Singleton<CharacterManager>
{
    [SerializeField] private Transform heroTopSlot;
    [SerializeField] private Transform heroMidSlot;
    [SerializeField] private Transform heroBottomSlot;

    [SerializeField] private Transform enemyTopSlot;
    [SerializeField] private Transform enemyMidSlot;
    [SerializeField] private Transform enemyBottomSlot;

    public List<CharacterBase> ActiveHeroes { get; private set; } = new List<CharacterBase>();
    public List<CharacterBase> ActiveEnemies { get; private set; } = new List<CharacterBase>();

    public void SpawnHeroes()
    {
        ActiveHeroes.Clear();

        List<HeroType> party = GameManager.Instance.currentParty;

        // Data Dummy Testing if no Party happen
        if (party == null || party.Count == 0)
        {
            Debug.LogWarning("Empty Team");
            Transform targetSlot = GetHeroSlot(1, 0);
            SpawnUnit(HeroType.Warrior, targetSlot, true); 
            return;
        }

        int totalHeroes = party.Count;
        for (int i = 0; i < totalHeroes; i++)
        {
            Transform targetSlot = GetHeroSlot(totalHeroes, i);
            SpawnUnit(party[i], targetSlot, true); 
        }
    }

    public void SpawnEnemies()
    {
        ActiveEnemies.Clear();
        
        ScriptableEncounter currentEncounter = EncounterManager.Instance.CurrentEncounter;

        //IF Encounter has no Data, Use Dummy Enemy
        if (currentEncounter == null || currentEncounter.enemiesInBattle.Count == 0)
        {
            Debug.LogWarning("Tidak ada data encounter di GameManager! Memunculkan 1 Slime sebagai fallback.");
            Transform targetSlot = GetEnemySlot(1, 0);
            Transform targetSlot2 = GetEnemySlot(2, 0);
            SpawnUnit(EnemyType.Slime, targetSlot, false);
            SpawnUnit(EnemyType.GreenSlime, targetSlot2, false);
            return;
        }

        int totalEnemies = currentEncounter.enemiesInBattle.Count;

        for (int i = 0; i < totalEnemies; i++)
        {
            EnemyType currentEnemyType = currentEncounter.enemiesInBattle[i];
            Transform targetSlot = GetEnemySlot(totalEnemies, i);
            
            SpawnUnit(currentEnemyType, targetSlot, false); 
        }
    }

    private void SpawnUnit(HeroType t, Transform slot, bool isHero) 
    {
        var data = ResourceSystem.Instance.GetHero(t);
        var spawned = Instantiate(data.Prefab, slot.position, Quaternion.identity, slot);
        spawned.SetStats(data.BaseStats);
        
        SnapToGround snapper = spawned.GetComponent<SnapToGround>();
        if (snapper != null)
        {
            snapper.Snap();
        }
        
        ActiveHeroes.Add(spawned);
    }

    private void SpawnUnit(EnemyType t, Transform slot, bool isHero) 
    {
        var data = ResourceSystem.Instance.GetEnemy(t);
        var spawned = Instantiate(data.Prefab, slot.position, Quaternion.identity, slot);
        spawned.SetStats(data.BaseStats);
        
        SnapToGround snapper = spawned.GetComponent<SnapToGround>();
        if (snapper != null)
        {
            snapper.Snap();
        }
        
        ActiveEnemies.Add(spawned);
    }

    private Transform GetHeroSlot(int totalHeroes, int currentIndex)
    {
        switch (totalHeroes)
        {
            case 1: return heroMidSlot;
            case 2: return (currentIndex == 0) ? heroTopSlot : heroBottomSlot;
            case 3: 
                if (currentIndex == 0) return heroTopSlot;
                if (currentIndex == 1) return heroMidSlot;
                return heroBottomSlot;
            default: return heroMidSlot; 
        }
    }
    private Transform GetEnemySlot(int totalEnemies, int currentIndex)
    {
        switch (totalEnemies)
        {
            case 1: return enemyMidSlot;
            case 2: return (currentIndex == 0) ? enemyTopSlot : enemyBottomSlot;
            case 3: 
                if (currentIndex == 0) return enemyTopSlot;
                if (currentIndex == 1) return enemyMidSlot;
                return enemyBottomSlot;
            default: return enemyMidSlot; 
        }
    }
}