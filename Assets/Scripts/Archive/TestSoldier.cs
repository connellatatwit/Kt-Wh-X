using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestSoldier : MonoBehaviour, I_ModelActions
{
    [SerializeField] List<ModelAction> actionPrefabs;

    [SerializeField] List<GameObject> targets;

    private int currentAction;

    public List<ModelAction> GetActionPrefabs()
    {
        return actionPrefabs;
    }

    public void DoActions(int targetAction)
    {
        currentAction = targetAction;
        if(targetAction == 1)
        {
            // Shoot
            Debug.Log("Aiming");
            targets = GetComponent<ModelLineOfSight>().FindLineOfSight();
            GameMaster.instance.SetState(GameS.Targetting);
        }
    }
}
