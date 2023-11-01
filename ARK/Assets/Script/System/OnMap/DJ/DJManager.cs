using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using UnityEngine.UIElements;
using Slider = UnityEngine.UI.Slider;


public class DJManager : MonoBehaviour
{
    public GameObject DJContent;
    private Dictionary<int,string> CDDict;
    private AudioSource audioSource;
    string path = Application.streamingAssetsPath + "/DJ";
    public RectTransform djImage;
    private CancellationTokenSource cancellationTokenSource;
    public Slider slider;

    public void Awake()
    {
        CDDict = new Dictionary<int, string>();
        audioSource = GetComponent<AudioSource>();
        cancellationTokenSource = new CancellationTokenSource();
    }

    public void  LoadCDs()
    {
        
        if (CDDict.Count == 0)
        {
            int num = 0;
            GameObject prefab = Resources.Load<GameObject>("UI/Prefabs/OnMap/CDUI");
            float height = prefab.GetComponent<RectTransform>().sizeDelta.y;
            float space = DJContent.GetComponent<VerticalLayoutGroup>().spacing;
            int temp = 0;
            DirectoryInfo directoryInfo = new DirectoryInfo(path);
            FileInfo[] files = directoryInfo.GetFiles("*");
            foreach (var file in files)
            {
                if (!file.Name.EndsWith(".meta"))
                {
                    if (file.Name.EndsWith(".mp3"))
                    {
                        CDDict.Add(temp,file.Name);
                        GameObject go = Instantiate(prefab,DJContent.transform);
                        CDUI cdui = go.GetComponent<CDUI>();
                        if (cdui)
                        {
                            cdui.Init(temp,file.Name);
                            num++;
                        }
                        temp++;
                        
                    }
                }
            }

            DJContent.GetComponent<RectTransform>().sizeDelta =
                new Vector2(DJContent.GetComponent<RectTransform>().sizeDelta.x, num * (height + space));

        }
    }


    public async UniTaskVoid PlayDJ(int index)
    {
        cancellationTokenSource.Cancel();
        cancellationTokenSource.Dispose();
        cancellationTokenSource = new CancellationTokenSource();
        
        audioSource.Stop();
        string djname = CDDict[index];
        var unityWebRequest = UnityWebRequestMultimedia.GetAudioClip(path +"/"+ djname,AudioType.MPEG);
        var result=await unityWebRequest.SendWebRequest();
        audioSource.clip =DownloadHandlerAudioClip.GetContent(result);
        Rot().Forget();
        audioSource.Play();

    }

    public async UniTaskVoid Rot()
    {
        float length = audioSource.clip.length;
        while (true)
        {
            Vector3 temp = djImage.rotation.eulerAngles;
            djImage.rotation=Quaternion.Euler(temp.x,temp.y,temp.z-60*Time.deltaTime);
            slider.value = audioSource.time / length;
            await UniTask.Yield(cancellationTokenSource.Token);
        }
    }

    public void ShowDJUI()
    {
        UISystem_OnMap.ShowCanvasGroup(GetComponent<CanvasGroup>());
        if (audioSource.isPlaying)
        {
            cancellationTokenSource.Cancel();
            cancellationTokenSource.Dispose();
            cancellationTokenSource = new CancellationTokenSource();
            Rot().Forget();
        }
    }

    public void HideDJUI()
    {
        cancellationTokenSource.Cancel();
        cancellationTokenSource.Dispose();
        cancellationTokenSource = new CancellationTokenSource();
        UISystem_OnMap.HideCanvasGroup(GetComponent<CanvasGroup>());
    }
}
