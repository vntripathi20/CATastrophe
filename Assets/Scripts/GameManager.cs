using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public enum GameState { Playing, Won, Lost }

    public static GameManager Instance;
    public GameState State = GameState.Playing;

    void Awake() { Instance = this; State = GameState.Playing; }

    public void Lose()
    {
        if (State != GameState.Playing) return;
        State = GameState.Lost;
        Invoke(nameof(Restart), 1.2f);
    }

    public void Win()
    {
        if (State != GameState.Playing) return;
        State = GameState.Won;
    }

    public void Restart()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    void OnGUI()
    {
        var big = new GUIStyle(GUI.skin.label) { fontSize = 48, alignment = TextAnchor.MiddleCenter, fontStyle = FontStyle.Bold };
        big.normal.textColor = Color.white;
        var rect = new Rect(0, Screen.height * 0.25f, Screen.width, 80);

        if (State == GameState.Lost)
            GUI.Label(rect, "The cat is toast!", big);
        else if (State == GameState.Won)
        {
            GUI.Label(rect, "CATastrophe averted!", big);
            if (GUI.Button(new Rect(Screen.width / 2f - 100, Screen.height * 0.25f + 100, 200, 50), "Play again"))
                Restart();
        }
    }
}
