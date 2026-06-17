using System;
using System.IO;
using Unity;
using Unity.VisualScripting.Antlr3.Runtime.Misc;
using UnityEngine;

[CreateAssetMenu(fileName = "New Scriptable Object")]
public abstract class BaseHero : BaseCharacter
{
    public HeroType heroType;
}

public enum HeroType
{
    Base = 0
}