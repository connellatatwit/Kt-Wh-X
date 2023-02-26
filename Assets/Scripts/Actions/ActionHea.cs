using System.Collections.Generic;
using UnityEngine;

public class ActionHea : MonoBehaviour, IActions
{
    [SerializeField] int healAmount = 1;
    private BaseCharacterInfo BCI;
    [HideInInspector]
    [SerializeField] List<GameObject> possibleTargets = new List<GameObject>();
    [SerializeField] LayerMask unitMask = (1 << 11);
    [SerializeField] LayerMask unitMask2 = (1 << 10);
    private bool isHealing = false;
    public int ApCost => 1;

    public string ActionName => "Heal";

    private void Start()
    {
        BCI = GetComponent<BaseCharacterInfo>();
    }
    private void Update()
    {
        if (isHealing)
        {
            //Cancel Shoot
            if (Input.GetKeyDown(KeyCode.Escape) || Input.GetMouseButtonDown(1))
            {
                isHealing = false;
                GameManager.instance.SetState(GameState.SelectedUnit);
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
                isHealing = false;
                SetTargetsTargettable(false);

                Debug.Log("Tryna heal " + hoveredTarget.name);
                foreach (GameObject target in possibleTargets)
                {
                    if (target == hoveredTarget)
                    {
                        Debug.Log("Healed " + hoveredTarget.name);
                        target.GetComponent<BaseCharacterInfo>().HealDamage(healAmount);
                        // Shoot trge that is allowed to be shot
                        BCI.SpendAp(ApCost);
                        GameManager.instance.TurnOffPanel();
                    }
                }
                GameManager.instance.CheckIfUnitDone();
            }
        }
    }
    public void StartAction()
    {
        if(GameManager.instance.GetState() == GameState.SelectedUnit)
        {
            GameManager.instance.SetState(GameState.Waiting);
            GetLineOfSight();
            isHealing = true;
        }
    }

    private void GetLineOfSight()
    {
        GameObject[] allies = GameObject.FindGameObjectsWithTag("Ally");
        possibleTargets.Clear();

        foreach (GameObject target in allies)
        {
            BaseCharacterInfo currentEnemy = target.GetComponent<BaseCharacterInfo>();
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
            }
            if (inLineOfSight) // If in line of sight then add target
            {
                possibleTargets.Add(target);
            }
        }
        SetTargetsTargettable(true);
    }
    public void SetTargetsTargettable(bool yes)
    {
        for (int i = 0; i < possibleTargets.Count; i++)
        {
            possibleTargets[i].GetComponent<BaseCharacterInfo>().SetTargetable(yes);
        }
    }
}
