using UnityEngine;

public class UIPopup : MonoBehaviour
{
    [SerializeField] private Vector2 movementDirection;
    [SerializeField] private float movementSpeed;
    [SerializeField] private float lifetime;

    private void Start()
    {
        Destroy(gameObject, lifetime);
    }

    private void Update()
    {
        transform.Translate(movementDirection * movementSpeed * Time.deltaTime);
    }
}
