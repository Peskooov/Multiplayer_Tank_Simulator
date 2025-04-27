using UnityEngine;
using Mirror;

public class Projectile : MonoBehaviour
{
    [SerializeField] private ProjectileProperties properties;
    [SerializeField] private ProjectileMovement movement;
    [SerializeField] private ProjectileHit hit;
    
    [Space(5)]
    [SerializeField] private GameObject visualModel;

    [Space(5)]
    [SerializeField] private float delayBefoteDestroy;
    [SerializeField] private float lifeTime;
    
    public NetworkIdentity Owner { get; set; }
    public ProjectileProperties Properties => properties;
    
    private void Start()
    {
        Destroy(gameObject, lifeTime);
    }

    private void Update()
    {
        hit.Check();
        movement.Move();

        if (hit.IsHit)
            OhHit();
    }

    private void OhHit()
    {
        transform.position = hit.RaycastHit.point;

        if (NetworkSessionManager.Instance.IsServer)
        {
            ProjectileHitResult hitResult = hit.GetHitResult();

            if (hitResult.Type == ProjectileHitType.Penetration || hitResult.Type == ProjectileHitType.ModulePenetration)
            {
                SvTakeDamage(hitResult);
                SvAddFrags();
            }

            Owner.GetComponent<Player>().SvInvokeProjectileHit(hitResult);
        }

        Destroy();
    }

    private void SvTakeDamage(ProjectileHitResult result)
    { 
        if (hit == null)
        {
            Debug.LogWarning("Something went wrong");
            return;
        } /*

        if (hit == null || hit.HitArmor == null || hit.HitArmor.Destructible == null)
        {
            Debug.LogWarning("Null");
            return;
        }*/
        
        hit.HitArmor.Destructible.SvApplyDamage((int) result.Damage);
    }

    private void SvAddFrags()
    {
        if (hit.HitArmor.Type == ArmorType.Module) return;

        if (hit.HitArmor.Destructible.HitPoint <= 0)
        {
            if (Owner != null)
            {
                Player player = Owner.GetComponent<Player>();

                if (player != null)
                {
                    player.Frags++;
                }
            }
        }
    }

    private void Destroy()
    {
        visualModel.SetActive(false);
        enabled = false;
        
        Destroy(gameObject, delayBefoteDestroy);
    }

    public void SetProperties(ProjectileProperties properties)
    {
        this.properties = properties;
    }
}
