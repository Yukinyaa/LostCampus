using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using DG.Tweening;

public class Door : NetworkBehaviour
{
    [SerializeField] private bool isOpenable = true;
    [SyncVar]private bool open = false;
    public float doorOpenAngle = 90f;
    public float doorCloseAngle = 0f;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        

    }

    [Command(requiresAuthority =false)]
    public void ChangeDoorState()
    {
        if (isOpenable == false) return;
        open = !open;

        if (open)
            transform.DORotate(new Vector3(0, doorOpenAngle, 0), 1);
        else
            transform.DORotate(new Vector3(0, doorCloseAngle, 0), 1);
    }

    
}
