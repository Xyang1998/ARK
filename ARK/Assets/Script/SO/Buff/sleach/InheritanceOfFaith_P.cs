using System.Collections;
using System.Collections.Generic;
using Spine;
using Spine.Unity;
using UnityEngine;

/// <summary>
/// 琴柳回合开始时，回收军旗并获得COST
/// </summary>
[CreateAssetMenu(fileName = "InheritanceOfFaith_P",menuName = "ScriptableObject/InheritanceOfFaith_P")]
public class InheritanceOfFaith_P : BaseBuff
{
    public BaseBuff inheritanceOfFaith_Ori;
    private BaseBuff inheritanceOfFaith;
    private string temp;
    public override void AddBuffToTarget(BaseCharacter _initiator, BaseCharacter _target)
    {
        if (!inheritanceOfFaith)
        {
            inheritanceOfFaith = Instantiate(inheritanceOfFaith_Ori);
            inheritanceOfFaith.AddBuffToTarget(_initiator,_target);
        }
        base.AddBuffToTarget(_initiator, _initiator);
        target = _target;
        temp = initiator.AnimAndDamageController.idleName;
        initiator.AnimAndDamageController.idleName = "Skill_2_Loop";
    }

    public override void MyTurnStart()
    {
        if (initiator.CanDoAction())
        {
            initiator.RemoveBuff(this);
        }
    }

    public override void BuffRemove()
    {
        Character self = initiator as Character;
        if (self!=null)
        {
            (self.skill as InheritanceOfFaith).RemoveDrop();
        }
        target.RemoveBuff(inheritanceOfFaith);
        if (initiator.BattleCharacterStateData.isDead == false)
        {
            initiator.AnimAndDamageController.animationState.SetAnimation(0, "Skill_2_End", false);
            initiator.AnimAndDamageController.animationState.Complete += Complete;
        }
        else
        {
            base.BuffRemove();
        }

    }

    private  void Complete(TrackEntry e)
    {
        initiator.AnimAndDamageController.animationState.Complete -= Complete;
        initiator.AnimAndDamageController.idleName = temp;
        initiator.AnimAndDamageController.animationState.SetAnimation(0, initiator.AnimAndDamageController.idleName,
                true);
        
        base.BuffRemove();
    }
    
}
