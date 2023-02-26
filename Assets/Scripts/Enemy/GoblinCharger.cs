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

    BaseCharacterInfo IEnemy.BCI => BCI;

    private void Start()
    {
        BCI = GetComponent<BaseCharacterInfo>();
        agent = GetComponent<NavMeshAgent>();
        agentObs = GetComponent<NavMeshObstacle>();
        ChangeAgent(false);
    }
    public void DoTurn()
    {
        ChangeAgent(true); // Turn agent on
        Vector3 targetPos = GetTargetPosition();
        ChangeAgent(false);
        Debug.Log(targetPos);
        if (targetPos != Vector3.zero)
        {
            Debug.Log(targetPos);
            targetPos.y = .5f;
            transform.position = targetPos;
            
        }
        EndTurn();
    }
    private void EndTurn()
    {
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
            moveDistance = closestDistance - meleeDistance;
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
        if (movePosition == transform.position)
        {
            Debug.Log("Ear");
            return Vector3.zero;
        }
        NavMeshHit hit;
        NavMesh.SamplePosition(movePosition, out hit, moveDistance, 1);
        Vector3 finalPosition = hit.position;
        return finalPosition;
    }
    private void ChangeAgent(bool agentOn)
    {
        agent.enabled = agentOn;
        agentObs.enabled = !agentOn;
    }

    public void Init()
    {
        BCI.SetAp();
    }
}
