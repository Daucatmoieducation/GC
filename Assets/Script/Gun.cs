using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class Gun : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Camera mainCamera;
    [SerializeField] private Transform firePoint;
    [SerializeField] private GameObject bulletPrefab;
    [SerializeField] private AimVisualizer aimVisualizer;

    [Header("Gun Settings")]
    [SerializeField] private float shotDelay = 0.3f;
    [SerializeField] private int maxAmmo = 5;
    [SerializeField] private int bulletCount = 5;
    [SerializeField] private float spreadAngle = 50f;


    private int currentAmmo;
    private float nextShotTime;
    private SpriteRenderer spriteRenderer;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();

        mainCamera ??= Camera.main;
        if (mainCamera == null)
            Debug.LogError("Main Camera not found.");

        if (firePoint == null)
            Debug.LogError("Fire Point not assigned.");
        if (bulletPrefab == null)
            Debug.LogError("Bullet Prefab not assigned.");
        if (aimVisualizer == null)
            aimVisualizer = FindObjectOfType<AimVisualizer>();
    }

    private void Start()
    {
        currentAmmo = maxAmmo;
    }

    private void Update()
    {
        RotateGunTowardMouse();

        if (Input.GetMouseButtonDown(1))
        {
            if (currentAmmo <= 0)
            {
                Debug.Log("Out of ammo!");
                return;
            }

            // ❌ Nếu cung không hiển thị thì không bắn
            if (aimVisualizer != null && !aimVisualizer.IsVisible())
            {
                Debug.Log("Can't shoot: aim not visible.");
                return;
            }

            if (Time.time >= nextShotTime)
                Fire();
        }

    }

    private void Fire()
    {
        nextShotTime = Time.time + shotDelay;
        currentAmmo--;

        float coneAngle = aimVisualizer ? aimVisualizer.GetCurrentConeAngle() : spreadAngle;

        // Hướng chuẩn giống với AimVisualizer
        Vector3 mouseWorldPos = mainCamera.ScreenToWorldPoint(Input.mousePosition);
        mouseWorldPos.z = 0f;
        Vector2 baseDir = (mouseWorldPos - firePoint.position).normalized;

        for (int i = 0; i < bulletCount; i++)
        {
            float angleOffset = Random.Range(-coneAngle / 2f, coneAngle / 2f);
            Quaternion offsetRotation = Quaternion.Euler(0, 0, angleOffset);
            Vector2 shootDir = offsetRotation * baseDir;

            float angle = Mathf.Atan2(shootDir.y, shootDir.x) * Mathf.Rad2Deg;
            Quaternion rotation = Quaternion.Euler(0f, 0f, angle);

            GameObject bullet = Instantiate(bulletPrefab, firePoint.position, rotation);

            // 🔥 Random chiều dài
            float lengthScale = Random.Range(0.5f, 1.0f);
            bullet.transform.localScale = new Vector3(lengthScale, 1f, 1f);
        }

    }

    private void RotateGunTowardMouse()
    {
        if (mainCamera == null) return;

        Vector3 mouseWorldPos = mainCamera.ScreenToWorldPoint(Input.mousePosition);
        mouseWorldPos.z = 0f;

        Vector2 direction = mouseWorldPos - transform.position;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

        transform.rotation = Quaternion.Euler(0f, 0f, angle);
        transform.localScale = (angle < -90 || angle > 90) ? new Vector3(1, -1, 1) : new Vector3(1, 1, 1);
        spriteRenderer.sortingOrder = mouseWorldPos.y > transform.position.y ? -1 : 1;
    }

    public void Reload() => currentAmmo = maxAmmo;

    public int GetCurrentAmmo() => currentAmmo;
}
