using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyNormal : MonoBehaviour {

    public Transform target;
    //if more then better sense to find player
    public int distanceToReact;
    private float distanceToPlayer;
    private Animator animator;

    private NavMeshAgent navMeshAgent;

    private bool isRunning;
    private bool isAttack;

    // Use this for initialization
    void Start () {
        isRunning = false;
        isAttack = false;
        animator = GetComponent<Animator>();
        navMeshAgent = GetComponent<NavMeshAgent>();
    }
	
	// Update is called once per frame
	void FixedUpdate() {
        distanceToPlayer = Vector3.Distance(target.position, transform.position);
        if (distanceToPlayer < distanceToReact)
        {
            navMeshAgent.SetDestination(target.position);
            isRunning = true;
        }
        else
        {
            navMeshAgent.ResetPath();
            isRunning = false;
        }


        if (navMeshAgent.remainingDistance <= navMeshAgent.stoppingDistance && distanceToPlayer <= navMeshAgent.stoppingDistance)
        {
            transform.LookAt(target);
            isRunning = false;
            isAttack = true;
        }
        else
        {
            isAttack = false;
        }

        animator.SetBool("isRunning", isRunning);
        animator.SetBool("isAttack", isAttack);
    }
}
