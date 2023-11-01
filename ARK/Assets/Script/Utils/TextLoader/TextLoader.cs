using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using Cysharp.Threading.Tasks;
using UnityEngine;
using ExcelDataReader;
using JetBrains.Annotations;
using Object = UnityEngine.Object;


public class ExcelLoader<T> 
{
   protected Dictionary<int, T> IDTextDict;
   public ExcelLoader()
   {
      IDTextDict = new Dictionary<int, T>();
   }
   public virtual void LoadFromPath(string path)
   {
      if (File.Exists(Application.dataPath + path))
      {
         FileStream fileStream = File.Open(Application.dataPath + path, FileMode.Open, FileAccess.Read);
         IExcelDataReader excelDataReader = ExcelReaderFactory.CreateBinaryReader(fileStream);
         Type type = typeof(T);
         FieldInfo[] fieldInfos = type.GetFields();
        // Debug.Log(fieldInfos.Length);
         //跳过第一行的定义
         excelDataReader.Read();
         while (excelDataReader.Read())
         {
            System.Object t = Activator.CreateInstance<T>();
            for (int i = 0; i < fieldInfos.Length; i++)
            {
              // Debug.Log(excelDataReader.GetValue(i).ToString());
               if (i == 0)
               {
                  fieldInfos[i].SetValue(t, int.Parse(excelDataReader.GetValue(i).ToString()));
               }
               else
               {
                  if (fieldInfos[i].FieldType == typeof(int))
                  {
                     fieldInfos[i].SetValue(t, int.Parse(excelDataReader.GetValue(i).ToString()));
                  }
                  else
                  {
                     fieldInfos[i].SetValue(t, excelDataReader.GetValue(i).ToString());
                  }
               }
            }
            

            IDTextDict.Add(Int32.Parse(excelDataReader.GetValue(0).ToString()), (T)t);
         }
      }
   }

   public  T GetTextStruct(int id) 
   {
      if (IDTextDict.ContainsKey(id))
      {
         return  IDTextDict[id];
      }
      else return default;
   }

   public List<int> GetKeys()
   {
      List<int> l = new List<int>(IDTextDict.Keys);
      return l;
   }


   public async UniTaskVoid RemoveExcess(List<int> l)
   {
      Dictionary<int, T> newDict=new Dictionary<int, T>();
      foreach (var index in l)
      {
         if (IDTextDict.ContainsKey(index))
         {
            newDict.Add(index,IDTextDict[index]);
         }
      }
      IDTextDict.Clear();
      IDTextDict = newDict;
      await UniTask.Yield();
   }
   
}

public class EnemySettingTextLoader :ExcelLoader<MapEnemySetting> 
{
   public override void LoadFromPath(string path)
   {
      if (File.Exists(Application.dataPath + path))
      {
         FileStream fileStream = File.Open(Application.dataPath + path, FileMode.Open, FileAccess.Read);
         IExcelDataReader excelDataReader = ExcelReaderFactory.CreateBinaryReader(fileStream);

         //跳过第一行的定义
         excelDataReader.Read();
         while (excelDataReader.Read())
         {
            MapEnemySetting mapEnemySetting;
            //每行:地图敌人id;敌人id，数量#敌人id,数量;敌人id,数量#敌人id,名字，数量
            mapEnemySetting.EnemyID = int.Parse(excelDataReader.GetValue(0).ToString());
            string[] enemies = excelDataReader.GetValue(1).ToString().Split("#");
            EnemySetting[] enemySetting = new EnemySetting[enemies.Length];
            for (int i = 0; i < enemies.Length; i++)
            {
               string[] es = enemies[i].Split(','); //id,num
               enemySetting[i].ID = int.Parse(es[0]);
               enemySetting[i].num = int.Parse(es[1]);
            }

            mapEnemySetting.enemySetting = enemySetting;
            string[] rEnemies = excelDataReader.GetValue(2).ToString().Split("#");
            EnemySetting[] rEnemySetting;
            if (!rEnemies[0].Equals("null"))
            {
               rEnemySetting = new EnemySetting[rEnemies.Length];
               for (int i = 0; i < rEnemies.Length; i++)
               {
                  string[] es = rEnemies[i].Split(','); //id,num
                  rEnemySetting[i].ID = int.Parse(es[0]);
                  rEnemySetting[i].num = int.Parse(es[1]);
               }
            }
            else
            {
               rEnemySetting = new EnemySetting[0];
               
            }
            mapEnemySetting.reserveEnemySetting = rEnemySetting;
            IDTextDict.Add(mapEnemySetting.EnemyID,mapEnemySetting);
         }
         

            
         
      }
   }
}



