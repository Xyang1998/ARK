using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;




public class UISystem_OnMap : ISystem
{
    
    private UIActionQueueManager uiActionQueueManager;
    public RectTransform teamUI;
    public CharSelectView charSelectView;

    
    /// <summary>
    /// 当前可选角色，因为Defer不能带参
    /// </summary>
    

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
        CanvasGroup canvasGroup = teamUI.GetComponent<CanvasGroup>();
        canvasGroup.alpha = 1;
        canvasGroup.interactable = true;
        canvasGroup.blocksRaycasts = true;
    }

    public void HideTeamUI()
    {
        CanvasGroup canvasGroup = teamUI.GetComponent<CanvasGroup>();
        canvasGroup.alpha = 0;
        canvasGroup.interactable = false;
        canvasGroup.blocksRaycasts = false;
    }

    public void AddSelectCharActionToQueue()
    {
        var task = UniTask.Defer(charSelectView.CharSelectAction);
        uiActionQueueManager.AddAction(task);
        uiActionQueueManager.Play();
    }

   
    
    
    
}
