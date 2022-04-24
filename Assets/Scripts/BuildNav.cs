using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.AI.Navigation;

public class BuildNav : MonoBehaviour
{

    public NavMeshSurface[] surfaces;

    // Use this for initialization
    public void Build()
    {
        for (int i = 0; i < surfaces.Length; i++)
        {
            surfaces[i].BuildNavMesh();
        }
    }

}