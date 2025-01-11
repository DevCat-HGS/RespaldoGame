using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using TMPro;

public class ConnectionPhoton : MonoBehaviourPunCallbacks
{
    public TextMeshProUGUI statusText;

    void Start()
    {
        PhotonNetwork.ConnectUsingSettings();  // Conectar a Photon
        statusText.text = "Conectando a Photon...";
    }

    public override void OnConnectedToMaster()
    {
        base.OnConnectedToMaster();
        statusText.text = "Conectado a Photon!";

        // Intentar unirse a una sala aleatoria
        PhotonNetwork.JoinRandomRoom();
    }

    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        base.OnJoinRandomFailed(returnCode, message);
        statusText.text = "No se pudo unir a una sala. Creando una nueva sala...";
        CreateRoom();
    }

    private void CreateRoom()
    {
        string roomName = "Sala_" + Random.Range(1000, 9999).ToString();  // Generar nombre aleatorio para la sala
        RoomOptions roomOptions = new RoomOptions();
        roomOptions.MaxPlayers = 2;  // Limitar la cantidad de jugadores por sala

        // Crear la sala
        PhotonNetwork.CreateRoom(roomName, roomOptions);
        statusText.text = "Creando una nueva sala...";
    }

    public override void OnJoinedRoom()
    {
        base.OnJoinedRoom();
        statusText.text = "¡Has entrado a la sala! Jugadores: " + PhotonNetwork.CurrentRoom.PlayerCount;

        // Después de entrar a la sala, el jugador será instanciado
        // Este paso se delega al script SpawnPlayer.
    }

    // Esta función se llama cada vez que un jugador entra a la sala
    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        base.OnPlayerEnteredRoom(newPlayer);
        statusText.text = "Jugador entró a la sala. Jugadores: " + PhotonNetwork.CurrentRoom.PlayerCount;

    }

    // Esta función se llama cada vez que un jugador sale de la sala
    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        base.OnPlayerLeftRoom(otherPlayer);
        statusText.text = "Jugador salió de la sala. Jugadores: " + PhotonNetwork.CurrentRoom.PlayerCount;
    }

    public override void OnDisconnected(DisconnectCause cause)
    {
        base.OnDisconnected(cause);
        statusText.text = "Desconectado de Photon: " + cause.ToString();
    }
}
