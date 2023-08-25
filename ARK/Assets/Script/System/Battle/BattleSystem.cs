using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;
using Random = UnityEngine.Random;


public enum TrunType //当前状态
{
    Playing, //播放动画中(攻击等)
    Selecting //玩家选择攻击，技能以及目标状态
}






public class BattleSystem : SingletonMono<BattleSystem>
{
    
    private TeamState teamState;
    public  List<Character> characters;
    public  List<Enemy> enemies;
    public  List<Character_OnMap> reserveEnemies;
    private BattleUISystem battleUISystem;
    public GameObject playerSpawnPointParent;
    private Transform[] playerSpawnPoints = new Transform[4];
    public GameObject enemySpawnPointParent;
    private Transform[] enemySpawnPoints = new Transform[4];

    private List<Runner> runners;
    private Dictionary<GameObject, BaseCharacter> GOToCharacterDict;
    private GameObject selectedTargetGO; //选择的目标
    private CharacterCamp[] defualtCamps = new CharacterCamp[] { CharacterCamp.Enemy };//默认可选择敌人（在播放动画时等等。。）
    
    private Queue<Func<BaseCharacter,CancellationToken,UniTask>> actionQueue;
    private Queue<BaseCharacter> actionInitiatorQueue;
    private Queue<Func<BaseCharacter,UniTask>> priorityActionQueue;
    private Queue<BaseCharacter> priorityActionInitiatorQueue;
    private Queue<Func<BaseCharacter, UniTask>> priPriorityActionQueue;
    private Queue<BaseCharacter> priPriorityActionInitiatorQueue;
    private CancellationTokenSource cancellationTokenSource; //用于选择时取消释放大招

    private AudioSystem audioSystem;
    public List<Runner> Runners
    {
        get => runners;
    }
    
    
    private int totalTrun = 0;
    private int cost = 0;

    public int Cost
    {
        get => cost;
        set
        {
            cost = value < 0 ? 0 : value;
            if (battleUISystem)
            {
                battleUISystem.SetCostNum(cost);
            }
        }
    }
   
    //事件部分
    /// <summary>
    /// 生成角色时调用，用于全局buff,参数为目标
    /// </summary>
    public UnityAction<BaseCharacter> spawnAction;
    /// <summary>
    /// 角色死亡时调用，用于全局buff,参数为目标
    /// </summary>
    public UnityAction<BaseCharacter> deadAction;
    public UnityAction<BaseCharacter,BaseCharacter,BaseSkill> playerTurnEndAction;
    






    public void UpdateDetail(DetailUI ui)
    {
        ui.UpdateView(characters,enemies);
    }






    public void HideAllLock(bool uiUnSelect=false)
    {
        foreach (var character in characters)
        {
            if(uiUnSelect) {character.UnSelected();}
            else{character.HideLock();}
        }

        foreach (var enemy in enemies)
        {
            if (enemy != null)
            {
                if(uiUnSelect) {enemy.UnSelected();}
                else
                {
                    enemy.HideLock();
                }
            }
        }
    }
    
    public BaseCharacter GOToCharacter(GameObject go)
    {
        if (go)
        {
            BaseCharacter baseCharacter = GOToCharacterDict[go];
            if (baseCharacter != null)
            {
                return baseCharacter;
            }
        }

        return null;
    }





    public void Awake()
    {
        actionQueue = new Queue<Func<BaseCharacter,CancellationToken,UniTask>>();
        actionInitiatorQueue = new Queue<BaseCharacter>();
        
        priorityActionQueue = new Queue<Func<BaseCharacter,UniTask>>();
        priorityActionInitiatorQueue = new Queue<BaseCharacter>();

        priPriorityActionQueue = new Queue<Func<BaseCharacter, UniTask>>();
        priPriorityActionInitiatorQueue = new Queue<BaseCharacter>();
        teamState = FindObjectOfType<TeamState>().GetComponent<TeamState>();
        battleUISystem = FindObjectOfType<BattleUISystem>().GetComponent<BattleUISystem>();
        characters = CreateCharactersFromOnMaps(teamState.characterOnMaps);
        enemies = CreateEnemiesFromOnMaps(teamState.enemiesOnMaps);
        reserveEnemies = teamState.reserveEnemies;
        audioSystem = AudioSystem.Instance;
        cancellationTokenSource = new CancellationTokenSource();
        GOToCharacterDict = new Dictionary<GameObject, BaseCharacter>();
        runners = new List<Runner>();
        
        
        spawnAction = new UnityAction<BaseCharacter>(_=>{});
        deadAction = new UnityAction<BaseCharacter>(_ => { });
        playerTurnEndAction = new UnityAction<BaseCharacter, BaseCharacter, BaseSkill>((_, _, _) => { });

    }

    private List<Character> CreateCharactersFromOnMaps(List<Character_OnMap> list)
    {
        List<Character> baseCharacters = new List<Character>();
        foreach (var c in list)
        {

            Character ch = new Character(c.BaseCharacterState, c.CharacterDataStruct);
            baseCharacters.Add(ch);
        }

        return baseCharacters;
    }
    private List<Enemy> CreateEnemiesFromOnMaps(List<Character_OnMap> list)
    {
        List<Enemy> baseCharacters = new List<Enemy>();
        foreach (var c in list)
        {

            Enemy ch = new Enemy(c.BaseCharacterState, c.CharacterDataStruct);
            baseCharacters.Add(ch);
        }
        return baseCharacters;
    }




    private void InitGOToCharacterDict()
    {
        foreach (var character in characters)
        {
            if (character.BattleCharacterStateData.isDead == false)
            {
                GOToCharacterDict.Add(character.CharacterGO, character);
            }
        }
        foreach (var enemy in enemies)
        {
            GOToCharacterDict.Add(enemy.CharacterGO,enemy);
        }
    }

    private  void Start()
    {
        
        
        ToBattleCharacter();
        LinkLeftAndRight();
        GameStart().Forget();
    }

    private async UniTaskVoid GameStart()
    {
        await GlobalFlowManager().AttachExternalCancellation(this.GetCancellationTokenOnDestroy()).SuppressCancellationThrow();
    }
    
    private  void ToBattleCharacter() //大地图basecharacter生成战斗小人
    {
        //TODO:如何做到全部读取完成后开始战斗？
        //UniTaskCompletionSource source = new UniTaskCompletionSource();
        //teamState.InitBattle(source).Forget();
        //await source.Task;
        GetPointsAndSpawn(); //设置小人位置
        PriPriorityActionAdd(SpawnToPlay, null);
        InitGOToCharacterDict();
        battleUISystem.InitCharacterState(characters); //初始化ui
        battleUISystem.InitEnemyState(enemies); //初始化ui
        
    }

    private void GetPointsAndSpawn()
    {
        int playerPointNum = playerSpawnPointParent.transform.childCount;
        int enemyPointNum = enemySpawnPointParent.transform.childCount;
        for (int i = 0; i < playerPointNum; i++)
        {
            playerSpawnPoints[i] = playerSpawnPointParent.transform.GetChild(i);
            if (i < characters.Count)
            {
                characters[i].CreateAndSetSpawnPos(playerSpawnPoints[i]);
            }

        }
        for (int i = 0; i < enemyPointNum; i++)
        {
            enemySpawnPoints[i] = enemySpawnPointParent.transform.GetChild(i);
            if (i < enemies.Count)
            {
                
                enemies[i].CreateAndSetSpawnPos(enemySpawnPoints[i]);
            }

        }

    }

    private void InitRunners() //初始话runners
    {
        //Runner baseRunner=
        foreach (var character in characters)
        {
            if (!character.BattleCharacterStateData.isDead)
            {
                Runner runner = new Runner(character);
                runners.Add(runner);
            }
        }

        foreach (var enemy in enemies)
        {
            Runner runner = new Runner(enemy);
            runners.Add(runner);
        }
    }

    private void Run() //找到最快抵达，所有runner移动fastesttime,排序runners
    {
        runners.Sort(); //runners排序但不前进
        float fastestTime = runners[0].RemainTime;
        //找到了最快抵达的runner
        foreach (var runner in runners)
        {
            runner.Move(fastestTime);
        }
        runners[0].ToEnd();

    }

    public void FirstFinishRun()
    {
        runners[0].FinishRun();
    }

    private async UniTask GlobalFlowManager()
    {
        
        InitRunners();
        InitBuff();
        Cost = NumericalDefinition.startCost;
        //TODO:回合开始时执行什么？（道具，加成等等）
        Run();
        audioSystem.PlayBGM(teamState.bgm);
        Runner first=runners[0];//最快抵达
        battleUISystem.InitQueueUI();
        battleUISystem.HiddenAllButton();
        await UniTask.Delay(1000);
        while (!GameOver())  //胜利或失败条件
        {
            actionQueue.Enqueue(OneTurn);
            actionInitiatorQueue.Enqueue(first.Character);
            //Debug.log(actionQueue.Count);
            while (actionQueue.Count!=0 || priorityActionQueue.Count!=0|| priPriorityActionQueue.Count!=0)
            {
                await UniTask.Yield(this.GetCancellationTokenOnDestroy());
                first=runners[0];
                cancellationTokenSource.Dispose();
                cancellationTokenSource = new CancellationTokenSource();
                while (priPriorityActionQueue.Count!=0)
                {

                    var action = priPriorityActionQueue.Dequeue();
                    var initiator = priPriorityActionInitiatorQueue.Dequeue();
                    if (initiator != null)
                    {
                        await action(initiator);
                    }
                    
                }
                
                while(priPriorityActionQueue.Count==0&&priorityActionQueue.Count!=0)
                {

                    if (GameOver()&& priPriorityActionQueue.Count==0)
                    {
                        Debug.Log("游戏结束");
                        break;
                    }
                    var action = priorityActionQueue.Dequeue();
                    var initiator = priorityActionInitiatorQueue.Dequeue();
                    if (initiator == null || initiator.BattleCharacterStateData.isDead == false)
                    {
                        await action(initiator);
                    }

                    

                }

                if (priPriorityActionQueue.Count==0&&actionQueue.Count!=0)
                {
                    //await UniTask.Delay(5000);
                    var action = actionQueue.Dequeue();
                    var initiator = actionInitiatorQueue.Dequeue();
                    if (initiator == first.Character &&initiator.BattleCharacterStateData.isDead==false)
                    {
                        await action(first.Character, cancellationTokenSource.Token).SuppressCancellationThrow();
                    }
                    //Debug.log("action invoke finish");
                }
                if (GameOver()&& priPriorityActionQueue.Count==0)
                {
                    Debug.Log("游戏结束");
                    break;
                }
                
                
            }
            
            if (GameOver())
            {
                Debug.Log("游戏结束");
                BattleToWorld();
            }
            
            FirstFinishRun();
            Run();         //回合结束，runners移动
            first = runners[0];
            //Debug.log($"first:{first.Character.Name}");
            battleUISystem.UpdateAfterRun();
            await UniTask.Delay(1000,cancellationToken:this.GetCancellationTokenOnDestroy());
            
        }
        

    }

    public void BattleToWorld()
    {
        foreach (var character in characters)
        {
            character.BattleToWorld();
        }
        SystemMediator.Instance.mySceneManager.BattleToWorld();
    }

    public GameObject SetDefaultTarget(CharacterCamp camp)
    {
        if (camp == CharacterCamp.Player)
        {
            foreach (var character in characters)
            {
                if (character .BattleCharacterStateData.isDead==false)
                {
                    return character.CharacterGO;
                }
            }
            
        }

        else
        {
            foreach (var character in enemies)
            {
                if (character != null)
                {
                    return character.CharacterGO;
                }
            }
        }

        return null;
    }

    public BaseCharacter GetDefaultEnemy()
    {
        foreach (var character in enemies)
        {
            if (character != null)
            {
                return character;
            }
        }

        return null;
    }

    private void InitBuff() //实现加成等等。。
    {
        foreach (var character in characters)
        {
            character.InitPassiveBuff();
        }

        foreach (var enemy in enemies)
        {
            enemy.InitPassiveBuff();
        }

        foreach (var character in characters)
        {
            spawnAction.Invoke(character);
        }
        foreach (var enemy in enemies)
        {
            spawnAction.Invoke(enemy);
        }
        

    }

    private async UniTask  OneTurn(BaseCharacter first,CancellationToken cancellationToken)
    {
        //Debug.log("OneTurn Invoke");
        UniTaskCompletionSource source = new UniTaskCompletionSource();
        bool iscancel = false;
        SelectResult result;
        result.target = null;
        result.skill = null;
        result.turnEnd = false;
        first.myTurnStartAction?.Invoke();
        if (first.CharacterDataStruct.characterCamp == CharacterCamp.Player)
        {
            Character character=first as Character;
            while (character.BattleCharacterStateData.isDead == false&& character.CanDoAction()&&!iscancel&&!result.turnEnd)
            {
                audioSystem.PlayVoice(character.characterAudio.MyTurnClip);
                var (isCancel, (clickType, selectedGO)) = await battleUISystem
                    .AttackOrSkill(character,cancellationToken).
                    SuppressCancellationThrow();
                iscancel = isCancel;
                if (!iscancel)
                {
                    result.target = GOToCharacterDict[selectedGO];
                    if (clickType == ClickButtonType.Attack)
                    {
                        audioSystem.PlayVoice(character.characterAudio.AttackClip);
                        result.skill = character.attack;
                        result.turnEnd = character.attack.turnEnd;
                    }
                    else if (clickType == ClickButtonType.Skill)
                    {
                        audioSystem.PlayVoice(character.characterAudio.SkillClip);
                        result.skill = character.skill;
                        result.turnEnd = character.skill.turnEnd;
                    }
                    
                    first.PlaySkill(result.skill, first, result.target, source);
                    if (result.turnEnd)
                    {
                        first.beforeAttackAction?.Invoke(first, result.target, result.skill);
                    }
                    else
                    {
                        source = new UniTaskCompletionSource();
                    }
                    if (result.skill)
                    {
                        Cost -= result.skill.cost;
                    }
                }
                else
                {
                    actionQueue.Enqueue(OneTurn);
                    actionInitiatorQueue.Enqueue(first);
                    source.TrySetResult();
                }
            }
        }
        else //敌方
        {
            Enemy enemy=first as Enemy;
            for (int i = 0; i < enemy.BattleCharacterStateData.ActionNum;i++)
            {
                source = new UniTaskCompletionSource();
                if (enemy.BattleCharacterStateData.isDead == false && enemy.CanDoAction())
                {
                    result = first.AI.SelectTarget(enemy, characters, enemies);
                    if (result.skill == null)
                    {
                        result.skill = enemy.attack;

                    }
                    if (result.target != null)
                    {
                        enemy.beforeAttackAction(first, result.target, result.skill);
                        enemy.PlaySkill(result.skill, enemy, result.target, source);
                    }
                    else
                    {
                        source.TrySetResult();
                    }

                }
                else
                {
                    source.TrySetResult();
                }
                await source.Task;
                if (GameOver())
                {
                    break;
                }
            }
            

        }

        if (first.BattleCharacterStateData.isDead|| !first.CanDoAction())
        {
            source.TrySetResult();
        }
        await source.Task;
        if (!iscancel&&!first.BattleCharacterStateData.isDead)
        {
            first.myTurnEndAction?.Invoke(first,result.target,result.skill);
            //first.afterAttackAction?.Invoke(first,result.target,result.skill);
            first.RemoveObsoleteBuff();
        }

        if (!iscancel && first.CharacterDataStruct.characterCamp == CharacterCamp.Player)
        {
            playerTurnEndAction?.Invoke(first,result.target,result.skill);
        }
        
        
    }


    public void PriorityActionAdd(Func<BaseCharacter,UniTask> priorityAction,BaseCharacter initiator)
    {
        priorityActionQueue.Enqueue(priorityAction);
        priorityActionInitiatorQueue.Enqueue(initiator);
        if (cancellationTokenSource.Token!=CancellationToken.None)
        {
            cancellationTokenSource.Cancel();
            cancellationTokenSource.Dispose();
        }

        cancellationTokenSource = new CancellationTokenSource();

        
    }

    public void PriPriorityActionAdd(Func<BaseCharacter, UniTask> priorityAction, BaseCharacter initiator)
    {
        priPriorityActionQueue.Enqueue(priorityAction);
        priPriorityActionInitiatorQueue.Enqueue(initiator);
    }

    public async UniTask PlayerUltimateUse(BaseCharacter initiator)
    {
        if (initiator.CanDoAction())
        {
            battleUISystem.AddPriRunner(initiator.CharacterDataStruct.icon);
            initiator.UltimateUse();
            UniTaskCompletionSource source = new UniTaskCompletionSource();
            audioSystem.PlayVoice(initiator.characterAudio.UTurnClip);
            var selectedGO = await battleUISystem.Ultimate(initiator);
            initiator.PlaySkill(initiator.ultimate, initiator, GOToCharacterDict[selectedGO], source);
            Cost += initiator.ultimate.cost;
            await source.Task;
            battleUISystem.RemovePriRunner();
        }
        
    }

    public List<Enemy> AddEnemies(List<Enemy> _enemies)
    {
        List<Enemy> SpawnEnemies = new List<Enemy>();
        List<Runner> spawnRunners = new List<Runner>();
        foreach (var enemy in _enemies)
        {
            //enemy.CreateBattleCharacter();
            if (SpawnEnemy(enemy))
            {

                GOToCharacterDict.Add(enemy.CharacterGO, enemy);
                SpawnEnemies.Add(enemy);
                Runner runner = new Runner(enemy);
                runners.Add(runner);
                spawnRunners.Add(runner);
            }

        }
        battleUISystem.InitEnemyState(SpawnEnemies);
        runners.Sort();
        LinkLeftAndRight();
        battleUISystem.AddRunnersUI(spawnRunners);
        foreach (var enemy in SpawnEnemies)
        {
            spawnAction.Invoke(enemy);
        }
        return SpawnEnemies;

    }


    public bool SpawnEnemy(Enemy _enemy)
    {
        for (int i = 0; i < enemies.Count; i++)
        {
            if (enemies[i] == null)
            {
                _enemy.CreateAndSetSpawnPos(enemySpawnPoints[i]);
                enemies[i] = _enemy;
                return true;
            }
        }

        if (enemies.Count < NumericalDefinition.maxEnemies)
        {
            enemies.Add(_enemy);
            _enemy.CreateAndSetSpawnPos(enemySpawnPoints[enemies.Count-1]);
            return true;
        }

        return false;
    }
    
    public void LinkLeftAndRight() //连接每个character的左和右，用于群攻
    {
        BaseCharacter temp=null;
        for (int i = 0; i < characters.Count; i++)
        {
            if (!characters[i].BattleCharacterStateData.isDead)
            {
                for (int j = i-1; j >=0; j--)
                {
                    if (!characters[j].BattleCharacterStateData.isDead)
                    {
                        temp = characters[j];
                        break;
                    }
                }
                characters[i].LinkLeft(i - 1 < 0 ? null : temp);
                temp=null;
                for (int j = i+1; j <characters.Count; j++)
                {
                    if (!characters[j].BattleCharacterStateData.isDead)
                    {
                        temp = characters[j];
                        break;
                    }
                }
                characters[i].LinkRight(i + 1 >= characters.Count ? null : temp);
            }
        }
        temp=null;
        for (int i = 0; i < enemies.Count; i++)
        {
            if (enemies[i] != null)
            {
                for (int j = i-1; j >=0; j--)
                {
                    if (enemies[j] != null)
                    {
                        temp = enemies[j];
                        break;
                    }
                }
                enemies[i].LinkLeft(i-1<0?null:temp);
                temp=null;
                for (int j = i+1; j <enemies.Count; j++)
                {
                    if (enemies[j] != null)
                    {
                        temp = enemies[j];
                        break;
                    }
                }
                enemies[i].LinkRight(i+1>=enemies.Count?null:temp);
            }
        }
    }

    private async UniTask Spawn(List<Character> spawns)
    {
        List<UniTask> sourcesList = new List<UniTask>();
        foreach (var spawn in spawns)
        {
            if (!spawn.BattleCharacterStateData.isDead)
            {
                UniTaskCompletionSource source = new UniTaskCompletionSource();
                spawn.Spawn(source);
                sourcesList.Add(source.Task);
            }
        }

        await UniTask.WhenAll(sourcesList);
    }
    public async UniTask Spawn(List<Enemy> spawns)
    {
        List<UniTask> sourcesList = new List<UniTask>();
        foreach (var spawn in spawns)
        {
            UniTaskCompletionSource source = new UniTaskCompletionSource();
            spawn.Spawn(source);
            sourcesList.Add(source.Task);
        }

        await UniTask.WhenAll(sourcesList);
    }

    private async UniTask SpawnCharacter()
    {
        await Spawn(characters);
    }

    private async UniTask SpawnEnemies()
    {
        await Spawn(enemies);
    }

    private async UniTask SpawnToPlay(BaseCharacter _)
    {
        var spawnC = SpawnCharacter();
        var spawnE = SpawnEnemies();
        await UniTask.WhenAll(spawnC, spawnE);
    }


    public void AddEnemiesFromReserve(BaseCharacter _)
    {
        PriPriorityActionAdd(AddEnemiesFromReserveUniTask,_);
    }
    public async UniTask AddEnemiesFromReserveUniTask(BaseCharacter _)
    {
        
        
        
        List<Enemy> tempEnemies = new List<Enemy>();
        if (reserveEnemies.Count != 0)
        {
            tempEnemies.Add(new Enemy(reserveEnemies[0].BaseCharacterState,reserveEnemies[0].CharacterDataStruct));
            reserveEnemies.RemoveAt(0);
        }
        List<Enemy> spawnEnemies= AddEnemies(tempEnemies);
        Debug.Log("AddEnemies");
        await Spawn(spawnEnemies);
    }

    public async  UniTask DeadAction(BaseCharacter character)
    {
        
        audioSystem.PlayVoice(character.characterAudio.DieClip);
        character.RemoveAllBuff();
        int index = -1;
        if (character.CharacterDataStruct.characterCamp == CharacterCamp.Player)
        {
            index=characters.IndexOf(character as Character);
            //characters[index] = null;
        }
        else
        {
            index=enemies.IndexOf(character as Enemy);
            enemies[index] = null;
        }
        
        LinkLeftAndRight();
        runners.Remove(character.runner);
        runners.Sort();
        character.runner.runnerUI.UnSelect();
        Destroy(character.runner.runnerUI.gameObject);
        character.UnBindRunner();
        battleUISystem.UpdateAfterRun();
        UniTaskCompletionSource source = new UniTaskCompletionSource();
        if (character.CharacterDataStruct.characterCamp == CharacterCamp.Enemy)
        {
            AddEnemiesFromReserve(character);
        }
        character.AnimAndDamageController.DeadAnim(character,source);
        await source.Task;
        
        
    }




    private bool GameOver()
    {
        int num = 0;
        foreach (var character in characters)
        {
            if (character.BattleCharacterStateData.isDead==false)
            {
                num += 1;
            }
        }

        if (num == 0) return true;
        num = 0;
        foreach (var enemy in enemies)
        {
            if (enemy != null)
            {
                num += 1;
            }
        }
        if (num == 0) return true;
        return false;
    }
    
    








}
