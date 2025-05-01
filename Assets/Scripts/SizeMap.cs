using UnityEngine;

public class SizeMap : MonoBehaviour
{
    [SerializeField] private Vector2 size;
    public Vector2 Size { get { return size; } }

    public Vector3 GetNormPos(Vector3 pos)
    {
        return new Vector3(pos.x / (size.x *0.5f), 0, pos.z / (size.y *0.5f));
    }
 
    void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireCube(transform.position, new Vector3(size.x, 0, size.y));
    }
}
