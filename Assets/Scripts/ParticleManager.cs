using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ParticleManager : MonoBehaviour
{

    [SerializeField] private GameObject DamageIndicator;
    //private TextMeshPro damageIndicatorText;
    
    void Start()
    {
        //damageIndicatorText = DamageIndicator.GetComponent<TextMeshPro>();
    }

    public void PlayDamageIndicator(GameObject gameObject,float dmg)
    {
        this.PlayDamageIndicator(gameObject, dmg.ToString());
    }
    public void PlayDamageIndicator(GameObject gameObject, string dmg)
    {
        GameObject obj = Instantiate(this.DamageIndicator);
        obj.GetComponent<TextMeshPro>().text = dmg;
        obj.transform.position = gameObject.transform.position;
    }
}
