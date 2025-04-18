using System;
using UnityEngine;

public class VehicleModule : Destructible
{
    [SerializeField] private string title;
    [SerializeField] private Armor armor;
    [SerializeField] private float recoveredTime;

    public float remainingRecoveryTime { get; protected set; }
    public float RecoveredTime => recoveredTime;
    
    private void Awake()
    {
        armor.SetDestructible(this);
    }

    private void Start()
    {
        Destroyed += OnModuleDestroyed;
        enabled = false;
    }

    private void OnDestroy()
    {
        Destroyed -= OnModuleDestroyed;
    }

    private void Update()
    {
        if (isServer)
        {
            remainingRecoveryTime -= Time.deltaTime;

            if (remainingRecoveryTime <= 0)
            {
                remainingRecoveryTime = 0;
                
                SvRecovery();
                
                enabled = false;
            }
        }
    }

    private void OnModuleDestroyed(Destructible destructible)
    {
        remainingRecoveryTime = recoveredTime;
        enabled = true;
    }
}
