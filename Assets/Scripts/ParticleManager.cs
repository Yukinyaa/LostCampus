using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ParticleManager : MonoBehaviour
{

    [SerializeField] private GameObject DamageIndicator;
    private TextMeshPro damageIndicatorText;
    // Start is called before the first frame update
    void Start()
    {
        this.damageIndicatorText = DamageIndicator.GetComponent<TextMeshPro>();
    }

    public void PlayDamageIndicator(GameObject gameObject,float dmg)
    {
        this.PlayDamageIndicator(gameObject, dmg.ToString());
    }
    public void PlayDamageIndicator(GameObject gameObject, string dmg)
    {
        this.damageIndicatorText.text = dmg;
        this.DamageIndicator.transform.position = gameObject.transform.position;
        Instantiate(this.DamageIndicator);
    }
}
