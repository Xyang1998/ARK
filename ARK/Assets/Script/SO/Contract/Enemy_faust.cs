using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName = "Enemy_faust",menuName = "ScriptableObject/Enemy_faust")]
public class Enemy_faust : NormalContractBuff
{
   [Tooltip("技能修改倍率")]
   public float skillChangedRate = 0;

   public override void AddBuffToTarget(BaseCharacter _initiator, BaseCharacter _target)
   {
      base.AddBuffToTarget(_initiator, _target);
      Enemy enemy=_initiator as Enemy;
      if (enemy!=null)
      {
         foreach (var skill in enemy.skills)
         {
            skill.damageRate += skill.damageRate * skillChangedRate;
         }
      }
   }
}
