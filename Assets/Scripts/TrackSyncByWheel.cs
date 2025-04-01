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
            Vector3 meshPosition = syncPoints[i].mesh.position + syncPoints[i].offset;
            syncPoints[i].bone.position = new Vector3(
                syncPoints[i].bone.position.x, // сохраняем оригинальный X
                meshPosition.y,                // применяем новый Y
                syncPoints[i].bone.position.z  // сохраняем оригинальный Z
            );
        }
    }
}
