using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class DamageIndicator : MonoBehaviour
{
    private Camera mainCamera;
    // Start is called before the first frame update
    void Awake()
    {
        mainCamera = Camera.main;
        StartCoroutine(this.Play());
        gameObject.GetComponent<Rigidbody>().AddExplosionForce(1000,gameObject.transform.position,00);
    }

    void Update()
    {
        transform.LookAt(mainCamera.transform);
    }

    IEnumerator Play()
    {
        yield return new WaitForSeconds(1f);
        Destroy(this.gameObject);
    }
}
