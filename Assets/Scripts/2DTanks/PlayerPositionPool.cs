using UnityEngine;
using System.Collections.Generic;

public class PlayerPositionPool : MonoBehaviour
{
    public static PlayerPositionPool Instance;

    [SerializeField] private List<Transform> allPos;

    private List<Transform> avalibalePos;
    
    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        
        avalibalePos = new List<Transform>(allPos);
    }
    
    public Transform TakeRandomPosition()
    {
        int index = Random.Range(0, avalibalePos.Count);
        Transform pos = avalibalePos[index];

        avalibalePos.RemoveAt(index);

        return pos;
    }

    public void PutPosition(Transform pos)
    {
        if (allPos.Contains(pos))
        {
            if (!avalibalePos.Contains(pos))
            {
                avalibalePos.Add(pos);
            }
        }
    } 
}
