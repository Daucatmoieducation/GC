using UnityEngine;

public class Bullet : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private float moveSpeed = 15f;
    [SerializeField] private float lifeTime = 0.001f;

    private void Start()
    {
        Destroy(gameObject, lifeTime);
    }
    private void Update()
    {
        Move();
    }
    private void Move()
    {
        transform.Translate(transform.right * moveSpeed * Time.deltaTime, Space.World);
    }
}
