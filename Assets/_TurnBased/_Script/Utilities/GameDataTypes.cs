using System;

[Serializable]
public enum GameState
{
    MainMenu = 0,
    Exploring = 1,
    InDialog = 2,
    InBattle = 3,
    Paused = 4
}

public enum BattleState
{
    LoadBattleScene = 0,
    SpawningEnemies = 1,
    SpawningHeroes = 2,
    HeroTurn = 3,         
    SelectTarget = 4,     
    ExecutingTurn = 5,    
    Victory = 8,
    Defeat = 9
}

[Serializable]
public enum HeroType
{
    Theo = 0,
    Cyrus = 1,
    Primrose = 2,
}

[Serializable]
public enum EnemyType
{
    Slime = 0,
    GreenSlime = 1,
    Goblin = 2,
    Dragon = 3
}

[Serializable]
public enum CharacterType
{
    Heroes = 0,
    Enemies = 1,
    NPC = 2
}

[Serializable]
public enum SpawnId
{
    None = 0,
    StartLeft = 1,
    StartRight = 2,
    EncounterSlimeLeft = 3,
    EncounterSlimeRight = 4,
    EncounterPartyLeft = 5,
    EncounterpartyRight = 6,
}

public enum  TargetScope
{ 
    Single = 0, 
    All = 1, 
    Random = 2,     
    Self = 3 

}

public enum VFXSpawnLocation
{
    ActionCenter,
    PerTarget
}

public enum SkillElement
{
    None = 0,
    Axe_Physical = 1,
    Staff_Physical = 2,
    Grimoire_Physical = 3,
    Fire = 4,
    Ice = 5,
    Lightning = 6,
}

public enum SkillCategory
{
    None = 0,
    Phys = 1,
    Elem = 2,
    Recovery = 3,
    Enfeebling = 4,
    Augment = 5,
}

public enum DamageEffectiveness 
{ 
    None = 0, 
    Weak = 1, 
    Strong = 2
}

public enum EventStatus
{
    NotStarted = 0,
    InProgress = 1,
    Completed = 2
}

[Serializable]
public struct Stats
{
    public int maxHp;
    public int maxSp;
    public int Attack;
    public int speed; 

}   
