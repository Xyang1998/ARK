using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

[Serializable]
public abstract class BaseAI : ScriptableObject
{
    public float skillProb = 0.2f;
    public virtual SelectResult SelectTarget(Enemy enemy,List<Character> characters,List<Enemy> enemies)
    {
        SelectResult result;
        result.turnEnd = true;
        float[] weights = new float[characters.Count];
        for (int i = 0; i < characters.Count; i++)
        {
            if (characters[i].BattleCharacterStateData.isDead==false)
            {
                weights[i] = characters[i].BattleCharacterStateData.TauntValue;
            }
        }
        

        var index = DamageCal.WeightedSampling(weights);
        if (index == -1) {result.target = null;}
        else
        {
            result.target = characters[index];
        }

        if (enemy.ultimate)
        {
            if (enemy.isNPMax())
            {
                result.skill = enemy.ultimate;
                enemy.UltimateUse();
                return result;


            }
        }


        if (enemy.GetSkillCount() == 0)
        {
            result.skill = null;
            return result;
        }

        float uesSkillProb = Random.value;
        if (uesSkillProb <= skillProb)
        {
            
            result.skill = enemy.GetSkill(Random.Range(0,enemy.GetSkillCount()));
        }
        else
        {
            result.skill = null;
        }


        return result;
    }
}
