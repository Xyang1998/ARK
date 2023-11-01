using System.Collections;
using System.Collections.Generic;
using Spine;
using Spine.Unity;
using UnityEngine;

/// <summary>
/// 琴柳回合开始时，回收军旗并获得COST
/// </summary>
//[CreateAssetMenu(fileName = "InheritanceOfFaith_P",menuName = "ScriptableObject/InheritanceOfFaith_P")]
public class InheritanceOfFaith_P : BaseBuff
{
    public BaseBuff targetBuff_Ori;
    public string endAnimName;
    public string loopAnimName;
    private BaseBuff targetBuff;
    private string temp;
    public override void AddBuffToTarget(BaseCharacter _initiator, BaseCharacter _target)
    {
        if (!targetBuff)
        {
            targetBuff = Instantiate(targetBuff_Ori);
            targetBuff.AddBuffToTarget(_initiator,_target);
        }
        base.AddBuffToTarget(_initiator, _initiator);
        target = _target;
        temp = initiator.AnimAndDamageController.idleName;
        initiator.AnimAndDamageController.idleName = loopAnimName;
        initiator.ultimateUseAction += Ultimate;
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
        initiator.ultimateUseAction -= Ultimate;
        Character self = initiator as Character;
        if (self!=null)
        {
            (self.skill as InheritanceOfFaith).RemoveDrop();
            (self.ultimate as InheritanceOfFaith).RemoveDrop();
        }
        target.RemoveBuff(targetBuff);
        if (initiator.BattleCharacterStateData.isDead == false)
        {
            initiator.AnimAndDamageController.animationState.SetAnimation(0, endAnimName, false);
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

    /// <summary>
    /// 使用终结技时，强制回收军旗
    /// </summary>
    public void Ultimate()
    {
        initiator.RemoveBuff(this);
    }
    
}
