using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class BaseCharacterInfo : MonoBehaviour
{
    [Header("Body")]
    [SerializeField] Transform head;
    [SerializeField] List<Transform> bodyParts;
    [SerializeField] GameObject targetAbleObject;
    [SerializeField] float baseSize;

    [Header("Unity Stuff")]
    [SerializeField] TextMeshProUGUI apText;
    [SerializeField] TextMeshProUGUI hpText;
    [SerializeField] Image hpBar;

    [Header("Basic Stats")]
    [SerializeField] int maxHealth;
    private int currentHealth;
    [SerializeField] int maxAp;
    private int currentAp;
    [SerializeField] float baseMoveDistance = 6;
    private float currentMoveDistance;
    [SerializeField] float baseChargeDistance = 2; // Added to move distance
    private float currentChargeDistance;
    [SerializeField] float baseDashDistance = 3;
    private float currentDashDistance;

    [Header("Advanced Stats")]
    [SerializeField] int addedRangedDice;
    private int currentRDice; // Dice added to ranged attacks
    [SerializeField] int addedMeleeDice;
    private int currentMDice; // Dice added to melee attacks
    [SerializeField] int addedRangedDamage;
    private int currentRDamage; // Damage added to ranged damage
    [SerializeField] int addedMeleeDamage;
    private int currentMDamage; // Damage added to melee damage
    [SerializeField] int addedDefDice;
    private int currentDefDice;

    private bool ready; // Has ap means ready

    [Header("Equipment")]
    [SerializeField] Equipment equipment;

    private void Start()
    {
        SetBaseStats();

        apText.text = "Ap: " + currentAp + "/" + maxAp;
        hpText.text = currentHealth + "/" + maxHealth;
    }

    private void SetBaseStats()
    {
        baseSize = transform.GetChild(0).localScale.x / 2;
        ready = true;

        currentHealth = maxHealth;
        currentAp = maxAp;
        currentMoveDistance = baseMoveDistance;
        currentChargeDistance = baseChargeDistance;
        currentDashDistance = baseDashDistance;

        currentRDice = addedRangedDice;
        currentMDice = addedMeleeDice;
        currentRDamage = addedRangedDamage;
        currentMDamage = addedMeleeDamage;
        currentDefDice = addedDefDice;
    }

    public void SpendAp(int amount)
    {
        currentAp -= amount;
        if(currentAp <= 0)
        {
            ready = false;
        }
        apText.text = "Ap: " + currentAp + "/" + maxAp;
    }
    public void SetAp()
    {
        currentAp = maxAp;
        apText.text = "Ap: " + currentAp + "/" + maxAp;
        ready = true;
    }
    public bool CheckReady()
    {
        if (currentAp <= 0)
            return false;
        return true;
    }
    public void SetTargetable(bool yes)
    {
        targetAbleObject.SetActive(yes);
    }

    public void TakeDamage(int amount)
    {
        currentHealth -= amount;
        if(currentHealth <= 0)
        {
            Debug.Log("Died");
            Destroy(gameObject);
        }
        hpBar.fillAmount = (float)currentHealth / (float)maxHealth;
        hpText.text = currentHealth + "/" + maxHealth;
    }
    public void HealDamage(int amount)
    {
        currentHealth += amount;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);
        hpBar.fillAmount = (float)currentHealth / (float)maxHealth;
        hpText.text = currentHealth + "/" + maxHealth;
    }
    public void AddTempAp(int amount)
    {
        currentAp += amount;
        ready = true;
        apText.text = "Ap: " + currentAp + "/" + maxAp;
    }

    //Stats
    public int Health
    {
        get { return currentHealth; }
    }
    public int Ap
    {
        get{ return currentAp; }
    }
    public float MoveDistance
    {
        get { return currentMoveDistance; }
    }
    public float ChargeDistance
    {
        get { return currentChargeDistance; }
    }
    public float DashDistance
    {
        get { return currentDashDistance; }
    }
    // Adv
    public int RangedDice
    {
        get { return currentRDice; }
    }
    public int RangedDamage
    {
        get { return currentRDamage; }
    }
    public int MeleeDice
    {
        get { return currentMDice; }
    }
    public int MeleeDamage
    {
        get { return currentMDamage; }
    }
    public Transform Head
    {
        get { return head; }
    }
    public List<Transform> BodyParts
    {
        get { return bodyParts; }
    }
    public Equipment Equipment
    {
        get { return equipment; }
    }
    public int DefenceDice
    {
        get { return currentDefDice; }
    }
    public float BaseSize
    {
        get { return baseSize; }
    }
}
