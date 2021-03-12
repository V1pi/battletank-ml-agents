using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShellController : MonoBehaviour
{
    [SerializeField] private float shellSpeed = 30f;
    [SerializeField] private float explosionRadius = 5f;
    [SerializeField] private float maxDamage = 300f;
    [SerializeField] private float explosionForce = 120f;
    private float timer = 0;
    [SerializeField] private LayerMask tankMask;
    public HealthController healthParent;
    void Update() {
        timer += Time.fixedDeltaTime;
        if (timer >= 2f) {
            timer = 0;
            this.gameObject.SetActive(false);
        }
    }

    public void BoomBoom(HealthController parent) {
        timer = 0;
        this.gameObject.SetActive(true);
        this.healthParent = parent;
        this.gameObject.GetComponent<Rigidbody>().velocity = this.gameObject.transform.forward * shellSpeed;
    }

    private void OnTriggerEnter(Collider other) {
        if (other.tag == this.tag) {
            return;
        }
        Collider[] colliders = Physics.OverlapSphere(transform.position, explosionRadius, tankMask);
        for (int i = 0; i < colliders.Length; i++) {
            
            // ... and find their rigidbody.
            Rigidbody targetRigidbody = colliders[i].GetComponent<Rigidbody>();

            // If they don't have a rigidbody, go on to the next collider.
            if (!targetRigidbody)
                continue;

            // Add an explosion force.
            targetRigidbody.AddExplosionForce(explosionForce, transform.position, explosionRadius);

            // Find the TankHealth script associated with the rigidbody.
            HealthController hc = targetRigidbody.GetComponent<HealthController>();

            // If there is no TankHealth script attached to the gameobject, go on to the next collider.
            if (hc == null)
                continue;
            // Calculate the amount of damage the target should take based on it's distance from the shell.
            float damage = CalculateDamage(targetRigidbody.position);

            if (healthParent != null && healthParent.gameObject.GetInstanceID() != hc.gameObject.GetInstanceID()) {
                this.healthParent.InflictDamage(damage);
            }

            // Deal this damage to the tank.
            hc.TakeDamage(damage, this.healthParent.GetComponent<AgentController>());
        }

        // Destroy the shell.
        this.gameObject.SetActive(false);
    }

    private float CalculateDamage(Vector3 targetPosition) {
        // Create a vector from the shell to the target.
        Vector3 explosionToTarget = targetPosition - transform.position;

        // Calculate the distance from the shell to the target.
        float explosionDistance = explosionToTarget.magnitude;

        // Calculate the proportion of the maximum distance (the explosionRadius) the target is away.
        float relativeDistance = (explosionRadius - explosionDistance) / explosionRadius;

        // Calculate damage as this proportion of the maximum possible damage.
        float damage = relativeDistance * maxDamage;

        // Make sure that the minimum damage is always 0.
        damage = Mathf.Max(0f, damage);

        return damage;
    }

}
