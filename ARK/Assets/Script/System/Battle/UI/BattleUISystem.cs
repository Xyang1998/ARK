using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;
using Timer = System.Threading.Timer;


public enum ClickButtonType
{
    Cancel,
    Attack,
    Skill,
    
}
/// <summary>
/// 判断当前状态是否在选择技能（显示Lock）
/// </summary>
public enum State
{
    Selecting,
    Playing
}



public class ClickButtonManager //用于实现攻击和技能的按钮选择逻辑
{
    public ClickButtonType[] clickButtonLists = new ClickButtonType[2];
    public int pivot=0;

    public ClickButtonManager()
    {
        Reset();
    }

    public void Reset()
    {
        clickButtonLists[pivot] = ClickButtonType.Attack;
        pivot = pivot==0?1:0;
        clickButtonLists[pivot] = ClickButtonType.Skill;
    }

    public void ClickAttack()
    {
        clickButtonLists[pivot] = ClickButtonType.Attack;
        pivot = pivot==0?1:0;
    }
    
    public void ClickSkill()
    {
        clickButtonLists[pivot] = ClickButtonType.Skill;
        pivot = pivot==0?1:0;
    }

    public bool IsSame() //判断两次点击按钮是否是同一个
    {
        return clickButtonLists[0] == clickButtonLists[1];
    }

    public ClickButtonType SameClickType()
    {
        return clickButtonLists[0];
    }
}


public class BattleUISystem : SingletonMono<BattleUISystem>
{

    private BattleSystem battleSystem;
    public  Camera battleCamera;
    
    private bool showBattle = false;
    public Button detailButton;


    public RectTransform battleCharactersUI; //角色栏
    public RectTransform runnersQueue;
    public RectTransform[] runnersPoints=new RectTransform[20];
    public Button attackButton;
    public Button skillButton;
    public Button ultimateButton;
    public CostUI costUI;
    public Text skillCostNum;
    
    private GameObject runnerPrefab;
    private PriRunnerUI priRunnerUI;
    private GameObject battleCharacterUIPrefab;
    private GameObject battleEnemyUIPrefab;
    private List<BattleCharacterUI> battleCharacterUIs; //Player
    private List<BattleEnemyUI> battleEnemyUIs; //Enemy
    private UIActionQueueManager uiActionQueueManager;
    private float moveCostTime = NumericalDefinition.moveCostTime;

    private ClickButtonManager clickButtonManager;
    
    private CharacterCamp[] optionalCharacterTypes;
    private MultipleTarget multipleTarget;
    
    
    private CharacterCamp[] attackOptionalCharacterTypes;
    private MultipleTarget attackMultipleTarget;
    
    private CharacterCamp[] skillOptionalCharacterTypes;
    private MultipleTarget skillMultipleTarget;
    
    private CharacterCamp[] ultimateOptionalCharacterTypes;
    private MultipleTarget ultimateMultipleTarget;
    private static  State battleState = State.Playing;
    


    private float clickInterval = 0.1f; //点击选择目标的间隔
    private float endTime = 0; //可点击的时间点
    private bool isButtonEnable = true;
    private GameObject selectedTargetGO;


    public Color physicalColor;
    public Color magicColor;
    public Color realColor;
    public Color healColor;
    public float damageTextDurTime;
    public float damageTextSpeed;
    

    public RectTransform battleUI;
    public RectTransform detailui;
    
    
    

    public void SetCostNum(int n)
    {
        costUI.SetCostNum(n);
    }

    public async UniTaskVoid ShowDamageText(float _damage,Transform targetPos,DamageType damageType,bool critical)
    {
        int damage = (int)_damage;
        Vector3 pos = battleCamera.WorldToScreenPoint(targetPos.position);
        float offsetX = Random.Range(-15f, 15f);
        float offsetY = Random.Range(-15f, 15f);
        pos += new Vector3(offsetX, offsetY, 0);
        pos.z = 0;
        GameObject textGO = DamageTextPool.GetGO();
        textGO.transform.SetParent(battleUI.transform);
        textGO.transform.position = pos;
        Text text = textGO.GetComponent<Text>();
        text.text = damage.ToString();
        if (damageType == DamageType.Physical)
        {
            text.material.SetColor("_Color",physicalColor);
        }
        else if (damageType == DamageType.Magic)
        {
            text.material.SetColor("_Color",magicColor);
        }
        else if(damageType==DamageType.Real)
        {
            text.material.SetColor("_Color",realColor);
        }
        else
        {
            text.material.SetColor("_Color",healColor);
        }

        text.fontStyle =  critical?FontStyle.BoldAndItalic:FontStyle.Italic;
        text.material.SetFloat("_StartTime",Time.timeSinceLevelLoad);
        text.material.SetFloat("_EndTime",Time.timeSinceLevelLoad+damageTextDurTime);
        text.material.SetFloat("_Interval",damageTextDurTime);
        text.material.SetFloat("_Speed",damageTextSpeed);
        await UniTask.Delay((int)damageTextDurTime*1000);
        DamageTextPool.AddToPool(textGO);
    }
    
    private void Awake()
    {
       
        uiActionQueueManager = new UIActionQueueManager();
        battleCharacterUIs = new List<BattleCharacterUI>();
        battleEnemyUIs = new List<BattleEnemyUI>();
        battleSystem = FindObjectOfType<BattleSystem>().GetComponent<BattleSystem>();
        runnerPrefab = Resources.Load<GameObject>(FilePath.battleUIPrefabPath+"Runner");
        battleCharacterUIPrefab = Resources.Load<GameObject>(FilePath.battleUIPrefabPath+"CharacterUI");
        battleEnemyUIPrefab = Resources.Load<GameObject>(FilePath.battleUIPrefabPath+"EnemyUI");
        clickButtonManager = new ClickButtonManager();
        
        detailButton.onClick.AddListener(DetailClick);
    }

    public void Start()
    {
        DetailClick();
    }

    public void DetailClick()
    {
        CanvasGroup battleCanvas = battleUI.GetComponent<CanvasGroup>();
        CanvasGroup detailCanvas = detailui.GetComponent<CanvasGroup>();
        if (showBattle)
        {
            showBattle = false;
            battleCanvas.interactable = false;
            battleCanvas.blocksRaycasts = false;
            detailCanvas.alpha = 1;
            detailCanvas.interactable = true;
            detailCanvas.blocksRaycasts = true;
            battleSystem.UpdateDetail(detailui.GetComponent<DetailUI>());
        }
        else
        {
            showBattle = true;
            battleCanvas.interactable = true;
            battleCanvas.blocksRaycasts = true;
            detailCanvas.alpha = 0;
            detailCanvas.interactable = false;
            detailCanvas.blocksRaycasts = false;

        }
    }

    public void Update()
    {
        
        if (Input.GetMouseButtonDown(0))
        {
            if (battleState == State.Selecting)
            {
                if (Time.unscaledTime > endTime)
                {
                    SelectCharacter();

                }
            }

        }
    }



    public void SelectCharacter(GameObject defaultTarget=null)
    {
        if (defaultTarget)
        {
            if (CanSelect(defaultTarget))
            {
                //Debug.log(defaultTarget.name);
                //Debug.log("可以选择的目标");
                HighLightTarget(defaultTarget);
                selectedTargetGO = defaultTarget;
                endTime = Time.unscaledTime + clickInterval;
            }
        }
        else
        {
            Ray ray = battleCamera.ScreenPointToRay(Input.mousePosition);
            RaycastHit rayHit;
            if (Physics.Raycast(ray, out rayHit,100f,LayerMask.GetMask("ClickableCharacter")))
            {

                Debug.Log(rayHit.transform.name);
                if (CanSelect(rayHit.transform.gameObject))
                {
                    
                    //Debug.Log("可以选择的目标");
                    HighLightTarget(rayHit.transform.gameObject);
                    selectedTargetGO = rayHit.transform.gameObject;
                    endTime = Time.unscaledTime + clickInterval;
                }

            }
        }
       
       
    }

    public void ForceSelectCharacter(GameObject defaultTarget)
    {
        HighLightTarget(defaultTarget);
        selectedTargetGO = defaultTarget;
    }


    public void InitCharacterState(List<Character> characters)
    {
        foreach (var character in characters)
        {
            InitPlayerStateUI(character);
        }
        
    }
    public void InitEnemyState(List<Enemy> enemies)
    {
        foreach (var enemy in enemies)
        {
            InitEnemyStateUI(enemy,battleCamera);
            //InitEnemyStateUI(enemy,battleCamera).Forget();
        }
        
    }

    public void  InitPlayerStateUI(BaseCharacter character)
    {
        if (!character.CharacterStateData.isDead)
        {
            GameObject battleCharacterUIgo;
            battleCharacterUIgo = Instantiate(battleCharacterUIPrefab, battleCharactersUI.transform);
            BattleCharacterUI battleCharacterUI = battleCharacterUIgo.GetComponent<BattleCharacterUI>();
            character.BindUI(battleCharacterUI);
            
            battleCharacterUI.Init(character.BattleCharacterStateData,character.CharacterDataStruct);
            character.BattleCharacterStateData.OnHPChanged += battleCharacterUI.UpdateHP;
            character.BattleCharacterStateData.OnNPChanged += battleCharacterUI.UpdateNP;
            battleCharacterUI.BindUButton(character);
            character.NPCheck();
            battleCharacterUIs.Add(battleCharacterUI);
        }
    }
    public async void InitEnemyStateUI(BaseCharacter enemy,Camera battleCamera)
    {
        GameObject battleEnemyUIgo;
        battleEnemyUIgo = Instantiate(battleEnemyUIPrefab,battleUI.transform);
        BattleEnemyUI battleEnemyUI = battleEnemyUIgo.GetComponent<BattleEnemyUI>();
        enemy.BindUI(battleEnemyUI);
        battleEnemyUI.Init(enemy.BattleCharacterStateData,enemy.CharacterDataStruct);
        enemy.BattleCharacterStateData.OnHPChanged += battleEnemyUI.UpdateHP;
        enemy.BattleCharacterStateData.OnNPChanged += battleEnemyUI.UpdateNP;
        battleEnemyUI.BindBattleCamera(battleCamera,enemy.AnimAndDamageController.HPBar.transform);
        enemy.NPCheck();
        battleEnemyUIs.Add(battleEnemyUI);
    }

    public void InitQueueUI() //初始化拉条ui并将QueueUIStart加入动作队列并启动队列
    {
        for (int i = 0; i < runnersQueue.transform.childCount; i++)
        {
            RectTransform point = runnersQueue.transform.GetChild(i).GetComponent<RectTransform>();
            runnersPoints[i] = point;
        }
        
        uiActionQueueManager.AddAction(QueueUIStart(battleSystem.Runners));
        uiActionQueueManager.Play();
        RunnersMoveAddToQueue();
    }

    public void AddRunnersUI(List<Runner> runners)
    {
        uiActionQueueManager.AddAction(QueueUIStart(runners));
        uiActionQueueManager.Play();
        UpdateAfterRun();
    }

    public void UpdateAfterRun()
    {
        UpdateRemainTime();
        RunnersMoveAddToQueue();
       

    }

    public void UpdateRemainTime()
    {
        foreach (var runner in battleSystem.Runners)
        {
            runner.runnerUI.UpdateRemainTime(runner.RemainTime);
        }
    }

    public async UniTask QueueUIStart(List<Runner> runners) //第一次初始化拉条ui，初始化到屏幕外向目标移动，该函数需要加入动作队列
    {
        //List<Runner> runners = battleSystem.Runners;
        RectTransform spawnPoint = runnersPoints[12].GetComponent<RectTransform>();
        //Debug.log(spawnPoint.transform.position);
        //Debug.log(runnersPoints[0].transform.position);
        foreach (var runner in runners)
        {
            if (!runner.Character.BattleCharacterStateData.isDead)
            {
                GameObject go = Instantiate(runnerPrefab, runnersQueue.transform);
                go.GetComponent<RectTransform>().anchoredPosition = spawnPoint.anchoredPosition;
                RunnerUI ui = go.GetComponent<RunnerUI>();
                ui.Init(runner);
                runner.runnerUI = ui;
            }
        }
    }
    
    

    public async UniTask RunnerMove(RectTransform _transform,RectTransform target) //所有Runners移动到目标位置
    {

        float distance = Vector3.Distance(_transform.anchoredPosition, target.anchoredPosition);
        float speed = distance/moveCostTime; 
        float startTime = 0;
        
        if (distance > 1)
        {
            while (startTime < moveCostTime)
            {
                if (_transform)
                {
                    _transform.anchoredPosition = Vector3.MoveTowards(_transform.anchoredPosition, target.anchoredPosition, speed * Time.fixedDeltaTime);
                }
                startTime += Time.fixedDeltaTime;
                await UniTask.Yield(PlayerLoopTiming.FixedUpdate);
            }
        }

        if (_transform)
        {
            _transform.anchoredPosition = target.anchoredPosition;
        }

    }

    public void RunnersMoveAddToQueue() //runners移动unitask添加到队列并执行
    {

        var task = UniTask.Defer(QueueRunnersMove);
        uiActionQueueManager.AddAction(task);
        uiActionQueueManager.Play();
    }
    public async UniTask QueueRunnersMove() //所有runner移动到指定位置
    {
        List<Runner> runners = battleSystem.Runners;
        List<UniTask> uniTasks = new List<UniTask>();
        int hasPriRunner = 0;
        if (priRunnerUI)
        {
            hasPriRunner = 1;
            uniTasks.Add(RunnerMove(priRunnerUI.GetComponent<RectTransform>(), runnersPoints[0]));
        }
        for (int i = 0; i < runners.Count; i++)
        {
            if (runners[i].runnerUI)
            {
                uniTasks.Add(RunnerMove(runners[i].runnerUI.GetComponent<RectTransform>(), runnersPoints[i+hasPriRunner]));
            }
        }
        await UniTask.WhenAll(uniTasks);
        ////Debug.log("QueueRunnersMove finish");
    }

    public void AddPriRunner(Sprite icon)
    {
        priRunnerUI = Instantiate(Resources.Load<PriRunnerUI>(FilePath.battleUIPrefabPath+"PriRunner"),runnersQueue.transform);
        priRunnerUI.Init(icon);
        RectTransform rectTransform = priRunnerUI.GetComponent<RectTransform>();
        rectTransform.anchoredPosition = runnersPoints[0].anchoredPosition + new Vector2(0, runnersPoints[0].sizeDelta.y);
        RunnersMoveAddToQueue();


    }

    public void RemovePriRunner()
    {
        Destroy(priRunnerUI.gameObject);
        priRunnerUI = null;
        RunnersMoveAddToQueue();
    }

    

    
    

    public void HighLightTarget(GameObject CurTargetGO)
    {
        
        BaseCharacter CurTarget = battleSystem.GOToCharacter(CurTargetGO);
        if (CurTarget != null)
        {
            HideAll(true);
            CurTarget.Selected(true, battleState);
            if (battleState == State.Selecting){
                SecTarget(CurTarget); }
    
        }
    }

    public void SecTarget(BaseCharacter CurTarget)
    {
        BaseCharacter temp=CurTarget.leftCharacter;
        for (int i = 0; i < multipleTarget.LeftNum; i++)
        {
            if (temp!= null)
            {
                temp.Selected(false,battleState);
                temp = temp.leftCharacter;
            }
            else
            {
                break;
            }
        }
        temp=CurTarget.rightCharacter;
        for (int i = 0; i < multipleTarget.RightNum; i++)
        {
            if (temp!= null)
            {
                temp.Selected(false,battleState);
                temp = temp.rightCharacter;
            }
            else
            {
                break;
            }
        }
    }

    public async UniTask<(ClickButtonType,GameObject)> AttackOrSkill(Character initiator,CancellationToken cancellationToken)
    {
        CharacterCamp[] attackCamp = initiator.attack.optionalType;
        Sprite attackIcon = initiator.attack.skillDataStruct.icon;
        attackMultipleTarget = initiator.attack.multipleTarget;
        
        CharacterCamp[] skillCamp = initiator.skill.optionalType;
        Sprite skillIcon = initiator.skill.skillDataStruct.icon;
        skillCostNum.text = ((int)battleSystem.CostRate*initiator.skill.cost).ToString();
        if (initiator.skill.cost >battleSystem.CostRate* battleSystem.Cost|| initiator.skill.canUse==false)
        {
            skillButton.interactable=false;
        }
        else
        {
            skillButton.interactable = true;
        }
        skillMultipleTarget = initiator.skill.multipleTarget;
        ShowAandSButton();
        ButtonEnable();
        Debug.Log(attackIcon);
        if (attackIcon != null)
        {
            attackButton.image.sprite = attackIcon;
        }

        if (skillIcon != null)
        {
            skillButton.image.sprite = skillIcon;
        }

        ChooseAttack();
        attackOptionalCharacterTypes = attackCamp;
        skillOptionalCharacterTypes = skillCamp;
        battleState = State.Selecting;
        
        //Debug.log(attackCamp);

        BaseCharacter preSelected = battleSystem.GOToCharacter(selectedTargetGO);
        if (preSelected==null || preSelected.BattleCharacterStateData.isDead)
        {
            OptionalCharacterTypes(attackCamp,attackMultipleTarget);
            SelectCharacter(battleSystem.SetDefaultTarget(attackCamp[0]));
        }
        else if (optionalCharacterTypes==null)
        {
            OptionalCharacterTypes(attackCamp,attackMultipleTarget);
            SelectCharacter(battleSystem.SetDefaultTarget(attackCamp[0]));
        }
        else if(!optionalCharacterTypes.SequenceEqual(attackCamp))
        {
            OptionalCharacterTypes(attackCamp,attackMultipleTarget);
            SelectCharacter(battleSystem.SetDefaultTarget(attackCamp[0]));
        }
        else
        {
            OptionalCharacterTypes(attackCamp,attackMultipleTarget);
            SelectCharacter(selectedTargetGO);
        }
        
        while (!clickButtonManager.IsSame())
        {
            var (isCancel_,index)=await UniTask.WhenAny(attackButton.OnClickAsync(), skillButton.OnClickAsync()).AttachExternalCancellation(cancellationToken).SuppressCancellationThrow();
            Debug.Log(cancellationToken.IsCancellationRequested);
            if (!isCancel_)
            {
                if (index == 0)
                {
                    AttackButtonClick(initiator);
                }
                else
                {
                    SkillButtonClick(initiator);
                }
            }
            else
            {
                throw new OperationCanceledException();
            }
            
        }

        ButtonDisable();
        HideAll();
        battleState =  State.Playing;
        HiddenAllButton();
        ClickButtonType selectedType = clickButtonManager.SameClickType();
        clickButtonManager.Reset();
        return (selectedType, selectedTargetGO);

    }

    public async UniTask<GameObject> Ultimate(BaseCharacter initiator)
    {
        ShowUButton();
        ButtonEnable();
        battleState = State.Selecting;
        ultimateButton.image.sprite = initiator.ultimate.skillDataStruct.icon;
        CharacterCamp[] UltimateCamp = initiator.ultimate.optionalType;
        MultipleTarget _multipleTarget = initiator.ultimate.multipleTarget;
        OptionalCharacterTypes(UltimateCamp,_multipleTarget);
        Debug.Log("显示");
        if (UltimateCamp[0] == CharacterCamp.Self)
        {
            ForceSelectCharacter(initiator.CharacterGO);
        }
        SelectCharacter(battleSystem.SetDefaultTarget(UltimateCamp[0]));
        await ultimateButton.OnClickAsync();
        HiddenAllButton();
        ButtonDisable();
        HideAll();
        battleState = State.Playing;
        clickButtonManager.Reset();
        return selectedTargetGO;
    }

    public void AttackButtonClick(BaseCharacter initiator)
    {
        if (!isButtonEnable) return;
        clickButtonManager.ClickAttack();
        ChooseAttack();
        HideAll(true);
        if (!clickButtonManager.IsSame())
        {

            if (attackOptionalCharacterTypes[0] == CharacterCamp.Self)
            {
                OptionalCharacterTypes(attackOptionalCharacterTypes,attackMultipleTarget);
                ForceSelectCharacter(initiator.CharacterGO);
                
            }
            else
            {
                if (!optionalCharacterTypes.SequenceEqual(attackOptionalCharacterTypes) && selectedTargetGO)
                {
                    OptionalCharacterTypes(attackOptionalCharacterTypes,attackMultipleTarget);
                    SelectCharacter(battleSystem.SetDefaultTarget(attackOptionalCharacterTypes[0]));
                }

                OptionalCharacterTypes(attackOptionalCharacterTypes,attackMultipleTarget);
                SelectCharacter(selectedTargetGO);
                
            }


        }
    }
    public void SkillButtonClick(BaseCharacter initiator)
    {
        if (!isButtonEnable) return;
        clickButtonManager.ClickSkill();
        ChooseSkill();
        HideAll(true);
        if (!clickButtonManager.IsSame())
        {
            if (skillOptionalCharacterTypes[0] == CharacterCamp.Self)
            {
                OptionalCharacterTypes(skillOptionalCharacterTypes,skillMultipleTarget);
                ForceSelectCharacter(initiator.CharacterGO);
                
            }

            else
            {

                if (!optionalCharacterTypes.SequenceEqual(skillOptionalCharacterTypes) && selectedTargetGO)
                {
                    OptionalCharacterTypes(skillOptionalCharacterTypes,skillMultipleTarget);
                    SelectCharacter(battleSystem.SetDefaultTarget(skillOptionalCharacterTypes[0]));
                }
                OptionalCharacterTypes(skillOptionalCharacterTypes,skillMultipleTarget);
                SelectCharacter(selectedTargetGO);
            }
            
        }
    }

    public bool CanSelect(GameObject go)
    {
        if (optionalCharacterTypes != null)
        {
            foreach (var type in optionalCharacterTypes)
            {
                if (go.CompareTag(type.ToString()))
                {
                    return true;
                }
            }
        }

        return false;
    }
    public void OptionalCharacterTypes(CharacterCamp[] _characterCamps,MultipleTarget _multipleTarget) //当前行动可以选择的目标
    {
        optionalCharacterTypes = _characterCamps;
        multipleTarget = _multipleTarget;
    }

    public void ButtonEnable()
    {
        isButtonEnable = true;
    }
    public void ButtonDisable()
    {
        isButtonEnable = false;
    }

    public void UltimateButtonClick()
    {
        if (!isButtonEnable) return;
    }

    public void ShowAandSButton()
    {
        attackButton.gameObject.SetActive(true);
        skillButton.gameObject.SetActive(true);
        ultimateButton.gameObject.SetActive(false);
    }
    public void ShowUButton()
    {
        //Debug.log("ShowUButton");
        attackButton.gameObject.SetActive(false);
        skillButton.gameObject.SetActive(false);
        ultimateButton.gameObject.SetActive(true);
    }
    public void HiddenAllButton()
    {
        //Debug.log("HiddenAllButton");
        attackButton.gameObject.SetActive(false);
        skillButton.gameObject.SetActive(false);
        ultimateButton.gameObject.SetActive(false);
    }

    private void ChooseAttack()
    {
        attackButton.GetComponent<Outline>().enabled = true;
        skillButton.GetComponent<Outline>().enabled = false;
    }

    private void ChooseSkill()
    {
        attackButton.GetComponent<Outline>().enabled = false;
        skillButton.GetComponent<Outline>().enabled = true;
    }

    /// <summary>
    /// 隐藏所有lock
    /// </summary>
    private void HideAll(bool unSelect=false)
    {

        battleSystem.HideAllLock(unSelect);
    }
    
    
    
    





}
