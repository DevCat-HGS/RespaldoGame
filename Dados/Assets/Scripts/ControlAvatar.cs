 using UnityEngine;

public class ControlAvatar : MonoBehaviour
{
    public Material materialSeleccionado; // Asigna tus materiales en el inspector
    public Material materialNoSeleccionado;
    public Renderer[] avatares;

    void ChangeFaceMaterial(int indice, bool seleccionado)
    {
        Renderer renderer = avatares[indice];
        Material[] currentMaterials = renderer.materials; // Obtiene los materiales actuales
        currentMaterials[0] = seleccionado ? materialSeleccionado : materialNoSeleccionado; // Cambia el material de la cara específica
        renderer.materials = currentMaterials; // Asigna los nuevos materiales
    }
}
