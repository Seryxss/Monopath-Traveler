using System.Collections.Generic;
using System.Linq;
using UnityEngine;

// One Repo for all ScriptableObject
public class ResourceSystem : Singleton<ResourceSystem>
{
    public List<ScriptableHero> Heroes { get; private set; }
    private Dictionary<HeroType, ScriptableHero> _HeroesDict;

    // Tambahan untuk Enemy
    public List<ScriptableEnemy> Enemies { get; private set; }
    private Dictionary<EnemyType, ScriptableEnemy> _EnemiesDict;

    protected override void Awake()
    {
        base.Awake();
        AssembleResources();
    }

    private void AssembleResources()
    {
        // Load Heroes from Resources/Heroes
        Heroes = Resources.LoadAll<ScriptableHero>("Heroes").ToList();
        _HeroesDict = Heroes.ToDictionary(r => r.heroType, r => r);

        // Load Enemies from Resources/Enemies
        Enemies = Resources.LoadAll<ScriptableEnemy>("Enemies").ToList();
        _EnemiesDict = Enemies.ToDictionary(r => r.enemyType, r => r);
    }

    // Search Hero
    public ScriptableHero GetHero(HeroType t) => _HeroesDict[t];
    public ScriptableHero GetRandomHero() => Heroes[Random.Range(0, Heroes.Count)];

    // Search Enemy
    public ScriptableEnemy GetEnemy(EnemyType t) => _EnemiesDict[t];
    public ScriptableEnemy GetRandomEnemy() => Enemies[Random.Range(0, Enemies.Count)];
}