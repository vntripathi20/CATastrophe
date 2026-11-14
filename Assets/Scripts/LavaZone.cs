using UnityEngine;

public class LavaZone : MonoBehaviour
{
    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.GetComponentInParent<CatController>() != null)
        {
            GameManager.Instance.Lose();
            return;
        }
        var d = other.GetComponentInParent<Draggable>();
        if (d != null) Destroy(d.gameObject, 0.6f); // furniture sinks and is gone
    }
}
