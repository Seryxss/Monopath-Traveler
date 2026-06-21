using NUnit.Framework.Constraints;
using Unity.VisualScripting;
using UnityEngine;

public abstract class HeroCharBase : CharacterBase
{
    private bool _canMove;
    private void Awake() => BattleManager.OnPreStateChange += OnStateChanged;

    private void OnDestroy() => BattleManager.OnPreStateChange -= OnStateChanged;

    private void OnStateChanged(BattleState newState)
    {
        if (newState == BattleState.HeroTurn) _canMove = true;
    }

    private void OnMouseDown()
    {
        //Only Allow Interaction when it's hero turn
        if (BattleManager.Instance.State != BattleState.HeroTurn) return;

        //don't move if we're already move
        if (!_canMove) return;

        //Show Movement/Attack options

        //Eventually either deselect or ExecutueMove(). can split ExecuteMove into multiple functions 
        // Like Move() / Attack() / Heal();
        Debug.Log("Char Clicked");
    }

    public virtual void ExecuteMove()
    {
        //Overrie to do some hero specific logic , then call this base method to clean up the turn

        _canMove = false;
    }
}
