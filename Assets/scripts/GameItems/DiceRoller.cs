using UnityEngine;

public class DiceRoller : MonoBehaviour
{
    public float moveSpeed = 5f;    // Velocidad de movimiento horizontal
    public float jumpForce = 5f;    // Fuerza de salto vertical
    public float gravityMultiplier = 2f;  // Para ajustar la gravedad del jugador y hacer el salto más realista

    private Rigidbody rb;
    private bool isGrounded;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    void Update()
    {
        MovePlayer();
        HandleJump();
    }

    void MovePlayer()
    {
        // Obtener el movimiento horizontal (izquierda/derecha)
        float moveX = Input.GetAxis("Horizontal");  // A/D o flechas izquierda/derecha

        // Obtener el movimiento vertical (adelante/atrás)
        float moveZ = Input.GetAxis("Vertical");  // W/S o flechas arriba/abajo

        // Dirección del movimiento
        Vector3 moveDirection = new Vector3(moveX, 0f, moveZ).normalized;

        // Aplicar movimiento
        rb.MovePosition(transform.position + moveDirection * moveSpeed * Time.deltaTime);
    }

    void HandleJump()
    {
        // Verificar si el jugador está en el suelo antes de permitir el salto
        if (isGrounded && Input.GetKeyDown(KeyCode.Space))
        {
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);  // Aplicar fuerza de salto
        }

        // Aplicar gravedad extra para hacer el salto más realista
        rb.AddForce(Vector3.down * gravityMultiplier, ForceMode.Acceleration);
    }

    void OnCollisionStay(Collision other)
    {
        // Verificar si el jugador está tocando el suelo
        if (other.gameObject.CompareTag("Tapete"))
        {
            isGrounded = true;
        }
    }

    void OnCollisionExit(Collision other)
    {
        // El jugador dejó de tocar el suelo
        if (other.gameObject.CompareTag("Tapete"))
        {
            isGrounded = false;
        }
    }
}
