using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "XueNu",menuName = "ScriptableObject/XueNu")]
public class XueNu : BaseBuff
{
    public BaseBuff bleed;
    //private BaseBuff Bleed;
    public override void AddBuffToTarget(BaseCharacter _initiator, BaseCharacter _target)
    {
        base.AddBuffToTarget(_initiator, _target);
        //Bleed = Instantiate(bleed);
        for (int i = 0; i < 5; i++)
        {
            target.attack.buffs.Add(bleed);
        }
        
    }

    public override void BuffRemove()
    {
        for (int i = 0; i < 5; i++)
        {
            target.attack.buffs.Remove(bleed);
        }
        base.BuffRemove();
    }
}
