using UnityEngine;
using Photon.Pun;
using TMPro;
using System.Linq;

public class TurnControl : MonoBehaviourPunCallbacks
{
    public TextMeshProUGUI turnText;
    public float turnDuration = 3f;
    private int currentPlayerIndex = 0;
    private Photon.Realtime.Player[] players;

    private bool isTurnActive = false;
    private float turnTimer;

    void Start()
    {
        // Inicialización de la lista de jugadores
        players = PhotonNetwork.PlayerList;

        // Asegúrate de que turnText esté asignado
        if (turnText == null)
        {
            Debug.LogError("turnText no está asignado en el Inspector.");
            return;
        }

        // Verificación si hay suficientes jugadores
        if (players != null && players.Length >= 2)
        {
            if (PhotonNetwork.IsMasterClient)
            {
                Debug.Log("El Master Client está iniciando el primer turno...");
                StartNextTurn(); // Iniciar el primer turno solo si hay 2 jugadores
            }
        }
        else
        {
            Debug.Log("Esperando a que haya al menos 2 jugadores.");
        }
    }

    void Update()
    {
        // Si el turno está activo, actualizamos el temporizador
        if (isTurnActive)
        {
            turnTimer -= Time.deltaTime;
            if (turnTimer <= 0)
            {
                EndTurn();  // Termina el turno cuando el temporizador llegue a cero
            }
        }
    }

    public void StartNextTurn()
    {
        // Asegurarnos de que la sala está completamente cargada y tiene jugadores
        if (PhotonNetwork.CurrentRoom == null || PhotonNetwork.CurrentRoom.PlayerCount < 2)
        {
            Debug.LogError("No hay suficientes jugadores o la sala no está disponible para iniciar el turno.");
            return;
        }

        // Obtener los jugadores de la sala solo si está disponible
        players = PhotonNetwork.CurrentRoom.Players.Values.ToArray();

        // Verificamos si la lista de jugadores está correctamente asignada y tiene jugadores
        if (players == null || players.Length < 2)
        {
            Debug.LogError("La lista de jugadores está vacía o no tiene suficientes jugadores.");
            return;
        }

        // Solo el jugador actual puede tomar acciones
        if (PhotonNetwork.IsMasterClient)
        {
            currentPlayerIndex = (currentPlayerIndex + 1) % players.Length;  // Avanzamos al siguiente jugador

            // Inicia el turno del jugador actual
            isTurnActive = true;
            turnTimer = turnDuration;  // Reseteamos el temporizador para el turno actual

            // Aseguramos que el turnText esté asignado antes de actualizarlo
            if (turnText != null)
            {
                turnText.text = players[currentPlayerIndex].NickName + " tiene el turno";
            }
            else
            {
                Debug.LogError("turnText no está asignado en el Inspector.");
            }

            // Sincronizamos el turno con los demás jugadores en la red
            photonView.RPC("UpdateTurn", RpcTarget.OthersBuffered, currentPlayerIndex);
        }
        else
        {
            Debug.LogError("Solo el Master Client puede iniciar el turno.");
        }
    }

    void EndTurn()
    {
        // Desactiva el turno actual
        isTurnActive = false;

        // Avanzamos al siguiente turno
        Debug.Log("El turno ha terminado, avanzando al siguiente.");
        StartNextTurn();
    }

    [PunRPC]
    void UpdateTurn(int newPlayerIndex)
    {
        currentPlayerIndex = newPlayerIndex;
        if (turnText != null)
        {
            turnText.text = players[currentPlayerIndex].NickName + " tiene el turno";
        }

        // Debugging: Asegurarnos de que el turno se actualiza correctamente
        Debug.Log($"El turno ha sido actualizado para {players[currentPlayerIndex].NickName}.");
    }

    public void EndTurnButton()
    {
        if (isTurnActive)
        {
            EndTurn();
        }
    }

    public bool IsMyTurn()
    {
        return PhotonNetwork.LocalPlayer == players[currentPlayerIndex];
    }
}
