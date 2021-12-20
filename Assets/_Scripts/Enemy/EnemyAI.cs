using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Random = UnityEngine.Random;

public class EnemyAI : MonoBehaviour
{
    public static EnemyAI instance;
    public bool canAttack;

    private Animator animator;


    public float lookRadius = 10f;

    private Transform target;
    private NavMeshAgent agent;
    
    public LayerMask whatIsGround, whatIsPlayer;

    public Vector3 walkPoint;
    bool walkPointSet;
    public float walkPointRange;

    public float timeBetweenAttacks;
    bool alreadyAttacked;

    public float slightRange, atttackRange;
    public bool playerInSightRange, playerInAttackRange;

    public int damageCount=0;
    
    // Start is called before the first frame update
    void Start()
    {
        instance = this;
        agent = GetComponent<NavMeshAgent>();
        target = PlayerManager.instance.player.transform;
        animator = GetComponentInChildren<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        // if (canAttack)
        // {
        //     animator.SetBool("isAttacking", true);
        // }
        // else
        // {
        //     animator.SetBool("isAttacking", false);
        // }

        float distance = Vector3.Distance(target.position, transform.position);
        
        playerInSightRange = Physics.CheckSphere(transform.position, slightRange, whatIsPlayer);
        playerInAttackRange = Physics.CheckSphere(transform.position, atttackRange, whatIsPlayer);
        if (!playerInSightRange && !playerInAttackRange)
        {
            patrolling();
            animator.SetBool("isWalking", true);
            
        }

        else if (playerInSightRange && !playerInAttackRange)
        {
            chasePlayer();
            animator.SetBool("isWalking", true);
        }
        else
        {
            animator.SetBool("isWalking", false);
        }
        if (playerInAttackRange && playerInSightRange) attackPlayer();
        
    }

    public IEnumerator stunAnimate()
    {
        canAttack = false;
        animator.SetTrigger("isStunned");
        yield return new WaitForSeconds(0.5f);
        canAttack = true;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, lookRadius);
    }
    
    private void patrolling()
    {

        if (!walkPointSet)
        {
            SearchWalkPoint();
            animator.SetBool("isAttacking", true);
        }

        if (walkPointSet)
        {
            
            agent.SetDestination(walkPoint);
        }
            

        Vector3 distanceToWalkPoint = transform.position - walkPoint;
        if (distanceToWalkPoint.magnitude <= 2f)
        {
            print("Reset");
            walkPointSet = false;
        }
            
    }
    private void SearchWalkPoint()
    {
        float randomZ = Random.Range(-walkPointRange, walkPointRange);
        float randomX = Random.Range(-walkPointRange, walkPointRange);

        walkPoint = new Vector3(transform.position.x + randomX, transform.position.y, transform.position.z+randomZ);

        if(Physics.Raycast(walkPoint, -transform.up, 2f, whatIsGround))
        {
            walkPointSet = true;
        }
    }
    private void chasePlayer()
    {
        agent.SetDestination(target.position);
        animator.SetBool("isAttacking", false);

    }
    private void attackPlayer()
    {
        agent.SetDestination(transform.position-new Vector3(1.0f,0,1.0f));
        transform.LookAt(target);

        if (!alreadyAttacked)
        {
            animator.SetBool("isAttacking", true);
            animator.SetTrigger("attack");
            alreadyAttacked = true;
            Invoke(nameof(ResetAttack), timeBetweenAttacks);
        }
    }
    private void ResetAttack()
    {
        alreadyAttacked = false;
    }
}
