using System;
using System.Collections.Generic;
using UnityEngine.XR.ARSubsystems;

namespace UnityEngine.XR.ARFoundation
{
    [RequireComponent(typeof(ARPlane))]
    public sealed class MeshVisual : MonoBehaviour
    {
        public Mesh mesh { get; private set; }

        void OnBoundaryChanged(ARPlaneBoundaryChangedEventArgs eventArgs)
        {
            var boundary = m_Plane.boundary;
            if (!MeshGenerators.GenerateMesh2(mesh, new Pose(transform.localPosition, transform.localRotation), boundary))
                return;

            var lineRenderer = GetComponent<LineRenderer>();
            if (lineRenderer != null)
            {
                lineRenderer.positionCount = boundary.Length;
                for (int i = 0; i < boundary.Length; ++i)
                {
                    var point2 = boundary[i];
                    //float z = Mathf.PerlinNoise(point2.x * 2f, point2.y * 2f) * 5f;
                    lineRenderer.SetPosition(i, new Vector3(point2.x, .2f, point2.y));
                }
            }

            var meshFilter = GetComponent<MeshFilter>();
            if (meshFilter != null)
                meshFilter.sharedMesh = mesh;

            var meshCollider = GetComponent<MeshCollider>();
            if (meshCollider != null)
                meshCollider.sharedMesh = mesh;
        }

        void DisableComponents()
        {
            enabled = false;

            var meshCollider = GetComponent<MeshCollider>();
            if (meshCollider != null)
                meshCollider.enabled = false;

            UpdateVisibility();
        }

        void SetVisible(bool visible)
        {
            var meshRenderer = GetComponent<MeshRenderer>();
            if (meshRenderer != null)
                meshRenderer.enabled = visible;

            var lineRenderer = GetComponent<LineRenderer>();
            if (lineRenderer != null)
                lineRenderer.enabled = visible;
        }

        void UpdateVisibility()
        {
            var visible = enabled &&
                (m_Plane.trackingState != TrackingState.None) &&
                (ARSession.state > ARSessionState.Ready) &&
                (m_Plane.subsumedBy == null);

            SetVisible(visible);
        }

        void Awake()
        {
            mesh = new Mesh();
            m_Plane = GetComponent<ARPlane>();
        }

        void OnEnable()
        {
            m_Plane.boundaryChanged += OnBoundaryChanged;
            UpdateVisibility();
            OnBoundaryChanged(default(ARPlaneBoundaryChangedEventArgs));
        }

        void OnDisable()
        {
            m_Plane.boundaryChanged -= OnBoundaryChanged;
            UpdateVisibility();
        }

        void Update()
        {
            if (transform.hasChanged)
            {
                var lineRenderer = GetComponent<LineRenderer>();
                if (lineRenderer != null)
                {
                    if (!m_InitialLineWidthMultiplier.HasValue)
                        m_InitialLineWidthMultiplier = lineRenderer.widthMultiplier;

                    lineRenderer.widthMultiplier = m_InitialLineWidthMultiplier.Value * transform.lossyScale.x;
                }
                else
                {
                    m_InitialLineWidthMultiplier = null;
                }

                transform.hasChanged = false;
            }

            if (m_Plane.subsumedBy != null)
            {
                DisableComponents();
            }
            else
            {
                UpdateVisibility();
            }
        }

        float? m_InitialLineWidthMultiplier;
        ARPlane m_Plane;
    }
}
