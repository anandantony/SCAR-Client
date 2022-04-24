using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AssignTarget : MonoBehaviour
{
    public Transform target;

    public void AssignTargetsAndStartPath()
    {
        transform.position = target.position;
    }
}
