using UnityEngine;
using EventsTemplate;
using DreadZitoTypes;

public class GameEvents : MonoBehaviour
{
    /* Esta clase contiene todos los eventos que ocurren dentro del juego
        ¿Que es un evento? se define un evento cuando una accion se realiza dentro del juego, ejemplo
                            el player se mueve; entonces la camara va a responder a ese evento de
                            alguna forma.
    */

    // diccionario de acciones que un player tiene segun la ID
    private static DreDictionary<string, PlayerEvTemplate> player = new DreDictionary<string, PlayerEvTemplate>();

    // Bomb dictionary
    private static DreDictionary<string, BombEvTemplate> bomb = new DreDictionary<string, BombEvTemplate>();

    private static MapEvTemplate playableMap = new MapEvTemplate();
    public static MapEvTemplate PlayableMap => playableMap;
    public static GameEvTemplate Game = new GameEvTemplate();
    // ================ Player
    public static void ClaimPlayerEvents(string id)
    {
        player.Add(id, new PlayerEvTemplate());
    }
    public static PlayerEvTemplate Player(string id)
    {
        return player[id];
    }
    // ==============================

    // =============== Bomb
    public static void ClaimBombEvents(string id)
    {
        bomb.Add(id, new BombEvTemplate());
    }
    public static BombEvTemplate Bomb(string id)
    {
        return bomb[id];
    }
    // ======================
}

