using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class APHandler : MonoBehaviour
{
    [SerializeField] int apAmount;
    private int currentAp;

    private void Start()
    {
        currentAp = apAmount;
    }

    public bool SpendAp(int amount)
    {
        if (currentAp - amount >= 0)
        {
            currentAp -= amount;
            return true;
        }
        else
            return false;
    }
    public bool CheckAp(int amount)
    {
        if (currentAp - amount >= 0)
        {
            return true;
        }
        else
            return false;
    }
}
