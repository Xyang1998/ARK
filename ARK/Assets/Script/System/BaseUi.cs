using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

public class BaseUI : MonoBehaviour
{

   public Image HP;
   public Image NP;
   protected CharacterStateData characterStateData;
   protected CharacterDataStruct characterDataStruct;
   private float hpTargetPer;
   private float hpCurPer;
   private bool updatingHP=false;
   private bool hpFlag = false;
   
   protected float npTargetPer;
   protected float npCurPer;
   protected bool updatingNP=false;
   protected bool npFlag = false;

   public virtual void Init(CharacterStateData data, CharacterDataStruct dataStruct)
   {
      characterStateData = data;
      characterDataStruct = dataStruct;
      hpCurPer = (characterStateData.HP / characterStateData.MaxHP);
      hpTargetPer = hpCurPer;
      UpdateHP();
      npCurPer = (characterStateData.NP / characterStateData.MaxNP);
      npTargetPer = npCurPer;
      UpdateNP();
      HP.material = Instantiate(Resources.Load<Material>("UI/Material/HPBar"));
      HP.GetComponent<Image>().material.SetFloat("_CurPer", hpCurPer);
      HP.GetComponent<Image>().material.SetFloat("_TargetPer",hpTargetPer);
      NP.material = Instantiate(Resources.Load<Material>("UI/Material/NPBar"));
      NP.GetComponent<Image>().material.SetFloat("_CurPer", npCurPer);
      NP.GetComponent<Image>().material.SetFloat("_TargetPer",npTargetPer);
   }
   
   
   
   
   public virtual void UpdateHP()
   {
      
      hpTargetPer = (characterStateData.HP / characterStateData.MaxHP);
      HP.GetComponent<Image>().material.SetFloat("_TargetPer",hpTargetPer);
      hpFlag = hpTargetPer > hpCurPer ;
      if (!updatingHP)
      {
         UpdateHPTask().Forget();
      }
   }  
   
   
   private async UniTaskVoid UpdateHPTask()
   {
       updatingHP = true;
       while (true)
       {
    
           hpCurPer +=hpCurPer<hpTargetPer? Time.fixedDeltaTime *NumericalDefinition.hpFadeSpeed:-Time.fixedDeltaTime *NumericalDefinition.hpFadeSpeed;
           HP.GetComponent<Image>().material.SetFloat("_CurPer", hpCurPer);
           await UniTask.Yield(PlayerLoopTiming.FixedUpdate,this.GetCancellationTokenOnDestroy());
           if (hpFlag)
           {
               if (hpCurPer >= hpTargetPer) break;
           }
           else
           {
               if (hpCurPer <= hpTargetPer) break;
           }
       }
       hpCurPer = hpTargetPer;
       HP.GetComponent<Image>().material.SetFloat("_CurPer", hpCurPer);
       updatingHP = false;
    
    }
        
    public virtual void UpdateNP()
    {
        npTargetPer = (characterStateData.NP / characterStateData.MaxNP);
        NP.GetComponent<Image>().material.SetFloat("_TargetPer",npTargetPer);
        npFlag = npTargetPer > npCurPer ;
        if (!updatingNP)
        {
            UpdateNPTask().Forget();
        }
    }
        
    private async UniTaskVoid UpdateNPTask()
    {
            updatingNP = true;
            while (true)
            {
                npCurPer +=npCurPer<npTargetPer? Time.fixedDeltaTime *NumericalDefinition.hpFadeSpeed:-Time.fixedDeltaTime *NumericalDefinition.hpFadeSpeed;
                NP.GetComponent<Image>().material.SetFloat("_CurPer", npCurPer);
                await UniTask.Yield(PlayerLoopTiming.FixedUpdate,this.GetCancellationTokenOnDestroy());
                if (npFlag)
                {
                    if (npCurPer >= npTargetPer) break;
                }
                else
                {
                    if (npCurPer <= npTargetPer) break;
                }
            }
            npCurPer = npTargetPer;
            NP.GetComponent<Image>().material.SetFloat("_CurPer", npCurPer);
            updatingNP = false;
    }


}
