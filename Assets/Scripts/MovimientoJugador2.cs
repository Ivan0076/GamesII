using UnityEngine;

public class MovimientoJugador2 : MonoBehaviour
{
    [Header("Movimiento")]
    public float velocidadMovimiento = 10f;

    [Header("Camara")]
    public Transform transformacionCamara;
    public float sensibilidadMirada = 80f;
    public float anguloMaximo = 80f;

    [Header("Referencias")]
    public Rigidbody cuerpoRigido;

    private Vector3 direccionMovimiento;
    private float rotacionVertical = 0f;

    private void Update()
    {
        direccionMovimiento = Vector3.zero;

        // Movimiento
        if (Input.GetKey(KeyCode.UpArrow))
            direccionMovimiento += transform.forward;

        if (Input.GetKey(KeyCode.DownArrow))
            direccionMovimiento -= transform.forward;

        // Rotacion horizontal
        float movimientoHorizontalCamara = 0f;

        if (Input.GetKey(KeyCode.J))
            movimientoHorizontalCamara = -1f;

        if (Input.GetKey(KeyCode.L))
            movimientoHorizontalCamara = 1f;

        transform.Rotate(
            Vector3.up * movimientoHorizontalCamara *
            sensibilidadMirada * Time.deltaTime
        );

        // Rotacion vertical
        float movimientoVerticalCamara = 0f;

        if (Input.GetKey(KeyCode.I))
            movimientoVerticalCamara = 1f;

        if (Input.GetKey(KeyCode.K))
            movimientoVerticalCamara = -1f;

        rotacionVertical -= movimientoVerticalCamara *
                            sensibilidadMirada *
                            Time.deltaTime;

        rotacionVertical = Mathf.Clamp(
            rotacionVertical,
            -anguloMaximo,
            anguloMaximo
        );

        if (transformacionCamara != null)
        {
            transformacionCamara.localRotation =
                Quaternion.Euler(rotacionVertical, 0f, 0f);
        }
    }

    private void FixedUpdate()
    {
        if (cuerpoRigido == null)
            return;

        Vector3 velocidad =
            direccionMovimiento.normalized *
            velocidadMovimiento;

        velocidad.y = cuerpoRigido.linearVelocity.y;

        cuerpoRigido.linearVelocity = velocidad;
    }
}