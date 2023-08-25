using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;

public class CharAudio
{
    /// <summary>
    /// 到该角色回合播放的音效
    /// </summary>
    private List<AudioClip> myTurnAudios = new List<AudioClip>();
    public AudioClip MyTurnClip
    {
        get
        {
            if (myTurnAudios.Count != 0)
            {
                int index = Random.Range(0, myTurnAudios.Count);
                return myTurnAudios[index];
            }
            else
            {
                return null;
            }
        }
    }
    
    /// <summary>
    /// 该角色选择了攻击的音效
    /// </summary>
    private List<AudioClip> attackAudios = new List<AudioClip>();
    public AudioClip AttackClip
    {
        get
        {
            if (attackAudios.Count != 0)
            {
                int index = Random.Range(0, attackAudios.Count);
                return attackAudios[index];
            }
            else
            {
                return null;
            }
        }
    }
    
    /// <summary>
    /// 该角色选择了技能的音效
    /// </summary>
    private List<AudioClip> skillAudios = new List<AudioClip>();
    public AudioClip SkillClip
    {
        get
        {
            if (skillAudios.Count != 0)
            {
                int index = Random.Range(0, skillAudios.Count);
                return skillAudios[index];
            }
            else
            {
                return null;
            }
        }
    }
    
    /// <summary>
    /// 该角色大招回合
    /// </summary>
    private List<AudioClip> UTurnAudios = new List<AudioClip>();
    public AudioClip UTurnClip
    {
        get
        {
            if (UTurnAudios.Count != 0)
            {
                int index = Random.Range(0, UTurnAudios.Count);
                return UTurnAudios[index];
            }
            else
            {
                return null;
            }
        }
    }

    /// <summary>
    /// 该角色死亡播放的音效
    /// </summary>
    private List<AudioClip> dieAudios = new List<AudioClip>();
    public AudioClip DieClip
    {
        get
        {
            if (dieAudios.Count != 0)
            {
                int index = Random.Range(0, dieAudios.Count);
                return dieAudios[index];
            }
            else
            {
                return null;
            }
        }
    }
    

    private string Name;
    private int ID;
    public CharAudio(string name,int _id)
    {
        Name = name;
        ID = _id;
        LoadAudios(myTurnAudios,"MyTurn").Forget();
        LoadAudios(attackAudios,"Attack").Forget();
        LoadAudios(skillAudios,"Skill").Forget();
        LoadAudios(UTurnAudios,"UTurn").Forget();
        LoadAudios(dieAudios,"Die").Forget();
    }

    public async UniTaskVoid LoadAudios(List<AudioClip> list,string clipName)
    {
        list.Clear();
        int index = 0;
        while (true)
        {
            var request = Resources.LoadAsync<AudioClip>($"Audios/CharacterVoice/{Name}_{ID}/{clipName}_{index}");
            var obj=await request;
            AudioClip clip=obj as AudioClip;
            if (clip)
            {
                list.Add(clip);
            }
            else
            {
                break;
            }
            index += 1;
        }
    }

}
