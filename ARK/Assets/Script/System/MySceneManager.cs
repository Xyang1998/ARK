using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MySceneManager : ISystem
{
    private static Dictionary<string, Scene> scenes=new Dictionary<string, Scene>();
    public GameObject WorldGO;



    


    public override void Init()
    {
        DontDestroyOnLoad(this);
        scenes.Add("World",SceneManager.GetActiveScene());
    }

    public override void Tick()
    {
        
    }

    public async void WorldToBattle()
    {
        WorldGO.SetActive(false);
        var asyncOperation = SceneManager.LoadSceneAsync(1, LoadSceneMode.Additive);
        await asyncOperation;
        Scene battleScene = SceneManager.GetSceneByName("BattleScene");
        ////Debug.log(battleScene.name);
        scenes.Add("BattleScene",battleScene);



    }

    public async void BattleToWorld()
    {
        var asyncOperation =SceneManager.UnloadSceneAsync(scenes["BattleScene"]);
        await asyncOperation;
        WorldGO.SetActive(true);
        scenes.Remove("BattleScene");
        SceneManager.SetActiveScene(scenes["World"]);
        _systemMediator.playerController.UnLock();
    }
}
