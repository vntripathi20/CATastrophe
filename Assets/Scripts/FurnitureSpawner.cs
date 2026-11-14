using UnityEngine;

// Furniture rains down from above. Catch it before it hits the lava and use it to build a bridge.
public class FurnitureSpawner : MonoBehaviour
{
    public float firstDelay = 0.3f;
    public float interval = 3.5f;
    public float spawnY = 6.5f;
    public float minX = -4f;
    public float maxX = 4f;
    public float fallGravity = 0.4f;

    struct Piece
    {
        public string name; public Vector2 size; public Color color;
        public Piece(string n, float w, float h, Color c) { name = n; size = new Vector2(w, h); color = c; }
    }

    static readonly Piece[] Pieces =
    {
        new Piece("Bookshelf", 6f,   0.8f, new Color(0.55f, 0.35f, 0.2f)),
        new Piece("Couch",     5f,   1.0f, new Color(0.35f, 0.5f, 0.75f)),
        new Piece("Table",     4f,   0.7f, new Color(0.7f, 0.55f, 0.3f)),
        new Piece("Crate",     1.2f, 1.2f, new Color(0.6f, 0.45f, 0.25f)),
    };

    float timer;

    void Start() { timer = firstDelay; }

    void Update()
    {
        if (GameManager.Instance != null && GameManager.Instance.State != GameManager.GameState.Playing) return;
        timer -= Time.deltaTime;
        if (timer <= 0f) { Spawn(); timer = interval; }
    }

    void Spawn()
    {
        var p = Pieces[Random.Range(0, Pieces.Length)];
        var go = new GameObject(p.name);
        go.transform.position = new Vector3(Random.Range(minX, maxX), spawnY, 0f);
        PlaceholderArt.MakeBox(go, p.size, p.color, 2);

        var col = go.AddComponent<BoxCollider2D>();
        col.size = Vector2.one; // scaled by transform
        var rb = go.AddComponent<Rigidbody2D>();
        rb.mass = 5f;
        rb.gravityScale = fallGravity;
        rb.constraints = RigidbodyConstraints2D.FreezeRotation;
        go.AddComponent<Draggable>();
    }
}
