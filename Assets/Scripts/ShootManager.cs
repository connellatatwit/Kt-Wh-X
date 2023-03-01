using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShootManager : MonoBehaviour
{
    public static ShootManager instance;

    [SerializeField] private List<Dice> launchedDice;
    [SerializeField] private List<Dice> launchedAttackDice;
    [SerializeField] private List<Dice> launchedDefendDice;
    [SerializeField] private bool fightStarted = false;

    [Header("Preset Unity Stuff")]
    [SerializeField] GameObject attackDicePrefab;
    [SerializeField] GameObject defenceDicePrefab;
    [SerializeField] GameObject diceBox;
    [SerializeField] Transform leftSpawn;
    [SerializeField] Transform rightSpawn;
    [SerializeField] Transform throwTarget;
    [SerializeField] Transform camObject;

    [SerializeField] Vector3 throwLeft;
    [SerializeField] Vector3 throwRight;

    private BaseCharacterInfo attacker;
    private BaseCharacterInfo defender;
    private bool inCover;

    private void Awake()
    {
        if(instance != null)
        {
            Debug.Log("Two shoot Managers");
            return;
        }
        instance = this;
        diceBox.SetActive(false);
    }

    private void FixedUpdate()
    {
        if (fightStarted) 
        {
            bool rolling = true;
            for (int i = 0; i < launchedDice.Count; i++)
            {
                if (launchedDice[i].GetState() != DiceState.Done)
                {
                    rolling = true;
                    break;
                }
                else
                    rolling = false;
            }
            if (!rolling)
            {
                // Get info
                CheckDiceResults();
            }
        }
    }

    private void CheckDiceResults()
    {
        fightStarted = false;
        int hits = 0;
        int critHits = 0;
        int blocks = 0;
        int critBlocks = 0;
        Debug.Log("HERE IT IS FOLKS BEGINNING ----------------------- " + defender.name);
        foreach (Dice dice in launchedDefendDice)
        {
            if (dice.GetSide() >= defender.Equipment.armour.SaveChance)
            {
                //Debug.Log("Defence saved on a " + dice.GetSide());
                if (dice.GetSide() == 6)
                    critBlocks++;
                else
                    blocks++;
            }
            //else
                //Debug.Log("Defence did not save on a " + dice.GetSide());
        }
        foreach (Dice dice in launchedAttackDice)
        {
            if (dice.GetSide() >= attacker.Equipment.rangedWeapon.HitChance)
            {
                //Debug.Log("Attacked hit on a " + dice.GetSide());
                if (dice.GetSide() == 6)
                    critHits++;
                else
                    hits++;
            }
            //else
                //Debug.Log("Attack did not hit on a " + dice.GetSide());
        }
        int CritHits = critHits - critBlocks;
        int extraCritBlocks = critBlocks - critHits;
        int NormHits = hits - blocks;
        if (critBlocks > 0)
            NormHits -= extraCritBlocks;
        /*
        Debug.Log("Amount of blocks are... " + blocks + " " + defender.name);
        Debug.Log("Amount of Crit Blocks are... " + critBlocks + " " + defender.name);
        Debug.Log("-------" + " " + defender.name);
        Debug.Log("Amount of Hits are... " + hits + " " + defender.name);
        Debug.Log("Amount of Critical hits are..." + critHits + " " + defender.name);
        */
        if (inCover)
        {
            NormHits--;
            Debug.Log("Target was in cover so -1 hit");
        }
        NormHits = Mathf.Clamp(NormHits, 0, 10000);
        CritHits = Mathf.Clamp(CritHits, 0, 10000);
        /*
        Debug.Log("NET----------------" + " " + defender.name);
        Debug.Log("NormHits... " + NormHits + " " + defender.name);
        Debug.Log("CritHits... " + CritHits + " " + defender.name);
        Debug.Log("HERE IT IS FOLKS END ----------------------- " + defender.name + " 2");
        */

        DoDamage(NormHits, CritHits);

        StartCoroutine(DestroyDice());
    }

    public void RollDice(BaseCharacterInfo attacker, BaseCharacterInfo defender, bool inCover)
    {
        diceBox.SetActive(true);

        int attackerDiceAmount = attacker.Equipment.rangedWeapon.DiceAmount + attacker.RangedDice;
        int defenderDiceAmount = defender.Equipment.armour.DefenceDice + defender.DefenceDice;
        launchedAttackDice.Clear();
        launchedDefendDice.Clear();

        this.attacker = attacker;
        this.defender = defender;
        this.inCover = inCover;

        for (int i = 0; i < attackerDiceAmount; i++)
        {
            GameObject newDice = Instantiate(attackDicePrefab, RandomCircle(leftSpawn, 1f), Quaternion.identity);
            launchedAttackDice.Add(newDice.GetComponent<Dice>());
            launchedDice.Add(newDice.GetComponent<Dice>());
            newDice.GetComponent<Dice>().InitDice(attacker.Equipment.rangedWeapon.HitChance);
            Vector3 throwDir = throwTarget.position - leftSpawn.position;
            throwDir = throwDir.normalized * 10;
            throwDir.y = 3f;
            newDice.GetComponent<Rigidbody>().velocity = throwDir;
            newDice.transform.parent = camObject;
        }
        for (int i = 0; i < defenderDiceAmount; i++)
        {
            GameObject newDice = Instantiate(defenceDicePrefab, RandomCircle(rightSpawn, 1f), Quaternion.identity);
            launchedDefendDice.Add(newDice.GetComponent<Dice>());
            launchedDice.Add(newDice.GetComponent<Dice>());
            newDice.GetComponent<Dice>().InitDice(defender.Equipment.armour.SaveChance);
            Vector3 throwDir = throwTarget.position - rightSpawn.position;
            throwDir = throwDir.normalized * 10;
            throwDir.y = 3f;
            newDice.GetComponent<Rigidbody>().velocity = throwDir;
            newDice.transform.parent = camObject;

        }

        fightStarted = true;
    }

    private IEnumerator DestroyDice()
    {
        yield return new WaitForSeconds(2f);
        for (int i = launchedDice.Count - 1; i >= 0; i--)
        {
            Destroy(launchedDice[i].gameObject);
        }
        diceBox.SetActive(false);
        launchedDice.Clear();
    }

    private void DoDamage(int hits, int critHits)
    {
        int dmg = attacker.Equipment.rangedWeapon.Damage;
        int critDmg = attacker.Equipment.rangedWeapon.CritDamage;
        int normDmg = dmg * hits;
        int criticalDmg = critDmg * critHits;
        Debug.Log("Damage is " + normDmg);
        Debug.Log("Critical Damage is " + criticalDmg);
        if (hits <= 0 && critHits <= 0)
            return;
        defender.TakeDamage(normDmg);
        if (defender != null)
            defender.TakeDamage(criticalDmg);
    }

    Vector3 RandomCircle(Transform spawnLocation, float radius)
    {
        float ang = Random.value * 360;
        Vector3 pos;
        pos.x = spawnLocation.position.x + radius * Mathf.Sin(ang * Mathf.Deg2Rad);
        pos.y = spawnLocation.position.y + radius * Mathf.Cos(ang * Mathf.Deg2Rad);
        pos.z = spawnLocation.position.z;
        return pos;
    }

    public bool FightStillGoing
    {
        get { return !fightStarted; }
    }
}
