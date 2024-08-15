using FMODUnity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletCasing : MonoBehaviour
{
    [SerializeField] private float lifetime = 3f;

    private void OnEnable()
    {
        StartCoroutine(RemoveRoutine());
    }

    private void Start()
    {
        Rigidbody rb = GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.AddForce(Random.onUnitSphere * 5f, ForceMode.Impulse);
            rb.AddTorque(Random.onUnitSphere * 10f, ForceMode.Impulse);
        }

    }

    private IEnumerator RemoveRoutine()
    {

        yield return new WaitForSeconds(lifetime);

        ObjectPooling.ReturnObjectToPool(gameObject);
    }
}
