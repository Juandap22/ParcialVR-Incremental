using System;
using System.Linq;
using UnityEngine;

public class ElementSpawner : MonoBehaviour
{
    [Tooltip("Posicion de generacion sobre la Conveyorde elementos")]
    [SerializeField] Transform SpawnPoint;
    [Tooltip("Elegir elemento a spawnear")]
    public int SelectElement = 0;
    [Tooltip("Conjunto de elementos spawneables")]
    [SerializeField] GameObject[] Spawnables;
    

    public void SpawnElement()
    {
        //no hay nada :O
        if(Spawnables != null) return;

        //hay algo y escogiste algo :O
        Instantiate(Spawnables[SelectElement], SpawnPoint.position, SpawnPoint.rotation);
    }

    //objeto siguiente en el array  
    public void NextObject()
    {
        //Si el selector es menor que el total de elementos en el array
        if(SelectElement < Spawnables.Length - 1)
        {
            SelectElement++;
        }
    }
    
    //objeto anterior en array
    public void LastObject()
    {
        //Si el elemento a escoger no es 0, puede volver al anterior
        if(SelectElement > 0)
        {
            SelectElement--;
        }
    }
}
