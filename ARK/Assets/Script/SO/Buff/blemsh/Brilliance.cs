using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;


//[CreateAssetMenu(fileName = "Brilliance",menuName = "ScriptableObject/Brilliance")]
public class Brilliance : BaseBuff
{
    private PlayableAsset tempAsset;
    public override void ChangeTargetProperties()
    {
        tempAsset = initiator.attack.Asset;
        initiator.attack.damageType = DamageType.Magic;
        initiator.attack.Asset = initiator.ultimate.Asset;
        base.ChangeTargetProperties();
    }

    public override void ResetTargetProperties()
    {
        initiator.attack.damageType = DamageType.Physical;
        initiator.attack.Asset = tempAsset;
        base.ResetTargetProperties();
    }
}
