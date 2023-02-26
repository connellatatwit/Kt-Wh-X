using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class TestShooter : MonoBehaviour, IEnemy
{
    [SerializeField] List<PossibleTargets> possibleTargets = new List<PossibleTargets>();

    private BaseCharacterInfo BCI;
    private bool ready;
    public NavMeshAgent agent;

    private void Start()
    {
        BCI = GetComponent<BaseCharacterInfo>();
        agent = GetComponent<NavMeshAgent>();
    }

    public void DoTurn()
    {
        //Move
        Vector3 movePos = GetRandomMoveSpot();
        agent.SetDestination(movePos);
        Debug.Log(movePos);
        //Shoot
        StartCoroutine(WaitForMoveDone());
        
    }

    private IEnumerator WaitForMoveDone()
    {
        yield return new WaitForEndOfFrame();
        while(Vector3.Distance(transform.position, agent.destination) > .1f)
        {
            yield return new WaitForEndOfFrame();
        }
        Debug.Log("Finding Targets");
        if (DetermineLineOfSight())
        {
            Debug.Log("Found some targets.");
            ShootARandomEnemy();
        }
        else
        {
            Debug.Log("No one in sight, Done");
        }
        BCI.SpendAp(2);
        GameManager.instance.EnemyTurnDone();
        ready = false;
    }
    
    private void ShootARandomEnemy()
    {
        int randTarget = Random.Range(0, possibleTargets.Count);
        Debug.Log("Shot " + possibleTargets[randTarget].target.name);
        // Shoot trge that is allowed to be shot
        ShootManager.instance.RollDice(BCI, possibleTargets[randTarget].target.GetComponent<BaseCharacterInfo>(), possibleTargets[randTarget].inCover);
        SetTargetsTargettable(false);
    }
    private void FixedUpdate()
    {
        
    }
    public bool DetermineLineOfSight()
    {
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Ally");
        possibleTargets.Clear();
        bool foundTargets = false;

        foreach (GameObject enemy in enemies)
        {
            BaseCharacterInfo currentEnemy = enemy.GetComponent<BaseCharacterInfo>();
            bool hitCover = false;
            bool inLineOfSight = false;
            //Draw lines
            for (int i = 0; i < currentEnemy.BodyParts.Count; i++)
            {
                bool hitCoverTemp = false;
                Vector3 direction = currentEnemy.BodyParts[i].position - BCI.Head.position;
                Ray ray = new Ray(BCI.Head.position, direction);
                RaycastHit[] hits = Physics.RaycastAll(ray, Vector3.Distance(currentEnemy.BodyParts[i].position, BCI.Head.position));
                for (int j = 0; j < hits.Length; j++)
                {
                    if (hits[j].collider.gameObject.layer == 13)
                    {
                        // Hit a wall
                        hitCoverTemp = true;
                    }
                }
                if (!hitCoverTemp)
                {
                    Debug.DrawRay(BCI.Head.position, direction, Color.red, 10f);
                    inLineOfSight = true;
                    foundTargets = true;
                }
                else
                {
                    Debug.DrawRay(BCI.Head.position, direction, Color.yellow, 10f);
                    hitCover = true;
                }
            }
            if (inLineOfSight) // If in line of sight then add target
            {
                possibleTargets.Add(new PossibleTargets(hitCover, enemy));
            }
        }
        SetTargetsTargettable(true);
        return foundTargets;
    }
    public void SetTargetsTargettable(bool yes)
    {
        for (int i = 0; i < possibleTargets.Count; i++)
        {
            possibleTargets[i].target.GetComponent<BaseCharacterInfo>().SetTargetable(yes);
        }
    }


    private Vector3 GetRandomMoveSpot()
    {
        Vector3 randomDirection = Random.insideUnitSphere * BCI.MoveDistance;
        Debug.Log(randomDirection);
        randomDirection = new Vector3(10, 0, 0);
        randomDirection += transform.position;
        NavMeshHit hit;
        NavMesh.SamplePosition(randomDirection, out hit, BCI.MoveDistance, 1);
        Vector3 finalPosition = hit.position;
        Debug.Log(finalPosition);
        return finalPosition;
    }

    public void Init()
    {
        ready = true;
        BCI.SetAp();
    }
    public bool Ready
    {
        get { return ready; }
    }

    BaseCharacterInfo IEnemy.BCI => BCI;
}
