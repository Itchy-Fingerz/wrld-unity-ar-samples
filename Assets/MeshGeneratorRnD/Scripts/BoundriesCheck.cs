using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;


[RequireComponent(typeof(ARRaycastManager))]
public class BoundriesCheck : MonoBehaviour
{
    private ARRaycastManager m_raycastManager;
    private List<ARRaycastHit> m_hits = new List<ARRaycastHit>();

    public delegate void ClickAction(string name);
    public static event ClickAction OnSurfaceDetected;
    public static event ClickAction OnSurfaceNotDetected;
    // Start is called before the first frame update
    void Awake()
    {
        m_raycastManager = GetComponent<ARRaycastManager>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        RaycastHit hit;
        // Does the ray intersect any objects excluding the player layer
        //(touchPosition, m_hits, TrackableType.PlaneWithinPolygon)
        if (m_raycastManager.Raycast(transform.TransformDirection(Vector3.down), m_hits, TrackableType.All))
        {
            //Debug.DrawRay(transform.position, transform.TransformDirection(Vector3.down) * , Color.yellow);
            //Debug.Log("Did Hit");
            OnSurfaceDetected?.Invoke(gameObject.name);
        }
        else
        {
            //Debug.DrawRay(transform.position, transform.TransformDirection(Vector3.down) * 1000, Color.white);
            OnSurfaceNotDetected?.Invoke(gameObject.name);
            //Debug.Log("Did not Hit");
        }
    }
}
