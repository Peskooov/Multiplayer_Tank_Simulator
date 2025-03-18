using System;
using UnityEngine;

[RequireComponent(typeof(TrackTank))]
public class TankTurret : Turret
{
    [SerializeField] private Transform tower;
    [SerializeField] private Transform mask;
    
    [Header("Spread")]
    [SerializeField] private float spreadAngle = 1.0f; 
    
    [SerializeField] private float horizontalRotationSpeed;
    [SerializeField] private float verticalRotationSpeed;

    [SerializeField] private float maxTopAngle;
    [SerializeField] private float maxButtonAngle;

    [Header("SFX")] 
    [SerializeField] private AudioSource fireSound;
    [SerializeField] private ParticleSystem fireParticle;
    [SerializeField] private float forceRecoil;
    
    private TrackTank tank;
    private float maskCurrentAngle;

    private Rigidbody rigidBody;
    
    private void Start()
    {
        tank = GetComponent<TrackTank>();
        rigidBody = tank.GetComponent<Rigidbody>();
    }

    protected override void Update()
    {
        base.Update();
        
        ControlTurretAim();
    }
    
    protected override void OnFire()
    {
        base.OnFire();

        GameObject projectile = Instantiate(projectilePrefab[projectileIndex].gameObject);

        Vector3 direction = ApplySpread(launchPoint.forward);
        
        projectile.transform.position = launchPoint.position;
        projectile.transform.forward = direction;
        
        FireSFX();
    }

    private Vector3 ApplySpread(Vector3 direction)
    {
        float spreadX = UnityEngine.Random.Range(-spreadAngle, spreadAngle);
        float spreadY = UnityEngine.Random.Range(-spreadAngle, spreadAngle);

        Quaternion spreadRotation = Quaternion.Euler(spreadY, spreadX, 0);
        return spreadRotation * direction;
    }
    
    private void FireSFX()
    {
        fireSound.Play();
        fireParticle.Play();

        rigidBody.AddForceAtPosition(-mask.forward * forceRecoil, mask.position, ForceMode.Impulse);
    }

    private void ControlTurretAim()
    {
        // Tower
        Vector3 localPosition = tower.InverseTransformPoint(tank.NetAimPoint);
        localPosition.y = 0;

        Vector3 localPositionGlobal = tower.TransformPoint(localPosition);

        tower.rotation = Quaternion.RotateTowards(tower.rotation, 
            Quaternion.LookRotation((localPositionGlobal - tower.position).normalized, tower.up), horizontalRotationSpeed * Time.deltaTime);
        
        // Mask
        mask.localRotation = Quaternion.identity;
        
        localPosition = mask.InverseTransformPoint(tank.NetAimPoint);
        localPosition.x = 0;
        localPositionGlobal = mask.TransformPoint(localPosition);

        float targetAngle = -Vector3.SignedAngle((localPositionGlobal - tower.position).normalized, mask.forward, mask.right);
        targetAngle = Mathf.Clamp(targetAngle, maxTopAngle, maxButtonAngle);
        
        maskCurrentAngle = Mathf.MoveTowards(maskCurrentAngle, targetAngle, Time.deltaTime * verticalRotationSpeed);
        mask.localRotation = Quaternion.Euler( maskCurrentAngle,0, 0);
    }
}
