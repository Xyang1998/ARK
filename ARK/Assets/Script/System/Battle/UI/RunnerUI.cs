using System.Collections;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.PlayerLoop;
using UnityEngine.UI;

public class RunnerUI : MonoBehaviour
{
    public Image icon;

    public Text remainTime;
    
    public Image background;
    private CancellationTokenSource tokenSource;
    private Color originalColor;

    public void Init(Runner runner)
    {
        Sprite sprite = runner.Character.CharacterDataStruct.icon;
        if (sprite) icon.sprite = sprite; 
        remainTime.text = runner.RemainTime.ToString();
        tokenSource = new CancellationTokenSource();
        originalColor = background.color;
    }

    public void UpdateRemainTime(float time)
    {
        remainTime.text = ((int)time).ToString();
    }


    public void Select()
    {
        Blink(tokenSource.Token).Forget();
    }

    public void UnSelect()
    {
        tokenSource.Cancel();
        tokenSource.Dispose();
        tokenSource = new CancellationTokenSource();
        background.color = originalColor;

    }

    private async UniTaskVoid Blink(CancellationToken token)
    {
        float a = background.color.a;
        while (true)
        {
            float x = Mathf.Abs(Mathf.Sin(3.14f*Time.unscaledTime));
            Color c = new Color(x, x, x, a);
            background.color = c;
            await UniTask.WaitForFixedUpdate(token);
        }
        
    }

}
