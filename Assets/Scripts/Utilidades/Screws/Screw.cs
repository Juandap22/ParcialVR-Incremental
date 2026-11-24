using UnityEngine;
using UnityEngine.Events;

// Driver visual del tornillo: rota y hunde el modelo segun el giro que le manda el destornillador.
// Ponlo en el MODELO 3D (hijo) y no en la raiz que agarra el socket, para que no peleen por la posicion.
public class Screw : MonoBehaviour
{
    [Header("Rosca")]
    [Tooltip("Eje del tornillo (local). Al apretar, el tornillo avanza hacia este eje")]
    [SerializeField] Vector3 localAxis = Vector3.forward;
    [Tooltip("Vueltas totales")]
    [SerializeField] float VueltasTotales = 5f;
    [Tooltip("Profundidad total del tornillo")]
    [SerializeField] float Profundidad = 0.02f;
    [Tooltip("1 = apretar es sentido horario mirando hacia el eje. Pon -1 para invertirlo")]
    [SerializeField] float dirGiro = 1f;

    [Header("Eventos")]
    [Tooltip("Se dispara UNA sola vez, la primera vez que el tornillo llega al final")]
    public UnityEvent OnApretado;

    Vector3 startPos;
    Quaternion startRot;
    float anguloActual; // grados acumulados: 0 .. VueltasTotales * 360
    float progreso;
    bool yaApretado;

    public float Progress => progreso;
    public bool Apretado => progreso >= 1f;
    public Vector3 EjeMundo => transform.TransformDirection(localAxis.normalized);

    void Awake()
    {
        startPos = transform.localPosition;
        startRot = transform.localRotation;
    }

    // deltaDegrees: giro de ESTE frame alrededor del eje (positivo = apretar). El tornillo acumula el total.
    public void ApplyRotation(float deltaDegrees)
    {
        if (VueltasTotales <= 0f) return;

        float maxAngulo = VueltasTotales * 360f;
        anguloActual = Mathf.Clamp(anguloActual + deltaDegrees * dirGiro, 0f, maxAngulo);
        progreso = anguloActual / maxAngulo;

        transform.localRotation = startRot * Quaternion.AngleAxis(anguloActual, localAxis);
        transform.localPosition = startPos + (startRot * localAxis.normalized) * (progreso * Profundidad);

        if (!yaApretado && progreso >= 1f)
        {
            yaApretado = true;
            OnApretado.Invoke();
        }
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawRay(transform.position, EjeMundo * 0.1f);
    }
}
