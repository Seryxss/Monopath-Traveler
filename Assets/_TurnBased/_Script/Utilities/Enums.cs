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
    SpawningHeroes = 0,
    SpawningEnemies = 1,
    HeroTurn = 2,
    EnemyTurn = 3,
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
    Goblin = 1,
    Dragon = 2
}

[Serializable]
public enum CharacterType
{
    Heroes = 0,
    Enemies = 1,
    NPC = 2
}