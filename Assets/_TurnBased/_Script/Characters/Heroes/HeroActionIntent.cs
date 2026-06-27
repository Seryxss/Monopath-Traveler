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
    public void ResetToDefault(List<CharacterBase> allEnemies, ScriptableSkill fallbackSkill)
    {
        ChosenSkill = fallbackSkill;
        BoostAmount = 0;
        AllyTarget = null;

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
