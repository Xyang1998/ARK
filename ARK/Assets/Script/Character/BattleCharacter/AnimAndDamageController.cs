using System;
using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Spine;
using Spine.Unity;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;
using Random = UnityEngine.Random;
using AnimationState=Spine.AnimationState;
using Object = UnityEngine.Object;

public class AnimAndDamageController : MonoBehaviour
{
    public GameObject HPBar; //血条位置
    public GameObject meleePoint; //近战受击点
    public GameObject magicPoint; //远程受击点
    
    private BaseCharacter initiator;
    private BaseCharacter mainTarget;
    private BaseSkill usedSkill;
    private List<BaseCharacter> leftCharacters;
    private List<BaseCharacter> rightCharacters;
    private float[] attackDamages; //主目标，左目标1,2..，右目标1,2...]
    private float initiatorNP; //释放者获得的Np
    private float targetNP; //受伤者获得的Np
    private float[] NPHitsRate; //每段攻击获得NP的比例
    private float[] attackHitsRates;
    private float[] attackLeftAttackRates;
    private float[] attackRightAttackRates;

    [SpineAnimation] 
    public string idleName;
    
    private SkeletonAnimation skeletonAnimation;
    public AnimationState animationState;
    private PlayableDirector director;
    private UniTaskCompletionSource source;
    

    /// <summary>
    /// 技能的第几次OnAttack?
    /// </summary>
    private int attackHitIndex = 0;
    /// <summary>
    /// 第几个目标？
    /// </summary>
    private int attackIndex = 0;
    private List<BaseBuff> buffs;

    private DamageType type;
    private bool critical;
    private bool isDamage;


    private void Awake()
    {
        leftCharacters = new List<BaseCharacter>();
        rightCharacters = new List<BaseCharacter>();
        skeletonAnimation = GetComponent<SkeletonAnimation>();
        director = GetComponent<PlayableDirector>();
        animationState = skeletonAnimation.AnimationState;
        animationState.SetAnimation(0,idleName,true);
        animationState.Event += HandleOnAttack;
        director.stopped += TimelineEnd;
        director.played += TimelineBegin;
        buffs = new List<BaseBuff>();
        
    }

    private void DeadComplete(TrackEntry e)
    {
        if (e.Animation.Name.Equals("Die"))
        {
            Debug.Log("die");
            Destroy(gameObject);
            initiator.HideUI();
            source.TrySetResult();
            
        }
    }

    public void DeadAnim(BaseCharacter _initiator,UniTaskCompletionSource _source)
    {
        source = _source;
        initiator = _initiator;
        animationState.Complete += DeadComplete;
        animationState.SetAnimation(0,"Die",false);
        AudioSystem.Instance.PlayVoice(_initiator.characterAudio.DieClip);
    }
    
    public void RespawnAnim(PlayableAsset respawnAsset,BaseCharacter _initiator,UniTaskCompletionSource _source)
    {        
        
        source = _source;
        initiator = _initiator;
        director.playableAsset = respawnAsset;
        director.stopped += RespawnAnimComplete;
        PlayableBinding? pd = DirectorExtend.GetPlayableBinding(director, "Anim");
        if (pd!=null)
        {
            director.SetGenericBinding(pd.Value.sourceObject,skeletonAnimation);
        }
        director.Play();
        
    }

    public void RespawnAnimComplete(PlayableDirector _director)
    {
        director.stopped -= RespawnAnimComplete;
        initiator.GetDamage(initiator,null,-initiator.BattleCharacterStateData.MaxHP,false,DamageType.Healing,false);
    }
    

    private void TimelineBegin(PlayableDirector _director)
    {
      //  Debug.Log("timeline begin");
        //Debug.Log(_director.gameObject.name);
        _director.gameObject.GetComponent<MeshRenderer>().sortingLayerName = "Playing";


    }

    private void TimelineEnd(PlayableDirector _director)
    {
       // Debug.Log("timeline end");
        _director.gameObject.GetComponent<MeshRenderer>().sortingLayerName = "Default";
        if (director == _director)
        {
            Debug.Log("timeline结束");
            if (source!=null)
            {
                source.TrySetResult();
            }

            animationState.SetAnimation(0,idleName,true);
        }
    }

    public void HandleOnAttack(TrackEntry trackEntry, Spine.Event e)
    {
        GetType().GetMethod(e.Data.Name)?.Invoke(this, null);
    }


    

    public void InitSkill(BaseSkill skill) //拿到需要的数据，开辟存储对应位置伤害的数组。
    {
        

        usedSkill = skill;
        buffs.Clear();
        attackHitsRates = skill.hitsRate;
        attackDamages = new float[1 + skill.multipleTarget.LeftNum + skill.multipleTarget.RightNum];
        attackLeftAttackRates = skill.leftAttackRates;
        attackRightAttackRates = skill.rightAttackRates;
        isDamage = skill.damageType != DamageType.Buff;
        NPHitsRate = skill.NPHitsRate;
        foreach (var buff in skill.buffs)
        {
            buffs.Add(buff);
        }
    }

    public void Spawn(PlayableAsset spawnAsset,UniTaskCompletionSource _source)
    {
        source = _source;
        director.playableAsset = spawnAsset;
        PlayableBinding? pd = DirectorExtend.GetPlayableBinding(director, "Anim");
        if (pd!=null)
        {
            director.SetGenericBinding(pd.Value.sourceObject,skeletonAnimation);
        }
        director.Play();
    }





    public void PlaySkill(BaseSkill skill, BaseCharacter self, BaseCharacter target, UniTaskCompletionSource _source)
    {
        source = _source;
        leftCharacters.Clear();
        rightCharacters.Clear();
        InitSkill(skill);
        mainTarget = target;
        initiator = self;
        type = skill.damageType;
        BaseCharacter begin = target;
        if (skill)
        {
            //Debug.log("attack null");
        }

        for (int i = 0; i < skill.multipleTarget.LeftNum; i++)
        {
            if (begin.leftCharacter != null)
            {
                leftCharacters.Add(begin.leftCharacter);
                begin = begin.leftCharacter;
            }
            else
            {
                break;
            }
        }

        begin = target;
        for (int i = 0; i < skill.multipleTarget.RightNum; i++)
        {
            if (begin.rightCharacter != null)
            {
                rightCharacters.Add(begin.rightCharacter);
                begin = begin.rightCharacter;
            }
            else
            {
                break;
            }
        }

        if (skill.damageType == DamageType.Healing || skill.damageType == DamageType.Real)
        {
            critical = false;
        }
        else
        {
            if (self.BattleCharacterStateData.CritRate == 0)
            {
                critical = false;
            }
            else
            {
                float threshold = Random.value*100;
                critical = self.BattleCharacterStateData.CritRate >= threshold;

            }
        }

        if (isDamage)
        {
            //伤害计算
            int index = 0;
            if (target == null)
            {
                //Debug.log(target);
            }

            attackDamages[index] = DamageCal.CalDamage(DamageCal.GetValueByAttribute(initiator.BattleCharacterStateData,usedSkill.baseAttribute),self.BattleCharacterStateData, target.BattleCharacterStateData,
                skill.damageRate, skill.damageType, critical);
            index += 1;
            foreach (var character in leftCharacters)
            {
                attackDamages[index] = DamageCal.CalDamage(DamageCal.GetValueByAttribute(initiator.BattleCharacterStateData,usedSkill.baseAttribute),self.BattleCharacterStateData, character.BattleCharacterStateData,
                    skill.damageRate, skill.damageType, critical);
                index += 1;
            }

            foreach (var character in rightCharacters)
            {
                attackDamages[index] = DamageCal.CalDamage(DamageCal.GetValueByAttribute(initiator.BattleCharacterStateData,usedSkill.baseAttribute),self.BattleCharacterStateData, character.BattleCharacterStateData,
                    skill.damageRate, skill.damageType, critical);
                index += 1;
            }
        }

        //NP计算
        initiatorNP = DamageCal.CalNp(self.BattleCharacterStateData, skill,critical);

        //受伤NP
        targetNP = (self.CharacterDataStruct.characterCamp == skill.optionalType[0]) ? 0 : DamageCal.CalHitNp(target.BattleCharacterStateData, critical);
        targetNP = (skill.damageType == DamageType.Healing || skill.damageType == DamageType.Buff )? 0 : targetNP;



       // Debug.Log(self.BattleCharacterState.ATK);
        director.playableAsset = skill.Asset;
        if (target.AnimAndDamageController)
        {
            InitPlayableAsset(self.SpawnPoint.position, target.AnimAndDamageController.meleePoint.transform.position);
        }
        attackHitIndex = 0;
        director.Play();
        usedSkill.ManualApply(initiator,mainTarget);

    }
    
    public void OnAttack()
    {
        
        bool isFinalHit = attackHitsRates.Length==0||attackHitIndex==attackHitsRates.Length-1;
        bool isFirstHit = attackHitIndex == 0;
        attackIndex = 0; //[0,1+left+right)
        usedSkill.OnAttack(initiator,mainTarget,attackDamages[attackIndex] * attackHitsRates[attackHitIndex],critical,isFirstHit,isFinalHit,isDamage,
                initiatorNP * NPHitsRate[attackHitIndex],targetNP * NPHitsRate[attackHitIndex],true);
        attackIndex += 1;
        foreach (var leftCharacter in leftCharacters)
        {
                usedSkill.OnAttack(initiator,leftCharacter,attackDamages[attackIndex] * attackHitsRates[attackHitIndex],critical,isFirstHit,isFinalHit,isDamage,
                    0,targetNP * NPHitsRate[attackHitIndex],false);
                attackIndex += 1;
        }

        foreach (var rightCharacter in rightCharacters)
        {
                usedSkill.OnAttack(initiator,rightCharacter,attackDamages[attackIndex] * attackHitsRates[attackHitIndex],critical,isFirstHit,isFinalHit,isDamage,
                    0,targetNP * NPHitsRate[attackHitIndex],false);
                attackIndex += 1;
        }

        attackHitIndex += 1;
        
    }
    

    private void InitPlayableAsset(Vector3 startPos,Vector3 targetPos)
    {
        PlayableBinding? AnimPD = DirectorExtend.GetPlayableBinding(director, "Anim");
        if (AnimPD!=null)
        {
            director.SetGenericBinding(AnimPD.Value.sourceObject,skeletonAnimation);
        }
        PlayableBinding? MovePD = DirectorExtend.GetPlayableBinding(director, "Move");
        if (MovePD != null)
        {
            object source = MovePD.Value.sourceObject;
            PlayableTrack track = source as PlayableTrack;
            foreach (var clip in track.GetClips())
            {

                if (clip.displayName == "MoveBeforeAttack")
                {
                    Attack_PreMoveAsset asset=clip.asset as Attack_PreMoveAsset;
                    if (asset)
                    {
                        asset.target = targetPos;
                    }

                }

                if (clip.displayName == "MoveAfterAttack")
                {
                    Attack_PreMoveAsset asset=clip.asset as Attack_PreMoveAsset;
                    if (asset)
                    {
                  
                        asset.target = startPos;
                    }

                }

            }
        }
        
        /*PlayableBinding? VoicePD = DirectorExtend.GetPlayableBinding(director, "Voice");
        if (VoicePD != null)
        {
            object source = VoicePD.Value.sourceObject;
            if (source!=null)
            {
                AudioTrack track = source as AudioTrack;
                foreach (var clip in track.GetClips())
                {

                    if (clip.displayName == "CharVoice")
                    {
                        
                        AudioPlayableAsset asset = clip.asset as AudioPlayableAsset;
                        if (asset)
                        {
                            asset.clip = Instantiate(Resources.Load<AudioClip>("作战中3"));
                            clip.duration = asset.clip.length;
                        }

                    }


                }
            }
        }*/


    }


}
