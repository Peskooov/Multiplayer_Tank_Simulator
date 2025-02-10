using UnityEngine;

[RequireComponent( typeof( TrackTank))]
public class TankEffect : MonoBehaviour
{
    [SerializeField] private ParticleSystem[] dust;
    [SerializeField] private Vector2 minMaxDustEmission;

    [SerializeField] private AudioSource engineAudioSource;
    
    public float minPitch = 0.8f;
    public float maxPitch = 1.5f; 
    public float minSpeed = 0f; 
    public float maxSpeed = 50f;
    
    private TrackTank tank;
    private float currentPitch;
    private void Start()
    {
        tank = GetComponent<TrackTank>();
    }

    private void Update()
    {
        float speed = tank.NormalizedLinearVelocity;

        float targetPitch = Mathf.Lerp(0.6f, 1f, speed * 2);
        currentPitch = Mathf.Lerp(currentPitch, targetPitch,  50000f);

        engineAudioSource.pitch = currentPitch;
    }
}
