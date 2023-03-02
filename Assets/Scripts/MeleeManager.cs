using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeleeManager : MonoBehaviour
{
    public static MeleeManager instance;

    [SerializeField] private List<Dice> launchedDice;
    [SerializeField] GameObject attackDicePrefab;
    [SerializeField] GameObject diceBox;
    [SerializeField] Transform leftSpawn;
    [SerializeField] Transform throwTarget;
    [SerializeField] Transform camObject;

    [SerializeField] Vector3 throwRight;

    [SerializeField] CombatText combatText;

    private bool fightStarted;
    private BaseCharacterInfo defender;
    private BaseCharacterInfo attacker;

    private void Awake()
    {
        if (instance != null)
        {
            Debug.Log("Two shoot Managers");
            return;
        }
        instance = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        
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
        int dmg = 0;
        int normHits = 0;
        int critHits = 0;
        foreach (Dice dice in launchedDice)
        {
            if(dice.GetSide() >= attacker.Equipment.meleeWeapon.CritChance) // Do higher Damage
            {
                dmg += attacker.Equipment.meleeWeapon.CritDamage;
                normHits++;
            }
            else
            {
                dmg += attacker.Equipment.meleeWeapon.Damage;
                critHits++;
            }
        }
        int blockedDmg = defender.Equipment.armour.MeleeDefense;

        dmg -= blockedDmg;
        dmg = Mathf.Clamp(dmg, 0, 1000);
        Debug.Log("Melee did... " + dmg);
        combatText.MeleeAttack(normHits * attacker.Equipment.meleeWeapon.Damage, critHits * attacker.Equipment.rangedWeapon.CritDamage, defender.Equipment.armour.MeleeDefense, dmg);

        // Do the Damage
        defender.TakeDamage(dmg);

        StartCoroutine(DestroyDice());
    }

    public void RollDice(BaseCharacterInfo attacker, BaseCharacterInfo defender)
    {
        combatText.ResetMeleeText(true);
        diceBox.SetActive(true);
        this.attacker = attacker;
        this.defender = defender;

        int attackerDiceAmount = attacker.Equipment.meleeWeapon.DiceAmount + attacker.MeleeDice;

        for (int i = 0; i < attackerDiceAmount; i++)
        {
            GameObject newDice = Instantiate(attackDicePrefab, RandomCircle(leftSpawn, 1f), Quaternion.identity);
            launchedDice.Add(newDice.GetComponent<Dice>());
            newDice.GetComponent<Dice>().InitDice(attacker.Equipment.meleeWeapon.CritChance);
            Vector3 throwDir = throwTarget.position - leftSpawn.position;
            throwDir = throwDir.normalized * 10;
            newDice.GetComponent<Rigidbody>().velocity = throwDir;
            newDice.transform.parent = camObject;
        }
        combatText.MeleeAttack(0, 0, defender.Equipment.armour.MeleeDefense, 0);
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
