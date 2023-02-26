using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitInfoLookAtCamera : MonoBehaviour
{
    private GameObject cameraToLookAt;

    private void Start()
    {
        cameraToLookAt = Camera.main.gameObject;
    }
    void Update()
    {
        Vector3 v = cameraToLookAt.transform.position - transform.position;
        v.x = v.z = 0.0f;
        transform.LookAt(cameraToLookAt.transform.position - v);
        transform.rotation = cameraToLookAt.transform.rotation; // Take care about camera rotation  
    }
}

