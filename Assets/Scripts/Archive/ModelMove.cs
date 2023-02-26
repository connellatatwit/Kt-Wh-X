using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ModelMove : MonoBehaviour
{
    [SerializeField] float tempRange = 5f;
    [SerializeField] float speed;
    [SerializeField] LayerMask ignoreLayer;
    [SerializeField] LayerMask ignoreLayerTwo;
    [SerializeField] LayerMask ignoreLayerThree;
 
    private Vector3 position;
    private Rigidbody rb;
    private bool clicked;
    private bool moving;
    private Vector3 startPosition;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        startPosition = transform.position;
    }
    private void OnMouseEnter()
    {
        //Highlight
    }
    private void FixedUpdate()
    {
        if (clicked)
        {
            rb.MovePosition(position);
        }
    }
    private void Update()
    {
        if (moving)
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit, 1000, ~ignoreLayer & ~ignoreLayerTwo & ~ignoreLayerThree))
            {
                Vector3 mousePosition = hit.point;
                mousePosition = new Vector3(mousePosition.x, 1.5f, mousePosition.z);

                Vector3 tempPos;

                var allowedPos = mousePosition - startPosition;
                allowedPos = Vector3.ClampMagnitude(allowedPos, tempRange);
                tempPos = startPosition + allowedPos;
                position = Vector3.Lerp(transform.position, tempPos, speed * Time.deltaTime);
            }
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(startPosition, tempRange);
    }

    public void SetTrueMoving()
    {
        moving = true;
        rb.useGravity = false;
        clicked = true;
        startPosition = transform.position;
    }
    public void SetFalseMoving()
    {
        moving = false;
        rb.useGravity = true;
        clicked = false;
    }
    public void CancelMove()
    {
        transform.position = startPosition;
        SetFalseMoving();
    }
}
