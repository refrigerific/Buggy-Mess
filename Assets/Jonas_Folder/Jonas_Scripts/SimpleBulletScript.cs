using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimpleBulletScript : MonoBehaviour
{
    [SerializeField] private int damage = 10;
    [SerializeField] private float lifeTime = 5f;


    private void Update()
    {
        //lifeTime -= Time.deltaTime;
        //if (lifeTime <= 0)
        //{
        //    Destroy(gameObject);
        //}
    }

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("Test!");
        Health health = other.gameObject.GetComponent<Health>();
      
        if (other.gameObject.tag == "Player")
        {
            if (health != null)
            {
                health.TakeDamage(damage);
                Debug.Log("Takes damage!" + damage);
            }

            Destroy(gameObject);
        }
    }
}
