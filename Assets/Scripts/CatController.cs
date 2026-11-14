using UnityEngine;

// The cat hops forward on its own every time it lands. The player never steers it.
[RequireComponent(typeof(Rigidbody2D))]
public class CatController : MonoBehaviour
{
    public float jumpForce = 9f;
    public float moveSpeed = 2.2f;
    public float startDelay = 2.5f;      // grace period so you can set up before the cat starts hopping
    public Transform groundCheck;
    public float checkRadius = 0.18f;

    Rigidbody2D rb;
    float timer;

    void Awake() { rb = GetComponent<Rigidbody2D>(); timer = startDelay; }

    void FixedUpdate()
    {
        if (GameManager.Instance != null && GameManager.Instance.State != GameManager.GameState.Playing)
            return;

        if (timer > 0f) { timer -= Time.fixedDeltaTime; return; }

        if (IsGrounded() && rb.linearVelocity.y <= 0.05f)
            rb.linearVelocity = new Vector2(moveSpeed, jumpForce);
    }

    bool IsGrounded()
    {
        var hits = Physics2D.OverlapCircleAll(groundCheck.position, checkRadius);
        foreach (var h in hits)
        {
            if (h.isTrigger) continue;
            if (h.attachedRigidbody == rb) continue;
            return true;
        }
        return false;
    }
}
