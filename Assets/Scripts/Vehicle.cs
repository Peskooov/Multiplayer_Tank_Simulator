using System;
using Mirror;
using UnityEngine;
using UnityEngine.Serialization;

public class Vehicle :  Destructible
{
    [SerializeField] protected float maxLinearVelocity;
    
    [Header("Engine Sound")] 
    [SerializeField] private AudioSource engineSound;
    [SerializeField] private float enginePitchModifier = 0.5f;
    [SerializeField] private float minEngineVolume = 0.3f;
    [SerializeField] private float maxEngineVolume = 1f;
    [SerializeField] private float idlePitch = 0.8f;
    [SerializeField] private float maxPitch = 1.5f;

    [Header("Zoom")] 
    [SerializeField] protected Transform zoomOpticPosition;
    public Transform ZoomOpticPosition => zoomOpticPosition;

    public Turret Turret;
    
    [SyncVar(hook = "T")] public NetworkIdentity Owner;

    private void T(NetworkIdentity oldValue, NetworkIdentity newValue)
    {
    }
    
    public virtual float LinearVelocity => 0;
    
    public float NormalizedLinearVelocity
    {
        get
        {
            if (Mathf.Approximately(0, LinearVelocity) == true) return 0;

            return Mathf.Clamp01(LinearVelocity / maxLinearVelocity);
        }
    }

    [SyncVar] private Vector3 netAimPoint;
    
    public Vector3 NetAimPoint
    {
        get => netAimPoint;

        set
        {
            netAimPoint = value; //client local
        
            // Проверяем, имеет ли клиент права на управление объектом
            if (isOwned)
            {
                CmdSetNetAimPoint(value); //server
            }
        }
    }


    private void Start()
    {
        if (engineSound != null)
        {
            engineSound.loop = true;
            engineSound.playOnAwake = false;
            engineSound.volume = minEngineVolume;
            engineSound.pitch = idlePitch;
        }
    }

    public void Fire()
    {
        Turret.Fire();
    }

    public void SetVisible(bool visible)
    {
        if(visible)
            SetLayerToAll("Default");
        else
            SetLayerToAll("IgnoreMainCamera");
    }

    public void SetLayerToAll(string layerName)
    {
        gameObject.layer = LayerMask.NameToLayer(layerName);

        foreach (Transform t in transform.GetComponentsInChildren<Transform>())
        {
            t.gameObject.layer = LayerMask.NameToLayer(layerName);
        }
    }
    
    [Command]
    private void CmdSetNetAimPoint(Vector3 v)
    {
        netAimPoint = v;
    }
    
    protected Vector3 targetInputControl;

    public void SetTargetControl(Vector3 control)
    {
        targetInputControl = control.normalized;
    }

    protected virtual void Update()
    {
        UpdateEngineSFX();
    }

    private void UpdateEngineSFX()
    {
        if (engineSound != null)
        {
            float currentSpeed = LinearVelocity;
            float normalizedSpeed = Mathf.Clamp01(currentSpeed / maxLinearVelocity);

            if (normalizedSpeed > 0.01f)
            {
                // Если звук не воспроизводится, запускаем его
                if (!engineSound.isPlaying)
                {
                    engineSound.Play();
                }

                // Плавное изменение тона и громкости
                engineSound.pitch = Mathf.Lerp(idlePitch, maxPitch, normalizedSpeed);
                engineSound.volume = Mathf.Lerp(minEngineVolume, maxEngineVolume, normalizedSpeed);
            }
            else
            {
                // Режим холостого хода
                if (engineSound.isPlaying)
                {
                    engineSound.pitch = Mathf.Lerp(engineSound.pitch, idlePitch, Time.deltaTime);
                    engineSound.volume = Mathf.Lerp(engineSound.volume, minEngineVolume, Time.deltaTime);
                }
            }
        }
    }
}