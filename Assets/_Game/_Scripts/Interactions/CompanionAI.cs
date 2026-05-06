using UnityEngine;

public class CompanionAI : MonoBehaviour
{
    [Header("Settings")]
    public Transform player;       // Who to follow
    public Vector3 offset = new Vector3(1f, 1.5f, -1f); // Right shoulder position
    public float smoothSpeed = 3f; // How "laggy" (smooth) the movement is
    public float bobSpeed = 2f;    // Floating up and down speed
    public float bobHeight = 0.2f; // How high it floats

    void LateUpdate()
    {
        if (player == null) return;

        // 1. Calculate where it SHOULD be (Relative to player's rotation)
        // TransformPoint converts "Right Shoulder" logic into world coordinates
        Vector3 targetPos = player.TransformPoint(offset);

        // 2. Add the "Floating" effect (Sine Wave)
        // This makes it bob up and down so it looks alive
        targetPos.y += Mathf.Sin(Time.time * bobSpeed) * bobHeight;

        // 3. Smoothly fly to that spot (Lerp)
        transform.position = Vector3.Lerp(transform.position, targetPos, smoothSpeed * Time.deltaTime);

        // 4. Look at the same thing the player is looking at
        // (Slerp makes the rotation smooth too)
        transform.rotation = Quaternion.Slerp(transform.rotation, player.rotation, smoothSpeed * Time.deltaTime);
    }
}