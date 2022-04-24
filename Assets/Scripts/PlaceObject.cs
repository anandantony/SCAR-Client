using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using System;

public class PlaceObject : MonoBehaviour
{

    public GameObject m_Object;
    public Transform parent;
    public float distance = 8f;
    RaycastHit[] hits;
    public UnityEvent onPlaced;
    public bool isTarget = false;

    void Start()
    {
        Input.simulateMouseWithTouches = true;
    }

    void Update()
    {
        if (!isTarget)
            return;
        // //Detect when there is a mouse click
        if (Input.GetMouseButtonDown(0))
        {
            PlaceTarget(Input.mousePosition);
        }
    }

    public void Place(Vector2 pos, float width = 1f, float length = 1f)
    {
        Ray ray = Camera.main.ScreenPointToRay(pos);
        hits = Physics.RaycastAll(ray, distance);
        Array.Sort(hits, (RaycastHit x, RaycastHit y) => x.distance.CompareTo(y.distance));
        if (hits[0].transform.tag == "Obstacle")
            return;
        Debug.Log(hits[0].transform.name);
        var obj = Instantiate(m_Object, hits[0].point, Quaternion.Euler(hits[0].normal), parent);
        if (onPlaced != null)
        {
            onPlaced.Invoke();
        }
    }

    public void Place(Ray ray, float width = 1f, float length = 1f, Vector3 rotation = new Vector3())
    {
        hits = Physics.RaycastAll(ray, distance);
        Array.Sort(hits, (RaycastHit x, RaycastHit y) => x.distance.CompareTo(y.distance));
        if (hits[0].transform.tag == "Obstacle")
            return;
        Debug.Log(hits[0].transform.name);
        var obj = Instantiate(m_Object, hits[0].point, Quaternion.identity, parent);
        obj.transform.localScale = new Vector3(width, obj.transform.localScale.y, length);
        obj.transform.rotation = Quaternion.Euler(rotation);
        if (onPlaced != null)
        {
            onPlaced.Invoke();
        }
    }

    public void PlaceTarget(Vector2 pos)
    {
        Ray ray = Camera.main.ScreenPointToRay(pos);
        hits = Physics.RaycastAll(ray, distance);
        if (hits == null)
            return;
        Array.Sort(hits, (RaycastHit x, RaycastHit y) => x.distance.CompareTo(y.distance));
        if (hits[0].transform.tag == "Obstacle")
            return;
        m_Object.transform.position = hits[0].point;
        if (onPlaced != null)
        {
            onPlaced.Invoke();
        }
    }


}