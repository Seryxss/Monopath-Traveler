using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "New Enemy", menuName = "Game/Enemy")]
public class ScriptableEnemy : ScriptableBaseCharacter
{
    public EnemyType enemyType;
    public EnemyBase Prefab; 

    [Header("Elemental Affinities")]
    public List<DamageType> weaknesses;
    public List<DamageType> resistances;
}