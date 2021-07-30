using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Wander : MonoBehaviour
{

    public float wanderRadius;
    public float wanderTimer;
    public bool isChase;
    public bool isAttack;
    public BoxCollider meleeArea;

    private NavMeshAgent agent;
    private float timer;
    private float distance;
    private Transform target;
    private Animator anim;
    private Rigidbody rigid;


    // Use this for initialization
    void OnEnable()
    {
        rigid = GetComponent<Rigidbody>();
        agent = GetComponent<NavMeshAgent>();
        timer = wanderTimer;
        anim = GetComponent<Animator>();

    }

    // Update is called once per frame
    void Update()
    {
        target = GameObject.FindWithTag("Player").transform;
        distance = Vector3.Distance(target.position, transform.position);
        timer += Time.deltaTime;
        if (distance < 10)
        {
            anim.SetBool("isWalk", true);
            agent.SetDestination(target.position);
            isChase = true;

        }

        else
        {
            isChase = false;
            if (timer >= wanderTimer)
            {
                anim.SetBool("isWalk", true);
                Vector3 newPos = RandomNavSphere(transform.position, wanderRadius, -1);
                agent.SetDestination(newPos);
                timer = 0;
            }
        }
    }
    
    void FreezeVelocity()
    {
        if (isChase)
        {
            rigid.velocity = Vector3.zero;
            rigid.angularVelocity = Vector3.zero;
        }
    }

    void Targeting()
    {
        float targetRadius = 0.5f;
        float targetRange = 1f;

        RaycastHit[] rayHits = Physics.SphereCastAll(transform.position, targetRadius, transform.forward, targetRange, LayerMask.GetMask("Player"));
        if (rayHits.Length > 0 && !isAttack)
        {
            StartCoroutine(Attack());
        }

    }

    IEnumerator Attack()
    {
        isChase = false;
        isAttack = true;
        anim.SetBool("isAttack", true);
        yield return new WaitForSeconds(0.2f);
        meleeArea.enabled = true;

        yield return new WaitForSeconds(1f);
        meleeArea.enabled = false;

        yield return new WaitForSeconds(1f);

        isChase = true;
        isAttack = false;
        anim.SetBool("isAttack", false);
    }

    private void FixedUpdate()
    {
        Targeting();
        FreezeVelocity();
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
