using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Character : BaseCharacter
{
    //private BaseSkill attack;


    public BaseSkill skill
    {
        get;
        private set;
    }



    
    public Character(BaseCharacterState state,CharacterDataStruct dataStruct):base(state,dataStruct)
    {
        
        
        
    }

    protected override void CreateBattleCharacter()
    {
        //Debug.log("创建Character");
        base.CreateBattleCharacter();
        skill=Object.Instantiate(Resources.Load<BaseSkill>($"SkillSO/{CharacterDataStruct.name}_{ID}/Skill"));
        LoadAsset(skill, $"Timelines/BattleCharacter/{CharacterDataStruct.name}_{ID}/Skill");
        //animAndDamageController.InitPlayableAsset($"Timelines/BattleCharacter/{Name}_{ID}/Attack",$"Timelines/BattleCharacter/{Name}_{ID}/Skill");

    }



    public override void NPCheck()
    {
        base.NPCheck();
        BattleCharacterUI CharacterUI=(ui as BattleCharacterUI);
        if (CharacterUI)
        {
            
            if (isNPMax())
            {
                CharacterUI.UltimateReady();
            }
            else
            {
                CharacterUI.UltimateBlock();
            }
        }
    }
    




    public void UltimateClick()
    {
        BattleCharacterUI battleCharacterUI=ui as BattleCharacterUI;
        if (battleCharacterUI)
        {
            battleCharacterUI.UltimateBlock();
        }

        BattleSystem.Instance.PriorityActionAdd(BattleSystem.Instance.PlayerUltimateUse,this);
    }

    public void BattleToWorld()
    {
        //characterStateData = DeepCopy.DeepCopyByReflect(originalCharacterStateData);
        originalCharacterStateData.ToWorldState(battleCharacterStateData.HP,battleCharacterStateData.NP);
    }
}
