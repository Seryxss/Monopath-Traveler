using Unity.VisualScripting.Antlr3.Runtime.Misc;
using UnityEngine;

// Logic for unit in space, could be friend or enemy, controlled or not like taking damage, dying, animation triggers etc
public class CharacterBase : MonoBehaviour
{
    public Stats Stats { get; private set; }
    
    [Header("Current Status")]
    public int currentHp { get; protected set; } 
    public int currentSp { get; protected set; }

    public AudioSource charAudioSource { get; protected set; }

    protected virtual void Awake()
    {
        charAudioSource = gameObject.AddComponent<AudioSource>();
        charAudioSource.playOnAwake = false;
        charAudioSource.spatialBlend = 0f;
    }

    public virtual void SetStats(Stats stats) 
    {
        Stats = stats;
        
        currentHp = stats.maxHp;
        currentSp = stats.maxSp;
    }

    public virtual void TakeDamage(int damage)
    {
        currentHp -= damage;
        if (currentHp < 0) 
        {
            currentHp = 0;
        }

        Debug.Log($"[BATTLE] {gameObject.name} menerima {damage} damage! Sisa HP: {currentHp}/{Stats.maxHp}");
    }
}