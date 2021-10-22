using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnPoint_System : MonoBehaviour
{
    [SerializeField] private List<GameObject> SpawnList = new List<GameObject>();

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public Vector3 RetriveSpawnPoint(int index) 
    {
        return SpawnList[index].transform.position;
    }


}
