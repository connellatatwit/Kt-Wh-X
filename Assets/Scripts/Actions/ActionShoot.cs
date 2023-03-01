using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActionShoot : MonoBehaviour, IActions
{
    private BaseCharacterInfo BCI;
    private bool doingActionShoot;
    [SerializeField] List<PossibleTargets> possibleTargets = new List<PossibleTargets>();
    [SerializeField] LayerMask unitMask = (1 << 11);
    [SerializeField] LayerMask unitMask2 = (1 << 10);

    public int ApCost => 1;

    public string ActionName => "Shoot";

    private void Start()
    {
        BCI = GetComponent<BaseCharacterInfo>();
    }
    public void StartAction()
    {
        if (GameManager.instance.GetState() == GameState.SelectedUnit)
        {
            GameManager.instance.SetState(GameState.Waiting);

            //Start aiming
            doingActionShoot = true;
            possibleTargets = DetermineLineOfSight();
        }
    }

    private void Update()
    {
        if (doingActionShoot)
        {
            //Cancel Shoot
            if(Input.GetKeyDown(KeyCode.Escape) || Input.GetMouseButtonDown(1))
            {
                doingActionShoot = false;
                GameManager.instance.SetState(GameState.PlayerTurn);
                SetTargetsTargettable(false);
                return;
            }

            //Look for target that is clicked
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            GameObject hoveredTarget = null;

            if (Physics.Raycast(ray, out hit, 1000, unitMask | unitMask2))
            {
                hoveredTarget = hit.collider.gameObject;
            }

            if (Input.GetMouseButtonDown(0) && hoveredTarget != null)
            {
                doingActionShoot = false;
                SetTargetsTargettable(false);

                Debug.Log("Tryna Shoot " + hoveredTarget.name);
                foreach (PossibleTargets target in possibleTargets)
                {
                    if(target.target == hoveredTarget)
                    {
                        Debug.Log("Shot " + hoveredTarget.name);
                        // Shoot trge that is allowed to be shot
                        ShootManager.instance.RollDice(BCI, target.target.GetComponent<BaseCharacterInfo>(), target.inCover);
                        BCI.SpendAp(ApCost);
                        GameManager.instance.TurnOffPanel();
                    }
                }
                StartCoroutine(WaitForDice());
            }
        }
    }

    private IEnumerator WaitForDice()
    {
        yield return new WaitUntil(() => ShootManager.instance.FightStillGoing);

        yield return new WaitForSeconds(3f);
        GameManager.instance.CheckIfUnitDone();
    }

    public List<PossibleTargets> DetermineLineOfSight()
    {
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
        possibleTargets.Clear();

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
        return possibleTargets;
    }

    public void SetTargetsTargettable(bool yes)
    {
        for (int i = 0; i < possibleTargets.Count; i++)
        {
            possibleTargets[i].target.GetComponent<BaseCharacterInfo>().SetTargetable(yes);
        }
    }
}
[System.Serializable]
public class PossibleTargets
{
    public bool inCover;
    public GameObject target;

    public PossibleTargets(bool inCover, GameObject target)
    {
        this.inCover = inCover;
        this.target = target;
    }
}
