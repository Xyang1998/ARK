using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;

public class UIActionQueueManager
{
    //TODO:unitask改为Func<unitask>
    private Queue<UniTask> actionQueue = new Queue<UniTask>();
    private bool isPlaying = false; //检查是否在执行动作队列

    public void AddAction(UniTask uniTask)
    {
        actionQueue.Enqueue(uniTask);
    }



    public async void Play() //开始执行动作队列
    {
        if (!isPlaying)
        {

            ////Debug.log("Play执行！");
            isPlaying = true;
            while (actionQueue.Count > 0)
            {
                //  //Debug.log("取元素执行！");
                UniTask uniTask = actionQueue.Dequeue();
                await uniTask;
                // //Debug.log("取元素执行完毕！");

            }

            isPlaying = false;
        }



    }
    
}
