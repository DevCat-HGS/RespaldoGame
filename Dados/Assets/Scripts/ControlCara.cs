using UnityEngine;

public class ControlCara : MonoBehaviour
{
    private bool enSuelo = false;

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Tapete"))
        {
            enSuelo = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        enSuelo = false;
    }

    public bool CompruebaSuelo()
    {
        return enSuelo;
    }
}
