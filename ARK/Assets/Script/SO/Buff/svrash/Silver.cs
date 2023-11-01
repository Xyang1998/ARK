using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.Playables;
//[CreateAssetMenu(fileName = "Silver",menuName = "ScriptableObject/Silver")]
public class Silver : BaseBuff
{
    private BaseSkill silverAttack;
    public override void AddBuffToTarget(BaseCharacter _initiator, BaseCharacter _target)
    {
        base.AddBuffToTarget(_initiator, _target);
        if (!silverAttack)
        {
            silverAttack = Instantiate(Resources.Load<BaseSkill>($"SkillSO/{_initiator.CharacterDataStruct.name}_{_initiator.CharacterDataStruct.id}/Ultimate_P"));
            PlayableAsset asset = Resources.Load<PlayableAsset>($"Timelines/BattleCharacter/{_initiator.CharacterDataStruct.name}_{_initiator.CharacterDataStruct.id}/Ultimate");
            if (asset)
            {
                silverAttack.Asset = asset;
            }
        }
        BaseBuff exist = _target.HasBuff(buffID);
        if (exist)
        {
            exist.CurLayers = maxLayers;
            exist.UpdateLayer();
        }
        else
        {
            CurLayers = maxLayers;
            UpdateLayer();
        }
    }

    public override void PlayerTurnEnd(BaseCharacter _initiator, BaseCharacter _target, BaseSkill skill)
    {
        BattleSystem.Instance.PriorityActionAdd(SilverAttack,initiator);
    }

    private async UniTask SilverAttack(BaseCharacter _initiator)
    {
        if (initiator.CanDoAction())
        {
            BattleUISystem.Instance.AddPriRunner(initiator.CharacterDataStruct.icon);
            UniTaskCompletionSource source = new UniTaskCompletionSource();
            AudioSystem.Instance.PlayVoice(initiator.characterAudio.UTurnClip);
            BaseCharacter skillTarget = BattleSystem.Instance.GetDefaultEnemy();
            if (skillTarget != null)
            {
                initiator.PlaySkill(silverAttack, initiator, skillTarget, source);
            }
            await source.Task;
            BattleUISystem.Instance.RemovePriRunner();
        }

        CurLayers -= 1;
        UpdateLayer();
        if (CurLayers == 0)
        {
            initiator.RemoveBuffByID(buffID);
        }

    }
}
