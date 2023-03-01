using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public enum CamState
{
    Free,
    FollowEnemy
}
public class VirtualCamMove : MonoBehaviour
{
    [SerializeField] CinemachineVirtualCamera CMVC;
    [SerializeField] float minY = 1f;
    [SerializeField] float maxY = 7;
    [SerializeField] float speed;
    [SerializeField] float rotateSpeed;

    private bool dragPanMoveActive;
    private Vector2 lastMousePos;

    private Vector3 followOffset;

    private CamState state;
    private Transform targetEnemy;

    private void Awake()
    {
        followOffset = CMVC.GetCinemachineComponent<CinemachineTransposer>().m_FollowOffset;
        CMVC.GetCinemachineComponent<CinemachineTransposer>().m_FollowOffset = Vector3.Lerp(CMVC.GetCinemachineComponent<CinemachineTransposer>().m_FollowOffset, new Vector3(0,6f,0), Time.deltaTime * 3f);
    }

    private void Update()
    {
        if (state == CamState.Free)
        {
            Vector3 iputDir = new Vector3(0, 0, 0);

            if (Input.GetKey(KeyCode.W)) iputDir.z = +1f;
            if (Input.GetKey(KeyCode.S)) iputDir.z = -1f;
            if (Input.GetKey(KeyCode.A)) iputDir.x = -1f;
            if (Input.GetKey(KeyCode.D)) iputDir.x = +1f;
            if (Input.GetMouseButtonDown(1))
            {
                dragPanMoveActive = true;
                lastMousePos = Input.mousePosition;
            }
            if (Input.GetMouseButtonUp(1))
            {
                dragPanMoveActive = false;
            }
            if (dragPanMoveActive)
            {
                Vector2 mouseMoveDelt = (Vector2)Input.mousePosition - lastMousePos;
                float dragPanSpeed = 1f;
                iputDir.x = -mouseMoveDelt.x * dragPanSpeed;
                iputDir.z = -mouseMoveDelt.y * dragPanSpeed;

                lastMousePos = Input.mousePosition;
            }

            Vector3 moveDir = transform.forward * iputDir.z + transform.right * iputDir.x;
            transform.position += moveDir * speed * Time.deltaTime;



            float rotDir = 0f;
            if (Input.GetKey(KeyCode.Q)) rotDir += 1f;
            if (Input.GetKey(KeyCode.E)) rotDir -= 1f;

            transform.eulerAngles += new Vector3(0, rotDir * rotateSpeed * Time.deltaTime, 0);

            HandleCamZoom();
        }
        else if(state == CamState.FollowEnemy)
        {
            if(targetEnemy != null)
            {
                transform.position = targetEnemy.position;

                float rotDir = 0f;
                if (Input.GetKey(KeyCode.Q)) rotDir += 1f;
                if (Input.GetKey(KeyCode.E)) rotDir -= 1f;

                transform.eulerAngles += new Vector3(0, rotDir * rotateSpeed * Time.deltaTime, 0);
            }
        }
    }

    public void SetState(CamState state)
    {
        this.state = state;
    }
    public void SetTarget(Transform targetEnemy)
    {
        this.targetEnemy = targetEnemy;
    }

    private void HandleCamZoom()
    {
        float zoomAmount = 1f;
        if(Input.mouseScrollDelta.y > 0)
        {
            followOffset.y -= zoomAmount;
        }
        if (Input.mouseScrollDelta.y < 0)
        {
            followOffset.y += zoomAmount;
        }

        followOffset.y = Mathf.Clamp(followOffset.y, minY, maxY);

        float zoomSpeed = 2f;
        CMVC.GetCinemachineComponent<CinemachineTransposer>().m_FollowOffset = Vector3.Lerp(CMVC.GetCinemachineComponent<CinemachineTransposer>().m_FollowOffset, followOffset, Time.deltaTime * zoomSpeed);
    }
}
