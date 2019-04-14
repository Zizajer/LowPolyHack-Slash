using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlockControler : MonoBehaviour
{
    public Transform target;
    public int distanceToCheck = 35;
    float distance;
    // Use this for initialization
    void Start()
    {
        Component flockChildren = GetComponentInChildren<FlockUnit>();
        transform.position = flockChildren.transform.position;
        distance = 0;
    }
    
    void Update()
    {
        distance = Vector3.Distance(target.position, transform.position);
        Ray ray = new Ray(transform.position, target.position - transform.position);
        RaycastHit hit;
        if (distance < distanceToCheck)
        {
            if (Physics.Raycast(ray, out hit))
            {
                if (hit.transform.tag == target.tag)
                {
                    transform.position = target.position + new Vector3(0, 5, 0);
                }
            }
        }
    }

}
