using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum DiceState
{
    Idle,
    Rolling,
    Done
}

public class Dice : MonoBehaviour
{
    [SerializeField] GameObject highlight;
    [SerializeField] ParticleSystem succeedAnim;
    [SerializeField] ParticleSystem critSucceedAnim;
    [SerializeField] List<Transform> sides;
    private Rigidbody rb;
    public bool dne = false;
    private BaseCharacterInfo target;

    private float timer = 12f;

    private int side = 0;
    private int animGoal;

    private float waitTime = 15f;

    private DiceState state = DiceState.Idle;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        int maxX = Random.Range(-3, 3);
        rb.AddTorque(new Vector3(Random.Range(-3,4), Random.Range(-3, 4), Random.Range(-3,4)));
    }

    public int GetSide()
    {
        return side;
    }

    public void InitDice(int goal)
    {
        state = DiceState.Rolling;
        animGoal = goal;
        highlight.SetActive(false);
    }
    public DiceState GetState()
    {
        return state;
    }

    private void Update()
    {
        //Debug.Log("Velocity is " + rb.velocity.magnitude);
        if (state == DiceState.Rolling)
        {
            if (rb.velocity.magnitude <= 0)
            {
                int currentSide = 0;
                float highestSide = sides[0].position.y;
                //Find the lowest Side
                for (int i = 1; i < sides.Count; i++)
                {
                    //Debug.Log("current side is... " + currentSide + " versus " + i);
                    //Debug.Log("That is... " + sides[currentSide].position.y +" vs. " + sides[i].position.y);
                    if(sides[i].position.y > highestSide)
                    {
                        currentSide = i;
                        highestSide = sides[i].position.y;
                    }
                }
                side = sides[currentSide].name.Length;
                if (side >= animGoal)
                {
                    if (side == 6)
                        critSucceedAnim.Play();
                    else
                        succeedAnim.Play();
                    highlight.SetActive(true);
                }
                state = DiceState.Done;
            }
        }
        timer -= Time.deltaTime;
        waitTime -= Time.deltaTime;
        if (timer <= 0 && state != DiceState.Done)
        {
            Debug.Log("DESTUCK DICE");
            timer = 4f;
            rb.velocity = (new Vector3(0, 5, 0));
        }
        if(waitTime <= 0)
        {
            Debug.Log("DICE LIVED TOO LONG");
            state = DiceState.Done;
            side = 1;
        }
    }
}
