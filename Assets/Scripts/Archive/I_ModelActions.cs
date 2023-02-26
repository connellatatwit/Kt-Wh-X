using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface I_ModelActions
{
    public List<ModelAction> GetActionPrefabs();
    public void DoActions(int targetAction);
}

[System.Serializable]
public class ModelAction
{
    public string actionName;
    public int actionNumber;
}
