using System;
using UnityEngine;

[System.Serializable]
public class WheelSyncPoint
{
    public Transform bone;
    public Transform mesh;
    [HideInInspector] 
    public Vector3 offset;
}

public class TrackSyncByWheel : MonoBehaviour
{
    [SerializeField] private WheelSyncPoint[] syncPoints;

    private void Start()
    {
        for (int i = 0; i < syncPoints.Length; i++)
        {
            syncPoints[i].offset = syncPoints[i].bone.position - syncPoints[i].mesh.position;
        }
    }

    private void Update()
    {
        for (int i = 0; i < syncPoints.Length; i++)
        {
            syncPoints[i].bone.position = syncPoints[i].mesh.position + syncPoints[i].offset;
        }
    }
}
