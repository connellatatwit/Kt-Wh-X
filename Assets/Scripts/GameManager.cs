using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public enum GameState
{
    Busy,
    PlayerSettingUnits,
    SettingUnit,
    Waiting,
    PlayerTurn,
    SelectedUnit,
    EnemyTurn
}

public class GameManager : MonoBehaviour
{
    [Header("Unity Stuff")]
    [SerializeField] GameState state;
    [SerializeField] LayerMask unitMask;
    [SerializeField] GameObject unitSelectedPanel;
    [SerializeField] List<ActionButton> actionButtons;

    [SerializeField] GameObject selectedUnit;

    [SerializeField] Image turnImage;

    [SerializeField] StatManager sM;

    [SerializeField] VirtualCamMove cameraSystem;

    [Header("Initial Set up Stuff")]
    [SerializeField] Transform unitButtonParent;
    [SerializeField] GameObject unitButtonPrefab;
    [SerializeField] List<UnitButton> unitButtons;
    private LayerMask floorMask = (1 << 14);
    [SerializeField] World world;

    [Header("Test Set Enemies and Allies")]
    [SerializeField] List<GameObject> PresetTestEnemies;
    [SerializeField] List<IEnemy> enemies = new List<IEnemy>();
    [SerializeField] List<BaseCharacterInfo> allies;
    private bool enemeisDone;
    private bool alliesDone;

    public static GameManager instance;

    private void Awake()
    {
        if (instance != null)
        {
            Debug.Log("2 GM's --");
            return;
        }

        instance = this;
    }

    private void Start()
    {
        state = GameState.PlayerSettingUnits;
        enemeisDone = false;
        alliesDone = false;
        turnImage.color = Color.green;

        ClearUnitPanel();
        //TEMP:
        for (int i = 0; i < PresetTestEnemies.Count; i++)
        {
            enemies.Add(PresetTestEnemies[i].GetComponent<IEnemy>());
        }

        //TEMP:

        InitUnitButtons();
    }

    private void Update()
    {
        if (state == GameState.PlayerTurn)
        {
            //Click on unit then select it
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            GameObject hoveredTarget = null;

            if (Physics.Raycast(ray, out hit, 1000, unitMask))
            {
                hoveredTarget = hit.collider.gameObject;
            }

            if (Input.GetMouseButtonDown(0))
            {
                if (!EventSystem.current.IsPointerOverGameObject())
                {
                    //Clear Panel of any possible stuff
                    ClearUnitPanel();
                }
                if (hoveredTarget != null)
                {
                    if (hoveredTarget.GetComponent<BaseCharacterInfo>().CheckReady()) // Unit has AP
                    {
                        selectedUnit = hoveredTarget;
                        // Set up panel for new Unit
                        InitUnitPanel();
                        state = GameState.SelectedUnit;
                    }
                    else
                        ClearUnitPanel(); // Unit doesnt have ap so stop
                }
            }
            if (Input.GetMouseButtonDown(1))
            {
                //Clear Panel of any possible stuff
                ClearUnitPanel();
            }
        }
        else if (state == GameState.SettingUnit)
        {
            // Allow player to set up units... 
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit, 1000, floorMask))
            {
                Vector3 mousePosition = hit.point;
                mousePosition = new Vector3(mousePosition.x, mousePosition.y, mousePosition.z);

                Vector3 tempPos;

                var allowedPos = mousePosition - world.center.position;
                Debug.Log(allowedPos);
                allowedPos = Vector3.ClampMagnitude(allowedPos, world.spawnArea.x); // Calculates circle
                tempPos = world.center.position + allowedPos;
                tempPos = new Vector3(tempPos.x, tempPos.y + 2f, tempPos.z);
                Debug.Log(tempPos);
                selectedUnit.transform.position = Vector3.Lerp(selectedUnit.transform.position, tempPos, 3f * Time.deltaTime);
            }
            if (Input.GetMouseButtonDown(0))
            {
                MakeSelectedUnitMovable(false);
                Debug.Log("here");
                selectedUnit = null;
                if (CheckIfAnyButtonsLeft())
                {
                    state = GameState.PlayerSettingUnits;
                }
                else
                    state = GameState.PlayerTurn;
            }
        }
    }
    private void InitUnitButtons()
    {
        for (int i = 0; i < allies.Count; i++)
        {
            GameObject temp = Instantiate(unitButtonPrefab, unitButtonParent);
            unitButtons.Add(temp.GetComponent<UnitButton>());
        }
        for (int i = 0; i < unitButtons.Count; i++)
        {
            unitButtons[i].SetTargetUnit(allies[i].transform);
        }
    }
    public void StartSettingUnit(GameObject targetObject)
    {
        selectedUnit = targetObject.gameObject;
        MakeSelectedUnitMovable(true);
        selectedUnit.transform.position = world.center.position;
    }
    private void MakeSelectedUnitMovable(bool yes)
    {
        state = GameState.SettingUnit;
        selectedUnit.GetComponent<Collider>().isTrigger = yes;
        selectedUnit.GetComponent<Rigidbody>().useGravity = !yes; // Remove Gravity for ease of moving
        selectedUnit.GetComponent<Rigidbody>().velocity = Vector3.zero;
    }
    private bool CheckIfAnyButtonsLeft()
    {
        if (unitButtonParent.childCount > 0)
            return true;
        else
            return false;
    }
    private void InitUnitPanel()
    {
        sM.SetInformation(selectedUnit.GetComponent<BaseCharacterInfo>());
        //Turn on panel
        unitSelectedPanel.SetActive(true);
        //Grab Selected units scripts
        IActions[] unitsActions = selectedUnit.GetComponents<IActions>();
        //Fill panels buttons with on click events of selected units scripts.
        for (int i = 0; i < unitsActions.Length; i++)
        {
            actionButtons[i].gameObject.SetActive(true);
            actionButtons[i].InitButton(unitsActions[i]);
            if(selectedUnit.GetComponent<BaseCharacterInfo>().Ap < unitsActions[i].ApCost)
            {
                actionButtons[i].SetInteract(false);
            }
            else
            {
                actionButtons[i].SetInteract(true);
            }
        }
    }
    private void ClearUnitPanel()
    {
        sM.ClearInformation();
        selectedUnit = null;
        unitSelectedPanel.SetActive(false);
        for (int i = 0; i < actionButtons.Count; i++)
        {
            actionButtons[i].gameObject.SetActive(false);
        }
    }

    public void SetState(GameState newState)
    {
        state = newState;
    }

    public void EnemyTurnDone()
    {
        // Check if allies are done
        if (CheckAlliesActive()) // An ally is ready
        {
            AllyTurn();
        }
        else if (CheckEnemiesActive())
        { // no ally is ready but an enemy is, then do an overworked shot or pass or heal
            // TEMP -----
            DoEnemyTurn(); // Do an enemy turn
            // TEMP -----
        }
        else // No enemy or ally is ready
        {
            NextRound();
        }
    }
    public void CheckIfUnitDone()
    {
        state = GameState.Busy;
        if(selectedUnit.GetComponent<BaseCharacterInfo>().Ap <= 0) // iF current unit is done, then it is time to check the enemies
        {
            Debug.Log("Unit turn end");
            unitSelectedPanel.SetActive(false);
            state = GameState.EnemyTurn;
            if (CheckEnemiesActive()) // If an enemy is active
            {
                DoEnemyTurn(); // Do enemy turn
            }
            else if (CheckAlliesActive()) // if no enemy is ready and an ally is ready then an enemy does an Overworked shot; FOR NOW IT JUST GOES TO PLAYER TURN
            {
                //TEMP -----
                AllyTurn();
                //TEMP -----
            }
            else // No one is ready
                NextRound();
        }
        else // Continue this units turn.
        {
            state = GameState.SelectedUnit;
            InitUnitPanel();
            //Check if any more player units can go TODO   
        }
    }
    private bool CheckAlliesActive()
    {
        bool active = false;
        foreach (BaseCharacterInfo ally in allies)
        {
            if (ally.CheckReady())
            {
                active = true;
                break;
            }
        }
        return active;
    }
    private bool CheckEnemiesActive()
    {
        bool active = false;
        //enemies.RemoveAll(item => item == null);
        for (int i = enemies.Count - 1; i >= 0; i--) // Clear enemies
        {
            if (enemies[i].ToString() == "null")
                enemies.RemoveAt(i);
        }

        foreach (IEnemy enemy in enemies)
        {
            if (enemy.BCI.CheckReady())
            {
                active = true;
                break;
            }
        }
        return active;
    }
    private void DoEnemyTurn()
    {
        turnImage.color = Color.red;
        foreach (IEnemy enemy in enemies)
        {
            if (enemy.BCI.CheckReady())
            {
                cameraSystem.SetTarget(enemy.transform);
                cameraSystem.SetState(CamState.FollowEnemy);
                enemy.DoTurn();
                break;
            }
        }
    }
    private void AllyTurn()
    {
        cameraSystem.SetState(CamState.Free);
        state = GameState.PlayerTurn;
        turnImage.color = Color.green;
        ClearUnitPanel();
    }
    private void NextRound()
    {
        Debug.Log("Round Done");
        // Temp -----
        AllyTurn();
        // Temp-----
        InitEnemies();
        InitAllies();
    }
    private void InitAllies()
    {
        foreach (BaseCharacterInfo ally in allies)
        {
            ally.SetAp();
        }
    }
    private void InitEnemies()
    {
        foreach (IEnemy enemy in enemies)
        {
            enemy.Init();
        }
    }

    public void TurnOffPanel()
    {
        unitSelectedPanel.SetActive(false);
    }

    public void SetState(int newState)
    {
        SetState((GameState)newState);
    }
    public GameState GetState()
    {
        return state;
    }
    public BaseCharacterInfo GetCurrentCharacter()
    {
        return selectedUnit.GetComponent<BaseCharacterInfo>();
    }
}
