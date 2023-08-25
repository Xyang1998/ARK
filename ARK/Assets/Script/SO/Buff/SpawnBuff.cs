using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName = "SpawnBuff",menuName = "ScriptableObject/SpawnBuff")]
public class SpawnBuff : BaseBuff
{
   public CharacterCamp camp;
   public BaseBuff buff;
   [Tooltip("当该角色退场时，是否清除buff？")]
   public bool removeWhenDie=false;
   public override void AddBuffToTarget(BaseCharacter _initiator, BaseCharacter _target)
   {
      base.AddBuffToTarget(_initiator, _target);
      BattleSystem.Instance.spawnAction += AddBuff;
   }
   private void AddBuff(BaseCharacter _target)
   {
      if (_target.CharacterDataStruct.characterCamp == camp)
      {
         BaseBuff baseBuff = Instantiate(buff);
         baseBuff.AddBuffToTarget(initiator,_target);
      }
   }
   public override void BuffRemove()
   {
      if (removeWhenDie)
      {
         if (camp == CharacterCamp.Enemy)
         {
            foreach (var enemy in BattleSystem.Instance.enemies)
            {
               if (enemy != null)
               {
                  enemy.RemoveBuffByID(buff.buffID);
               }
            }
         }
         else
         {
            foreach (var character in BattleSystem.Instance.characters)
            {
               if (character.CharacterStateData.isDead==false)
               {
                  character.RemoveBuffByID(buff.buffID);
               }
            }
         }
      }

      BattleSystem.Instance.spawnAction -= AddBuff;
      base.BuffRemove();
        
   }
}
