using SplineMeshTools.Misc;
using UnityEngine;

public class DetenerConveyor : MonoBehaviour
{
    AnimateTextureOffset ato;
    ConveyorBeltMover cbm;

    void Start()
    {
        ato = GetComponentInParent<AnimateTextureOffset>();
        cbm = GetComponentInParent<ConveyorBeltMover>();
    }

    void OnTriggerEnter(Collider other)
    {
        // solo reaccionar a objetos con Rigidbody (los que transporta la cinta)
        if (other.attachedRigidbody == null) return;

        // detiene el movimiento de la cinta
        if (cbm != null) cbm.enabled = false;

        // congela la animacion del material
        if (ato != null) ato.enabled = false;
    }
    void OnTriggerExit(Collider other)
    {
        if (cbm != null) cbm.enabled = true;

        // congela la animacion del material
        if (ato != null) ato.enabled = true;
    }
}
