using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMove : MonoBehaviour
{
    [SerializeField] GameObject mainCam;
    [SerializeField] float speed;

    private bool canMove = true;

    private bool onZ;
    // Start is called before the first frame update
    private void Update()
    {
        if (canMove)
        {
            float x = Input.GetAxis("Horizontal") * Time.deltaTime * speed;
            float z = Input.GetAxis("Vertical") * Time.deltaTime * speed;

            transform.Translate(x, 0, z, Space.World);

            if (Input.GetKeyDown(KeyCode.Q))
            {

            }
        }
    }

    public void SetMove(bool canMove)
    {
        this.canMove = canMove;
    }
}
