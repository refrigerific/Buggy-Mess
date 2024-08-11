using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeleeZoneDamage : MonoBehaviour
{
    [SerializeField] private PillbugAI pillbugAI;
    GameObject taggedObject;
   
    private void OnEnable()
    {
        taggedObject = GameObject.FindWithTag("Player");

        if (taggedObject != null)
        {
            Health health = taggedObject.GetComponent<Health>();
            if (health != null)
            {
                Debug.Log("Gör damage");
                health.TakeDamage(pillbugAI.Damage);
            }           
        }        
    }
}
