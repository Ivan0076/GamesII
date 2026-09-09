using UnityEngine;

public class MirarObjetivo : MonoBehaviour
{
    [Header("Configuración de Objetivos")]
    [SerializeField] private Transform objetivo; // Jugador
    [SerializeField] private Transform cabeza;   // Head

    [Header("Huesos del Cuello (En orden desde el cuerpo a la cabeza)")]
    [SerializeField] private Transform[] huesosCuello; // Neck_02, Neck_03, Neck_04

    [Header("Ajustes de Rotación")]
    [SerializeField] private Vector3 ejeDeCompensacion = new Vector3(0, 0, 0); // Lo ajustaremos en el inspector
    [SerializeField] private float velocidadGiro = 5f;
    [SerializeField] private float anguloMaximo = 70f;

    // Ya no guardamos rotaciones fijas en Start para no romper las animaciones

    void LateUpdate()
    {
        if (cabeza == null || objetivo == null) return;

        // 1. Obtener la dirección horizontal hacia el jugador
        Vector3 posicionObjetivoPlana = new Vector3(objetivo.position.x, cabeza.position.y, objetivo.position.z);
        Vector3 direccionAlObjetivo = posicionObjetivoPlana - cabeza.position;

        if (direccionAlObjetivo.sqrMagnitude < 0.01f) return;

        // 2. Comprobar si el jugador está frente al dinosaurio
        float angulo = Vector3.Angle(transform.forward, direccionAlObjetivo);

        // Si el jugador no está en su rango de visión, dejamos que actúen las animaciones normales
        if (angulo > anguloMaximo) return;

        // 3. Calcular la rotación global deseada hacia el jugador con la compensación
        Quaternion rotacionMirarA = Quaternion.LookRotation(direccionAlObjetivo) * Quaternion.Euler(ejeDeCompensacion);

        // 4. Repartir el giro suave entre el cuello y la cabeza sumándose a la animación actual
        if (huesosCuello != null && huesosCuello.Length > 0)
        {
            float pesoPorHueso = 1f / (1f + huesosCuello.Length);

            for (int i = 0; i < huesosCuello.Length; i++)
            {
                if (huesosCuello[i] == null) continue;

                // Mezclamos de forma fluida la rotación que trae la animación con la dirección del jugador
                Quaternion rotacionTargetHueso = Quaternion.Slerp(huesosCuello[i].rotation, rotacionMirarA, pesoPorHueso);
                huesosCuello[i].rotation = Quaternion.Slerp(huesosCuello[i].rotation, rotacionTargetHueso, Time.deltaTime * velocidadGiro);
            }
        }

        // 5. Aplicar el giro final a la cabeza
        cabeza.rotation = Quaternion.Slerp(cabeza.rotation, rotacionMirarA, Time.deltaTime * velocidadGiro);
    }
}
