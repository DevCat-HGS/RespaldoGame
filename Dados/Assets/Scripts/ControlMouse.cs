using UnityEngine;

namespace Assets.Scripts
{
    public class ControlMouse : MonoBehaviour
    {

        public Transform dado1; // Asigna este transform en el Inspector
        public Transform dado2; // Asigna este transform en el Inspector
        public float rotationSpeed = 360f; // Velocidad de rotación en grados por segundo

        private Camera cam;
        private bool isDragging = false;

        private Rigidbody rb1;
        private Rigidbody rb2;

        private float fixedY; // Posición Y fija para dado1

        private float initialDistanceX; // Distancia inicial en X entre los dados

        void Start()
        {
            cam = Camera.main;
            rb1 = dado1.GetComponent<Rigidbody>();
            rb2 = dado2.GetComponent<Rigidbody>();

            if (rb1 == null || rb2 == null)
            {
                Debug.LogError("Los dados deben tener un componente Rigidbody.");
            }

            // Guarda las posiciones Y iniciales
            fixedY = dado1.position.y;

            // Asegúrate de que los Rigidbody sean cinemáticos al inicio
            rb1.isKinematic = true; // No caen al inicio
            rb2.isKinematic = true; // No caen al inicio

            // Calcula la distancia inicial en X
            initialDistanceX = dado2.position.x - dado1.position.x;
        }

        void Update()
        {
            // Comienza a arrastrar al hacer clic
            if (Input.GetMouseButtonDown(0))
            {
                Ray ray = cam.ScreenPointToRay(Input.mousePosition);

                if (Physics.Raycast(ray, out RaycastHit hit))
                {
                    // Verifica si uno de los objetos es el que se ha clicado
                    if (hit.transform == dado1 || hit.transform == dado2)
                    {
                        isDragging = true;
                    }
                }
            }

            // Mueve ambos objetos mientras se arrastran
            if (isDragging)
            {
                Ray ray = cam.ScreenPointToRay(Input.mousePosition);
                Plane plane = new(Vector3.up, fixedY); // Plane en la posición fija de Y

                if (plane.Raycast(ray, out float distance))
                {
                    Vector3 targetPosition = ray.GetPoint(distance);

                    // Mantiene la altura fija mientras se arrastra
                    targetPosition.y = fixedY; // O fixedY2, ya que ambas deben ser iguales
                    dado1.position = targetPosition;

                    // Mantiene la distancia en X entre dado1 y dado2
                    Vector3 dado2Position = dado1.position;
                    dado2Position.x = dado1.position.x + initialDistanceX; // Ajusta la posición en X
                    dado2.position = dado2Position;

                    // Rotar los dados
                    // Rotar los dados en todos los ejes
                    dado1.Rotate(Vector3.right, rotationSpeed * Time.deltaTime);
                    dado1.Rotate(Vector3.up, rotationSpeed * Time.deltaTime);
                    dado1.Rotate(Vector3.forward, rotationSpeed * Time.deltaTime);

                    dado2.Rotate(Vector3.right, rotationSpeed * Time.deltaTime);
                    dado2.Rotate(Vector3.up, rotationSpeed * Time.deltaTime);
                    dado2.Rotate(Vector3.forward, rotationSpeed * Time.deltaTime);
                }
            }

            // Al soltar el botón, permite que los dados caigan
            if (Input.GetMouseButtonUp(0))
            {
                isDragging = false;

                // Activa la gravedad para que los dados caigan
                rb1.isKinematic = false; // Permitir que dado1 caiga
                rb2.isKinematic = false; // Permitir que dado2 caiga
            }
        }
    }
}