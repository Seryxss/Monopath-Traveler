using System.Collections.Generic;
using System.Linq;
using UnityEngine;

// One Repo for all ScriptableObject
public class ResourceSystem : Singleton<ResourceSystem>
{
    public List<ScriptableHero> Heroes { get; private set; }
    private Dictionary<HeroType, ScriptableHero> _HeroesDict;

    protected override void Awake()
    {
        base.Awake();
        AssembleResources();
    }

    private void AssembleResources()
    {
        Heroes = Resources.LoadAll<ScriptableHero>("Heroes").ToList();
        _HeroesDict = Heroes.ToDictionary(r => r.heroType, r => r);

    }

    public ScriptableHero GetHero(HeroType t) => _HeroesDict[t];
    public ScriptableHero GetRandomHero() => Heroes[Random.Range(0, Heroes.Count)];
}