using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;




public class UISystem_OnMap : ISystem
{
    
    private UIActionQueueManager uiActionQueueManager;
    public RectTransform teamUI;
    public CharSelectView charSelectView;
    public ContractSelectView contractSelectView;
    public DJManager DjManager;

    


    public override void Init()
    {
        uiActionQueueManager = new UIActionQueueManager();
    }

    public override void Tick()
    {
        
    }

    /// <summary>
    /// 添加角色到状态栏
    /// </summary>
    /// <param name="character"></param>
    public void AddCharacterUI(Character_OnMap character)
    {
        CharacterUI_OnMap ui =
            Instantiate(Resources.Load<CharacterUI_OnMap>(FilePath.onMapUIPrefabPath + "CharacterUI"),teamUI);
        ui.Init(character.BaseCharacterState.CharacterStateData,character.CharacterDataStruct);
    }

    public void ShowTeamUI()
    {
        ShowCanvasGroup(teamUI.GetComponent<CanvasGroup>());

    }

    public void HideTeamUI()
    {
        HideCanvasGroup(teamUI.GetComponent<CanvasGroup>());

    }

    public void AddSelectCharActionToQueue()
    {
        var task = UniTask.Defer(charSelectView.CharSelectAction);
        uiActionQueueManager.AddAction(task);
        uiActionQueueManager.Play();
    }

    public void AddContractSelectActionToQueue()
    {
        var task = UniTask.Defer(contractSelectView.ContractSelectAction);
        uiActionQueueManager.AddAction(task);
        uiActionQueueManager.Play();
    }

    public static void ShowCanvasGroup(CanvasGroup canvasGroup)
    {
        canvasGroup.alpha = 1;
        canvasGroup.interactable = true;
        canvasGroup.blocksRaycasts = true;
    }

    public static void HideCanvasGroup(CanvasGroup canvasGroup)
    {
        canvasGroup.alpha = 0;
        canvasGroup.interactable = false;
        canvasGroup.blocksRaycasts = false;
    }

   
    
    
    
}
