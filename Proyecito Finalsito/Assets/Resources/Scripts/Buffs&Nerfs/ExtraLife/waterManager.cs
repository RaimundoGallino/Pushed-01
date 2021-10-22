using System.Collections;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using UnityEngine;

public class waterManager : MonoBehaviour
{

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            var playerTaken = other.gameObject.GetComponent<PlayerController>();
            var hasExtraLife = GameEvents.Player(playerTaken.id).getExtraLife();
            if (hasExtraLife == true)
            {
                GameEvents.Player(playerTaken.id).onLastLife();
                var currentMapBounds = GameEvents.PlayableMap.GetCurrentMapBound?.Invoke();
                other.transform.position = currentMapBounds.center + (Random.insideUnitSphere * currentMapBounds.radious);
            }
        }
    }

}
