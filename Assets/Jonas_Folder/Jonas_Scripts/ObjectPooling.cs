using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ObjectPooling : MonoBehaviour
{
    public static List<PooledObjectInfo> objectPools = new List<PooledObjectInfo>();

    private GameObject objectPoolEmptyHolder;
    private static GameObject bulletsEmpty;
    private static GameObject enemyImpactObjectEmpty;
    private static GameObject wallImpactObjectEmpty;
    private static GameObject groundImpactObjectEmpty;
    private static GameObject bulletCaseObjectsEmpty;

    public enum PoolType
    {
        //Addera allt efter att nya saker ska poolas
        revolverBullet,
        shotgunBullet,
        rocketLauncherBullet,
        enemyImpactObject,
        wallImpactObject,
        groundImpactObject,
        bulletCaseObjects,
        None
    }
    public static PoolType poolingType;

    private void Awake()
    {
        SetupEmpties();
    }

    private void SetupEmpties()
    {
        objectPoolEmptyHolder = new GameObject("Pooled Objects");

        bulletsEmpty = new GameObject("Bullets");
        bulletsEmpty.transform.SetParent(objectPoolEmptyHolder.transform);

        enemyImpactObjectEmpty = new GameObject("Enemyimpact objects");
        enemyImpactObjectEmpty.transform.SetParent(objectPoolEmptyHolder.transform);

        wallImpactObjectEmpty = new GameObject("Wallimpact objects");
        wallImpactObjectEmpty.transform.SetParent(objectPoolEmptyHolder.transform);

        groundImpactObjectEmpty = new GameObject("Groundimpact objects");
        groundImpactObjectEmpty.transform.SetParent(objectPoolEmptyHolder.transform);

        bulletCaseObjectsEmpty = new GameObject("Bullet case objects");
        bulletCaseObjectsEmpty.transform.SetParent(objectPoolEmptyHolder.transform);
    }

    public static GameObject SpawnObject(GameObject objectToSpawn, Vector3 spawnPos, Quaternion spawnRot, PoolType poolType = PoolType.None, Vector3 initialVelocity = default)
    {
        PooledObjectInfo pool = objectPools.Find(p => p.LookUpString == objectToSpawn.name);

        if (pool == null)
        {
            pool = new PooledObjectInfo() { LookUpString = objectToSpawn.name };
            objectPools.Add(pool);
        }

        GameObject spawnableObj = pool.InactiveObjects.FirstOrDefault();

        if (spawnableObj == null)
        {
            GameObject parentObject = SetParentObject(poolType);

            spawnableObj = Instantiate(objectToSpawn, spawnPos, spawnRot);

            if (parentObject != null)
            {
                spawnableObj.transform.SetParent(parentObject.transform);
            }
        }
        else
        {
            spawnableObj.transform.position = spawnPos;
            spawnableObj.transform.rotation = spawnRot;
            pool.InactiveObjects.Remove(spawnableObj);
            spawnableObj.SetActive(true);
        }

        // Initialize the bullet's velocity
        BulletBase bullet = spawnableObj.GetComponent<BulletBase>();
        if (bullet != null)
        {
            bullet.InitializeBullet(initialVelocity);
        }

        return spawnableObj;
    }

    public static void ReturnObjectToPool(GameObject obj)
    {
        string goName = obj.name.Substring(0, obj.name.Length - 7);
        PooledObjectInfo pool = objectPools.Find(p => p.LookUpString == goName);

        if (pool == null)
        {
            Debug.Log("Trying to release an object that is not pooled: " + obj.name);
        }
        else
        {
            Rigidbody rb = obj.GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.velocity = Vector3.zero;
                rb.angularVelocity = Vector3.zero;
                rb.Sleep();

                rb.isKinematic = true;
                rb.isKinematic = false;
            }

            // Call ResetCasing if this is a bullet casing
            BulletCasing bulletCasing = obj.GetComponent<BulletCasing>();
            if (bulletCasing != null)
            {
                bulletCasing.ResetCasing();
            }

            obj.transform.position = Vector3.zero;
            obj.transform.rotation = Quaternion.identity;

            obj.SetActive(false);
            pool.InactiveObjects.Add(obj);
        }
    }
    private static GameObject SetParentObject(PoolType poolType)
    {
        switch (poolType)
        {
            //Addera mer här vid nya saker
            case PoolType.revolverBullet:
                return bulletsEmpty;
            case PoolType.shotgunBullet:
                return bulletsEmpty;
            case PoolType.rocketLauncherBullet:
                return bulletsEmpty;
            case PoolType.enemyImpactObject:
                return enemyImpactObjectEmpty;
            case PoolType.wallImpactObject: 
                return wallImpactObjectEmpty;
            case PoolType.groundImpactObject: 
                return groundImpactObjectEmpty;
            case PoolType.bulletCaseObjects:
                return bulletCaseObjectsEmpty;
            case PoolType.None:
                return null;
            default:
                return null;
        }
    }
}

public class PooledObjectInfo // en av dessa är en pool av object
{
    public string LookUpString;
    public List<GameObject> InactiveObjects = new List<GameObject>();
}
