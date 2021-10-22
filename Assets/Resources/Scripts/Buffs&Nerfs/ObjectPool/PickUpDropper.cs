using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickUpDropper : MonoBehaviour
{
    [SerializeField] private bool dropPickUps = true;

    [System.Serializable]
    public class Pool
    {
        public string tag;
        public GameObject prefab;
        public int amount;
        [Tooltip("Que tan comun es este objeto")]
        public string normaliy = "100%";
    }

    [SerializeField] private List<Pool> pools;
    private List<Pool> selectedBuffos = new List<Pool>();

    private static Dictionary<GameObject, Queue<GameObject>> poolDictionary;

    [SerializeField] private float averageDropTime = 3.4f;

    private void OnEnable() {
        GameEvents.Game.onGameReady += InitializeMe;
    }

    private void OnDisable() {
        GameEvents.Game.onGameReady -= InitializeMe;
    }

    private void InitializeMe()
    {
        if (!dropPickUps)
            return;

        poolDictionary = new Dictionary<GameObject, Queue<GameObject>>();

        PickObjectsFromPool();
        RandomizeSelectedPoolDrops();
    }

    private void PickObjectsFromPool()
    {
        foreach (var item in pools)
        {
            // Si la rareza no termina con el caracter '%' rompo todo
            if (item.normaliy[item.normaliy.Length - 1] != '%')
            {
                Debug.LogError($"Pool element called {item.tag} normality field not ends with a '%'");
                gameObject.SetActive(false);
                return;
            }
            var normalityString = item.normaliy.Substring(0, item.normaliy.Length - 1);
            var normalityInt = int.Parse(normalityString); // 80
            var rarity = Random.Range(0, 101);

            // Append
            if (normalityInt >= rarity) {
                selectedBuffos.Add(item);
            }
        }
    }

    private void RandomizeSelectedPoolDrops()
    {
        var allObjects = 0;
        var startTime = 0f;
        foreach (var item in selectedBuffos) {
            // Initialize queue
            Queue<GameObject> objectPool = new Queue<GameObject>();

            for (int i = 0; i < item.amount; i++)
            {
                GameObject obj = Instantiate(item.prefab);
                obj.SetActive(false);
                objectPool.Enqueue(obj);
            }
            poolDictionary.Add(item.prefab, objectPool);
            // ==============

            allObjects += item.amount;
        }

        for (int i = 0; i < allObjects; i++)
        {
            foreach (var item in selectedBuffos) {
                startTime += averageDropTime + Random.Range(2f, 10f);
                var mapBounds = GameEvents.PlayableMap.GetCurrentMapBound?.Invoke();
                var randomPosition = mapBounds.center + (Random.insideUnitSphere * mapBounds.radious);
                StartCoroutine(SpawnCall(item.prefab, randomPosition, Quaternion.identity, startTime));
            }
        }
    }

    private IEnumerator SpawnCall(GameObject obj, Vector3 pos, Quaternion rot, float differCounter)
    {
        yield return new WaitForSeconds(differCounter);

        var currentMapBound = GameEvents.PlayableMap.GetCurrentMapBound?.Invoke();
        var randomPosition = currentMapBound.center + (Random.insideUnitSphere * currentMapBound.radious);

        SpawnFromPool(obj, randomPosition, rot);
    }

    public static GameObject SpawnFromPool(GameObject targetObj, Vector3 position, Quaternion rotation)
    {
        if (!poolDictionary.ContainsKey(targetObj))
        {
            Debug.LogWarning($"Pool with key {targetObj} doesn't excist");
            return null;
        }

        GameObject objectToSpawn = poolDictionary[targetObj].Dequeue();

        objectToSpawn.SetActive(true);
        objectToSpawn.transform.position = position;
        objectToSpawn.transform.rotation = rotation;

        poolDictionary[targetObj].Enqueue(objectToSpawn);

        return objectToSpawn;
    }
}
