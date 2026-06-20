using UnityEngine;

[CreateAssetMenu(fileName = "New Enemy", menuName = "Game/Enemy")]
public class ScriptableEnemy : ScriptableBaseCharacter
{
    public EnemyType enemyType;
    
    // SEKARANG WADAHNYA KHUSUS UNTUK ENEMY BASE
    public EnemyBase Prefab; 
}