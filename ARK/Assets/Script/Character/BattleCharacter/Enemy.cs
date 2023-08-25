using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : BaseCharacter
{
    //private BaseSkill attack;
    //private BaseSkill skill; //TODO：临时使用
    public List<BaseSkill> skills= new List<BaseSkill>();
    public Enemy(BaseCharacterState state,CharacterDataStruct dataStruct):base(state,dataStruct)
    {
    }

    protected override void CreateBattleCharacter()
    {        
        //Debug.log("创建Enemy");
        base.CreateBattleCharacter();
        int index = 1;
        while (true)
        {
            BaseSkill s = Resources.Load<BaseSkill>($"SkillSO/{CharacterDataStruct.name}_{ID}/Skill_{index}");
            if (s)
            {
                BaseSkill skill = Object.Instantiate(s);
                LoadAsset(skill, $"Timelines/BattleCharacter/{CharacterDataStruct.name}_{ID}/Skill_{index}");
                skills.Add(skill);
                index += 1;
            }
            else
            {
                break;
            }
        }
        
        //animAndDamageController.InitPlayableAsset($"Timelines/BattleCharacter/{Name}_{ID}/Attack",$"Timelines/BattleCharacter/{Name}_{ID}/Skill");

        
    }



    public BaseSkill GetSkill(int index)
    {
        return skills[index];
    }
    public int GetSkillCount()
    {
        return skills.Count;
    }


} 

