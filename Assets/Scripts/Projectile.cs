using System;
using Unity.VisualScripting;
using UnityEngine;
using Random = UnityEngine.Random;

public class Projectile : MonoBehaviour
{
    [SerializeField] private float velocity;
    [SerializeField] private float lifeTime;
    [SerializeField] private float mass;
    
    [SerializeField] private float damage;
    [Range(0.0f, 1.0f)] 
    [SerializeField] private float damageScatter;
    
    [SerializeField] private float impactForce;
    
    [SerializeField] private GameObject visualModel;
    [SerializeField] private GameObject destroySFX;

    private Transform parent;
    
    private float RayAdvance = 1.1f;
    
    private void Start()
    {
        Destroy(gameObject, lifeTime);
    }

    private void Update()
    {
        UpdateProjectile();
    }

    private void UpdateProjectile()
    {
        transform.forward = Vector3.Lerp(transform.forward, -Vector3.up, Mathf.Clamp01(Time.deltaTime * mass)).normalized;

        Vector3 step = transform.forward * velocity * Time.deltaTime;
        
        RaycastHit hit;
        
        if (Physics.Raycast(transform.position, transform.forward, out hit, velocity * Time.deltaTime * RayAdvance))
        {
            transform.position = hit.point;
            
            var destructible = hit.transform.root.GetComponent<Destructible>();

            if (destructible != null)
            {
                if (NetworkSessionManager.Instance.IsServer)
                {
                    float dng = damage + Random.Range(-damageScatter, damageScatter) * damage;
                
                    destructible.SvApplyDamage((int)dng);
                }
            }
            
            OnProjectileLifeEnd(hit.collider, hit.point, hit.normal);
            
            return;
        }

        transform.position += step;
    }

    private void OnProjectileLifeEnd(Collider hitCollider, Vector3 hitPoint, Vector3 hitNormal)
    {
        visualModel.SetActive(false);
        enabled = false;
    }

    public void SetParent(Transform p)
    {
        parent = p;
    }
}
