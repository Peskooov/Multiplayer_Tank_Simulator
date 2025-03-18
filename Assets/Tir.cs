using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tir : Destructible
{
    protected override void OnDestructibleDestroy()
    {
        base.OnDestructibleDestroy();
        
        Destroy(gameObject);
    }
}
