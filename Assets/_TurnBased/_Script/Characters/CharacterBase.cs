using Unity.VisualScripting.Antlr3.Runtime.Misc;
using UnityEngine;


//Logic for unit in space, like taking damage, dying, animation triggers etc
public class CharacterBase : MonoBehaviour
{
    public Stats Stats { get; private set; }
    public virtual void SetStats(Stats stats) => Stats = stats;

    public virtual void TakeDamage (int damage)
    {
        
    }
}
