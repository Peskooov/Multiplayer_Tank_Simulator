using UnityEngine;
using UnityEngine.Serialization;

public class Vehicle :  MonoBehaviour //Destructible
{
    [SerializeField] protected float maxLinearVelocity;
    
    [Header("Engine Sound")] 
    [SerializeField] private AudioSource engineSound;
    [SerializeField] private float enginePitchModifier;

    [Header("Zoom")] 
    [SerializeField] protected Transform zoomOpticPosition;
    public Transform ZoomOpticPosition => zoomOpticPosition;
    
    
    public virtual float LinearVelocity => 0;

    public float NormalizedLinearVelocity
    {
        get
        {
            if (Mathf.Approximately(0, LinearVelocity) == true) return 0;

            return Mathf.Clamp01(LinearVelocity / maxLinearVelocity);
        }
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