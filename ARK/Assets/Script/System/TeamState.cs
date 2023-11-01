using System;
using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEditor;
using UnityEngine;




[Serializable]
public class TeamState : ISystem  //队伍状态
{
   public List<Character_OnMap> characterOnMaps
   {
      get;
      private set;
   }

   public List<Character_OnMap> enemiesOnMaps
   {
      get;
      private set;
   }
   //private  List<Character> characters; //<int,character>
   //private  List<Enemy> enemies;
   public List<Character_OnMap> reserveEnemies
   {
      get;
      private set;
   }
   public AudioClip bgm;

   private int gold;

   public int Gold
   {
      get => gold;
      set
      {
         if (value < 0)
         {
            gold = 0;
         }

         else
         {
            gold = value;
         }

      }
   }

   public List<BaseContract> selectedContracts;

   
   public void InitEnemies(EnemySetting[] enemySettings,EnemySetting[] reserveEnemiesSettings)
   {

      enemiesOnMaps.Clear();
      reserveEnemies.Clear();
      foreach (var enemy in enemySettings)
      {
         for (int i = 0; i < enemy.num; i++)
         {
            CharacterText text = TextSystem.characterExcelLoader.GetTextStruct(enemy.ID);
            CharacterDataStruct dataStruct = TextSystem.ParseCharacterText(text);
            BaseCharacterState baseCharacterState=Instantiate(Resources.Load<BaseCharacterState>($"Character/CharacterData/{dataStruct.name}_{dataStruct.id}"));
            Character_OnMap characterOnMap = new Character_OnMap(baseCharacterState, dataStruct);
            enemiesOnMaps.Add(characterOnMap);
         }
      }
      foreach (var enemy in reserveEnemiesSettings)
      {
         ////Debug.log(enemy.enemyName.ToString());
         for (int i = 0; i < enemy.num; i++)
         {
            CharacterText text = TextSystem.characterExcelLoader.GetTextStruct(enemy.ID);
            CharacterDataStruct dataStruct = TextSystem.ParseCharacterText(text);
            BaseCharacterState baseCharacterState=Instantiate(Resources.Load<BaseCharacterState>($"Character/CharacterData/{dataStruct.name}_{dataStruct.id}"));
            Character_OnMap characterOnMap = new Character_OnMap(baseCharacterState, dataStruct);
            reserveEnemies.Add(characterOnMap);
         }

         
      }
      
   }

   public override void Init()
   {
      DontDestroyOnLoad(this);
      reserveEnemies = new List<Character_OnMap>();
      characterOnMaps = new List<Character_OnMap>();
      enemiesOnMaps = new List<Character_OnMap>();
      selectedContracts = new List<BaseContract>();
      Gold = 999;
   }

   public override void Tick()
   {
      
   }

   //添加角色到队伍，队伍最多四人
   public void AddCharacterToTeam(CharacterDataStruct dataStruct)
   {
      if (characterOnMaps.Count > 4) return;
      BaseCharacterState baseCharacterState=Instantiate(Resources.Load<BaseCharacterState>($"Character/CharacterData/{dataStruct.name}_{dataStruct.id}"));
      Character_OnMap characterOnMap = new Character_OnMap(baseCharacterState, dataStruct);
      characterOnMaps.Add(characterOnMap);
      _systemMediator.uiSystemOnMap.AddCharacterUI(characterOnMap);
   }

   public async UniTaskVoid AddContractsToList(List<ContractUI> selectedUIs)
   {
      foreach (var ui in selectedUIs)
      {
         ContractStruct contractStruct = ui.ContractStruct;
         string path = contractStruct.iconName;
         var go = Resources.LoadAsync<BaseContract>($"ContractSO/{contractStruct.iconName}");
         var res=await go;
         BaseContract contract = Instantiate((BaseContract)res);
         contract.Init(contractStruct);
         selectedContracts.Add(contract);

      }
      
   }
 
   


  
}
