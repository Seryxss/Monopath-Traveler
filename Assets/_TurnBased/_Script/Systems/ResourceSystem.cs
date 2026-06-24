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
    // Load Heroes
    Heroes = Resources.LoadAll<ScriptableHero>("Heroes").ToList();
    _HeroesDict = new Dictionary<HeroType, ScriptableHero>();

    foreach (var hero in Heroes)
    {
        if (_HeroesDict.ContainsKey(hero.heroType))
        {
            Debug.LogWarning($"[ResourceSystem] Duplikat ditemukan! HeroType '{hero.heroType}' sudah ada. Mengabaikan aset: {hero.name}");
        }
        else
        {
            _HeroesDict.Add(hero.heroType, hero);
        }
    }

    // Load Enemies
    Enemies = Resources.LoadAll<ScriptableEnemy>("Enemies").ToList();
    _EnemiesDict = new Dictionary<EnemyType, ScriptableEnemy>();

    foreach (var enemy in Enemies)
    {
        if (_EnemiesDict.ContainsKey(enemy.enemyType))
        {
            Debug.LogWarning($"[ResourceSystem] Duplikat ditemukan! EnemyType '{enemy.enemyType}' sudah ada. Mengabaikan aset: {enemy.name}");
        }
        else
        {
            _EnemiesDict.Add(enemy.enemyType, enemy);
        }
    }
}

    // Search Hero
    public ScriptableHero GetHero(HeroType t) => _HeroesDict[t];
    public ScriptableHero GetRandomHero() => Heroes[Random.Range(0, Heroes.Count)];

    // Search Enemy
    public ScriptableEnemy GetEnemy(EnemyType t) => _EnemiesDict[t];
    public ScriptableEnemy GetRandomEnemy() => Enemies[Random.Range(0, Enemies.Count)];
}