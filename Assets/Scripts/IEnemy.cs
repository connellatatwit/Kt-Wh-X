using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IEnemy
{
    public void DoTurn();
    public void Init();

    public BaseCharacterInfo BCI
    {
        get;
    }
}
