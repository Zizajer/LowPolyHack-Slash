using Devdog.General;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.AI;

public class FlyingEnemy : MonoBehaviour {

    public Transform target;
    //if more then better sense to find player
    public int distanceToReact;
    private float distanceToPlayer;

    public float health = 100;
    public float damage = 3;

    private Animator animator;
    
    private bool isAttack;
    private bool isFlying;
    private bool isDeath;

    // Use this for initialization
    void Start () {
        isAttack = false;
        isFlying = true;
        isDeath = false;
        animator = GetComponent<Animator>();
    }
	
	// Update is called once per frame
	void Update() {
        distanceToPlayer = Vector3.Distance(target.position, transform.position);

        if (distanceToPlayer < distanceToReact)
        {
            transform.LookAt(target);
            isFlying = false;
            isAttack = true;
        }
        else
        {
            isFlying = true;
            isAttack = false;
        }

        if (health < 1)
        {
            isDeath = true;
            isFlying = false;
            isAttack = false;
        }

        animator.SetBool("isAttack", isAttack);
        animator.SetBool("isFlying", isFlying);
        animator.SetBool("isDeath", isDeath);
    }

    public void DealDamage() {
        PlayerManager.instance.currentPlayer.inventoryPlayer.stats.Get("Default", "Health").SetCurrentValueRaw(PlayerManager.instance.currentPlayer.inventoryPlayer.stats.Get("Default", "Health").currentValue - ((100 - PlayerManager.instance.currentPlayer.inventoryPlayer.stats.Get("Default", "Armor").currentValue) / 100) * Random.Range(damage - 2, damage));
        Debug.Log(PlayerManager.instance.currentPlayer.inventoryPlayer.stats.Get("Default", "Health").currentValue);
    }

    public void GetHurt()
    {
        health -= PlayerManager.instance.currentPlayer.inventoryPlayer.stats.Get("Default", "Damage").currentValue;
        Debug.Log("Bat's health remaing : " + health.ToString());
    }

    public void Die()
    {
        this.gameObject.SetActive(false);
    }


}
