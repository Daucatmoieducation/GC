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
    [SerializeField] private int bulletCount = 5;
    [SerializeField] private int maxAmmo = 2;
    [SerializeField] private int maxReserveAmmo = 6;

    private int currentAmmo;
    private int reserveAmmo;

    private int bulletsFiredThisTurn = 0;
    private bool firedThisTurn = false;
    private bool reloadedThisTurn = false;

    [SerializeField] private BulletDisplay mainBulletDisplay;
    [SerializeField] private BulletDisplay reserveBulletDisplay;

    private float nextShotTime;
    private SpriteRenderer spriteRenderer;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        mainCamera ??= Camera.main;

        if (mainCamera == null) Debug.LogError("Main Camera not found.");
        if (firePoint == null) Debug.LogError("Fire Point not assigned.");
        if (bulletPrefab == null) Debug.LogError("Bullet Prefab not assigned.");
        if (aimVisualizer == null) aimVisualizer = FindObjectOfType<AimVisualizer>();
    }

    private void Start()
    {
        currentAmmo = maxAmmo;
        reserveAmmo = maxReserveAmmo;

        mainBulletDisplay?.SetBulletCount(currentAmmo);
        reserveBulletDisplay?.SetBulletCount(reserveAmmo);
    }

    private void Update()
    {
        RotateGunTowardMouse();

        if (Input.GetKeyDown(KeyCode.R))
        {
            ManualReload();
        }

        if (Input.GetMouseButtonDown(1))
        {
            if (currentAmmo <= 0) return;
            if (aimVisualizer != null && !aimVisualizer.IsVisible()) return;
            if (Time.time >= nextShotTime) Fire();
        }
    }

    private void Fire()
    {
        if (currentAmmo <= 0) return;

        currentAmmo--;
        bulletsFiredThisTurn++;
        firedThisTurn = true;
        reloadedThisTurn = false;

        mainBulletDisplay?.SetBulletCount(currentAmmo);
        TurnManager.Instance?.AddTurn();

        Vector3 mouseWorldPos = mainCamera.ScreenToWorldPoint(Input.mousePosition);
        mouseWorldPos.z = 0f;
        Vector2 baseDir = (mouseWorldPos - firePoint.position).normalized;
        float padding = 5f;
        float coneAngle = aimVisualizer.GetCurrentConeAngle() + padding;

        for (int i = 0; i < bulletCount; i++)
        {
            float angleOffset = Random.Range(-coneAngle / 2f, coneAngle / 2f);
            Quaternion offsetRotation = Quaternion.Euler(0, 0, angleOffset);
            Vector2 shootDir = offsetRotation * baseDir;

            float angle = Mathf.Atan2(shootDir.y, shootDir.x) * Mathf.Rad2Deg;
            Quaternion rotation = Quaternion.Euler(0f, 0f, angle);
            GameObject bullet = Instantiate(bulletPrefab, firePoint.position, rotation);
            bullet.transform.localScale = new Vector3(Random.Range(0.75f, 1.0f), 1f, 1f);
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

    public void AddReserveAmmo(int amount)
    {
        reserveAmmo = Mathf.Min(reserveAmmo + amount, maxReserveAmmo);
        reserveBulletDisplay?.SetBulletCount(reserveAmmo);
    }

    public int AutoReload()
    {
        int needed = maxAmmo - currentAmmo;
        int toLoad = Mathf.Min(needed, reserveAmmo);

        if (toLoad > 0)
        {
            currentAmmo += toLoad;
            reserveAmmo -= toLoad;

            mainBulletDisplay?.SetBulletCount(currentAmmo);
            reserveBulletDisplay?.SetBulletCount(reserveAmmo);
        }

        return toLoad;
    }

    public void ManualReload()
    {
        int needed = maxAmmo - currentAmmo;
        int toLoad = Mathf.Min(needed, reserveAmmo);

        currentAmmo += toLoad;
        reserveAmmo -= toLoad;

        mainBulletDisplay?.SetBulletCount(currentAmmo);
        reserveBulletDisplay?.SetBulletCount(reserveAmmo);

        reloadedThisTurn = false;
    }

    public void OnPlayerMove(int loadedAmount = 0)
    {
        if (reloadedThisTurn) return;

        if (loadedAmount > 0 && !firedThisTurn)
        {
            TurnManager.Instance?.AddTurn();
        }

        reloadedThisTurn = true;
        bulletsFiredThisTurn = 0;
        firedThisTurn = false;

        Debug.Log($"[AFTER RELOAD] currentAmmo={currentAmmo}, reserveAmmo={reserveAmmo}");
    }
    public int GetCurrentAmmo() => currentAmmo;
    public int GetReserveAmmo() => reserveAmmo;
    public int GetMaxAmmo() => maxAmmo;
    public int GetMaxReserveAmmo() => maxReserveAmmo;
    public void HandleReloadOnMove()
    {
        int currentAmmo = GetCurrentAmmo();
        int reserveAmmo = GetReserveAmmo();


        if (currentAmmo < maxAmmo && reserveAmmo > 0)
        {
            int loaded = AutoReload();
            OnPlayerMove(loaded);
        }
        else if (currentAmmo == maxAmmo && reserveAmmo < maxReserveAmmo)
        {
            AddReserveAmmo(1);
            OnPlayerMove(0);
        }
        else if (currentAmmo < maxAmmo && reserveAmmo == 0)
        {
            AddReserveAmmo(1);
            OnPlayerMove(0);
        }
        else
        {
            OnPlayerMove(0);
        }
    }


}
