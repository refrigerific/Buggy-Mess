using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BezierProjectileMovement : MonoBehaviour
{
    [SerializeField] private float speed = 5f;
    [SerializeField] private float curveMagnitude = 10f;
    [SerializeField] private ProjectileType projectileType = ProjectileType.Explosion;
    [SerializeField] private ExplodingPillBugAI spawnableEnemy;

    [SerializeField] private GameObject explosionZonePrefab;
    [SerializeField] private Vector3 explosionSize;
    [SerializeField] private float explosionTime = 2f;
    [SerializeField] private float spawnTime = 2f;

    private Transform target;
    private Vector3 startPos;
    private Vector3 endPos;

    private float t = 0f;

    private MeshRenderer meshRenderer;

    void Start()
    {
        meshRenderer = GetComponent<MeshRenderer>();

        target = GameObject.FindGameObjectWithTag("Player").transform;
        startPos = transform.position;
        endPos = target.position;
    }

    void Update()
    {
        t += Time.deltaTime * speed / Vector3.Distance(startPos, endPos);

        t = Mathf.Clamp01(t);

        Vector3 newPos = CalculateBezierPoint(startPos, endPos, curveMagnitude, t);

        transform.position = newPos;

        if (t >= 0.89f)
        {
            HandleProjectileType();
        }
    }

    private Vector3 CalculateBezierPoint(Vector3 p0, Vector3 p1, float curveMagnitude, float t)
    {
        Vector3 midPoint = p0 + (p1 - p0) / 2f;

        Vector3 controlPoint = midPoint + Vector3.up * curveMagnitude;

        return Mathf.Pow(1 - t, 2) * p0 + 2 * (1 - t) * t * controlPoint + Mathf.Pow(t, 2) * p1;
    }

    private void HandleProjectileType()
    {
        switch (projectileType)
        {
            case ProjectileType.Explosion:
                StartCoroutine(ScaleExplosionZone());
                break;
            case ProjectileType.Pillbug:
                StartCoroutine(SpawnEnemy());
                break;
        }
    }

    private IEnumerator ScaleExplosionZone()
    {
        explosionZonePrefab.SetActive(true);
        float currentAttackTime = 0f;
        while (currentAttackTime < explosionTime)
        {
            float scaleFactor = currentAttackTime / explosionTime;

            explosionZonePrefab.transform.localScale = Vector3.Lerp(Vector3.zero, explosionSize, scaleFactor);

            currentAttackTime += Time.deltaTime;

            yield return null;
        }
        explosionZonePrefab.transform.localScale = explosionSize;
        explosionZonePrefab.SetActive(false);
        gameObject.SetActive(false);
    }

    private IEnumerator SpawnEnemy()
    {
        yield return new WaitForSeconds(spawnTime);
        meshRenderer.enabled = false;
        spawnableEnemy.IsSpawned = true;
        spawnableEnemy.transform.SetParent(null);
        spawnableEnemy.gameObject.SetActive(true);
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawLine(startPos, endPos);

        Vector3 lastPoint = startPos;
        for (float i = 0.05f; i <= 1f; i += 0.05f)
        {
            Vector3 nextPoint = CalculateBezierPoint(startPos, endPos, curveMagnitude, i);
            Gizmos.DrawLine(lastPoint, nextPoint);
            lastPoint = nextPoint;
        }
    }
}
public enum ProjectileType
{
    Explosion,
    Pillbug
}
