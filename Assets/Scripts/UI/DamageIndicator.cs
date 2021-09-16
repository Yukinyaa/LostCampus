using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class DamageIndicator : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(this.Play());
    }

    IEnumerator Play()
    {
        yield return new WaitForSeconds(1f);
        Destroy(this.gameObject);
    }
}
