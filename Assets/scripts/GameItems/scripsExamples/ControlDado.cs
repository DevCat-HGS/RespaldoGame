using UnityEngine;

public class ControlDado : MonoBehaviour
{
    private float ejeX;
    private float ejeY;
    private float ejeZ;
    private Vector3 posicionInicial;
    private Rigidbody rbDado;
    public bool dadoMovimiento = true;
    public ControlCara[] lados = new ControlCara[6];
    public int valorDado;
    private int ladoOculto;


    // Start is called before the first frame update
    void Start()
    {
        posicionInicial = transform.position;
        rbDado = GetComponent<Rigidbody>();
        PrepararDado();
    }

    // Update is called once per frame
    void Update()
    {

        if (Input.GetKeyDown(KeyCode.Space))
        {
            PrepararDado();
        }

        if (rbDado.IsSleeping() && dadoMovimiento)
        {
            dadoMovimiento = false;
            ladoOculto = ComprobarLados();
            valorDado = 7 - ladoOculto;
            if(valorDado == 7)
            {
                rbDado.AddForce(3f, 0, 0, ForceMode.Impulse);
                dadoMovimiento = true;
            }
        }
    }

    void PrepararDado()
    {
        transform.position = posicionInicial;
        rbDado.isKinematic = true;
        //ControlJugador.instancia.ActualizarMensaje();
        dadoMovimiento = true;
        ejeX = Random.Range(0, 4) * 90;
        ejeY = Random.Range(0, 4) * 90;
        ejeZ = Random.Range(0, 4) * 90;
        transform.Rotate(ejeX, ejeY, ejeZ);
    }

    int ComprobarLados()
    {
        int valor = 0;
        for (int i = 0; i < 6; i++)
        {
            if (lados[i].CompruebaSuelo())
            {
                valor = i + 1;
            }
        }

        return valor;
    }
}


