using System;
using Unity;
using Unity.VisualScripting.Antlr3.Runtime.Misc;
using UnityEngine;

public class ScriptableBaseCharacter : ScriptableObject
{
    public CharacterType characterType;
    [SerializeField] private Stats _stats;
    public Stats BaseStats => _stats;

    //For UI
    public string Description;
    public Sprite MenuSprite;

}