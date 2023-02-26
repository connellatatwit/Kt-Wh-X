using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class ActionButton : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI buttonText;

    public void InitButton(IActions action)
    {
        buttonText.text = action.ActionName;
        GetComponent<Button>().onClick.RemoveAllListeners();
        GetComponent<Button>().onClick.AddListener(() => action.StartAction());
    }

    public void SetInteract(bool canClick)
    {
        GetComponent<Button>().interactable = canClick;
    }
}
