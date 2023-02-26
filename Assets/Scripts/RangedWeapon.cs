using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class RangedWeapon
{
    [SerializeField] int damage;
    [SerializeField] int critDamage;
    [SerializeField] int diceAmount;
    [SerializeField] int hitChance;
    [SerializeField] float range;

    public RangedWeapon(int dmg, int critDmg, int diceAmount, int hitChance, float range)
    {
        damage = dmg;
        critDamage = critDmg;
        this.diceAmount = diceAmount;
        this.hitChance = hitChance;
        this.range = range;
    }

    public int Damage
    {
        get { return damage; }
    }
    public int CritDamage
    {
        get { return critDamage; }
    }
    public int DiceAmount
    {
        get { return diceAmount; }
    }
    public int HitChance
    {
        get { return hitChance; }
    }
    public float Range
    {
        get { return range; }
    }
}

[System.Serializable]
public class MeleeWeapon
{
    [SerializeField] int damage;
    [SerializeField] int critDamage;
    [SerializeField] int diceAmount;
    [SerializeField] int critChance;

    public MeleeWeapon(int dmg, int critDmg, int diceAmount, int hitChance)
    {
        damage = dmg;
        critDamage = critDmg;
        this.diceAmount = diceAmount;
        this.critChance = hitChance;
    }
    public int Damage
    {
        get { return damage; }
    }
    public int CritDamage
    {
        get { return critDamage; }
    }
    public int DiceAmount
    {
        get { return diceAmount; }
    }
    public int CritChance
    {
        get { return critChance; }
    }
}
[System.Serializable]
public class Armour
{
    [SerializeField] int defDice;
    [SerializeField] int saveChance;
    [SerializeField] int meleeDefense;

    public Armour(int defDice, int saveChance, int meleeDefense)
    {
        this.defDice = defDice;
        this.saveChance = saveChance;
        this.meleeDefense = meleeDefense;
    }

    public int DefenceDice
    {
        get { return defDice; }
    }
    public int SaveChance
    {
        get { return saveChance; }
    }
    public int MeleeDefense
    {
        get { return meleeDefense; }
    }
}

[System.Serializable]
public class Equipment
{
    [SerializeField] public Armour armour = new Armour(2, 5, 0);
    [SerializeField] public MeleeWeapon meleeWeapon = new MeleeWeapon(1, 2, 3, 5);
    [SerializeField] public RangedWeapon rangedWeapon; // Doesnt inherently start with ranged weapon
}
