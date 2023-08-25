using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

[CreateAssetMenu(fileName = "SnowSurvivalLaw",menuName = "ScriptableObject/SnowSurvivalLaw")]
public class SnowSurvivalLaw : BaseBuff
{
    private PlayableAsset tempAsset;



    public override void ChangeTargetProperties()
    {
        tempAsset = initiator.attack.Asset;
        initiator.attack.Asset = Resources.Load<PlayableAsset>($"Timelines/BattleCharacter/{initiator.CharacterDataStruct.name}_{initiator.ID}/Attack_P");
        base.ChangeTargetProperties();
    }

    public override void ResetTargetProperties()
    {
        initiator.attack.Asset = tempAsset;
        base.ResetTargetProperties();
    }
}
