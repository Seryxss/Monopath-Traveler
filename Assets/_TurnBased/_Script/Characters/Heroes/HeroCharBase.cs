using NUnit.Framework.Constraints;
using Unity.VisualScripting;
using UnityEngine;

public abstract class HeroCharBase : CharacterBase
{
    [Header("Combat Planning")]
    public ActionIntent currentIntent = new ActionIntent();
    private bool _canMove;
    private void Awake() => BattleManager.OnPreStateChange += OnStateChanged;

    private void OnDestroy() => BattleManager.OnPreStateChange -= OnStateChanged;

    private void OnStateChanged(BattleState newState)
    {
        if (newState == BattleState.HeroTurn) _canMove = true;
    }


    public virtual void ExecuteMove()
    {
        //Overrie to do some hero specific logic , then call this base method to clean up the turn

        _canMove = false;
    }
}
