using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

public class ContractSelectView : MonoBehaviour
{
    public Button confirm;
    private Dictionary<int, ContractStruct> allContracts;
    private List<int> groups;
    private List<ContractUI> contractUIs;
    public RectTransform content;
    private GameObject contractsColPrefab;
    private GameObject singleContractPrefab;
    private CancellationTokenSource cancellationTokenSource;
    private GameObject textUIPrefab;
    private List<ContractTextUI> textUIs;
    private List<ContractUI> selecteds;
    public RectTransform textContent;
    private GameObject textColPrefab;
    private List<GameObject> textCols; //<列>
    private CanvasGroup canvasGroup;

    public void Start()
    {
        canvasGroup = GetComponent<CanvasGroup>();
        contractsColPrefab = Resources.Load<GameObject>(FilePath.onMapUIPrefabPath + "ContractCol");
        singleContractPrefab=Resources.Load<GameObject>(FilePath.onMapUIPrefabPath + "SingleContract");
        textColPrefab=Resources.Load<GameObject>(FilePath.onMapUIPrefabPath + "ContractTextCol");
        textUIPrefab=Resources.Load<GameObject>(FilePath.onMapUIPrefabPath + "ContractText");
        allContracts = new Dictionary<int, ContractStruct>();
        groups = new List<int>();
        contractUIs = new List<ContractUI>();
        textUIs = new List<ContractTextUI>();
        textCols = new List<GameObject>();
        selecteds = new List<ContractUI>();
        var allStructs = TextSystem.contractExcelLoader.GetKeys();
        foreach (var id in allStructs)
        {
            ContractStruct contractStruct = TextSystem.ParseContractText(TextSystem.contractExcelLoader.GetTextStruct(id));
            allContracts.Add(id,contractStruct);
            if (!groups.Contains(contractStruct.group))
            {
                groups.Add(contractStruct.group);
            }
        }
        confirm.onClick.AddListener(ConfirmClick);
    }
    
    public async UniTask ContractSelectAction()
    {
        //加载UI布局
        int colNum = 0;
        cancellationTokenSource = new CancellationTokenSource();
        foreach (var group in groups)
        {
            GameObject col = Instantiate(contractsColPrefab,content);
            var l = allContracts.Where(s => s.Value.group == group).OrderBy(x=>x.Value.level).ToList();
            foreach (var item in l)
            {
                
                GameObject go = Instantiate(singleContractPrefab, col.transform);
                ContractUI cui = go.GetComponent<ContractUI>();
                if (cui)
                {
                    cui.Bind(this,item.Value);
                    contractUIs.Add(cui);
                }

            }
            
        }
        await UniTask.WhenAll(confirm.OnClickAsync());
        canvasGroup.alpha = 0;
        canvasGroup.interactable = false;
        canvasGroup.blocksRaycasts = false;
    }

    public void SelectOne(ContractUI ui)
    {
        var l = contractUIs.Where(s => (s.ContractStruct.group == ui.ContractStruct.group && s != ui)).ToList();
        foreach (var item in l)
        {
            item.Conflict();
        }
        ui.SelectedAction();
    }

    public void UnSelectOne(ContractUI ui)
    {
        ui.UnSelectedAction();
        var l = contractUIs.Where(s => (s.ContractStruct.group == ui.ContractStruct.group && s != ui)).ToList();
        foreach (var item in l)
        {
            item.UnSelectedAction();
        }
        
    }

    public async UniTaskVoid AddOne(ContractUI ui)
    {
        selecteds.Add(ui);
        GameObject go = Instantiate(textUIPrefab);
        ContractTextUI textUI =  go.GetComponent<ContractTextUI>();
        ui.textUI = textUI;
        textUIs.Add(textUI);
        if (textUI)
        {
            textUI.Init(ui.ContractStruct.level,ui.ContractStruct.description);
        }
        await UniTask.Yield();
        if (textCols.Count == 0)
        {
            CreateColAndAddOne(textUI);
        }
        else
        {
            GameObject lastCol = textCols[^1];
            if (CanAdd(lastCol, textUI))
            {         
                textUI.transform.SetParent(lastCol.transform);
 
                
            }
            else
            {
                CreateColAndAddOne(textUI);
            }
        }
        
    }

    private void CreateColAndAddOne(ContractTextUI textUI)
    {
        GameObject col = Instantiate(textColPrefab,textContent);
        textCols.Add(col);
        textUI.transform.SetParent(col.transform);

    }

    public void DeleteOne(ContractUI ui)
    {
        selecteds.Remove(ui);
        ContractTextUI textUI = ui.textUI;
        if (textUI)
        {
            int index = textUIs.IndexOf(textUI);
            if (index != -1)
            {

                GameObject deletedCol = textUI.transform.parent.gameObject;
                textUI.transform.SetParent(null);
                int colindex = textCols.IndexOf(deletedCol);
                colindex = colindex == 0 ? 0 : colindex - 1;
                textUIs.Remove(textUI);
                Destroy(textUI.gameObject);
                for (int i = index; i < textUIs.Count; i++)
                {
                    textUIs[i].transform.SetParent(null);
                }

                for (int i = index; i < textUIs.Count; i++)
                {

                    if (CanAdd(textCols[colindex], textUIs[i]))
                    {
                        textUIs[i].transform.SetParent(textCols[colindex].transform);


                    }
                    else
                    {
                        colindex++;
                        i--;
                    }
                }


                List<GameObject> needToDestroy = new List<GameObject>();
                for (int i = 0; i < textCols.Count; i++)
                {
                    if (textCols[i].transform.childCount == 0)
                    {
                        needToDestroy.Add(textCols[i]);
                    }
                }


                for (int i = 0; i < needToDestroy.Count; i++)
                {
                    textCols.Remove(needToDestroy[i]);
                    Destroy(needToDestroy[i]);
                }

            }

        }
    }

    private bool CanAdd(GameObject col,ContractTextUI textUI)
    {
        float space = col.GetComponent<VerticalLayoutGroup>().spacing;
        int count = col.transform.childCount;
        float curChildHeight = 0;
        for (int i = 0; i < count; i++)
        {
            curChildHeight += col.transform.GetChild(i).GetComponent<RectTransform>().sizeDelta.y;
        }
        curChildHeight += (count - 1) * space;
        float newHeight = textUI.GetComponent<RectTransform>().sizeDelta.y;
        if (curChildHeight + newHeight + space > col.GetComponent<RectTransform>().sizeDelta.y)
        {
            return false;
            
        }
        else
        {
            return true;
 
        }
    }

    private void ConfirmClick()
    {
        SystemMediator.Instance.teamState.AddContractsToList(selecteds).Forget();
        
    }


    
}
