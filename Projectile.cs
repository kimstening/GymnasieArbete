using UnityEngine;

public class Projectile : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private float moveSpeed = 20f;
    [SerializeField] private float lifetime = 5f; // Time before explosion if no hit
    
    [Header("Explosion")]
    [SerializeField] private GameObject explosionPrefab;
    [SerializeField] private float explosionRadius = 5f;
    [SerializeField] private float explosionDamage = 25f;
    [SerializeField] private LayerMask damageLayer;
    
    private Vector3 moveDirection;
    private float timeAlive = 0f;
    private bool hasExploded = false;
    private Rigidbody rb;
    private TrailRenderer trailRenderer;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        trailRenderer = GetComponent<TrailRenderer>();
        
        // Apply initial velocity
        if (rb != null)
        {
            rb.linearVelocity = moveDirection * moveSpeed;
        }
    }

    void Update()
    {
        timeAlive += Time.deltaTime;
        
        // Explode if lifetime exceeded
        if (timeAlive >= lifetime && !hasExploded)
        {
            Explode();
        }
    }

    void OnTriggerEnter(Collider collision)
    {
        // Explode on impact with anything except the player
        if (!hasExploded && collision.CompareTag("Enemy"))
        {
            Explode();
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        // Explode on collision with obstacles or enemies
        if (!hasExploded && !collision.gameObject.CompareTag("Player"))
        {
            Explode();
        }
    }

    public void SetDirection(Vector3 direction)
    {
        moveDirection = direction.normalized;
        // Rotate projectile to face direction of travel
        if (moveDirection != Vector3.zero)
        {
            transform.rotation = Quaternion.LookRotation(moveDirection);
        }
    }

    private void Explode()
    {
        hasExploded = true;

        // Instantiate explosion effect
        if (explosionPrefab != null)
        {
            Instantiate(explosionPrefab, transform.position, Quaternion.identity);
        }

        // Deal damage to all enemies in radius
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, explosionRadius, damageLayer);
        foreach (Collider hitCollider in hitColliders)
        {
            // Apply damage to enemies
            IDamageable damageable = hitCollider.GetComponent<IDamageable>();
            if (damageable != null)
            {
                damageable.TakeDamage(explosionDamage);
            }
        }

        // Destroy projectile
        Destroy(gameObject);
    }

    // Debug visualization
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, explosionRadius);
    }
}

// Interface for damaging things
public interface IDamageable
{
    void TakeDamage(float damage);
}
