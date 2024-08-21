using FMODUnity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletCasing : MonoBehaviour
{
    [Header("Components")]
    [SerializeField] private Rigidbody rb;
    [Header("Bulletcase Options")]
    [SerializeField] private float lifetime = 3f;
    [SerializeField] private float minForce = 3f;
    [SerializeField] private float maxForce = 7f;
    [SerializeField] private float minTorque = 5f;
    [SerializeField] private float maxTorque = 15f;

    private void OnEnable()
    {
        StartCoroutine(RemoveRoutine());
        ApplyRandomForceAndTorque();
    }

    private void ApplyRandomForceAndTorque()
    {
        if (rb != null)
        {
            Vector3 randomDirection = Random.onUnitSphere;
            float randomForce = Random.Range(minForce, maxForce);
            rb.AddForce(randomDirection * randomForce, ForceMode.Impulse);

            Vector3 randomTorque = Random.onUnitSphere * Random.Range(minTorque, maxTorque);
            rb.AddTorque(randomTorque, ForceMode.Impulse);
        }
    }

    public void ResetCasing()
    {
        if (rb != null)
        {
            rb.velocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
            rb.Sleep();
        }
    }

    private IEnumerator RemoveRoutine()
    {
        yield return new WaitForSeconds(lifetime);
        ObjectPooling.ReturnObjectToPool(gameObject);
    }
}
