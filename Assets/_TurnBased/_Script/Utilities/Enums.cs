using System;

[Serializable]
public enum GameState
{
    MainMenu = 0,
    Exploring = 1,
    InDialog = 2,
    InBattle = 3,
    Paused = 3
}

[Serializable]
public enum BattleState
{
    LoadBattleScene = 0,
    SpawningHeroes = 1,
    SpawningEnemies = 2,
    HeroTurn = 3,
    SelectTarget = 4,
    EnemyTurn = 5,
    Win = 8,
    Lose = 9
}

[Serializable]
public enum HeroType
{
    Warrior = 0,
    Mage = 1,
    Tank = 2,
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
    StartRight = 1,
    EncounterSlimeLeft = 2,
    EncounterSlimeRight = 3,
    EncounterPartyLeft = 4,
    EncounterpartyRight = 5,
}

[Serializable]
public struct Stats
{
    public int maxHp;
    public int maxSp;
    public int Attack;
    public int speed; 

}

public enum TargetType 
{ 
    Single, 
    All, 
    Random, 
    Self 

}

public enum DamageType
{
    None,
    Physical,
    Fire,
    Ice,
    Lightning,
}