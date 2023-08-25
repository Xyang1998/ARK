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
   private Dictionary<int, T> IDTextDict;

   public ExcelLoader()
   {
      IDTextDict = new Dictionary<int, T>();
   }
   public void LoadFromPath(string path)
   {
      if (File.Exists(Application.dataPath + path))
      {
         FileStream fileStream = File.Open(Application.dataPath + path, FileMode.Open, FileAccess.Read);
         IExcelDataReader excelDataReader = ExcelReaderFactory.CreateBinaryReader(fileStream);
         Type type = typeof(T);
         FieldInfo[] fieldInfos = type.GetFields();
         Debug.Log(fieldInfos.Length);
         //跳过第一行的定义
         excelDataReader.Read();
         while (excelDataReader.Read())
         {
            System.Object t = Activator.CreateInstance<T>();
            for (int i = 0; i < fieldInfos.Length; i++)
            {
               Debug.Log(excelDataReader.GetValue(i).ToString());
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
