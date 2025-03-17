using Mirror;
using UnityEngine;
using UnityEngine.Serialization;

public class Vehicle :  Destructible
{
    [SerializeField] protected float maxLinearVelocity;
    
    [Header("Engine Sound")] 
    [SerializeField] private AudioSource engineSound;
    [SerializeField] private float enginePitchModifier;

    [Header("Zoom")] 
    [SerializeField] protected Transform zoomOpticPosition;
    public Transform ZoomOpticPosition => zoomOpticPosition;

    public Turret Turret;
    
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
            CmdSetNetAimPoint(value); //server
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
            if (NormalizedLinearVelocity > 0.01f)
            {
                // Если звук не воспроизводится, запускаем его
                if (!engineSound.isPlaying)
                {
                    engineSound.Play();
                }

                // Обновляем параметры звука
                engineSound.pitch = 1f + enginePitchModifier * NormalizedLinearVelocity;
                engineSound.volume = 0.5f + NormalizedLinearVelocity;
            }
            else
            {
                // Если транспортное средство не движется, останавливаем звук
                if (engineSound.isPlaying)
                {
                    engineSound.Stop();
                }
            }
        }
    }
}