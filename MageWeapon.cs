using UnityEngine;
using UnityEngine.InputSystem;

public class MageWeapon : MonoBehaviour
{
    [Header("Attack Settings")]
    [SerializeField] private float attackCooldown = 0.5f;
    [SerializeField] private GameObject projectilePrefab;
    [SerializeField] private Transform projectileSpawnPoint; // Where projectile spawns
    [SerializeField] private Camera playerCamera;
    
    private float lastAttackTime = 0f;

    void Update()
    {
        // Check for left mouse click
        bool attackPressed = false;
        
        // Mouse input
        if (Mouse.current != null && Mouse.current.leftButton.wasPressedThisFrame)
        {
            attackPressed = true;
        }

        // Check if cooldown has passed and attack was pressed
        if (attackPressed && Time.time >= lastAttackTime + attackCooldown)
        {
            CastProjectile();
            lastAttackTime = Time.time;
        }
    }

    private void CastProjectile()
    {
        if (projectilePrefab == null)
        {
            Debug.LogError("Projectile prefab not assigned to MageWeapon!");
            return;
        }

        // Determine spawn position
        Vector3 spawnPos = projectileSpawnPoint != null ? projectileSpawnPoint.position : transform.position;

        // Get direction from camera (same as PlayerMovement uses)
        Vector3 direction = playerCamera != null ? playerCamera.transform.forward : transform.forward;

        // Instantiate projectile
        GameObject projectileObj = Instantiate(projectilePrefab, spawnPos, Quaternion.identity);
        
        // Set up the projectile
        Projectile projectile = projectileObj.GetComponent<Projectile>();
        if (projectile != null)
        {
            projectile.SetDirection(direction);
        }
        else
        {
            Debug.LogError("Projectile prefab doesn't have Projectile component!");
        }
    }
}
