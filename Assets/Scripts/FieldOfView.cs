﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class FieldOfView : NetworkBehaviour
{
    [Range(0,360)]
    public float viewAngle;
    public float viewRadius;

    public LayerMask targetMask;
    public LayerMask obstacleMask;
    [HideInInspector]
    public List<Transform> visibleTargets = new List<Transform>();

    [Range(0.01f,10.0f)]
    public float meshResolution;
    public MeshFilter viewMeshFilter;
    public int edgeResolveIterations;
    public float edgeDstThreshold;

    Mesh viewMesh;

    private void Start()
    {
        if (isLocalPlayer)
        {
            viewMesh = new Mesh();
            viewMesh.name = "ViewMesh";
            viewMeshFilter.mesh = viewMesh;
            StartCoroutine(nameof(FindTargetWithDelay), 0.1f);
        }
    }

    private void LateUpdate()
    {
        if (isLocalPlayer)
        {
            DrawFieldOfView(); 
        }
    }

    IEnumerator FindTargetWithDelay(float delay)
    {
        while (true)
        {
            yield return new WaitForSeconds(delay);
            FindVisibleTargets();
        }
    }

    private void toggleTargetChildrenRenderers(Transform target, bool enabled)
    {
        GameObject obj = target.parent ? target.parent.gameObject : target.gameObject;
        Renderer[] playerRenderers = obj.GetComponentsInChildren<Renderer>();
        for (int i = 0; i < playerRenderers.Length; i++)
        {
            playerRenderers[i].enabled = enabled;
        }
    }

    void FindVisibleTargets()
    {
        Collider[] targetsInViewRadius = Physics.OverlapSphere(transform.position,
            viewRadius, targetMask);
        for (int i = 0; i < visibleTargets.Count; i++)
        {
            bool targetHasLeftViewRadius = true;
            for (int j = 0; j < targetsInViewRadius.Length; j++)
            {
                if (visibleTargets[i] == targetsInViewRadius[j].transform)
                {
                    targetHasLeftViewRadius = false;
                    break;
                }
            }
            if (visibleTargets[i] && targetHasLeftViewRadius)
                toggleTargetChildrenRenderers(visibleTargets[i], false);
        }
        visibleTargets.Clear();
        for (int i = 0; i < targetsInViewRadius.Length; i++)
        {
            Transform target = targetsInViewRadius[i].transform;
            Vector3 dirToTarget = (target.position - transform.position)
                .normalized;
            bool targetIsInFOV = false;
            if(Vector3.Angle(transform.forward, dirToTarget) < viewAngle / 2)
            {
                float dstToTarget = Vector3.Distance(transform.position,
                    target.position);
                if(!Physics.Raycast(transform.position, dirToTarget, dstToTarget,
                    obstacleMask))
                {
                    targetIsInFOV = true;
                    visibleTargets.Add(target);
                }
                else
                    targetIsInFOV = false;

            }
            else
                targetIsInFOV = false;
            toggleTargetChildrenRenderers(target, targetIsInFOV);
        }
    }

    void DrawFieldOfView()
    {
        int stepCount = Mathf.RoundToInt(viewAngle * meshResolution);
        if (stepCount == 0) return;
        float stepAngleSize = viewAngle / stepCount;
        List<Vector3> viewPoints = new List<Vector3>();
        ViewCastInfo oldViewCast = new ViewCastInfo();

        for (int i = 0; i <= stepCount; i++)
        {
            float angle = transform.eulerAngles.y - viewAngle / 2 + stepAngleSize * i;
            ViewCastInfo newViewCast = ViewCast(angle);

            if (i > 0)
            {
                bool edgeDstThresholdExceeded = Mathf.Abs(oldViewCast.dst
                    - newViewCast.dst) > edgeDstThreshold;
                if (oldViewCast.hit != newViewCast.hit ||
                    (oldViewCast.hit && newViewCast.hit && edgeDstThresholdExceeded))
                {
                    EdgeInfo edge = FindEdge(oldViewCast, newViewCast);
                    if (edge.pointA != Vector3.zero)
                    {
                        viewPoints.Add(edge.pointA);
                    }
                    if (edge.pointB != Vector3.zero)
                    {
                        viewPoints.Add(edge.pointB);
                    }
                }
            }

            viewPoints.Add(newViewCast.point);
            oldViewCast = newViewCast;
        }
        int vertexCount = viewPoints.Count + 1;
        Vector3[] vertices = new Vector3[vertexCount];
        int[] triangles = new int[(vertexCount - 2) * 3];
        vertices[0] = Vector3.zero;
        for (int i = 0; i < vertexCount - 1; i++)
        {
            vertices[i + 1] = transform.InverseTransformPoint(viewPoints[i]);
            if (i < vertexCount - 2)
            {
                triangles[i * 3] = 0;
                triangles[i * 3 + 1] = i + 1;
                triangles[i * 3 + 2] = i + 2;
            }
        }
        viewMesh.Clear();
        viewMesh.vertices = vertices;
        viewMesh.triangles = triangles;
        viewMesh.RecalculateNormals();  
    }

    EdgeInfo FindEdge(ViewCastInfo minViewCast, ViewCastInfo maxViewCast)
    {
        float minAngle = minViewCast.angle;
        float maxAngle = maxViewCast.angle;
        Vector3 minPoint = Vector3.zero;
        Vector3 maxPoint = Vector3.zero;
        for (int i = 0; i < edgeResolveIterations; i++)
        {
            float angle = (minAngle + maxAngle) / 2;
            ViewCastInfo newViewCast = ViewCast(angle);
            bool edgeDstThresholdExceeded = Mathf.Abs(minViewCast.dst
                - newViewCast.dst) > edgeDstThreshold;
            if (newViewCast.hit == minViewCast.hit && !edgeDstThresholdExceeded)
            {
                minAngle = angle;
                minPoint = newViewCast.point;
            }
            else
            {
                maxAngle = angle;
                maxPoint = newViewCast.point;
            }
        }
        return new EdgeInfo(minPoint, maxPoint);
    }

    ViewCastInfo ViewCast(float globalAngle)
    {
        Vector3 dir = DirFromAngle(globalAngle, true);
        RaycastHit hit;
        if(Physics.Raycast(transform.position, dir, out hit, viewRadius, obstacleMask))
        {
            return new ViewCastInfo(true, hit.point, hit.distance, globalAngle);
        }
        else
        {
            return new ViewCastInfo(false, transform.position + dir * viewRadius,
                viewRadius, globalAngle);
        }
    }

    public Vector3 DirFromAngle(float angleInDegrees, bool angleIsGlobal)
    {
        if (!angleIsGlobal)
        {
            angleInDegrees += transform.eulerAngles.y;
        }
        return new Vector3(Mathf.Sin(angleInDegrees * Mathf.Deg2Rad), 0,
            Mathf.Cos(angleInDegrees * Mathf.Deg2Rad));
    }

    public struct ViewCastInfo
    {
        public bool hit;
        public Vector3 point;
        public float dst;
        public float angle;
        public ViewCastInfo(bool _hit, Vector3 _point, float _dst,float _angle)
        {
            hit = _hit;
            point = _point;
            dst = _dst;
            angle = _angle;
        }
    }

    public struct EdgeInfo
    {
        public Vector3 pointA;
        public Vector3 pointB;
        public EdgeInfo(Vector3 _pointA, Vector3 _pointB)
        {
            pointA = _pointA;
            pointB = _pointB;
        }
    }
}
