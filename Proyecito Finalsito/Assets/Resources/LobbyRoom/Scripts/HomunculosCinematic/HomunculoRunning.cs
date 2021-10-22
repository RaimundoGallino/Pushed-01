using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HomunculoRunning : MonoBehaviour
{
    [SerializeField] private Transform[] waypoints;
    private Transform currentWaypoint;
    private int currentIndex = 0;

    [SerializeField] private float speed;
    [SerializeField] private float dollyLifeTime = 7f;

    private void Awake()
    {
        currentWaypoint = waypoints[currentIndex];
        Invoke("DestroyMe", dollyLifeTime);
    }

    private void Update()
    {
        var distance = Vector3.Distance(transform.position, currentWaypoint.position);
        if (distance >= 0.5f)
        {
            //transform.position += (currentWaypoint.position - transform.position) * speed * Time.deltaTime;
            transform.position = Vector3.MoveTowards(transform.position, currentWaypoint.position, speed * Time.deltaTime);
            AudioManager.current.PlayRandomClipAtPosition("Player", "FootstepsMetal", transform.position);
        }
        else
            PickOther();

    }

    private void PickOther()
    {
        switch (currentIndex) {
            case 0:
                currentWaypoint = waypoints[1];
                currentIndex = 1;
            break;
            case 1:
                currentWaypoint = waypoints[0];
                currentIndex = 0;
            break;
        }

        transform.LookAt(currentWaypoint);
    }

    private void DestroyMe()
    {
        Destroy(transform.parent.gameObject);
    }
}
