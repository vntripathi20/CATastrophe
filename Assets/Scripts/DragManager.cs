using UnityEngine;
#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
#endif

// Click and hold a piece of furniture to carry it. Let go and gravity takes over.
public class DragManager : MonoBehaviour
{
    Camera cam;
    Rigidbody2D held;
    Vector2 offset;
    Vector2 target;

    void Awake() { cam = Camera.main; }

    static Vector2 MouseScreen()
    {
#if ENABLE_INPUT_SYSTEM
        return Mouse.current != null ? Mouse.current.position.ReadValue() : Vector2.zero;
#else
        return Input.mousePosition;
#endif
    }

    static bool PressedThisFrame()
    {
#if ENABLE_INPUT_SYSTEM
        return Mouse.current != null && Mouse.current.leftButton.wasPressedThisFrame;
#else
        return Input.GetMouseButtonDown(0);
#endif
    }

    static bool IsHeld()
    {
#if ENABLE_INPUT_SYSTEM
        return Mouse.current != null && Mouse.current.leftButton.isPressed;
#else
        return Input.GetMouseButton(0);
#endif
    }

    Vector2 MouseWorld() => cam.ScreenToWorldPoint(MouseScreen());

    void Update()
    {
        if (GameManager.Instance != null && GameManager.Instance.State != GameManager.GameState.Playing)
        {
            Release();
            return;
        }

        if (held == null && PressedThisFrame())
        {
            var p = MouseWorld();
            foreach (var c in Physics2D.OverlapPointAll(p))
            {
                if (c.GetComponentInParent<Draggable>() == null) continue;
                held = c.attachedRigidbody;
                held.bodyType = RigidbodyType2D.Kinematic;
                held.linearVelocity = Vector2.zero;
                held.angularVelocity = 0f;
                offset = held.position - p;
                break;
            }
        }

        if (held != null)
        {
            if (!IsHeld() || held.gameObject == null) { Release(); return; }
            target = MouseWorld() + offset;
        }
    }

    void FixedUpdate()
    {
        if (held != null) held.MovePosition(target);
    }

    void Release()
    {
        if (held == null) return;
        held.bodyType = RigidbodyType2D.Dynamic;
        held.linearVelocity = Vector2.zero;
        held = null;
    }
}
