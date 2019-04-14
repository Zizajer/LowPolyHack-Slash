using Devdog.General;
using Devdog.InventoryPro;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;


public class EnemyFuzzyWithFOV : MonoBehaviour
{

    public Transform target;
    private float distanceToPlayer;

    public float FieldOfView = 45;
    public float ViewDistance = 100;
    public float health = 100;
    public float damage = 1;
    private float CheckNearbyDistance = 25;
    private Animator animator;
    private Vector3 onGoingPosition = Vector3.zero;

    WayPointManager wayPointManager;

    string operation;

    private NavMeshAgent navMeshAgent;

    private Ray ray;
    private Vector3 rayDirection;

    private bool inFOV;
    private bool duringOperation;
    private bool isRunning;
    private bool isAttack;
    private bool isDeath;

    // Use this for initialization
    void Start()
    {
        isRunning = false;
        isAttack = false;
        isDeath = false;
        inFOV = false;
        duringOperation = false;
        wayPointManager = GetComponent<WayPointManager>();
        animator = GetComponent<Animator>();
        navMeshAgent = GetComponent<NavMeshAgent>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        RaycastHit hit;
        distanceToPlayer = Vector3.Distance(target.position, transform.position);
        ray = new Ray(transform.position, target.position - transform.position);
        rayDirection = target.position - transform.position;



        if ((Vector3.Angle(rayDirection, transform.forward)) < FieldOfView)
        {
            if (Physics.Raycast(transform.position, rayDirection, out hit, ViewDistance))
            {
                if (hit.transform.tag == target.tag)
                {
                    inFOV = true;
                }
                else inFOV = false;
            }
        }

        if(distanceToPlayer < CheckNearbyDistance)
        {
            if (Physics.Raycast(ray, out hit))
            {
                if (hit.transform.tag == target.tag)
                {
                    inFOV = true;
                }

            }
        }

        if (distanceToPlayer > ViewDistance)
        {
            inFOV = false;
        }

        if (inFOV)
        {
            operation = wayPointManager.getOperation();
            if (operation.Equals("Attack")) { 
                navMeshAgent.SetDestination(target.position);
                isRunning = true;
            }
            else if (operation.Equals("Retreat"))
            {
                onGoingPosition = wayPointManager.getRetreatPosition(target.position, transform.position);
                navMeshAgent.SetDestination(onGoingPosition);
                isRunning = true;
                inFOV = false;
            }
            else if (operation.Equals("FindItem"))
            {
                onGoingPosition = wayPointManager.findNearestTypeOfItem(transform.position, "Weapon", CheckNearbyDistance * 3);

                if (onGoingPosition != Vector3.zero)
                {
                    navMeshAgent.SetDestination(onGoingPosition);
                    isRunning = true;
                    inFOV = false;
                }
                else
                {
                    navMeshAgent.SetDestination(target.position);
                    isRunning = true;
                }
            }
            else if (operation.Equals("CallForHelp"))
            {
                onGoingPosition = wayPointManager.getMeetingPoint(transform.position, target.position, transform.tag, CheckNearbyDistance * 3);
                navMeshAgent.SetDestination(onGoingPosition);
                isRunning = true;
                inFOV = false;
            }
        }
        else
        {
            isRunning = false;
            navMeshAgent.ResetPath();
        }


        if (navMeshAgent.remainingDistance <= navMeshAgent.stoppingDistance && distanceToPlayer <= navMeshAgent.stoppingDistance)
        {
            navMeshAgent.ResetPath();
            isRunning = false;
            isAttack = true;
        }
        else
        {
            isAttack = false;
        }

        if (health < 1)
        {
            isDeath = true;
            isRunning = false;
            isAttack = false;
        }

        Vector3 velocity = navMeshAgent.velocity;
        if (velocity != Vector3.zero)
            isRunning = true;

        animator.SetBool("isRunning", isRunning);
        animator.SetBool("isAttack", isAttack);
        animator.SetBool("isDeath", isDeath);
    }

    //fovlines wrong calibrated TO DO fix
    private void OnDrawGizmos()
    {
        Vector3 fovLine1 = Quaternion.AngleAxis(FieldOfView/2,transform.up) * transform.forward * ViewDistance;
        Vector3 fovLine2 = Quaternion.AngleAxis(-FieldOfView/2, transform.up) * transform.forward * ViewDistance;

        Gizmos.color = Color.blue;
        Gizmos.DrawRay(transform.position, fovLine1);
        Gizmos.DrawRay(transform.position, fovLine2);

        
        Gizmos.color = Color.red;
        Gizmos.DrawRay(transform.position, (target.position - transform.position).normalized * ViewDistance);

        Gizmos.color = Color.black;
        Gizmos.DrawRay(transform.position, transform.forward * ViewDistance);
    }

    public void DealDamage()
    {
        PlayerManager.instance.currentPlayer.inventoryPlayer.stats.Get("Default", "Health").SetCurrentValueRaw(PlayerManager.instance.currentPlayer.inventoryPlayer.stats.Get("Default", "Health").currentValue 
            - ((100 - PlayerManager.instance.currentPlayer.inventoryPlayer.stats.Get("Default", "Armor").currentValue)/100) * damage);
        Debug.Log(PlayerManager.instance.currentPlayer.inventoryPlayer.stats.Get("Default", "Health").currentValue);
    }

    public void GetHurt()
    {
        health -= PlayerManager.instance.currentPlayer.inventoryPlayer.stats.Get("Default", "Damage").currentValue;
        Debug.Log("Goblin's health remaing : " + health.ToString());
    }

    public void Die()
    {
        this.gameObject.SetActive(false);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Weapon") {
            duringOperation = false;
            StatDecorator[] stats = other.gameObject.GetComponent<EquippableInventoryItem>().stats;
            foreach (var stat in stats)
            {
                Debug.Log(damage);
                if (stat.stat.name.Equals("Damage")) 
                    damage += stat.floatValue;
                Debug.Log(damage);
            }
            Destroy(other.gameObject);
        }
    }

    public void goToPosition(Vector3 position)
    {
        Debug.Log("otrzymałem metode do pójścia");
        Debug.Log(position);
        navMeshAgent.SetDestination(position);
    }

    public void attack() {
        Debug.Log("otrzymałem metode do ataku");
        navMeshAgent.SetDestination(target.position);
    }


}