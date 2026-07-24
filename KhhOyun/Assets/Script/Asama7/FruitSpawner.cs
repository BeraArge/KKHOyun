using UnityEngine;

public class FruitSpawner : MonoBehaviour
{
    [Header("Üretim Ayarlarý")]
    [SerializeField] private GameObject applePrefab;
    [SerializeField] private float spawnFreq = 1.2f;
    [SerializeField] private float widthEdge = 7f;

    private float counter;
    private bool isSpawnResume = true;

    private void OnEnable()
    {
        GameEvents.OnTaskCompleted += CheckMissionSituation;
    }

    private void OnDisable()
    {
        GameEvents.OnTaskCompleted -= CheckMissionSituation;
    }



    private void Update()
    {
        if (isSpawnResume != true) return;
       
        counter += Time.deltaTime;

        if (counter >= spawnFreq)
        {
            spawnApple();
            counter = 0f;
        }
    }

    private void spawnApple()
    {
        float randomX = Random.Range(-widthEdge, widthEdge);
        Vector3 spawnPoint = new Vector3(randomX, transform.position.y, transform.position.z);

        Instantiate(applePrefab, spawnPoint, Quaternion.identity);
    }

    private void CheckMissionSituation(int taskId) 
    {
        if(taskId == 1)
        {
            isSpawnResume = false;
            Debug.Log("Üretim durdu");

            CleanAllFruits();
        }
    }

    private void CleanAllFruits()
    {
        GameObject[] applesRemain = GameObject.FindGameObjectsWithTag("Apple");
        GameObject[] bananasRemain = GameObject.FindGameObjectsWithTag("Banana");
        GameObject[] melonsRemain = GameObject.FindGameObjectsWithTag("Melon");

        foreach(GameObject apples in applesRemain)
        {
            Destroy(apples);
        }
        foreach (GameObject bananas in bananasRemain)
        {
            Destroy(bananas);
        }
        foreach (GameObject melons in melonsRemain)
        {
            Destroy(melons);
        }
    }
}
