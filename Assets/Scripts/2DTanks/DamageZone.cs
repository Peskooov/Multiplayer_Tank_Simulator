using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class DamageZone : MonoBehaviour
{
    public static DamageZone Instance;
    
    [SerializeField] private int movementSpeed;
    [SerializeField] private float timeToDamage;
    [SerializeField] private float timeToMove;
[SerializeField] private float radius;
[SerializeField] private GameObject projectilePrefab;
    [SerializeField] private SpriteRenderer spriteRenderer;

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    private void Start()
    {
        StartCoroutine(Moving());
    }
    
    Vector3 GetRandomPointInsideCircle(float radius)
    {
        float randomAngle = Random.Range(0f, 2f * Mathf.PI);
        
        float randomRadius = Random.Range(0f, radius);
        
        float x = randomRadius * Mathf.Cos(randomAngle);
        float y = randomRadius * Mathf.Sin(randomAngle);

        return new Vector3(x, y,0);
    }

    IEnumerator Moving()
    {
        while (true)
        {
            spriteRenderer.DOFade(0, timeToDamage);
            Vector3 randomPoint = GetRandomPointInsideCircle(radius);
            transform.DOMove(randomPoint, timeToMove);
            yield return new WaitForSeconds(timeToMove + 0.1f);
            spriteRenderer.DOFade(0.5f, timeToDamage);
            yield return new WaitForSeconds(timeToDamage + 0.1f);
            Instantiate(projectilePrefab, transform.position, Quaternion.identity);
        }
    }
}
