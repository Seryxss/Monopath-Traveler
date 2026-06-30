using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class ActionIntent
{
    public ScriptableSkill ChosenSkill;
    public CharacterBase Target;
    public HeroCharBase AllyTarget;
    public int BoostAmount;

    public void ResetToDefault(List<CharacterBase> allEnemies, ScriptableSkill fallbackSkill, int heroCurrentSp)
    {
        if (ChosenSkill != null)
        {
            if (heroCurrentSp + 5 < ChosenSkill.spCost) 
            {
                ChosenSkill = fallbackSkill; 
            }
        }
        else
        {
            ChosenSkill = fallbackSkill;
        }

        BoostAmount = 0;

        if (AllyTarget != null)
        {
            if (AllyTarget.currentHp <= 0 || !AllyTarget.gameObject.activeInHierarchy)
            {
                AllyTarget = null;
            }
        }

        if (Target != null &&
            Target.currentHp > 0 &&
            Target.gameObject.activeInHierarchy &&
            allEnemies != null &&
            allEnemies.Contains(Target))
        {
            return;
        }

        if (allEnemies != null && allEnemies.Count > 0)
        {
            Target = allEnemies[0];
        }
        else
        {
            Target = null;
        }
    }

    public void ToggleBoost(bool isMax, int maxBoostAllowed)
    {
        if (isMax)
        {
            BoostAmount = maxBoostAllowed;
        }
        else
        {
            BoostAmount = 0;
        }
    }
}