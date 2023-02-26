using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ActionMove : MonoBehaviour, IActions
{
    private BaseCharacterInfo BCI;

    [SerializeField] float speed = 50;
    [SerializeField] int apCost;

    [SerializeField] LayerMask ignoreLayerOne = (1 << 10);
    [SerializeField] LayerMask ignoreLayerTwo = (1 << 11);
    [SerializeField] LayerMask ignoreLayerThree = (1 << 12);
    [SerializeField] LayerMask ignoreLayerFour = (1 << 8);

    [SerializeField] LayerMask wallMask = (1 << 13);
    [SerializeField] LayerMask floorMask = (1 << 14);

    [Header("Needs to be set every time")]
    [SerializeField] GameObject invisClose;
    [SerializeField] GameObject minusMSCanvas;
    [SerializeField] TextMeshProUGUI msCanvasText;
    [SerializeField] Transform basePosition;
    private BoxCollider hitbox;

    private float upDistance = 2f;

    private Vector3 startPosition;
    private Vector3 position;
    private Rigidbody rb;

    private bool doingActionMove = false;
    private bool canLand = true;

    public int ApCost => apCost;
    public string ActionName => "Move";

    private void Start()
    {
        BCI = GetComponent<BaseCharacterInfo>();
        rb = GetComponent<Rigidbody>();
        hitbox = GetComponent<BoxCollider>();
    }

    public void StartAction()
    {
        //GM STUFF
        if (GameManager.instance.GetState() == GameState.SelectedUnit)
        {
            //Set state
            GameManager.instance.SetState(GameState.Waiting);

            startPosition = transform.position + (new Vector3(0, .1f, 0)); // Set Start Pos
            rb.useGravity = false; // Remove Gravity for ease of moving
            doingActionMove = true; // Start the moving proces

            rb.constraints = RigidbodyConstraints.FreezeRotationY | RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;
            invisClose.SetActive(true);
        }
    }

    private void FixedUpdate()
    {
        if (doingActionMove)
        {
            rb.MovePosition(position);
        }
    }
    private void Update()
    {
        if (doingActionMove)
        {
            float moveDistance = BCI.MoveDistance;
            //Draw line to check if going over wall
            Vector3 offSetDown = transform.position - new Vector3(0, (upDistance - .1f), 0); // Because I lift the piece up, I need to lower the line to where it will be
            Vector3 dir = (offSetDown) - (startPosition);
            Ray ray2 = new Ray(startPosition, dir);
            Debug.DrawRay(startPosition, dir, Color.red);
            //RaycastHit hit2;
            float maxDistance = Vector3.Distance(offSetDown, startPosition);
            RaycastHit[] hits = Physics.RaycastAll(ray2, maxDistance, wallMask);
            if (hits.Length != 0)
            {
                int wallsHit = 0;
                for (int i = 0; i < hits.Length; i++)
                {
                    if (hits[i].collider.gameObject.layer == 13) // if layer is the 13 layer (The wall layer)
                    {
                        //If cross a wall then remove 2 movement
                        wallsHit++;
                        moveDistance -= 2;
                        msCanvasText.text = "-" + wallsHit*2 + " MS";
                        msCanvasText.gameObject.SetActive(true);
                        minusMSCanvas.transform.position = hits[i].point;
                    }
                }
            }
            else
            {
                msCanvasText.gameObject.SetActive(false);
            }

            // Move the unit
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit, 1000, ~ignoreLayerOne & ~ignoreLayerTwo & ~ignoreLayerThree & ~ignoreLayerFour))
            {
                Vector3 mousePosition = hit.point;
                mousePosition = new Vector3(mousePosition.x, mousePosition.y, mousePosition.z);

                Vector3 tempPos;

                var allowedPos = mousePosition - startPosition;
                allowedPos = Vector3.ClampMagnitude(allowedPos, moveDistance); // Calculates circle
                tempPos = startPosition + allowedPos;
                tempPos = new Vector3(tempPos.x, tempPos.y + upDistance, tempPos.z);
                position = Vector3.Lerp(transform.position, tempPos, speed * Time.deltaTime);
            }

            bool canLand2 = CheckLanding();
            // Wheck click, stop moving and tell GM that the move was cancelled or was done
            if (Input.GetMouseButtonDown(0) && canLand && canLand2)
            {
                FinishMove();
            }
            if (Input.GetMouseButtonDown(1))
            {
                CancelMove();
            }
        }
    }

    //Check if can be dropped
    private bool CheckLanding()
    {
        Ray ray = new Ray(basePosition.position, Vector3.down);
        RaycastHit hit;

        //Find distance of ground and put invis guy there
        if(Physics.Raycast(ray, out hit, 1000f, floorMask))
        {
            invisClose.transform.position = hit.point;
        }

        //Make rays from corners
        Ray[] rays = new Ray[5];
        rays[0] = new Ray(basePosition.position + new Vector3(hitbox.size.x/2, 0, 0), Vector3.down);
        rays[1] = new Ray(basePosition.position + new Vector3(-hitbox.size.x/2, 0, 0), Vector3.down);
        rays[2] = new Ray(basePosition.position + new Vector3(0, 0, -hitbox.size.z/2), Vector3.down);
        rays[3] = new Ray(basePosition.position + new Vector3(0, 0, -hitbox.size.z/2), Vector3.down);
        rays[4] = new Ray(basePosition.position, Vector3.down);

        Debug.DrawRay(basePosition.position + new Vector3(hitbox.size.x / 2, 0, 0), Vector3.down * 100, Color.green);
        Debug.DrawRay(basePosition.position + new Vector3(-hitbox.size.x / 2, 0, 0), Vector3.down * 100, Color.green);
        Debug.DrawRay(basePosition.position + new Vector3(0, 0, -hitbox.size.z / 2), Vector3.down * 100, Color.green);
        Debug.DrawRay(basePosition.position + new Vector3(0, 0, -hitbox.size.z / 2), Vector3.down * 100, Color.green);
        Debug.DrawRay(basePosition.position, Vector3.down * 100, Color.green);

        bool noLand = false;

        for (int i = 0; i < rays.Length; i++)
        {
            if(Physics.Raycast(rays[i], out hit, 1000f))
            {
                if (hit.collider.gameObject.layer == 13 || hit.collider.gameObject.layer == 11)
                {
                    noLand = true;
                    break;
                }
            }
        }
        
        return !noLand;
    }

    // Drop piece where it is
    private void FinishMove()
    {
        rb.useGravity = true; // Remove Gravity for ease of moving
        doingActionMove = false; // Start the moving proces
        rb.velocity = Vector3.zero;

        // TODO: This needs to check if the groud is a good plac eto stand needs a system in order to stand up\
        //Spend Ap
        BCI.SpendAp(apCost);

        //End Stuff
        GameManager.instance.CheckIfUnitDone();
        rb.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;
        invisClose.SetActive(false);
        msCanvasText.gameObject.SetActive(false);
    }

    //Drop piece where it was originally
    private void CancelMove()
    {
        rb.useGravity = true; // Remove Gravity for ease of moving
        doingActionMove = false; // Start the moving proces
        transform.position = startPosition;
        rb.velocity = Vector3.zero;

        // End stuff
        GameManager.instance.SetState(GameState.PlayerTurn);
        rb.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;
        invisClose.SetActive(false);
        msCanvasText.gameObject.SetActive(false);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
    }

    private void OnTriggerStay(Collider other)
    {
        canLand = false;
    }

    private void OnTriggerExit(Collider other)
    {
        canLand = true;
    }
}
