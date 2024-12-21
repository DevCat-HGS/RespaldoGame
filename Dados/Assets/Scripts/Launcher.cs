using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using TMPro;
using System.Collections;
using System.Linq;

public class Launcher : MonoBehaviourPunCallbacks
{
    public PhotonView playerPrefab;

    public Transform spawnPoint;

    public TMP_Text conteo;

    private GameObject jugador;

    private readonly GameObject[] jugadores = new GameObject[2];

    private float timeRemaining = 1f;  // Tiempo inicial en segundos

    private bool timerIsRunning = false; // Si el temporizador est� corriendo

    private int posicion = 0;

    void Start()
    {
        if (spawnPoint == null)
        {
            Debug.LogError("Punto de spawn no asignado");
            return;
        }
        
        // Verificar que la cámara principal existe
        if (Camera.main == null)
        {
            Debug.LogError("No se encuentra la cámara principal");
            return;
        }
        
        conteo.text = "Esperando otros jugadores";
    }

    public override void OnJoinedRoom()
    {
        Debug.Log($"Jugador unido a la sala. Posición de spawn: {spawnPoint.position}");
        Debug.Log($"Número total de jugadores: {PhotonNetwork.CurrentRoom.PlayerCount}");
        Vector3 spawnPosition = spawnPoint.position;
        jugador = PhotonNetwork.Instantiate(playerPrefab.name, spawnPosition, Quaternion.identity);
        Player[] players = PhotonNetwork.PlayerList;
        Player miJugador = PhotonNetwork.LocalPlayer;
        int posicion = 3;
        for (int i = players.Length - 1; i >= 0; i--)
        {
            var player = players[i];
            if (player.ActorNumber == miJugador.ActorNumber)
            {
                AgregarJugador(this.posicion, player.NickName, i);
                this.posicion++;
            }
            else
            {
                AgregarJugador(posicion, player.NickName, i);
                posicion--;
            }
        }
    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        var i = newPlayer.ActorNumber - 1;
        AgregarJugador(posicion, newPlayer.NickName, i);
        posicion++;
    }

    private void AgregarJugador(int posicion, string name, int indice)
    {
        Debug.Log($"Jugador: {name}, Posicion: {posicion + 1}");
        jugador.GetComponent<PhotonView>().RPC("SetJugador", RpcTarget.AllBuffered, posicion, name);
        jugadores[indice] = jugador;
        if (PhotonNetwork.CurrentRoom.PlayerCount == jugadores.Length && !timerIsRunning)
        {
            photonView.RPC("StartCountdown", RpcTarget.AllBuffered);
        }
    }

    [PunRPC]
    public void IniciarTurno(int nuevoJugador)
    {
        Debug.Log($"Iniciando turno del jugador {nuevoJugador}");
        var jugador = jugadores[nuevoJugador];
        
        // Obtener el componente de dados
        Transform dadosTransform = jugador.transform.Find("Dados");
        if (dadosTransform != null)
        {
            // Mostrar los dados
            dadosTransform.gameObject.SetActive(true);
            
            // Si es el jugador local y es su turno
            if (PhotonNetwork.LocalPlayer.ActorNumber - 1 == nuevoJugador)
            {
                // Habilitar interacción con los dados
                var dadosInteractivos = dadosTransform.GetComponentsInChildren<Collider>(true);
                foreach (var dado in dadosInteractivos)
                {
                    dado.enabled = true;
                }
            }
            else
            {
                // Deshabilitar interacción para los demás jugadores
                var dadosInteractivos = dadosTransform.GetComponentsInChildren<Collider>(true);
                foreach (var dado in dadosInteractivos)
                {
                    dado.enabled = false;
                }
            }
        }
    }

    [PunRPC]
    private void ActualizarDadosVisibles(int jugadorActual)
    {
        if (jugadores == null || jugadores.Length == 0) return;
        
        for (int i = 0; i < jugadores.Length; i++)
        {
            if (jugadores[i] != null)
            {
                Transform dados = jugadores[i].transform.Find("Dados");
                if (dados != null)
                {
                    bool esTurnoJugador = i == jugadorActual;
                    dados.gameObject.SetActive(esTurnoJugador);
                    
                    // Actualizar interactividad solo si es el jugador local
                    if (PhotonNetwork.LocalPlayer.ActorNumber - 1 == i)
                    {
                        var dadosInteractivos = dados.GetComponentsInChildren<Collider>(true);
                        foreach (var dado in dadosInteractivos)
                        {
                            dado.enabled = esTurnoJugador;
                        }
                    }
                }
            }
        }
    }

    [PunRPC]
    private void StartCountdown()
    {
        if (!timerIsRunning)
        {
            timerIsRunning = true;
            StartCoroutine(CountdownCoroutine());
        }
    }

    private IEnumerator CountdownCoroutine()
    {
        while (timeRemaining > 0)
        {
            conteo.text = $"Comenzando en {timeRemaining}";
            yield return new WaitForSeconds(1f);
            timeRemaining--;
        }

        conteo.text = "Comienza el juego!";
        yield return new WaitForSeconds(1f);
        conteo.enabled = false;
        jugadores[0] = jugador;
        photonView.RPC("IniciarTurno", RpcTarget.AllBuffered, 0);
    }

    public void SiguienteTurno(int jugadorActual)
    {
        int siguienteJugador = (jugadorActual + 1) % jugadores.Length;
        photonView.RPC("ActualizarDadosVisibles", RpcTarget.All, siguienteJugador);
        photonView.RPC("IniciarTurno", RpcTarget.All, siguienteJugador);
    }

    [PunRPC]
    private void ActualizarIndicadorTurno(int jugadorActual)
    {
        foreach (var jugador in jugadores)
        {
            if (jugador != null)
            {
                Transform indicador = jugador.transform.Find("IndicadorTurno");
                if (indicador != null)
                {
                    bool esTurnoJugador = jugadores.ToList().IndexOf(jugador) == jugadorActual;
                    indicador.gameObject.SetActive(esTurnoJugador);
                }
            }
        }
    }

}
