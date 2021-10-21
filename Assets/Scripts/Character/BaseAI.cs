
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using Mirror;
using Mirror.Examples.Pong;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Serialization;
using Debug = UnityEngine.Debug;

public enum ActionState
{
    IDLE, MOVING, ATTACKING, CHASING
}

public abstract class BaseAI : NetworkBehaviour
{
    protected ActionState actionState;
    protected bool isMoving;
    protected bool isAttacking;
    protected GameObject target;

    protected NavMeshAgent agent;
    protected float timer;
    protected float distance;
    protected Animator anim;
    protected Rigidbody rigid;
    protected Weapon weapon;
    
    public float attackRadius = 0.5f;
    public float attackRange = 1f;
    public float attackPreDelay = 1.2f;
    [FormerlySerializedAs("wanderRadius")] public float moveRadius;
    [FormerlySerializedAs("wanderTimer")] public float moveTimer;
    public float chaseRadius = 5f;

    // Use this for initialization
    protected virtual void OnEnable()
    {
        rigid = GetComponent<Rigidbody>();
        agent = GetComponent<NavMeshAgent>();
        anim = GetComponent<Animator>();
        this.weapon = GetComponentInChildren<Weapon>();
        actionState = ActionState.IDLE;

    }

    // Update is called once per frame
    protected void Update()
    {
        Move();
        //Debug.Log(gameObject.ToString() +" is "+actionState.ToString());
    }
    protected abstract void Move();
    protected abstract void Targeting();
    protected virtual IEnumerator Attack()
    {
        anim.SetBool("isAttack", true);
        yield return new WaitForSeconds(attackPreDelay);
        weapon.Set(true);
        while (anim.GetCurrentAnimatorStateInfo(0).IsTag("Attacking"))
        {
            //애니메이션 재생 중 실행되는 부분
            yield return null;
        }
        weapon.Set(false);
        actionState = ActionState.IDLE;
    }
    
    protected void FixedUpdate()
    {
        Targeting();
        FreezeVelocity();
    }
    protected GameObject FindClosestPlayer()
    {
        GameObject[] gos;
        gos = GameObject.FindGameObjectsWithTag("Player");
        GameObject closest = null;
        float distance = Mathf.Infinity;
        Vector3 position = transform.position;
        foreach (GameObject go in gos)
        {
            Vector3 diff = go.transform.position - position;
            float curDistance = diff.sqrMagnitude;
            if (curDistance < distance)
            {
                closest = go;
                distance = curDistance;
            }
        }
        return closest;
    }
    
    protected GameObject FindTarget()
    {
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, 10f);

        GameObject targetGameObject = null;

        float temp = 0;

        float shortTemp = 10;

        for (int i = 0; i < hitColliders.Length; i++)
        {

            if (hitColliders[i].tag == "Player")
            {

                temp = Vector3.Distance(hitColliders[i].transform.position, transform.position);

                if (temp < shortTemp)
                {

                    targetGameObject = hitColliders[i].gameObject;

                }

            }

        }

        return targetGameObject;
    }

    protected void FreezeVelocity()
    {
        if (actionState == ActionState.CHASING)
        {
            rigid.velocity = Vector3.zero;
            rigid.angularVelocity = Vector3.zero;
        }
    }


    public static Vector3 RandomNavSphere(Vector3 origin, float dist, int layermask)
    {
        Vector3 randDirection = Random.insideUnitSphere * dist;

        randDirection += origin;

        NavMeshHit navHit;

        NavMesh.SamplePosition(randDirection, out navHit, dist, layermask);

        return navHit.position;
    }
}
