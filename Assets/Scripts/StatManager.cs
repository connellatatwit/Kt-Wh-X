using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class StatManager : MonoBehaviour
{
    [SerializeField] GameObject uiObject;
    [Header("Melee Weapon")]
    [SerializeField] TextMeshProUGUI meleeAttackDiceText;
    [SerializeField] TextMeshProUGUI critChanceText;
    [SerializeField] TextMeshProUGUI meleedmgText;
    [SerializeField] List<GameObject> meleeObjects;
    [Header("Ranged Weapon")]
    [SerializeField] TextMeshProUGUI rangedAttackDiceText;
    [SerializeField] TextMeshProUGUI hitChanceText;
    [SerializeField] TextMeshProUGUI rangeddmgText;
    [SerializeField] List<GameObject> rangedObjects;

    public void SetInformation(BaseCharacterInfo targetCharacter)
    {
        uiObject.SetActive(true);
        //TODO: Change to check if they have the equipment at all instead of if they have a stat
        if(targetCharacter.Equipment.meleeWeapon.CritChance != 0)
        {
            for (int i = 0; i < meleeObjects.Count; i++)
            {
                meleeObjects[i].SetActive(true);
            }
            meleeAttackDiceText.text = targetCharacter.Equipment.meleeWeapon.DiceAmount.ToString();
            meleedmgText.text = targetCharacter.Equipment.meleeWeapon.Damage + "/" + targetCharacter.Equipment.meleeWeapon.CritDamage;
            critChanceText.text = targetCharacter.Equipment.meleeWeapon.CritChance.ToString();
        }
        else
        {
            for (int i = 0; i < meleeObjects.Count; i++)
            {
                meleeObjects[i].SetActive(false);
            }
        }
        if(targetCharacter.Equipment.rangedWeapon.HitChance != 0)
        {
            for (int i = 0; i < rangedObjects.Count; i++)
            {
                rangedObjects[i].SetActive(true);
            }
            rangedAttackDiceText.text = targetCharacter.Equipment.rangedWeapon.DiceAmount.ToString();
            rangeddmgText.text = targetCharacter.Equipment.rangedWeapon.Damage + "/" + targetCharacter.Equipment.rangedWeapon.CritDamage;
            hitChanceText.text = targetCharacter.Equipment.rangedWeapon.HitChance.ToString();
        }
        else
        {
            for (int i = 0; i < rangedObjects.Count; i++)
            {
                rangedObjects[i].SetActive(false);
            }
        }
    }
    public void ClearInformation()
    {
        uiObject.SetActive(false);
    }
}
