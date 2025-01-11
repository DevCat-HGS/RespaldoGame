using UnityEngine;
using Photon.Pun;
using System.Collections;

public class SpawnPlayer : MonoBehaviourPunCallbacks
{
    public GameObject playerPrefab;
    public TurnControl turnControl;

    public override void OnJoinedRoom()
    {
        base.OnJoinedRoom();
        SpawnMyPlayer();  // Llamamos a SpawnMyPlayer para instanciar al jugador
    }

    void SpawnMyPlayer()
    {
        if (playerPrefab != null)
        {
            // Definir la posición y rotación
            Vector3 spawnPosition = new Vector3(0, 2, 0);  // Ajusta la posición según lo necesites
            Quaternion spawnRotation = Quaternion.identity;  // Sin rotación (por defecto)

            // Instanciamos el jugador usando PhotonNetwork.Instantiate
            PhotonNetwork.Instantiate(playerPrefab.name, spawnPosition, spawnRotation);
            Debug.Log("Jugador instanciado en la sala.");

            // Verificamos que la referencia a TurnControl no sea nula
            if (turnControl != null)
            {
                StartCoroutine(CheckPlayersAndStartTurn());
            }
            else
            {
                Debug.LogError("No se ha asignado el script TurnControl en el Inspector.");
            }
        }
        else
        {
            Debug.LogError("No se ha asignado un prefab de jugador en el script SpawnPlayer.");
        }
    }

    private IEnumerator CheckPlayersAndStartTurn()
    {
        // Esperar hasta que haya exactamente 2 jugadores en la sala
        while (PhotonNetwork.CurrentRoom.PlayerCount < 2)
        {
            Debug.Log("Esperando a que se unan más jugadores...");
            yield return new WaitForSeconds(1f);  // Comprobar cada segundo
        }

        // Ahora que hay exactamente 2 jugadores, intentamos iniciar el turno
        if (PhotonNetwork.CurrentRoom.PlayerCount == 2)
        {
            Debug.Log("Hay 2 jugadores en la sala. Iniciando el primer turno...");
            turnControl.StartNextTurn();  // Llamar a StartNextTurn desde TurnControl
        }
        else
        {
            Debug.LogWarning("No hay exactamente 2 jugadores en la sala.");
        }
    }
}
