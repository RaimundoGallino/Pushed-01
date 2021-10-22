using System;
using System.Collections;
using UnityEngine;

public class BasePickUp : MonoBehaviour
{
    [System.NonSerialized] public string id;

    public GameObject effect;
    public GameObject model;
    [Space]
    public Collider myCol;

    // Tiempo de vida de cada uno de los objetos
    public float lifeTime = 8f;
    public float destroyDelay = 3f;
    [System.NonSerialized] public GameObject player;

    [System.NonSerialized] public bool reproducePickUpSound = true;
    [System.NonSerialized] public bool triggerEffects = true;

    // Funciones que se ejecutan al encontrar player
    public Action onPlayerSpotted;
    private bool playerEventAlreadyExecuted = false;    

    public void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player"))
            return;

        player = other.gameObject;

        if (reproducePickUpSound)
            AudioManager.current.PlayAtPosition("PickUp", "Main", "Pick", transform.position);

        // Active effects before destroy
        if (triggerEffects)
            effect.SetActive(true);

        if (!playerEventAlreadyExecuted) {
            onPlayerSpotted?.Invoke();
            playerEventAlreadyExecuted = true;
        }
    }

    public IEnumerator StartDestroyCountDown()
    {
        DisableMe();
        yield return new WaitForSeconds(destroyDelay);
        Destroy(gameObject);
    }

    public void DisableMe()
	{
        model.SetActive(false);
        foreach (var col in GetComponentsInChildren<Collider>()) {
            col.enabled = false;
		}

        Destroy(GetComponent<Rigidbody>());
    }
}
