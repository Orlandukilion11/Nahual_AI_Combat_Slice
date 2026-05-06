using UnityEngine;

public class Breakable : MonoBehaviour
{
    [Header("Settings")]
    public float breakForce = 5f;       // Velocity needed to break
    public GameObject fracturedPrefab;  // (Optional) The broken pieces model
    public GameObject brokenEffect;     // (Optional) Particle effect (dust/sparks)

    [Header("Explosion Tuning")]
    public float explosionPower = 2f;   // How hard pieces fly apart
    public float debrisLifetime = 5f;   // How long pieces stay

    void OnCollisionEnter(Collision collision)
    {
        // CHANGE 1: We removed the "Player" tag check.
        // Now, we only care about PHYSICS.
        // If a Rock, a Player, or a Meteor hits it hard enough... it breaks.
        if (collision.relativeVelocity.magnitude >= breakForce)
        {
            Shatter();
        }
    }

    void Shatter()
    {
        // 1. Spawn Particle Effect (Dust/Sparks)
        if (brokenEffect != null)
        {
            Instantiate(brokenEffect, transform.position, transform.rotation);
        }

        // 2. Spawn Broken Pieces (The "Fractured" Model)
        if (fracturedPrefab != null)
        {
            GameObject debris = Instantiate(fracturedPrefab, transform.position, transform.rotation);

            // Push the pieces outward so it looks like an explosion
            foreach (Rigidbody rb in debris.GetComponentsInChildren<Rigidbody>())
            {
                Vector3 force = (rb.transform.position - transform.position).normalized * explosionPower;
                rb.AddForce(force, ForceMode.Impulse);
            }

            Destroy(debris, debrisLifetime); // Clean up mess later
        }
        else
        {
            // Fallback: If you don't have a broken model yet, just log it
            Debug.Log("Smash! (No fractured prefab assigned, object simply vanished)");
        }

        // 3. Destroy the main Crate
        Destroy(gameObject);
    }
}