using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Bleed",menuName = "ScriptableObject/Bleed")]
public class Bleed : BaseBuff
{
    public BaseBuff XueNu;
    public BaseBuff addSpeed;
    public float DamageNum;
    public override void AddBuffToTarget(BaseCharacter _initiator, BaseCharacter _target)
    {
        base.AddBuffToTarget(_initiator, _target);
        BaseBuff exist = _target.HasBuff(buffID);
        if (exist.CurLayers == exist.maxLayers)
        {
            //BaseBuff xueNu = Instantiate(XueNu);
            //xueNu.AddBuffToTarget(_target,_initiator);

            target.GetDamage(initiator,null,DamageNum,true,DamageType.Real,false);
            BaseBuff buff = Instantiate(addSpeed);
            buff.AddBuffToTarget(target,initiator);

        }
    }


}
