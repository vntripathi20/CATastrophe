using System.IO;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

public static class SceneBuilder
{
    const string ScenePath = "Assets/Scenes/Game.unity";
    const string SpritePath = "Assets/Resources/Placeholder/white.png";
    const string MatPath = "Assets/Materials/NoFriction.physicsMaterial2D";

    static PhysicsMaterial2D noFriction;

    [MenuItem("CATastrophe/Build Scene")]
    public static void Build()
    {
        EnsureSprite();
        EnsureMaterial();

        var scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);

        // --- Camera ---
        var camGo = new GameObject("Main Camera");
        camGo.tag = "MainCamera";
        camGo.transform.position = new Vector3(0f, 0f, -10f);
        var cam = camGo.AddComponent<Camera>();
        cam.orthographic = true;
        cam.orthographicSize = 5.5f;
        cam.clearFlags = CameraClearFlags.SolidColor;
        cam.backgroundColor = new Color(0.1f, 0.08f, 0.16f);

        // --- Room ---
        Block("RoomWall", new Vector2(0f, 0f), new Vector2(20f, 12f), new Color(0.27f, 0.23f, 0.38f), -10, false, false);
        Block("LeftWall", new Vector2(-9.75f, 0f), new Vector2(1f, 14f), new Color(0.3f, 0.2f, 0.15f), 0, true, false);
        Block("RightWall", new Vector2(9.75f, 0f), new Vector2(1f, 14f), new Color(0.3f, 0.2f, 0.15f), 0, true, false);

        var wood = new Color(0.45f, 0.3f, 0.2f);
        Block("LeftLedge", new Vector2(-6.625f, -3.25f), new Vector2(5.25f, 1.5f), wood, 0, true, false);
        Block("Island", new Vector2(0f, -3.25f), new Vector2(2f, 1.5f), wood, 0, true, false);
        Block("RightLedge", new Vector2(6.625f, -3.25f), new Vector2(5.25f, 1.5f), wood, 0, true, false);

        // --- Lava (in front of everything so sinking furniture disappears into it) ---
        var lava = Block("Lava", new Vector2(0f, -4.75f), new Vector2(22f, 2.5f), new Color(1f, 0.35f, 0.05f), 3, true, true);
        lava.AddComponent<LavaZone>();

        // --- Goal (food bowl) ---
        var goal = Block("Goal_FoodBowl", new Vector2(7.5f, -2f), new Vector2(1f, 1f), new Color(1f, 0.9f, 0.2f), 1, true, true);
        goal.AddComponent<GoalZone>();

        // --- Cat ---
        var cat = new GameObject("Cat");
        cat.transform.position = new Vector3(-8f, -2f, 0f);
        PlaceholderArt.MakeBox(cat, new Vector2(0.8f, 0.8f), new Color(1f, 0.6f, 0.15f), 5);
        var col = cat.AddComponent<BoxCollider2D>();
        col.size = Vector2.one;
        col.sharedMaterial = noFriction;
        var rb = cat.AddComponent<Rigidbody2D>();
        rb.gravityScale = 2f;
        rb.mass = 1f;
        rb.constraints = RigidbodyConstraints2D.FreezeRotation;
        rb.collisionDetectionMode = CollisionDetectionMode2D.Continuous;
        rb.interpolation = RigidbodyInterpolation2D.Interpolate;

        var gc = new GameObject("GroundCheck");
        gc.transform.SetParent(cat.transform, false);
        gc.transform.localPosition = new Vector3(0f, -0.55f, 0f);

        foreach (float x in new[] { -0.3f, 0.3f })
        {
            var ear = new GameObject("Ear");
            ear.transform.SetParent(cat.transform, false);
            ear.transform.localPosition = new Vector3(x, 0.55f, 0f);
            PlaceholderArt.MakeBox(ear, new Vector2(0.3f, 0.3f), new Color(1f, 0.6f, 0.15f), 5);
            ear.transform.localScale = new Vector3(0.3f, 0.3f, 1f);
        }

        var cc = cat.AddComponent<CatController>();
        cc.groundCheck = gc.transform;

        // --- Systems ---
        new GameObject("GameManager").AddComponent<GameManager>();
        new GameObject("DragManager").AddComponent<DragManager>();
        new GameObject("FurnitureSpawner").AddComponent<FurnitureSpawner>();

        Directory.CreateDirectory("Assets/Scenes");
        EditorSceneManager.SaveScene(scene, ScenePath);
        EditorBuildSettings.scenes = new[] { new EditorBuildSettingsScene(ScenePath, true) };
        Debug.Log("CATastrophe: scene built and saved to " + ScenePath);
    }

    static GameObject Block(string name, Vector2 pos, Vector2 size, Color color, int order, bool collider, bool trigger)
    {
        var go = new GameObject(name);
        go.transform.position = pos;
        PlaceholderArt.MakeBox(go, size, color, order);
        if (collider)
        {
            var c = go.AddComponent<BoxCollider2D>();
            c.size = Vector2.one;
            c.isTrigger = trigger;
        }
        return go;
    }

    static void EnsureSprite()
    {
        if (File.Exists(SpritePath)) return;
        Directory.CreateDirectory(Path.GetDirectoryName(SpritePath));
        var tex = new Texture2D(4, 4, TextureFormat.RGBA32, false);
        var px = new Color[16];
        for (int i = 0; i < px.Length; i++) px[i] = Color.white;
        tex.SetPixels(px);
        File.WriteAllBytes(SpritePath, tex.EncodeToPNG());
        Object.DestroyImmediate(tex);
        AssetDatabase.ImportAsset(SpritePath);
        var imp = (TextureImporter)AssetImporter.GetAtPath(SpritePath);
        imp.textureType = TextureImporterType.Sprite;
        imp.spriteImportMode = SpriteImportMode.Single;
        imp.spritePixelsPerUnit = 4f;
        imp.filterMode = FilterMode.Point;
        imp.mipmapEnabled = false;
        imp.SaveAndReimport();
    }

    static void EnsureMaterial()
    {
        noFriction = AssetDatabase.LoadAssetAtPath<PhysicsMaterial2D>(MatPath);
        if (noFriction != null) return;
        Directory.CreateDirectory("Assets/Materials");
        noFriction = new PhysicsMaterial2D("NoFriction") { friction = 0f, bounciness = 0f };
        AssetDatabase.CreateAsset(noFriction, MatPath);
        AssetDatabase.SaveAssets();
    }
}
