using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Cysharp.Threading.Tasks;
using UnityEditor;
using UnityEngine.Events;
using UnityEngine.Playables;
using Object = UnityEngine.Object;


public class BaseCharacter 
{
    public int ID;
    //public string Name;
    //public Sprite icon;
    public BaseCharacter leftCharacter;
    public BaseCharacter rightCharacter;
    
    


    /// <summary>
    /// 多血条？
    /// </summary>
    private List<float> hpList;

    [Tooltip("原始数据")]
    protected CharacterStateData originalCharacterStateData;

    
    protected CharacterStateData characterStateData;

    /// <summary>
    /// 战斗中等同于基础值
    /// </summary>
    [Tooltip("基础值")]
    public CharacterStateData CharacterStateData
    {
        get
        {
            return characterStateData;
        }
    }
    [Tooltip("实时值")]
    protected CharacterStateData battleCharacterStateData;
    public CharacterStateData BattleCharacterStateData //战斗时状态
    {
        get { return battleCharacterStateData; }
    }
    public CharacterDataStruct CharacterDataStruct
    {
        get;
        private set;
    }
    
    
    
    public List<BaseBuff> buffs;
    private List<BaseBuff> stateBuffs;
    protected GameObject characterGO;
    public GameObject CharacterGO
    {
        get => characterGO;

    }
    
    protected Transform spawnPoint;
    public Transform SpawnPoint
    {
        get => spawnPoint;
    }
    
    protected BaseBattleUI ui;
    public Runner runner;
    private GameObject Lock;
    
    public CharAudio characterAudio;






    public BaseSkill attack
    {
        get;
        private set;
    }

    public BaseSkill ultimate
    {
        get;
        private set;
    }
    public List<BaseSkill> passiveSkills;
    
    //protected BaseSkill skill;
    protected AnimAndDamageController animAndDamageController;
    protected PlayableAsset spawnAsset;
    public PlayableAsset respawnAsset;

    public void Spawn(UniTaskCompletionSource source)
    {
        AnimAndDamageController.Spawn(spawnAsset,source);
    }

    public AnimAndDamageController AnimAndDamageController
    {
        get => animAndDamageController;
    }
    
    public BaseAI AI;
    
    //事件部分
    /// <summary>
    /// 创建者，目标
    /// </summary>
    public UnityAction myTurnStartAction;
    public UnityAction<BaseCharacter, BaseCharacter,BaseSkill> beforeAttackAction;
    public UnityAction<BaseCharacter, BaseCharacter, BaseSkill> afterAttackAction;
    public UnityAction<BaseCharacter, BaseCharacter, BaseSkill> myTurnEndAction;

    /// <summary>
    ///  自身，攻击者，攻击者释放的技能
    /// </summary>
    public UnityAction<BaseCharacter, BaseCharacter, BaseSkill> getHitAction;
    public UnityAction<BaseCharacter, BaseCharacter, BaseSkill> hpChangeAction;
    



    public virtual void PlaySkill(BaseSkill skill,BaseCharacter self,BaseCharacter target,UniTaskCompletionSource source)
    {
        if (skill)
        {
            //Debug.log("attack null?");
            animAndDamageController.PlaySkill(skill,this,target,source);
        }
        
    }





    public BaseCharacter(BaseCharacterState state,CharacterDataStruct dataStruct)
    {
        ID = dataStruct.id;
        CharacterDataStruct = dataStruct;
        //BaseCharacterState so = Object.Instantiate(Resources.Load<BaseCharacterState>($"Character/CharacterData/{CharacterDataStruct.name}_{ID}"));
        originalCharacterStateData = state.CharacterStateData;
        characterStateData = DeepCopy.DeepCopyByReflect(originalCharacterStateData);
        battleCharacterStateData = characterStateData.ToBattleState();
        hpList = state.hpList;
        AI = state.AI;
        //icon=Resources.Load<Sprite>($"UI/UIImage/CharacterIcon/{Name}_{ID}");

    }

    protected virtual void CreateBattleCharacter() //读取角色小人预制体,创建战斗用状态
    {
        buffs = new List<BaseBuff>();
        stateBuffs = new List<BaseBuff>();
        passiveSkills = new List<BaseSkill>();
        myTurnStartAction = new UnityAction(() => { });
        beforeAttackAction = new UnityAction<BaseCharacter, BaseCharacter,BaseSkill>((_,_,_)=>{});
        afterAttackAction = new UnityAction<BaseCharacter, BaseCharacter, BaseSkill>((_,_,_)=>{});
        getHitAction=new UnityAction<BaseCharacter, BaseCharacter, BaseSkill>((_,_,_)=>{});
        hpChangeAction=new UnityAction<BaseCharacter, BaseCharacter, BaseSkill>((_,_,_)=>{});
        myTurnEndAction = new UnityAction<BaseCharacter, BaseCharacter, BaseSkill>((_,_,_)=>{});
        characterAudio = new CharAudio(CharacterDataStruct.name, ID);
        attack = Object.Instantiate(Resources.Load<BaseSkill>($"SkillSO/{CharacterDataStruct.name}_{ID}/Attack"));
        BaseSkill u = Resources.Load<BaseSkill>($"SkillSO/{CharacterDataStruct.name}_{ID}/Ultimate");
        if (u)
        {
            ultimate = Object.Instantiate(u);
        }
        int index = 0;
        while (true)
        {
            BaseSkill pSkill = Resources.Load<BaseSkill>($"SkillSO/{CharacterDataStruct.name}_{ID}/Passive_{index}");
            if (pSkill)
            {
                BaseSkill p = Object.Instantiate(pSkill);
                passiveSkills.Add(p);
                LoadAsset(p,$"Timelines/BattleCharacter/{CharacterDataStruct.name}_{ID}/Passive_{index}");
                index += 1;
            }
            else
            {
                break;
            }
        }

        LoadAsset(attack, $"Timelines/BattleCharacter/{CharacterDataStruct.name}_{ID}/Attack");
        if (ultimate)
        {
            LoadAsset(ultimate, $"Timelines/BattleCharacter/{CharacterDataStruct.name}_{ID}/Ultimate");
        }
        characterGO = GameObject.Instantiate(Resources.Load<GameObject>($"Character/Prefab/Character_Battle/{CharacterDataStruct.name}_{ID}"));
        animAndDamageController = characterGO.GetComponent<AnimAndDamageController>();
       
        spawnAsset=GameObject.Instantiate(Resources.Load<PlayableAsset>($"Timelines/BattleCharacter/{CharacterDataStruct.name}_{ID}/Spawn"));
        respawnAsset = Resources.Load<PlayableAsset>($"Timelines/BattleCharacter/{CharacterDataStruct.name}_{ID}/Respawn");
        if (respawnAsset)
        {
            respawnAsset = GameObject.Instantiate(respawnAsset);
        }
        //characterState = CharacterState.Live;
    }

    public void BindUI(BaseBattleUI _ui)
    {
        ui = _ui;
    }

    public void UnBindUI()
    {
        ui = null;
    }

    public void BindRunner(Runner _runner)
    {
        runner = _runner;
    }

    public void UnBindRunner()
    {
        runner = null;
    }
    

    public void CreateAndSetSpawnPos(Transform point)
    {
        if (!battleCharacterStateData.isDead)
        {
            CreateBattleCharacter();
            characterGO.transform.SetParent(point);
            spawnPoint = point;
            characterGO.transform.localPosition = new Vector3(0, 0, 0);
        }
    }

    public void LoadFromJson()
    {
        
    }

    public void LinkLeft(BaseCharacter _left)
    {
        leftCharacter = _left;
    }

    public void LinkRight(BaseCharacter _right)
    {
        rightCharacter = _right;
    }
    public void Selected(bool isMain,State battleState)
    {

        if (battleState == State.Selecting)
        {
            if(isMain)
            {
                Lock = TargetGOPool.GetMainTargetGO();
            }
            else
            {
                Lock = TargetGOPool.GetSecTargetGO();
            }

            Lock.transform.position = AnimAndDamageController.magicPoint.transform.position+new Vector3(0,0,-0.1f);
        }

        if (ui)
        {
            ui.Selected();
        }

        if (runner.runnerUI)
        {
            runner.runnerUI.Select();
        }
    }

    public void UnSelected()
    {
        HideLock();
        if (ui)
        {
            ui.UnSelected();
        }

        if (runner!=null)
        {
            runner.runnerUI.UnSelect();
        }
    }

    public void HideLock()
    {
        TargetGOPool.AddToPool(Lock);
        Lock = null;
    }

    //如果skill为空，该伤害来源为buff
    public void GetDamage(BaseCharacter attacker,BaseSkill skill,float damage,bool final,DamageType damageType,bool critical)
    {

        float absDamage = Mathf.Abs(damage);
        if (absDamage > 1)
        {
            BattleUISystem.Instance.ShowDamageText(absDamage, AnimAndDamageController.magicPoint.transform,damageType,critical).Forget();
        }
        BattleCharacterStateData.HP -= damage;
        ui.UpdateHP();
        if (final)
        {
            hpChangeAction.Invoke(this,attacker,skill);
            //受击
            if (skill != null)
            {
                if (damage >= 0)
                {
                    getHitAction.Invoke(this, attacker, skill);
                }
            }

            if (hpList.Count != 0&&BattleCharacterStateData.HP <= 0)
            {
                battleCharacterStateData.isDead = false;
                BattleSystem.Instance.PriPriorityActionAdd(RespawnAction,this);
            }
            else if(hpList.Count == 0&&BattleCharacterStateData.HP <= 0 && battleCharacterStateData.isDead==false)
            {
                battleCharacterStateData.isDead = true;
                BattleCharacterUI battleCharacterUI=ui as BattleCharacterUI;
                if (battleCharacterUI)
                {
                    battleCharacterUI.UltimateBlock();
                }
                BattleSystem.Instance.PriPriorityActionAdd(BattleSystem.Instance.DeadAction,this);
            }
            
        }
        
    }

    public async UniTask RespawnAction(BaseCharacter _)
    {
        //isDead = false;
        BattleCharacterStateData.MaxHP = hpList[0];
        hpList.RemoveAt(0);
        UniTaskCompletionSource source = new UniTaskCompletionSource();
        AnimAndDamageController.RespawnAnim(respawnAsset, this, source);
        await source.Task;
    }

    public virtual void GetNp(float np)
    {
        BattleCharacterStateData.NP += np;
        ui.UpdateNP();
        if (CanDoAction())
        {
            NPCheck();
        }
    }

    public void LoadAsset(BaseSkill skill, string path)
    {
        
        PlayableAsset asset = Resources.Load<PlayableAsset>(path);
        if (asset)
        {
            skill.Asset = asset;
        }
        SkillText text = TextSystem.skillExcelLoader.GetTextStruct(skill.skillID);
        skill.skillDataStruct = new SkillDataStruct(ref text);
        skill.cost = text.cost;

    }

    public virtual void UltimateReady()
    {
        
    }
    
    public virtual void UltimateUse()
    {
        battleCharacterStateData.NP = 0;
        ui.UpdateNP();
        NPCheck();
    }

    public virtual void AddBuff(BaseBuff buff)
    {
        buffs.Add(buff);
        
        myTurnStartAction += buff.MyTurnStart;
        beforeAttackAction += buff.BeforeAttack;
        afterAttackAction += buff.AfterAttack;
        myTurnEndAction += buff.MyTurnEnd;
        BattleSystem.Instance.playerTurnEndAction += buff.PlayerTurnEnd;

        GameObject go=BuffIconGOPool.GetSpriteGO(buff.icon);
        if (go!=null)
        {
            buff.iconImage = go;
            ui.AddBuff(go);
        }

    }
    public BaseBuff  HasBuff(int  id)
    {
        foreach (var buff in buffs)
        {
            if (buff.buffID == id)
            {
                return buff;
            }
        }
        return null;
    }
    
    public virtual void AddBuffLayer(int id)
    {
        for (int i = 0; i < buffs.Count; i++)
        {
            if (buffs[i].buffID == id)
            {
                buffs[i].AddLayer();
                return;
            }
        }
    }

    public virtual void RemoveObsoleteBuff()
    {
        List<BaseBuff> needToRemove = new List<BaseBuff>();
        for (int i = 0; i < buffs.Count; i++)
        {
            if (buffs[i].IsFinish)
            {
                needToRemove.Add(buffs[i]);
            }
            
        }
        foreach (var buff in needToRemove)
        {
            RemoveBuff(buff);
        }
    }
    
    public virtual void RemoveAllBuff()
    {
        List<BaseBuff> needToRemove = new List<BaseBuff>();
        for (int i = 0; i < buffs.Count; i++)
        {

            needToRemove.Add(buffs[i]);
            
            
        }
        foreach (var buff in needToRemove)
        {
            RemoveBuff(buff);
        }
    }

    public void RemoveBuff(BaseBuff buff)
    {
        myTurnStartAction -= buff.MyTurnStart;
        beforeAttackAction -= buff.BeforeAttack;
        afterAttackAction -= buff.AfterAttack;
        myTurnEndAction -= buff.MyTurnEnd;
        BattleSystem.Instance.playerTurnEndAction -= buff.PlayerTurnEnd;
        
        if (buff.iconImage)
        {
            ui.RemoveBuff(buff.iconImage);
            BuffIconGOPool.AddToPool(buff.iconImage);
        }
        buffs.Remove(buff);
        buff.BuffRemove();
    }

    public void RemoveBuffByID(int id)
    {
        List<BaseBuff> needToRemove = new List<BaseBuff>();
        for (int i = 0; i < buffs.Count; i++)
        {
            if (buffs[i].buffID==id)
            {
                needToRemove.Add(buffs[i]);
            }
            
        }
        foreach (var buff in needToRemove)
        {
            RemoveBuff(buff);
        }
    }


    public void HideUI()
    {
        TargetGOPool.AddToPool(Lock);
        ui.gameObject.SetActive(false);
    }

    public virtual void NPCheck()
    {

        ui.NPBackground.color = isNPMax() ? Color.white : Color.black;

    }

    public void InitPassiveBuff()
    {

        if (battleCharacterStateData.isDead==false)
        {
            foreach (var passiveSkill in passiveSkills)
            {
                BaseSkill _passiveSkill = Object.Instantiate(passiveSkill);
                foreach (var passive in _passiveSkill.buffs)
                {
                    BaseBuff passiveBuff = Object.Instantiate(passive);
                    passiveBuff.AddBuffToTarget(this, this);
                }

            }
        }

    }

    public bool isNPMax()
    {
        return BattleCharacterStateData.NP >= BattleCharacterStateData.MaxNP - 0.1;
    }

    public void AddStateBuff(StateBuff stateBuff)
    {
        stateBuffs.Add(stateBuff);
        BattleCharacterUI battleCharacterUI=ui as BattleCharacterUI;
        if (battleCharacterUI)
        {
            battleCharacterUI.UltimateBlock();
        }
    }

    public void RemoveStateBuff(StateBuff stateBuff)
    {
        stateBuffs.Remove(stateBuff);
        if (stateBuffs.Count == 0&& battleCharacterStateData.isDead==false)
        {
            NPCheck();
        }
    }

    public bool CanDoAction()
    {
        return stateBuffs.Count == 0;
    }
    
    
    
    
    
    
    
    
    
}


