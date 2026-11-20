using System;
using UnityEngine;

/// <summary>
/// Preferencias de confort VR. Se guardan en PlayerPrefs como JSON desde el GameManager.
/// </summary>
[Serializable]
public class ComfortSettings
{
    public bool useSnapTurn = true;
    [Range(15f, 90f)] public float snapTurnDegrees = 45f;
    public bool useVignette = true;
    public bool seatedMode = false;
}
