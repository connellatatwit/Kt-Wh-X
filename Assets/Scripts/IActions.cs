using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IActions
{
    public void StartAction();
    public int ApCost { get; }
    public string ActionName { get; }
}
