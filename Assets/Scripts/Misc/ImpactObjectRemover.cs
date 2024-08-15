using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ImpactObjectRemover : MonoBehaviour
{
    [SerializeField] private float lifeTime = 2f;

    private void OnEnable()
    {
        StartCoroutine(RemoveRoutine());
    }

    private IEnumerator RemoveRoutine()
    {

        yield return new WaitForSeconds(lifeTime);

        ObjectPooling.ReturnObjectToPool(gameObject);

    }
}
