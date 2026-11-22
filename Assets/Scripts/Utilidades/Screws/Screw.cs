using UnityEngine;

public class Screw : MonoBehaviour
{
    [Header("Rosca")]
    [Tooltip("Eje del tornillo")]
    [SerializeField] Vector3 localAxis = Vector3.forward;
    [Tooltip("Vueltas totales")]
    [SerializeField] float VueltasTotales;
    [Tooltip("Profundidad total del tornillo")]
    [SerializeField] float Profundidad;

    [Header("Input")]
    [SerializeField] float deadzoneDeg = 0.3f;

    Vector3 startPos;
    Quaternion startRot;
    float progreso;
    float dirGiro = 1;

    public float Progress => progreso;
    public bool Apretado => progreso >= 1f;

    void Awake()
    {
        startPos = transform.localPosition;
        startRot = transform.localRotation;
    }

    public void ApplyRotation(float deltaDegrees)
    {
        if (Mathf.Abs(deltaDegrees) < deadzoneDeg) return;

        progreso= (deltaDegrees * dirGiro) / (360f * VueltasTotales);
        progreso = Mathf.Clamp01(progreso);

        float angle = progreso * VueltasTotales * 360f;
        transform.localRotation = startRot * Quaternion.AngleAxis(angle, localAxis);
        transform.localPosition = startPos + localAxis.normalized * (progreso * Profundidad);
    }
}
