using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CombatText : MonoBehaviour
{
    [SerializeField] GameObject rangedObject;
    [SerializeField] GameObject meleeObject;

    [Header("Ranged Texts")]
    [SerializeField] TextMeshProUGUI rangedHits;
    [SerializeField] TextMeshProUGUI rangedCrits;
    [SerializeField] TextMeshProUGUI blocks;
    [SerializeField] TextMeshProUGUI critBlocks;
    [SerializeField] TextMeshProUGUI rangedDmg;

    [Header("Melee Texts")]
    [SerializeField] TextMeshProUGUI normalMelee;
    [SerializeField] TextMeshProUGUI critMelee;
    [SerializeField] TextMeshProUGUI dmgBlocked;
    [SerializeField] TextMeshProUGUI meleeDmg;

    public void ResetRangedText(bool showInformation)
    {
        rangedObject.SetActive(showInformation);
        rangedHits.text = "0";
        rangedCrits.text = "0";
        blocks.text = "0";
        critBlocks.text = "0";
        rangedDmg.text = "0";
    }
    public void ResetMeleeText(bool showInformation)
    {
        meleeObject.SetActive(showInformation);
        normalMelee.text = "0";
        critMelee.text = "0";
        dmgBlocked.text = "0";
        meleeDmg.text = "0";
    }

    public void RangedAttack(int hits, int crits, int blocks, int critBlocks, int netDmg)
    {
        rangedHits.text = hits.ToString();
        rangedCrits.text = crits.ToString();
        this.blocks.text = blocks.ToString();
        this.critBlocks.text = critBlocks.ToString();
        rangedDmg.text = netDmg.ToString();
    }
    public void MeleeAttack(int meleeDmg, int critDmg, int dmgDenied, int netDmg)
    {
        normalMelee.text = meleeDmg.ToString();
        critMelee.text = critDmg.ToString();
        dmgBlocked.text = dmgDenied.ToString();
        this.meleeDmg.text = netDmg.ToString();
    }
}
