using System;
using UnityEngine;

public class HitboxHUD : MonoBehaviour
{
    [SerializeField] Canvas CanvasBotones;
    void OnTriggerEnter(Collider other)
    {
        CanvasBotones.gameObject.SetActive(true);
    }

    void OnTriggerExit(Collider other)
    {
        CanvasBotones.gameObject.SetActive(false);
    }
}
