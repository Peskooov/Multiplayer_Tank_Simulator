using UnityEngine;

public enum ProjectileHitType
{
    Penetration,
    NonPenetration, 
    Ricochet,
    ModulePenetration,
    ModuleNoPenetration,
    Environment
}

public class ProjectileHitResult
{
    public ProjectileHitType Type;
    public float Damage;
    public Vector3 Point;
}

[RequireComponent(typeof(Projectile))]
public class ProjectileHit : MonoBehaviour
{
    private const float RAY_ADVANCE = 1.1f;

    private Projectile projectile;
    
    private RaycastHit raycastHit;
    private Armor hitArmor;
    
    private bool isHit;
    
    public RaycastHit RaycastHit => raycastHit;
    public Armor HitArmor => hitArmor;
    public bool IsHit => isHit;

    private void Awake()
    {
        projectile = GetComponent<Projectile>();
    }
    
    public void Check()
    {
        if(isHit) return;
 
        if (Physics.Raycast(transform.position, transform.forward, out raycastHit,  projectile.Properties.Velocity * Time.deltaTime * RAY_ADVANCE))
        {
            Armor armor = raycastHit.collider.GetComponent<Armor>();

            if (armor != null)
            {
                hitArmor = armor;
            }
            
            isHit = true;
        }
    }

    public ProjectileHitResult GetHitResult()
    {
        ProjectileHitResult hitResult = new ProjectileHitResult();
        hitResult.Damage = 0;

        if (hitArmor == null)
        {
            hitResult.Type = ProjectileHitType.Environment;
            hitResult.Point = raycastHit.point;
            return hitResult;
        }

        float normalization = projectile.Properties.NormalizationAngle;

        if (projectile.Properties.Caliber > hitArmor.Thickness * 2)
        {
            normalization = (projectile.Properties.NormalizationAngle * 1.4f * projectile.Properties.Caliber) / hitArmor.Thickness;
        }

        float angle = Mathf.Abs(Vector3.SignedAngle(-projectile.transform.forward, raycastHit.normal, projectile.transform.right)) - normalization;
        float reducedArmor = hitArmor.Thickness / Mathf.Cos(angle * Mathf.Deg2Rad);
        float projectilePenetration = projectile.Properties.GetSpreadArmorPenetration();
        
        Debug.DrawRay(raycastHit.point, -projectile.transform.forward, Color.red);
        Debug.DrawRay(raycastHit.point, raycastHit.normal, Color.green);
        Debug.DrawRay(raycastHit.point, projectile.transform.right, Color.yellow);
        
        if (angle > projectile.Properties.RicochetAngle && projectile.Properties.Caliber < hitArmor.Thickness * 3 && hitArmor.Type == ArmorType.Vehicle)
        {
            hitResult.Type = ProjectileHitType.Ricochet;
        }
        else if (projectilePenetration >= reducedArmor)
        {
            hitResult.Type = ProjectileHitType.Penetration;
            
        }
        else if (projectilePenetration < reducedArmor)
        {
            hitResult.Type = ProjectileHitType.NonPenetration;
        }

        //Debug.LogError($"armor: {hitArmor.Thickness}, reducedArmor: {reducedArmor}, angle: {angle}, norm: {normalization}, penetration: {projectilePenetration} , type:{hitResult.Type}");
        
        if(hitResult.Type == ProjectileHitType.Penetration)
            hitResult.Damage = projectile.Properties.GetSpreadDamage();
        else
            hitResult.Damage = 0;

        if (hitArmor.Type == ArmorType.Module)
        {
            if (hitResult.Type == ProjectileHitType.Penetration)
                hitResult.Type = ProjectileHitType.ModulePenetration;
            
            if (hitResult.Type == ProjectileHitType.NonPenetration)
                hitResult.Type = ProjectileHitType.ModuleNoPenetration;
        }
        
        hitResult.Point = raycastHit.point;
        
        return hitResult;
    }
}
