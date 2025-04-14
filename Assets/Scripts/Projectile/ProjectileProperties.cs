using UnityEngine;
using Random = UnityEngine.Random;

public enum ProjectileType
{
    ArmorPiercing,
    HighExplosive,
    Subcaliber
}

[CreateAssetMenu]
public class ProjectileProperties : ScriptableObject
{
    [SerializeField] private ProjectileType type;

    [Header("Common")]
    [SerializeField] private Projectile projectilePrefab;
    [SerializeField] private Sprite icon;

    [Header("Movement")]
    [SerializeField] private float velocity;
    [SerializeField] private float mass;
    [SerializeField] private float impactForce;

    [Header("Damage")]
    [SerializeField] private float damage;
    [Range(0.0f, 1.0f)]
    [SerializeField] private float damageSpread;
    
    [Header("Caliber")]
    [SerializeField] private float caliber;

    [Header("Armor Penetration")]
    [SerializeField] private float armorPenetration;
    [Range(0.0f, 1.0f)]
    [SerializeField] private float armorPenetrationSpread;
    [Range(0.0f, 90.0f)]
    [SerializeField] private float normalizationAngle;
    [Range(0.0f, 90.0f)]
    [SerializeField] private float ricochetAngle;
    
    public ProjectileType Type => type;
    public Projectile ProjectilePrefab => projectilePrefab;
    public Sprite Icon => icon;
    
    public float Velocity => velocity;
    public float Mass => mass;
    public float ImpactForce => impactForce;
    
    public float Damage => damage;
    public float DamageSpread => damageSpread;
    
    public float Caliber => caliber;

    public float ArmorPenetration => armorPenetration;
    public float ArmorPenetrationSpread => armorPenetrationSpread;
    public float NormalizationAngle => normalizationAngle;
    public float RicochetAngle => ricochetAngle;
    
    public float GetSpreadDamage()
    {
        return damage * Random.Range(1 - damageSpread, 1 + damageSpread);
    }

    public float GetSpreadArmorPenetration()
    {
        return ArmorPenetration * Random.Range(1 - ArmorPenetrationSpread, 1 + ArmorPenetrationSpread);
    }
}
