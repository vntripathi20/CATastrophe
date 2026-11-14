using UnityEngine;

// Colored white-square placeholder sprite (Assets/Resources/Placeholder/white.png, made by the
// CATastrophe > Build Scene menu). Swap sprites on the SpriteRenderers once real art exists.
public static class PlaceholderArt
{
    static Sprite square;

    public static Sprite Square
    {
        get
        {
            if (square == null) square = Resources.Load<Sprite>("Placeholder/white");
            return square;
        }
    }

    public static SpriteRenderer MakeBox(GameObject go, Vector2 size, Color color, int order)
    {
        var sr = go.AddComponent<SpriteRenderer>();
        sr.sprite = Square;
        sr.color = color;
        sr.sortingOrder = order;
        go.transform.localScale = new Vector3(size.x, size.y, 1f);
        return sr;
    }
}
