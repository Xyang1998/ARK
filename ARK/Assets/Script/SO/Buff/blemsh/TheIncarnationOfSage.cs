using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//[CreateAssetMenu(fileName = "TheIncarnationOfSage",menuName = "ScriptableObject/TheIncarnationOfSage")]
public class TheIncarnationOfSage : BaseBuff
{
    public override void AfterAttack(BaseCharacter _initiator, BaseCharacter _target, BaseSkill skill)
    {
        if (skill == initiator.attack)
        {
            List<BaseCharacter> characters = new List<BaseCharacter>();
            foreach (var character in BattleSystem.Instance.characters)
            {
                if (character.BattleCharacterStateData.HP <= character.BattleCharacterStateData.MaxHP&& character.BattleCharacterStateData.isDead==false)
                {
                    characters.Add(character);
                }
            }

            if (characters.Count == 0) return;
            int index = Random.Range(0, characters.Count);
            foreach (var damageBuff in damageBuffs)
            {
                characters[index].GetDamage(initiator,null,DamageBuffCal(damageBuff),true,damageBuff.damageType,false);
            }


        }
    }

    public override void MyTurnStart()
    {
        
    }


}
