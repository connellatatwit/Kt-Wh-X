using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActionMelee : MonoBehaviour, IActions
{
    [SerializeField] Transform basePosition;
    private bool doingMelee = false;
    private BaseCharacterInfo BCI;

    [SerializeField] List<GameObject> possibleTargets;
    [SerializeField] LayerMask unitMask = (1 << 11);
    [SerializeField] LayerMask unitMask2 = (1 << 10);

    public int ApCost => 1;

    public string ActionName => "Melee";

    private void Start()
    {
        BCI = GetComponent<BaseCharacterInfo>();
    }
    public void StartAction()
    {
        // Range is 1f + base size
        if(GameManager.instance.GetState() == GameState.SelectedUnit)
        {
            if (EnemyCloseEnough())
            {
                GameManager.instance.SetState(GameState.Waiting);
                Debug.Log("Someone close enough");
                doingMelee = true;
            }
        }
    }

    private void Update()
    {
        if (doingMelee)
        {
            //Cancel Mellee
            if (Input.GetKeyDown(KeyCode.Escape) || Input.GetMouseButtonDown(1))
            {
                doingMelee = false;
                GameManager.instance.SetState(GameState.PlayerTurn);
                SetTargetsTargettable(false);
                return;
            }

            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            GameObject hoveredTarget = null;

            if (Physics.Raycast(ray, out hit, 10000, 1 << LayerMask.NameToLayer("Model")))
            {
                hoveredTarget = hit.collider.gameObject;
            }

            if (Input.GetMouseButtonDown(0) && hoveredTarget != null)
            {
                Debug.Log("Clicked guy " + hoveredTarget.name);
                doingMelee = false;
                SetTargetsTargettable(false);

                Debug.Log("Tryna Melee " + hoveredTarget.name);
                if (possibleTargets.Contains(hoveredTarget))
                {
                    Debug.Log("Smacked " + hoveredTarget.name);
                    // Shoot trge that is allowed to be shot
                    MeleeManager.instance.RollDice(BCI, hoveredTarget.GetComponent<BaseCharacterInfo>());
                    BCI.SpendAp(ApCost);
                    GameManager.instance.TurnOffPanel();
                }
                StartCoroutine(WaitForDice());
            }
        }
    }
    private IEnumerator WaitForDice()
    {
        yield return new WaitUntil(() => MeleeManager.instance.FightStillGoing);

        yield return new WaitForSeconds(2.5f);
        GameManager.instance.CheckIfUnitDone();
    }

    private bool EnemyCloseEnough()
    {
        possibleTargets.Clear();
        Ray ray = new Ray(basePosition.position, Vector3.down);
        RaycastHit hit;

        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
        bool closeEnough = false;
        foreach (GameObject enemy in enemies)
        {
            if (Physics.Raycast(ray, out hit, 1000f)) // Center of model
            {
                if (Vector3.Distance(hit.point, enemy.transform.position) < (2.1f + BCI.BaseSize + enemy.GetComponent<BaseCharacterInfo>().BaseSize))
                {
                    closeEnough = true;
                    possibleTargets.Add(enemy);
                }
            }
        }
        SetTargetsTargettable(true);
        return closeEnough;
    }
    public void SetTargetsTargettable(bool yes)
    {
        for (int i = 0; i < possibleTargets.Count; i++)
        {
            possibleTargets[i].GetComponent<BaseCharacterInfo>().SetTargetable(yes);
        }
    }
}
