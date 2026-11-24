using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

// Ponlo en el objeto raiz del destornillador (el que tiene XRGrabInteractable y Rigidbody).
// La punta debe tener un Collider con Is Trigger (puede ser un hijo).
[RequireComponent(typeof(XRGrabInteractable))]
public class Destornillador : MonoBehaviour
{
    [Header("Punta")]
    [Tooltip("Hacia donde apunta la punta, en espacio local")]
    [SerializeField] Vector3 ejePuntaLocal = Vector3.forward;
    [Tooltip("Vector local perpendicular a la punta; sirve para medir cuanto gira la muneca")]
    [SerializeField] Vector3 ejeReferenciaLocal = Vector3.right;
    [Tooltip("Maximo desalineo (grados) entre la punta y el eje del tornillo para poder girar")]
    [SerializeField] float toleranciaAlineacion = 35f;

    XRGrabInteractable grab;
    Screw tornillo;
    Vector3 refAnterior;
    bool tieneRef;

    void Awake()
    {
        grab = GetComponent<XRGrabInteractable>();
    }

    void OnDisable()
    {
        tornillo = null;
        tieneRef = false;
    }

    void OnTriggerEnter(Collider other)
    {
        Screw s = BuscarTornillo(other);
        if (s == null) return;

        tornillo = s;
        tieneRef = false;
    }

    void OnTriggerExit(Collider other)
    {
        if (tornillo != null && BuscarTornillo(other) == tornillo)
        {
            tornillo = null;
            tieneRef = false;
        }
    }

    Screw BuscarTornillo(Collider c)
    {
        Screw s = c.GetComponentInParent<Screw>();
        if (s == null) s = c.GetComponentInChildren<Screw>();
        return s;
    }

    void Update()
    {
        if (tornillo == null || !grab.isSelected) { tieneRef = false; return; }

        Vector3 punta = transform.TransformDirection(ejePuntaLocal.normalized);
        Vector3 eje = tornillo.EjeMundo;

        // la punta tiene que apuntar al tornillo
        if (Vector3.Angle(punta, eje) > toleranciaAlineacion) { tieneRef = false; return; }

        // referencia de la muneca proyectada en el plano perpendicular al eje del tornillo
        Vector3 refActual = Vector3.ProjectOnPlane(transform.TransformDirection(ejeReferenciaLocal.normalized), eje);
        if (refActual.sqrMagnitude < 1e-6f) { tieneRef = false; return; }
        refActual.Normalize();

        if (tieneRef)
        {
            Vector3 anterior = Vector3.ProjectOnPlane(refAnterior, eje);
            if (anterior.sqrMagnitude > 1e-6f)
            {
                float delta = Vector3.SignedAngle(anterior.normalized, refActual, eje);
                if (!Mathf.Approximately(delta, 0f)) tornillo.ApplyRotation(delta);
            }
        }

        refAnterior = refActual;
        tieneRef = true;
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawRay(transform.position, transform.TransformDirection(ejePuntaLocal.normalized) * 0.1f);
    }
}
