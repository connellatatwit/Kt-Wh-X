using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class TestFighter : MonoBehaviour, IEnemy
{
    [SerializeField] GameObject target;

    private BaseCharacterInfo BCI;
    BaseCharacterInfo IEnemy.BCI => BCI;
    private bool ready;
    public NavMeshAgent agent;
    public NavMeshObstacle agentObs;
    bool closeEnough = false;

    private void Start()
    {
        BCI = GetComponent<BaseCharacterInfo>();
        agent = GetComponent<NavMeshAgent>();
        agentObs = GetComponent<NavMeshObstacle>();
        agent.enabled = false;
        agentObs.enabled = true;
    }
    public void DoTurn()
    {
        agent.enabled = true;
        agentObs.enabled = false;
        Debug.Log("Doign turn");
        Vector3 movePos = GetClosestPosition();
        if(movePos != null)
            agent.SetDestination(movePos);
        StartCoroutine(WaitForMoveDone());
    }

    private Vector3 GetClosestPosition()
    {
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Ally");
        float closest = Mathf.Infinity;
        foreach (GameObject enemy in enemies)
        {
            if (Vector3.Distance(transform.position, enemy.transform.position) <= closest) // if enemy is closer than the closest
            {
                closest = Vector3.Distance(transform.position, enemy.transform.position);
                target = enemy;
            }
        }
        float moveDistance = 0f;
        Debug.Log(BCI.BaseSize);
        float meleeDistance = 1f + BCI.BaseSize + target.GetComponent<BaseCharacterInfo>().BaseSize;
        if (closest <= BCI.MoveDistance + BCI.ChargeDistance && closest! > meleeDistance) // If closesr than charge distance and is not within melee then go to melee
        {
            moveDistance = closest - meleeDistance;
            closeEnough = true;
        }
        else if (closest >= BCI.MoveDistance) // if the target is farther than we can walk then go full distance
        {
            moveDistance = BCI.MoveDistance;
            closeEnough = false;
        }
        else
            closeEnough = true;

        Vector3 enemyDirection = target.transform.position - transform.position;
        enemyDirection.Normalize();
        Vector3 movePosition = (transform.position + (moveDistance * enemyDirection));
        NavMeshHit hit;
        NavMesh.SamplePosition(movePosition, out hit, moveDistance, 1);
        Vector3 finalPosition = hit.position;
        return finalPosition;
    }
    private IEnumerator WaitForMoveDone()
    {
        float debugTime = 7f;
        bool tooLong = false;
        yield return new WaitForEndOfFrame();
        Debug.Log(agent.destination);
        while ((Vector3.Distance(transform.position, agent.destination) > .1f) || tooLong)
        {
            //Debug.Log(Vector3.Distance(transform.position, agent.destination));
            debugTime -= Time.deltaTime;
            if (debugTime <= 0)
            {
                Debug.Log("Took too long");
                tooLong = true;
                agent.SetDestination(transform.position);
                break;
            }
            yield return new WaitForEndOfFrame();
        }
        agent.enabled = false;
        agentObs.enabled = true;
        Debug.Log(transform.position);
        if (closeEnough) // Hit 'em!
        {
            MeleeManager.instance.RollDice(BCI, target.GetComponent<BaseCharacterInfo>());
        }
        BCI.SpendAp(2);
        GameManager.instance.EnemyTurnDone();
        ready = false;
    }
    public void Init()
    {
        ready = true;
        BCI.SetAp();
    }
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.magenta;

        Gizmos.DrawWireSphere(transform.position, 6);
        Gizmos.color = Color.green;

        Gizmos.DrawWireSphere(transform.position, 8);
    }
    public bool FastApproximately(float a, float b, float threshold)
    {
        return ((a - b) < 0 ? ((a - b) * -1) : (a - b)) <= threshold;
    }
}
