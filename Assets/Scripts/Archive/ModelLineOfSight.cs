using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ModelLineOfSight : MonoBehaviour
{
    [SerializeField] List<Transform> bodyParts;
    [SerializeField] Transform headPosition;
    [SerializeField] bool test;

    private List<GameObject> possileTargets = new List<GameObject>();

    private void Start()
    {
        if(test)
            StartCoroutine(Test());
    }
    private IEnumerator Test()
    {
        yield return new WaitForSeconds(1f);
        FindLineOfSight();
    }

    public List<GameObject> FindLineOfSight()
    {
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Model");
        possileTargets.Clear();

        foreach (GameObject enemy in enemies)
        {
            ModelLineOfSight currentEnemy = enemy.GetComponent<ModelLineOfSight>();
            //Draw lines
            for (int i = 0; i < currentEnemy.GetBodyParts().Count; i++)
            {
                Vector3 direction = currentEnemy.GetBodyParts()[i].position - headPosition.position;
                Ray ray = new Ray(headPosition.position, direction);
                RaycastHit hit;

                if(Physics.Raycast(ray, out hit, 1000))
                {
                    if(hit.collider.tag == "Terrain")
                    {
                        Debug.DrawRay(headPosition.position, direction, Color.yellow, 10f);
                    }
                    else
                    {
                        Debug.DrawRay(headPosition.position, direction, Color.red, 10f);
                        possileTargets.Add(enemy);
                        break;
                    }
                }
            }
        }
        return possileTargets;
    }

    public List<Transform> GetBodyParts()
    {
        return bodyParts;
    }
}
