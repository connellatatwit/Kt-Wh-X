using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class GoblinCharger : MonoBehaviour, IEnemy
{
    [SerializeField] GameObject target;

    private BaseCharacterInfo BCI;

    public NavMeshAgent agent;
    public NavMeshObstacle agentObs;
    bool closeEnough = false;
    private Collider collider;
    BaseCharacterInfo IEnemy.BCI => BCI;

    private void Start()
    {
        BCI = GetComponent<BaseCharacterInfo>();
        agent = GetComponent<NavMeshAgent>();
        agentObs = GetComponent<NavMeshObstacle>();
        collider = GetComponent<Collider>();
        ChangeAgent(false);
    }
    public void DoTurn()
    {
        ChangeAgent(true); // Turn agent on
        Vector3 targetPos = GetTargetPosition();
        ChangeAgent(false);
        if (targetPos != Vector3.zero)
        {
            StartCoroutine(WaitForMovement(targetPos));
        }
        else
            Smack();
    }
    private void EndTurn()
    {
        GetComponent<Rigidbody>().velocity = Vector3.zero;
        BCI.SpendAp(2);
        GameManager.instance.EnemyTurnDone();
    }
    public Vector3 GetTargetPosition()
    {
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Ally");
        float closestDistance = Mathf.Infinity;
        foreach (GameObject enemy in enemies)
        {
            if (Vector3.Distance(transform.position, enemy.transform.position) <= closestDistance) // if enemy is closer than the closest
            {
                closestDistance = Vector3.Distance(transform.position, enemy.transform.position);
                target = enemy;
            }
        }
        float moveDistance = 0f;
        float meleeDistance = 1f + BCI.BaseSize + target.GetComponent<BaseCharacterInfo>().BaseSize;
        if (closestDistance <= BCI.MoveDistance + BCI.ChargeDistance && closestDistance! > meleeDistance) // If closesr than charge distance and is not within melee then go to melee
        {
            moveDistance = closestDistance;
            closeEnough = true;
        }
        else if (closestDistance >= BCI.MoveDistance) // if the target is farther than we can walk then go full distance
        {
            moveDistance = BCI.MoveDistance;
            closeEnough = false;
        }

        Vector3 enemyDirection = target.transform.position - transform.position;
        enemyDirection.Normalize();
        Vector3 movePosition = (transform.position + (moveDistance * enemyDirection));
        Debug.Log(movePosition + " " + transform.position);
        if (Vector3.Distance(transform.position, movePosition) <= .1f)
        {
            return Vector3.zero;
        }
        NavMeshHit hit;
        NavMesh.SamplePosition(movePosition, out hit, moveDistance, 1);
        agent.SetDestination(hit.position);
        
        return agent.destination;
    }
    private void ChangeAgent(bool agentOn)
    {
        collider.isTrigger = agentOn;
        agent.enabled = agentOn;
        agentObs.enabled = !agentOn;
    }
    private void Smack()
    {
        MeleeManager.instance.RollDice(BCI, target.GetComponent<BaseCharacterInfo>());
        StartCoroutine(WaitForDice());
    }
    private IEnumerator WaitForDice()
    {
        yield return new WaitForSeconds(1f);
        yield return new WaitUntil(() => MeleeManager.instance.FightStillGoing);
        Debug.Log("Dice Done");
        yield return new WaitForSeconds(2.5f);
        EndTurn();
    }
        public IEnumerator WaitForMovement(Vector3 targetPos)
    {
        GetComponent<Rigidbody>().useGravity = false;
        GetComponent<Collider>().isTrigger = true;
        float debugTime = 7f;
        Debug.Log("Started Moving");
        yield return new WaitForEndOfFrame();
        while ((Vector3.Distance(transform.position, targetPos) > .1f))
        {
            yield return new WaitForEndOfFrame();
            debugTime -= Time.deltaTime;
            if (debugTime <= 0)
                break;
            Vector3 moveDir = targetPos - transform.position;
            moveDir = moveDir.normalized;
            transform.Translate(moveDir * 4f * Time.deltaTime);
        }
        GetComponent<Rigidbody>().useGravity = true;
        GetComponent<Collider>().isTrigger = false;
        Debug.Log("Done Moving");
        float meleeDistance = 2f + BCI.BaseSize + target.GetComponent<BaseCharacterInfo>().BaseSize;
        if (Vector3.Distance(transform.position, target.transform.position) <= meleeDistance)
        {
            Smack();
        }
        else
            EndTurn();
    }

    public void Init()
    {
        BCI.SetAp();
    }
}
