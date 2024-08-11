using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GauntletSpawner : MonoBehaviour
{
    [Header("Components")]
    [SerializeField] private Animator animator;
    [SerializeField] private List<Transform> spawnZones;
    [SerializeField] private List<GameObject> enemyTypesToSpawn;

    [Space]
    [Header("Variables")]
    [SerializeField] private float minSpawnDelay;
    [SerializeField] private float maxSpawnDelay;
    [SerializeField] private int enemiesToDefeat;

    [SerializeField] private int enemiesSpawned = 0;
    [SerializeField] private int enemiesDefeated = 0;

    private void Start()
    {
        StartCoroutine(SpawnSequence());//Ändra så att man måste "klicka" på en knapp för att starta!
    }

    private void Update()
    {
        if (enemiesDefeated >= enemiesToDefeat)
        {
            // Implement your logic for what happens when all enemies are defeated
            Debug.Log("All enemies defeated!");
            StartCoroutine(ChoosePoweUp());
            // For example, stop spawning or trigger some event
        }

        if (Input.GetKeyDown(KeyCode.E))
        {
            EnemyDefeated();
        }

    }

    private IEnumerator SpawnSequence()
    {
        yield return new WaitForSeconds(5f);//Ta bort
        animator.SetTrigger("movePanel");
        while (enemiesDefeated <= enemiesToDefeat)
        {
            float delay = Random.Range(minSpawnDelay, maxSpawnDelay);
            yield return new WaitForSeconds(delay);
            SpawnEnemy();
        }
    }

    private void SpawnEnemy()//Göra det mer wave spawnat?
    {
        Transform spawnZone = spawnZones[Random.Range(0, spawnZones.Count)];

        GameObject enemyPrefab = enemyTypesToSpawn[Random.Range(0, enemyTypesToSpawn.Count)];

        Instantiate(enemyPrefab, spawnZone.position, Quaternion.identity);

        enemiesSpawned++;
    }

    private IEnumerator ChoosePoweUp()
    {
        animator.SetTrigger("movePanelBack");
        yield return new WaitForSeconds(200);
    }

    public void EnemyDefeated()
    {
        enemiesDefeated++;
    }
}
