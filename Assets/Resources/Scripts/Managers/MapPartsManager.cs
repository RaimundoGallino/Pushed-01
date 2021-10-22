using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapBound {
    public Vector3 center;
    public float radious;
}

public class MapPartsManager : MonoBehaviour
{
    [System.Serializable]
    class PlatformInfo {
        public Transform[] extremos;
    }
    private int currentIndex = 0;
    [SerializeField] private PlatformInfo[] platformInfo;

    [SerializeField] private bool dropMapParts = true;
    public List<MapObjectBrain> mapDropOrder = new List<MapObjectBrain>();

    private MapBound currentMapBound = new MapBound();
    private MapBound CurrentMapBound() => currentMapBound;

    private List<GameObject> activeMapParts = new List<GameObject>();
    private GameObject[] GetActiveMapParts() => activeMapParts.ToArray();

    [SerializeField] private float averageDrop = 0.2f;

    private void OnEnable() {
        // Getter event
        GameEvents.PlayableMap.GetActiveMapParts += GetActiveMapParts;
        GameEvents.PlayableMap.GetCurrentMapBound += CurrentMapBound;

        GameEvents.Game.onGameReady += SetMapDropCountdown;
    }

    private void OnDisable() {
        // Getter event
        GameEvents.PlayableMap.GetActiveMapParts -= GetActiveMapParts;
        GameEvents.PlayableMap.GetCurrentMapBound -= CurrentMapBound;

        GameEvents.Game.onGameReady -= SetMapDropCountdown;
    }

    private void Start()
    {
        if (mapDropOrder == null || mapDropOrder.Count == 0) {
            mapDropOrder = new List<MapObjectBrain>();
            mapDropOrder.AddRange(FindObjectsOfType<MapObjectBrain>());
        }
    }

    private void SetMapDropCountdown()
    {
        if (!dropMapParts)
            return;

        float differenceCounter = 0f;

        foreach (var mapPart in mapDropOrder) {
            differenceCounter += averageDrop + Random.Range(2f, 10f);
            StartCoroutine(DropPartSignal(mapPart, differenceCounter));
        }

        var currentMaps = FindObjectsOfType<MapObjectBrain>();
        foreach (var item in currentMaps) {
            activeMapParts.Add(item.gameObject);
        }

        UpdateMapBound();
    }

    private void UpdateMapBound()
    {
        var currentExtremos = platformInfo[currentIndex].extremos;
        var diameter = Vector3.Distance(currentExtremos[0].position, currentExtremos[1].position);

        currentMapBound.center = transform.position;
        currentMapBound.radious = diameter / 2;
    }

    // Corrutina para dropear la parte del mapa
    private IEnumerator DropPartSignal(MapObjectBrain mapPart, float dropTime)
    {
        yield return new WaitForSeconds(dropTime);

        activeMapParts.Remove(mapPart.gameObject);
        currentIndex++;
        UpdateMapBound();

        mapPart.DropMe();
    }

    
    private void OnDrawGizmosSelected() {
        Gizmos.DrawWireSphere(currentMapBound.center, currentMapBound.radious);
    }
}
