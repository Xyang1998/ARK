using System;
using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Cysharp.Threading.Tasks.Triggers;
using UnityEngine;
using UnityEngine.UI;

public class CharSelectView : MonoBehaviour
{
    public Button confirm;
    public Button cancel;
    public Transform content;
    public Transform team;
    public Text gold;
    private UISystem_OnMap uiSystemOnMap;
    private TeamState teamState;
    private CharUI_Select preUI;
    private CharUI_Select curUI;
    public Detail_Select detailSelect;
    private GameObject characterUIPrefab;

    public void Awake()
    {
        characterUIPrefab = Resources.Load<GameObject>(FilePath.onMapUIPrefabPath + "CharacterUI_OnSelect");
    }

    public void Start()
    {
        characterUIPrefab = Resources.Load<GameObject>(FilePath.onMapUIPrefabPath + "CharacterUI_OnSelect");
    }


    public async UniTask CharSelectAction()
    {
        
        detailSelect.Hide();
        if (!uiSystemOnMap)
        {
            uiSystemOnMap = SystemMediator.Instance.uiSystemOnMap;
        }

        if (!teamState)
        {
            teamState=SystemMediator.Instance.teamState;
        }
        ShowTeam();
        ShowGold();
        uiSystemOnMap.HideTeamUI();
        confirm.interactable = false;
        List<GameObject> needToDestroy = new List<GameObject>();
        CanvasGroup canvasGroup  =GetComponent<CanvasGroup>();
        canvasGroup.alpha = 1;
        canvasGroup.interactable = true;
        canvasGroup.blocksRaycasts = true;
        List<CharacterDataStruct> list = SystemMediator.Instance.eventSystemOnMap.selectable;
        int columns = (int)Mathf.Ceil(list.Count / 4.0f);
        int charIndex = 0;
        for (int i = 0; i < columns; i++)
        {
            GameObject col = Instantiate(Resources.Load<GameObject>(FilePath.onMapUIPrefabPath + "Column"),content);
            needToDestroy.Add(col);
            for (; charIndex < (i + 1) * 4; charIndex++)
            {
                if (charIndex >= list.Count) break;
                GameObject CharSelectable =
                    Instantiate(characterUIPrefab,
                        col.transform);
                CharSelectable.GetComponent<CharUI_Select>().Init(list[charIndex],this);
                needToDestroy.Add(CharSelectable);
            }
        }
        //0确定，1取消
        int index=await UniTask.WhenAny(confirm.OnClickAsync(), cancel.OnClickAsync());
        if (index == 0)
        {
            if (curUI)
            {
                teamState.AddCharacterToTeam(curUI.dataStruct);
                teamState.Gold -= CostDef.costDict[curUI.dataStruct.stars];
            }
        }
        canvasGroup.alpha = 0;
        canvasGroup.interactable = false;
        canvasGroup.blocksRaycasts = false;
        foreach (var go in needToDestroy)
        {
            Destroy(go);
        }
        uiSystemOnMap.ShowTeamUI();
    }
    public void SelectCharGO(CharUI_Select ui)
    {
        if (curUI != null)
        {
            curUI.UnSelect();
            if (ui == curUI)
            {
                detailSelect.Hide();
                curUI = null;
                confirm.interactable = false;
                return;
            }
        }
        detailSelect.Show();
        ui.Select();
        curUI = ui;
        detailSelect.ShowCharacterDetail(ui.dataStruct);
        if (CostDef.costDict[ui.dataStruct.stars] <= SystemMediator.Instance.teamState.Gold)
        {
            confirm.interactable = true;
        }
        else
        {
            confirm.interactable = false;
        }
    }

    public void ShowTeam()
    {
        List<Character_OnMap> characterOnMaps = teamState.characterOnMaps;
        for (int i = 0; i < characterOnMaps.Count; i++)
        {
            Image image = team.transform.GetChild(i).GetChild(0).GetComponent<Image>();
            image.sprite = characterOnMaps[i].CharacterDataStruct.icon;
        }
    }

    public void ShowGold()
    {
        gold.text = teamState.Gold.ToString();
    }

    
}
