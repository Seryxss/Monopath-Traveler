using System;
using System.IO;
using Unity;
using Unity.VisualScripting.Antlr3.Runtime.Misc;
using UnityEngine;

[CreateAssetMenu(fileName = "New Scriptable Object")]
public class ScriptableHero : ScriptableBaseCharacter
{
    public HeroType heroType;
}

public enum HeroType
{
    Warrior = 0,
    Mage = 1,
    Tank = 2,
}