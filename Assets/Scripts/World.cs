using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class World : MonoBehaviour
{
    [SerializeField] public Vector3 spawnArea;
    [SerializeField] public Transform center;

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.white;

        Gizmos.DrawWireCube(center.position, spawnArea);
    }
}
