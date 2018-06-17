using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleSorting : MonoBehaviour
{

    public int sortingOrder;
    public string sortingLayerName;

    // Use this for initialization
    void Start()
    {
        // Set the sorting layer of the particle system.
        GetComponent<ParticleSystem>().GetComponent<Renderer>().sortingLayerName = sortingLayerName;
        GetComponent<ParticleSystem>().GetComponent<Renderer>().sortingOrder = sortingOrder;
    }
}
