using System;
using Unity;
using Unity.VisualScripting.Antlr3.Runtime.Misc;
using UnityEngine;

public abstract class BaseCharacter : ScriptableObject
{
    public Type type;
    [SerializeField] private Stats _stats;
    public Stats BaseStats => _stats;

    //Prefab --Nee to change gameobject to character prefab
    public GameObject prefab;

    //For UI
    public string Description;
    public Sprite MenuSprite;

}

[Serializable]
public struct Stats
{
    public int Health;
    public int Attack;
}

[Serializable]
public enum Type
{
    Heros = 0,
    Enemies = 2,
    NPC = 3
}