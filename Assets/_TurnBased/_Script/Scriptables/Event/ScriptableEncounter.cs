using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Encounter", menuName = "Game/Battle Encounter")]
public class ScriptableEncounter : ScriptableObject
{
    [Header("Enemy List")]
    public List<EnemyType> enemiesInBattle;
}