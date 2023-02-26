using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActionPass : MonoBehaviour, IActions
{
    private BaseCharacterInfo BCI;
    public int ApCost => 1;

    public string ActionName => "Pass";

    public void StartAction()
    {
        BCI.SpendAp(1);
        GameManager.instance.CheckIfUnitDone();
    }
    private void Start()
    {
        BCI = GetComponent<BaseCharacterInfo>();
    }
}
