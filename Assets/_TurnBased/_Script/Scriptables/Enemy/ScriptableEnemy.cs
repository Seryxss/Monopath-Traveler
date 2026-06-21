using UnityEngine;

[CreateAssetMenu(fileName = "New Enemy", menuName = "Game/Enemy")]
public class ScriptableEnemy : ScriptableBaseCharacter
{
    public EnemyType enemyType;
    public EnemyBase Prefab; 
}