using Devdog.General;
using Devdog.InventoryPro;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

public class KnightPlayer : MonoBehaviour {

    private Animator animator;
    public GameObject gameOverText;
    public Button exitButton;

    private Transform pointedEnemy;

    private NavMeshAgent navMeshAgent;
    
    private bool isRunning;
    private bool isAttack;

    void Start () {
        Time.timeScale = 1;
        isRunning = false;
        isAttack = false;
        animator = GetComponent<Animator>();
        navMeshAgent = GetComponent<NavMeshAgent>();
	}
	
	void Update () {

        RaycastHit hit;
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        if (Input.GetMouseButton(0))
        {
            if (Physics.Raycast(ray, out hit))
            {
                    navMeshAgent.SetDestination(hit.point);
                if (hit.transform.tag == "Enemy")
                {
                    pointedEnemy = hit.transform;
                    transform.LookAt(pointedEnemy);
                    if(Vector3.Distance(transform.position,pointedEnemy.position) < 5)
                    {
                        animator.CrossFade("Attack", 0.1f);
                    }
                }
                else
                    navMeshAgent.SetDestination(hit.point);
            }
        }

        if (checkIsInventoryOpen())
            navMeshAgent.ResetPath();

        if (navMeshAgent.remainingDistance <= navMeshAgent.stoppingDistance)
        {

            isRunning = false;
        }
        else
            isRunning = true;

        if (PlayerManager.instance.currentPlayer.inventoryPlayer.stats.Get("Default", "Health").currentValue < 1)
        {
            exitButton.gameObject.SetActive(true);
            gameOverText.SetActive(true);
            Time.timeScale = 0;

        }

        animator.SetBool("isRunning", isRunning);
        animator.SetBool("isAttack", isAttack);


        if (Input.GetKeyDown(KeyCode.D))
        {
            animator.CrossFade("Roll", 0.1f);
        }
	}

    public void DealDamage()
    {
        pointedEnemy.SendMessage("GetHurt");
    }

    private bool checkIsInventoryOpen()
    {
        bool isInventoryOpen = false;
        GameObject[] windows = GameObject.FindGameObjectsWithTag("Inventory");
        foreach (GameObject window in windows)
            if(window.GetComponent<Image>().IsActive())
                isInventoryOpen = true;

        return isInventoryOpen;
    }
}
