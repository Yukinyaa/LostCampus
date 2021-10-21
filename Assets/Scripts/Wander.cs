using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Mirror;

public class Wander : BaseAI 
{ 

    
    // Use this for initialization
    protected override void OnEnable()
    {
        base.OnEnable();
        timer = moveTimer;
    }

    protected override void Move()
    {
        target = FindTarget();
        if (target)
            distance = Vector3.Distance(target.transform.position, transform.position);

        timer += Time.deltaTime;
        if (distance < chaseRadius)
        {
            anim.SetBool("isWalk", true);
            agent.SetDestination(target.transform.position); 
            
            if (actionState == ActionState.ATTACKING)
            {
                agent.isStopped = true;
            }
            else
            {
                agent.isStopped = false;
                actionState = ActionState.CHASING;
            }
        }
        else if(actionState != ActionState.ATTACKING)
        {
            if (timer >= moveTimer)
            {
                actionState = ActionState.MOVING;
                anim.SetBool("isWalk", true);
                Vector3 newPos = RandomNavSphere(transform.position, moveRadius, -1);
                agent.SetDestination(newPos);
                timer = 0;
            }
        }
    }
    
    protected override void Targeting()
    {   
        RaycastHit[] rayHits = Physics.SphereCastAll(transform.position, attackRadius, transform.forward, attackRange, LayerMask.GetMask("Player"));
        if (rayHits.Length > 0 && actionState != ActionState.ATTACKING)
        {
            actionState = ActionState.ATTACKING;
            StartCoroutine(Attack());
        }
    }
}
