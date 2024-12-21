using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.UIElements;

public class ControlJugador : MonoBehaviourPunCallbacks
{
    public static ControlJugador instancia;
    private List<Jugador> jugadores;
    private List<GameObject> Dados;
    private int jugadorActual = 0;
    public TMP_Text mensaje;
    public Material materialSinSeleccionar;
    public Material materialSeleccionado;
    private bool turnoEnProceso = false;

    private enum EstadoJuego
    {
        Esperando,
        EnJuego,
        Finalizado
    }

    private EstadoJuego estadoActual;

    public override void OnEnable()
    {
        instancia = instancia != null ? instancia : this;
    }

    [PunRPC]
    public void SetJugador(int posicion, string name)
    {
        Player[] players = PhotonNetwork.PlayerList;
        Debug.Log($"Jugador{posicion + 1} {name}");
        Transform jugador = gameObject.transform.Find($"Jugador{posicion + 1}");
        Transform nombreTransform = jugador.transform.Find("Nombre");
        bool mesaCompleta = players.Length == 4;
        Transform canvasTransform = gameObject.transform.Find("Canvas");
        GameObject general = canvasTransform.Find("General").transform.gameObject;
        general.SetActive(mesaCompleta);
        if (nombreTransform != null)
        {
            TMP_Text nombre = nombreTransform.GetComponent<TMP_Text>();
            nombre.text = name;
        }

        if (photonView.IsMine)
        {
            jugador.gameObject.SetActive(true);
            gameObject.SetActive(true);
        }
    }

    public void SiguienteTurno()
    {
        photonView.RPC("IniciarTurno", RpcTarget.All, 0);
    }

    public bool PuedeJugar()
    {
        return PhotonNetwork.LocalPlayer.ActorNumber - 1 == jugadorActual && !turnoEnProceso;
    }

    public void ActualizarValor(int valor)
    {
        var jugador = jugadores[this.jugadorActual];
        if (jugador.valorDado1 == 0)
        {
            jugador.valorDado1 = valor;
        }
        else
        {
            jugador.valorDado2 = valor;
            jugador.Actualizar();
            this.jugadorActual++;
            for (int i = 0; i < jugadores.Count; i++)
            {
                Material material = this.jugadorActual == i ? materialSeleccionado : materialSinSeleccionar;
                jugadores[i].Seleccionar(material);
            }
            if (this.jugadorActual == jugadores.Count)
            {
                ActualizarGanador();
            }
        }
    }

    public void ActualizarMensaje()
    {
        if (!string.IsNullOrWhiteSpace(mensaje.text))
        {
            mensaje.text = null;
        }
    }

    private void ActualizarGanador()
    {
        var puntajeMaximo = jugadores.Max(jugador => jugador.puntos);

        var ganadores = jugadores.Where(jugador => jugador.puntos == puntajeMaximo).ToList();
        string mensaje = null;
        if (ganadores.Count() > 1)
        {
            var nombreGanadores = string.Join(" ", ganadores.Select(p => $"{p.nombre}"));
            mensaje = $"Los jugadores {nombreGanadores} han empatado";
            jugadores = ganadores;
            jugadores.ForEach(ganador =>
            {
                ganador.puntos = 0;
                ganador.valorDado1 = 0;
                ganador.valorDado2 = 0;
            });
            jugadorActual = 0;
            jugadores[0].Seleccionar(materialSeleccionado);
        }
        else
        {
            var ganador = ganadores[0];
            mensaje = $"El jugador {ganador.nombre} ha ganado con {ganador.puntos} puntos";
        }

        this.mensaje.text = mensaje;
    }

    private void Start()
    {
        jugadores = new List<Jugador>();
    }

    private void CambiarEstado(EstadoJuego nuevoEstado)
    {
        estadoActual = nuevoEstado;
        switch (estadoActual)
        {
            case EstadoJuego.Esperando:
                mensaje.text = "Esperando jugadores...";
                break;
            case EstadoJuego.EnJuego:
                mensaje.text = $"Turno de {jugadores[jugadorActual].nombre}";
                break;
            case EstadoJuego.Finalizado:
                ActualizarGanador();
                break;
        }
    }
}

public class Jugador
{
    public string nombre;
    public readonly Renderer avatar;
    public int valorDado1 = 0;
    public int valorDado2 = 0;
    public int puntos = 0;
    private readonly TMP_Text texto; // Referencia a un TextMeshPro

    public Jugador(string nombre, TMP_Text textoUI, Renderer avatar)
    {
        this.nombre = nombre;
        texto = textoUI;
        textoUI.color = Color.gray;
        this.avatar = avatar;
    }

    // Método para actualizar el texto del jugador
    public void Actualizar()
    {
        puntos = valorDado1 + valorDado2;
        texto.text = $"{puntos} puntos";
        texto.color = Color.white;
    }

    public void Seleccionar(Material material)
    {
        Material[] materials = avatar.materials;
        materials[0] = material;
        avatar.materials = materials;
    }
}