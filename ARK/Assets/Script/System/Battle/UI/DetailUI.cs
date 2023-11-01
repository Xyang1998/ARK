using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.PlayerLoop;
using UnityEngine.UI;

public class DetailUI : MonoBehaviour
{
    public Button attributeButton;
    public Button skillButton;
    public Button buffButton;
    public RectTransform controller;
    public float controllerSpeed=1000;
    private CancellationTokenSource controllerToken;
    /// <summary>
    /// 当目标点在controller左边时为false,右边时为true
    /// </summary>
    private bool LRFlag;
    
    public RectTransform selectContent;
    

    // 属性部分
    public Image icon;
    public Text characterName;
    public Text characterDescription;
    public Image classIcon;
    public Text hp;
    public Text np;
    public Text atk;
    public Text speed;
    public Text defense;
    public Text magicDefense;
    public Text critRate;
    public Text criticalDamage;
    public Text npRate;
    public Text healRate;
    public Text effectHitRate;
    public Text effectResistanceRate;
    private TextSystem textSystem;
    
    //技能部分
    public RectTransform skillContent;
    
    //Buff部分
    public RectTransform buffContent;

    private GameObject PreSelect;


    public void Start()
    {
        textSystem = SystemMediator.Instance.textSystem;
        attributeButton.onClick.AddListener(AttributeOnClick);
        skillButton.onClick.AddListener(SkillOnClick);
        buffButton.onClick.AddListener(BuffOnClick);
        controllerToken = new CancellationTokenSource();
    }


    private void AttributeOnClick()
    {
        HandleControllerMove(controller.sizeDelta.x);


    }

    private void SkillOnClick()
    {

        HandleControllerMove(0);

    }

    private void BuffOnClick()
    {
        HandleControllerMove(-controller.sizeDelta.x);

    }

    private void HandleControllerMove(float x)
    {
        controllerToken.Cancel();
        controllerToken.Dispose();
        controllerToken = new CancellationTokenSource();
        Vector2 targetPos = new Vector2(x,0);
        if (Mathf.Abs(targetPos.x - controller.anchoredPosition.x) > 1)
        {
            LRFlag = targetPos.x > controller.anchoredPosition.x;
            ControllerMove(controllerToken, targetPos).Forget();
        }
    }

    public void UpdateView(List<Character> characters,List<Enemy> enemies)
    {
        //重置界面
        int num=selectContent.childCount;
        for (int i = 0; i < num; i++)
        {
            SelectablePool.AddToPool(selectContent.GetChild(0).gameObject);
        }
        Vector2 size = new Vector2(0,selectContent.sizeDelta.y);
        float space = selectContent.GetComponent<HorizontalLayoutGroup>().spacing;
        int characterNum = characters.Count;
        foreach (var enemy in enemies)
        {
            if (enemy != null)
            {
                characterNum += 1;
            }
        }
        size.x=characterNum*(space+selectContent.sizeDelta.y);
        selectContent.sizeDelta = size;
        foreach (var character in characters)
        {
            GameObject selectableGO = SelectablePool.GetSelectable();
           
            selectableGO.GetComponent<SelectableUI>().BindCharacter(character,this);
            selectableGO.transform.SetParent(selectContent);
        }

        foreach (var enemy in enemies)
        {
            if (enemy != null)
            {
                GameObject selectableGO = SelectablePool.GetSelectable();
                selectableGO.GetComponent<SelectableUI>().BindCharacter(enemy,this);
                selectableGO.transform.SetParent(selectContent);
            }
        }
        ShowCharacterDetail(characters[0]);
        HighlightSelect(selectContent.transform.GetChild(0).gameObject);


    }

    public void ShowCharacterDetail(BaseCharacter character)
    {
        icon.sprite = character.CharacterDataStruct.icon;
        characterName.text = character.CharacterDataStruct.CName;
        //characterDescription=
        if (character.CharacterDataStruct.characterCamp == CharacterCamp.Enemy)
        {
            classIcon.enabled=false;
        }
        else
        {
            classIcon.enabled=true;
            classIcon.sprite = ClassIcons.GetClassIcon(character.CharacterDataStruct.characterClass);
        }
        CharacterStateData data = character.BattleCharacterStateData;
        hp.text = $"{(int)data.HP}/{(int)data.MaxHP}";
        np.text = $"{(int)data.NP}/{(int)data.MaxNP}";
        atk.text = data.ATK.ToString();
        speed.text = data.Speed.ToString();
        defense.text = data.Defense.ToString();
        magicDefense.text = data.MagicDefense.ToString();
        critRate.text = data.CritRate.ToString();
        criticalDamage.text = data.CriticalDamage.ToString();
        npRate.text = data.NPRate.ToString();
        healRate.text = data.Healing.ToString();
        effectHitRate.text = data.EffectHitRate.ToString();
        effectResistanceRate.text = data.EffectResistanceRate.ToString();
        SkillDetail(character);
        BuffDetail(character);


    }

    public void SkillDetail(BaseCharacter character)
    {
        //重置skill界面
        int num = skillContent.childCount;
        for (int i = 0; i < num; i++)
        {
            SkillViewPool.AddToPool(skillContent.GetChild(0).gameObject);
        }
        List<BaseSkill> skills = new List<BaseSkill>();
        //被动
        foreach (var skill in character.passiveSkills)
        {
            skills.Add(skill);
        }
        //攻击
        skills.Add(character.attack);
        //技能
        Character c=character as Character;
        if (c!=null)
        {
            skills.Add(c.skill);
        }
        else
        {
            Enemy enemy = character as Enemy;
            if (enemy!=null)
            {
                foreach (var s in enemy.skills)
                {
                    skills.Add(s);
                }
            }
        }
        if (character.ultimate != null)
        {
            skills.Add(character.ultimate);
        }
        GameObject go = SkillViewPool.GetSkillView();
        float height = go.GetComponent<RectTransform>().sizeDelta.y;
        SkillViewPool.AddToPool(go);
        Vector2 size = new Vector2(skillContent.sizeDelta.x,(skillContent.GetComponent<VerticalLayoutGroup>().spacing+height)*skills.Count);
        skillContent.sizeDelta = size;
        foreach (var skill in skills)
        {
            go = SkillViewPool.GetSkillView();
            go.GetComponent<SkillViewUI>().BindSkill(ref skill.skillDataStruct);
            go.transform.SetParent(skillContent);
        }

    }

    public void BuffDetail(BaseCharacter character)
    {
        //重置skill界面
        int num = buffContent.childCount;
        for (int i = 0; i < num; i++)
        {
            BuffViewPool.AddToPool(buffContent.GetChild(0).gameObject);
        }

        List<BaseBuff> l = new List<BaseBuff>();

        foreach (var buff in character.buffs)
        {
            if (buff.icon != null)
            {
                l.Add(buff);
            }
        }
        
        GameObject temp = BuffViewPool.GetBuffView();
        float height = temp.GetComponent<RectTransform>().sizeDelta.y;
        BuffViewPool.AddToPool(temp);
        Vector2 size = new Vector2(buffContent.sizeDelta.x,(buffContent.GetComponent<VerticalLayoutGroup>().spacing+height)*l.Count);
        buffContent.sizeDelta = size;
        foreach (var buff in l)
        {
            BuffText text = TextSystem.buffExcelLoader.GetTextStruct(buff.buffID);
            GameObject go = BuffViewPool.GetBuffView();
            go.GetComponent<BuffViewUI>()
                    .BindBuff(buff.icon, text.buffName, text.description, buff.CurLayers, buff.isPermanent, buff.RemainTurns);
            go.transform.SetParent(buffContent);
            
        }
    }

    public void HighlightSelect(GameObject go)
    {
        if (PreSelect != null)
        {
            PreSelect.GetComponent<Outline>().enabled = false;
        }

        go.GetComponent<Outline>().enabled = true;
        PreSelect = go;
    }
    
    
    
    

    private async UniTaskVoid ControllerMove(CancellationTokenSource cancellationTokenSource,Vector2 targetPos)
    {
        Vector2 curPos = controller.anchoredPosition;
        while (targetPos.x > controller.anchoredPosition.x==LRFlag)
        {
            curPos.x +=LRFlag? controllerSpeed * Time.fixedDeltaTime:-controllerSpeed*Time.fixedDeltaTime;
            if (targetPos.x > curPos.x != LRFlag) break;
            controller.anchoredPosition = curPos;
            await UniTask.Yield(PlayerLoopTiming.FixedUpdate, cancellationTokenSource.Token);
        }
        controller.anchoredPosition = targetPos;
    }
    

}
