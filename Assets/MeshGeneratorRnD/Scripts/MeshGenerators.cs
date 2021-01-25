using System;
using System.Collections.Generic;
using Unity.Collections;

namespace UnityEngine.XR.ARFoundation
{
    public static class MeshGenerators
    {
        public static bool GenerateMesh2(Mesh mesh, Pose pose, NativeArray<Vector2> convexPolygon, float areaTolerance = 1e-6f)
        {
            if (mesh == null)
                throw new ArgumentNullException("mesh");

            if (convexPolygon.Length < 3)
                return false;

            s_Vertices.Clear();
            var center = Vector3.zero;
            //bottom mesh
            foreach (var point2 in convexPolygon)
            {
                //float z = Mathf.PerlinNoise(point2.x * 2f, point2.y * 2f) * 5f;
                var point3 = new Vector3(point2.x, 0f, point2.y);
                center += point3;
                s_Vertices.Add(point3);

                //new test

                var point4 = new Vector3(point2.x, .1f, point2.y);
                center += point4;
                s_Vertices.Add(point4);
            }
            //new code
            //upper mesh
            //foreach (var point2 in convexPolygon)
            //{
            //    //float z = Mathf.PerlinNoise(point2.x * 2f, point2.y * 2f) * 5f;
            //    var point3 = new Vector3(point2.x, .1f, point2.y);
            //    center += point3;
            //    s_Vertices.Add(point3);
            //}


            center /= convexPolygon.Length;
            s_Vertices.Add(center);

            if (!GenerateIndices2(s_Indices, s_Vertices, areaTolerance))
                return false;

            mesh.Clear();
            mesh.SetVertices(s_Vertices);

            const int subMesh = 0;
            const bool calculateBounds = true;
            mesh.SetTriangles(s_Indices, subMesh, calculateBounds);

            GenerateUvs2(s_Uvs, pose, s_Vertices);
            mesh.SetUVs(0, s_Uvs);

            var normals = s_Vertices;
            for (int i = 0; i < normals.Count; ++i)
                normals[i] = Vector3.up;

            mesh.SetNormals(normals);

            return true;
        }

        public static void GenerateUvs2(List<Vector2> Uvs, Pose pose, List<Vector3> vertices)
        {

            var planeRotation = pose.rotation;
            var rotationAxis = new Vector3(planeRotation.x, planeRotation.y, planeRotation.z);
            var projection = Vector3.Project(rotationAxis, planeRotation * Vector3.up);
            var normalizedTwist = (new Vector4(projection.x, projection.y, projection.z, planeRotation.w)).normalized;
            var inverseTwist = new Quaternion(normalizedTwist.x, normalizedTwist.y, normalizedTwist.z, -normalizedTwist.w);
            var untwistedRotation = inverseTwist * pose.rotation;

            var sessionSpaceRight = untwistedRotation * Vector3.right;
            var sessionSpaceForward = untwistedRotation * Vector3.forward;

            Uvs.Clear();
            foreach (var vertex in vertices)
            {
                var vertexInSessionSpace = pose.rotation * vertex + pose.position;

                var uv = new Vector2(
                    Vector3.Dot(vertexInSessionSpace, sessionSpaceRight),
                    Vector3.Dot(vertexInSessionSpace, sessionSpaceForward));

                Uvs.Add(uv);
            }
        }

        public static bool GenerateIndices2(List<int> indices, List<Vector3> convexPolygon, float areaTolerance = 2e-6f)
        {
            indices.Clear();

            var numBoundaryVertices = convexPolygon.Count - 1;
            var centerIndex = numBoundaryVertices;
            var areaToleranceSquared = areaTolerance * areaTolerance;

            for (int i = 0; i < numBoundaryVertices; ++i)
            {
                int j = (i + 1) % numBoundaryVertices;

                var a = convexPolygon[i] - convexPolygon[centerIndex];
                var b = convexPolygon[j] - convexPolygon[centerIndex];

                var areaSquared = Vector3.Cross(a, b).sqrMagnitude * 0.25f;
                if (areaSquared < areaToleranceSquared)
                    return false;

                indices.Add(centerIndex);
                indices.Add(i);
                indices.Add(j);
            }

            return true;
        }

        static List<int> s_Indices = new List<int>();
        static List<Vector2> s_Uvs = new List<Vector2>();
        static List<Vector3> s_Vertices = new List<Vector3>();
    }
}