using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum GameS
{
    Busy,
    Nothing,
    SelectedUnit,
    MovingModel,
    Targetting
}
public class GameMaster : MonoBehaviour
{
    [SerializeField] LayerMask unit;
    private GameObject selectedUnit;

    [SerializeField] GameObject modelSelectedPanel;

    [SerializeField] GameS state;

    [SerializeField] List<GameObject> selectableTargets;

    public static GameMaster instance;

    private void Awake()
    {
        if(instance != null)
        {
            Debug.Log("2 gm");
            return;
        }
        instance = this;
    }
    private void Start()
    {
        state = GameS.Nothing;
    }
    private void Update()
    {
        if (state == GameS.Nothing)
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            GameObject hoveredTarget = null;

            if (Physics.Raycast(ray, out hit, 1000, unit))
            {
                hoveredTarget = hit.collider.gameObject;
            }

            if (Input.GetMouseButtonDown(0) && hoveredTarget != null)
            {
                selectedUnit = hoveredTarget;
                modelSelectedPanel.SetActive(true);
                state = GameS.SelectedUnit;
            }
        }
        else if (state == GameS.MovingModel)
        {
            if (Input.GetMouseButtonDown(0))
            {
                DoMove();
            }
            if (Input.GetMouseButtonDown(1) || Input.GetKeyDown(KeyCode.Escape))
            {
                CancelMove();
            }
        }
        else if(state == GameS.SelectedUnit)
        {
            if (Input.GetMouseButtonDown(1) || Input.GetKeyDown(KeyCode.Escape))
            {
                selectedUnit = null;
                modelSelectedPanel.SetActive(false);
                state = GameS.Nothing;
            }
            
            //Button Buttons
            if (Input.GetKeyDown(KeyCode.Alpha1))
            {
                // Move
                SelectMoveOption();
            }
            if (Input.GetKeyDown(KeyCode.Alpha2))
            {
                // Shoot
                SelectShootOption();
            }
        }
        else if(state == GameS.Targetting)
        {
            // Select target
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            GameObject hoveredTarget = null;

            if (Physics.Raycast(ray, out hit, 1000, unit))
            {
                hoveredTarget = hit.collider.gameObject;
            }

            if (Input.GetMouseButtonDown(0) && hoveredTarget != null)
            {
                if (selectableTargets.Contains(hoveredTarget))
                {
                    Debug.Log("Shooting guy! : " + hoveredTarget.name);
                    SpendAp(1);
                }
                state = GameS.Nothing;
                selectedUnit = null;
                modelSelectedPanel.SetActive(false);
            }
        }
        if (Input.GetKeyDown(KeyCode.Escape) && state != GameS.Busy)
        {
            state = GameS.Nothing;
            selectedUnit = null;
            modelSelectedPanel.SetActive(false);
        }
    }

    private void SpendAp(int amount)
    {
        selectedUnit.GetComponent<APHandler>().SpendAp(amount);
    }
    private bool CheckAp(int amount)
    {
        return selectedUnit.GetComponent<APHandler>().CheckAp(amount);
    }

    public void SelectMoveOption()
    {
        if(selectedUnit != null && CheckAp(1))
        {
            selectedUnit.GetComponent<ModelMove>().SetTrueMoving();
            state = GameS.MovingModel;
            modelSelectedPanel.SetActive(false);
        }
    }
    public void SelectShootOption()
    {
        if (CheckAp(1))
        {
            selectableTargets.Clear();
            selectableTargets = selectedUnit.GetComponent<ModelLineOfSight>().FindLineOfSight();
            state = GameS.Targetting;
        }
    }
    private void DoMove()
    {
        if(selectedUnit != null && state == GameS.MovingModel)
        {
            SpendAp(1);
            state = GameS.Nothing;
            selectedUnit.GetComponent<ModelMove>().SetFalseMoving();
            selectedUnit = null;
            modelSelectedPanel.SetActive(false);
        }
    }
    private void CancelMove()
    {
        if (selectedUnit != null && state == GameS.MovingModel)
        {
            state = GameS.Nothing;
            selectedUnit.GetComponent<ModelMove>().CancelMove();
            selectedUnit = null;
            modelSelectedPanel.SetActive(false);
        }
    }

    public void SelectAction()
    {
        state = GameS.Busy;
        modelSelectedPanel.SetActive(false);
    }
    public void SetState(GameS newState)
    {
        state = newState;
    }
}
