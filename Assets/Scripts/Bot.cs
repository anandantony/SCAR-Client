using UnityEngine;
using UnityEngine.AI;

public class Bot : MonoBehaviour
{

    public NavMeshAgent agent;
    public Transform destination;

    public void Move()
    {
        transform.position = new Vector3(transform.position.x, destination.position.y, transform.position.z);
        agent.SetDestination(destination.position);
    }
}
