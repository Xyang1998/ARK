using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using Random = UnityEngine.Random;

/// <summary>
/// 用于计算伤害的工具类
/// </summary>
public static class DamageCal 
{
    private static Type baseCharacterType = typeof(CharacterStateData);

    public static float GetValueByAttribute(CharacterStateData data,DataProperty baseAttribute)
    {
        PropertyInfo PropertyInfo = baseCharacterType.GetProperty(baseAttribute.ToString());
        return (float)PropertyInfo.GetValue(data);
    }

    public static void SetValueByAttribute(CharacterStateData data,DataProperty baseAttribute,float modify)
    {
        PropertyInfo PropertyInfo = baseCharacterType.GetProperty(baseAttribute.ToString());
        PropertyInfo.SetValue(data,modify);
    }
    
    public static float CalDamage(float baseAttribute,CharacterStateData initiator,CharacterStateData target,float damageRate,DamageType damageType,bool critical)
    {
        float finalDamage = 0.0f;
        if (damageType == DamageType.Healing)
        {
            float heal = -(1.0f + initiator.Healing/100) * (baseAttribute * damageRate);
            return heal<=0?heal:0;
        }
        else if (damageType == DamageType.Physical)
        {
            float minDamage = baseAttribute * damageRate * NumericalDefinition.minDamageRate;
            float damage = baseAttribute * damageRate-target.Defense;
            finalDamage= (damage > minDamage ? damage : minDamage) * 
                   (critical? 1.0f + initiator.CriticalDamage/100 : 1.0f);

        }
        else if (damageType == DamageType.Magic)
        {
            float damage = baseAttribute * damageRate * (1 - target.MagicDefense/100)*(critical? 1.0f + initiator.CriticalDamage/100 : 1.0f);
            finalDamage= damage >= 0 ? damage : 0;
        }
        else if (damageType == DamageType.Real)
        {
            float damage = baseAttribute * damageRate * (critical ? 1.0f + initiator.CriticalDamage / 100 : 1.0f);
            finalDamage= damage > 0 ? damage : 0;
        }

        finalDamage = finalDamage * initiator.DamageRate/100 * target.GetDamageRate/100;
        
        return finalDamage;
    }

    public static float CalNp(CharacterStateData initiator,BaseSkill skill,bool critical)
    {
        return skill.NPGet * initiator.NPRate/100 * (critical ? NumericalDefinition.criticalNPGet : 1.0f);
    }
    /// <summary>
    /// 受击者获得的NP
    /// </summary>
    /// <param name="initiator"></param>
    /// <param name="skill"></param>
    /// <param name="critical"></param>
    /// <returns></returns>
    public static float CalHitNp(CharacterStateData target,bool critical)
    {
        return NumericalDefinition.getHitNP * target.NPRate/100 * (critical ? NumericalDefinition.getHitNP : 1.0f);
    }

    public static int WeightedSampling(float[] weights)
    {
        try
        {
            float totalWeight = 0;
            foreach (var weight in weights)
            {
                totalWeight += weight;
            }

            if (totalWeight == 0)
            {
                return -1;
            }

            float random = Random.value;
            float preSum = 0;
            float nextSum = 0;
            for (int i=0;i < weights.Length;i++)
            {
           
                nextSum +=    weights[i] / totalWeight;
                if (random >= preSum && random <= nextSum)
                {
                    return i;
                }
                preSum = nextSum;
                
            }

            return 0;
        }
        catch (Exception e)
        {

            return 0;
        }

    }
}
