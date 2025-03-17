using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Tanks2D
{
public class Projectile : MonoBehaviour
{
    [SerializeField] private float movementSpeed;
    [SerializeField] private float lifeTime;
    [SerializeField] private int damage;
    [SerializeField] private GameObject destroySFX;

    private Transform parent;

    private void Start()
    {
        Destroy(gameObject, lifeTime);
    }

    private void Update()
    {
        float stepLength = movementSpeed * Time.deltaTime;
        Vector2 step = transform.up * stepLength;

        transform.position += new Vector3(step.x, step.y, 0);

        RaycastHit2D hit = Physics2D.Raycast(transform.position, transform.up, movementSpeed * Time.deltaTime);

        if (hit)
        {
            if (hit.collider.transform.root != parent)
            {
                if (NetworkSessionManager.Instance.IsServer)
                {
                    Destructible destructible = hit.collider.transform.root.GetComponent<Destructible>();

                    if (destructible != null)
                    {
                        destructible.SvAppyDamage(damage);
                    }
                }

                if (NetworkSessionManager.Instance.IsClient)
                {
                    Instantiate(destroySFX, transform.position, Quaternion.identity);
                }

                Destroy(gameObject);
            }
        }
    }

    public void SetParent(Transform p)
    {
        parent = p;
    }
}
}
