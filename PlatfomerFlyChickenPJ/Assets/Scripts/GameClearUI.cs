using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;

public class GameClearUI : MonoBehaviour
{
    public static GameClearUI Instance { get; private set; }

    [Header("UI Refs")]
    public GameObject panel;
    public Button restartButton;
    public Button exitButton;
    public Text titleText;

    private bool isShown = false;
    private bool isGameClear = false;

    private void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;


        if (restartButton != null) restartButton.onClick.AddListener(RestartGame);
        if (exitButton != null) exitButton.onClick.AddListener(ExitGame);

        if (panel != null) panel.SetActive(false);
        Time.timeScale = 1f;
    }

    private void Update()
    {
        if (Keyboard.current != null && Keyboard.current.escapeKey.wasPressedThisFrame)
        {
            if (!isShown) Show(false);
            else if (!isGameClear) Hide();
        }
    }

    public void Show(bool gameClear)
    {
        isShown = true;
        isGameClear = gameClear;

        if (titleText != null)
            titleText.text = gameClear ? "GAME CLEAR!" : "PAUSED";

        if (panel != null) panel.SetActive(true);

        Time.timeScale = 0f;
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }

    public void Hide()
    {
        isShown = false;
        isGameClear = false;

        if (panel != null) panel.SetActive(false);

        Time.timeScale = 1f;
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }

    private void RestartGame()
    {
        Hide();
        GameManager.Instance?.RestartGame();
    }

    private void ExitGame()
    {
        Hide();

        Application.Quit();
    }
}