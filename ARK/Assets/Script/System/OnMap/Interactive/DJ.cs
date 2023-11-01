using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DJ : InteractiveObject
{
    public override void Interact()
    {
        SystemMediator.Instance.uiSystemOnMap.DjManager.LoadCDs();
        SystemMediator.Instance.uiSystemOnMap.DjManager.ShowDJUI();
    }

    public override void TriggerExit()
    {
        SystemMediator.Instance.uiSystemOnMap.DjManager.HideDJUI();
    }
}
