using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class ActionIntent
{
    public ScriptableSkill ChosenSkill;
    public CharacterBase Target;
    public int BoostAmount;
    [SerializeField] private ScriptableSkill _basicAttackSkill;
    public ScriptableSkill BasicAttackSkill => _basicAttackSkill;

    public void ResetToDefault(List<CharacterBase> allEnemies)
    {
        ChosenSkill = _basicAttackSkill;
        BoostAmount = 0;

        // Kunci target ke musuh paling atas
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