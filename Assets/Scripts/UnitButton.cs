using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitButton : MonoBehaviour
{
    [SerializeField] public Transform targetUnit;

    public void SetTargetUnit(Transform newUnit)
    {
        targetUnit = newUnit;
    }
    public void OnClick()
    {
        GameManager.instance.StartSettingUnit(targetUnit.gameObject);
        Destroy(gameObject);
    }
}
