using UnityEngine;

public class GoalZone : MonoBehaviour
{
    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.GetComponentInParent<CatController>() != null)
            GameManager.Instance.Win();
    }
}
