using System;
using System.IO;
using Unity;
using Unity.VisualScripting.Antlr3.Runtime.Misc;
using UnityEngine;

[CreateAssetMenu(fileName = "New Hero", menuName = "Game/Hero")]
public class ScriptableHero : ScriptableBaseCharacter
{
    public HeroType heroType;
}
