using UnityEngine;

public class PickupSystem : MonoBehaviour
{
    [Header("Settings")]
    public Transform handPosition;  // Assign "HandPos" here
    public float throwForce = 15f;
    public float pickupRange = 2f;
    public LayerMask pickupLayer;   // What objects can we grab?

    private GameObject heldObject;
    private Rigidbody heldRb;

    void Update()
    {
        // Press 'E' to Interact
        if (Input.GetKeyDown(KeyCode.E))
        {
            if (heldObject == null)
                TryPickup();
            else
                DropObject();
        }

        // Left Click to THROW
        if (Input.GetMouseButtonDown(0) && heldObject != null)
        {
            ThrowObject();
        }
    }

    void TryPickup()
    {
        // Check for items in front of us
        Collider[] hits = Physics.OverlapSphere(handPosition.position, pickupRange, pickupLayer);

        if (hits.Length > 0)
        {
            // Grab the first thing we find
            heldObject = hits[0].gameObject;
            heldRb = heldObject.GetComponent<Rigidbody>();

            // 1. Disable physics so it doesn't fall out of our hands
            heldRb.isKinematic = true;

            // 2. Snap to hand
            heldObject.transform.SetParent(handPosition);
            heldObject.transform.localPosition = Vector3.zero;
            heldObject.transform.localRotation = Quaternion.identity;
        }
    }

    void DropObject()
    {
        // Release it
        heldRb.isKinematic = false;
        heldObject.transform.SetParent(null); // Unparent
        heldObject = null;
        heldRb = null;
    }

    void ThrowObject()
    {
        if (heldRb == null) return; // Safety check

        // 1. Save the Rigidbody reference before we drop it
        Rigidbody rbToThrow = heldRb;

        // 2. Unparent the object (Let go of it)
        DropObject();

        // 3. CALCULATE AIM DIRECTION (The Magic Part)
        // We find exactly where the Main Camera is pointing.
        Vector3 aimDirection = Camera.main.transform.forward;

        // 4. (Optional) Add a tiny bit of upward arc so it doesn't just drop instantly
        aimDirection += Vector3.up * 0.15f;

        // 5. Apply the force in the CAMERA'S direction
        rbToThrow.AddForce(aimDirection.normalized * throwForce, ForceMode.Impulse);
    }

}
