using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [Header("Global checkpoint (only one, in Level 1)")]
    public int checkpointSceneIndex = 0;
    public Vector3 checkpointPosition;
    public bool checkpointSet = false;

    [Header("Persist Player HP across levels")]
    public int playerMaxHP = 3;
    public int playerHP = 3;
    public bool playerHPSet = false;

    [Header("Last Level (optional)")]
    public int lastLevelBuildIndex = -1;

    private readonly HashSet<string> collectedCoinIds = new HashSet<string>();
    private int coinsRemaining = 0;

    private LevelExitFlag currentFlag;
    private GameClearTrigger currentClearTrigger;

    private bool pendingRespawn = false;

    private void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
        DontDestroyOnLoad(gameObject);

        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDestroy()
    {
        if (Instance == this)
            SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        currentFlag = Object.FindFirstObjectByType<LevelExitFlag>(FindObjectsInactive.Include);
        currentClearTrigger = Object.FindFirstObjectByType<GameClearTrigger>(FindObjectsInactive.Include);

        coinsRemaining = 0;
        var coins = Object.FindObjectsByType<CoinPickup>(FindObjectsInactive.Include, FindObjectsSortMode.None);
        foreach (var coin in coins)
        {
            if (IsCoinCollected(coin.CoinId))
            {
                Destroy(coin.gameObject);
            }
            else
            {
                coinsRemaining++;
            }
        }

        SetGoalVisibility(coinsRemaining == 0);

        if (pendingRespawn && checkpointSet && scene.buildIndex == checkpointSceneIndex)
        {
            var player = GameObject.FindGameObjectWithTag("Player");
            if (player != null)
            {
                player.transform.position = checkpointPosition;

                var rb = player.GetComponent<Rigidbody2D>();
                if (rb != null) rb.linearVelocity = Vector2.zero;
            }
            pendingRespawn = false;
        }
    }

    private bool IsLastLevel(int buildIndex)
    {
        int last = (lastLevelBuildIndex >= 0)
            ? lastLevelBuildIndex
            : SceneManager.sceneCountInBuildSettings - 1;

        return buildIndex == last;
    }

    private void SetGoalVisibility(bool coinsCleared)
    {
        bool lastLevel = IsLastLevel(SceneManager.GetActiveScene().buildIndex);

        if (lastLevel)
        {
            if (currentFlag != null) currentFlag.SetVisible(false);
            if (currentClearTrigger != null) currentClearTrigger.SetVisible(coinsCleared);
        }
        else
        {
            if (currentClearTrigger != null) currentClearTrigger.SetVisible(false);
            if (currentFlag != null) currentFlag.SetVisible(coinsCleared);
        }
    }

    public void RegisterMaxHP(int maxHp)
    {
        playerMaxHP = maxHp;
        if (!playerHPSet)
        {
            playerHP = maxHp;
            playerHPSet = true;
        }
    }

    public void SavePlayerHP(int hp)
    {
        playerHP = Mathf.Clamp(hp, 0, playerMaxHP);
        playerHPSet = true;
    }

    public int LoadPlayerHP(int fallback)
    {
        return playerHPSet ? playerHP : fallback;
    }

    public void ResetHPToFull()
    {
        playerHP = playerMaxHP;
        playerHPSet = true;
    }

    public void SetGlobalCheckpoint(Vector3 pos)
    {
        checkpointPosition = pos;
        checkpointSceneIndex = SceneManager.GetActiveScene().buildIndex;
        checkpointSet = true;
    }

    public bool IsCoinCollected(string coinId)
    {
        return !string.IsNullOrEmpty(coinId) && collectedCoinIds.Contains(coinId);
    }

    public void CollectCoin(string coinId)
    {
        if (string.IsNullOrEmpty(coinId)) return;

        if (!collectedCoinIds.Add(coinId)) return;

        coinsRemaining = Mathf.Max(0, coinsRemaining - 1);

        if (coinsRemaining == 0)
        {
            if (currentFlag == null)
                currentFlag = Object.FindFirstObjectByType<LevelExitFlag>(FindObjectsInactive.Include);

            if (currentClearTrigger == null)
                currentClearTrigger = Object.FindFirstObjectByType<GameClearTrigger>(FindObjectsInactive.Include);

            SetGoalVisibility(true);
        }
    }


    public void LoadNextLevel(int nextSceneBuildIndex)
    {
        SceneManager.LoadScene(nextSceneBuildIndex);
    }


    public void RespawnToGlobalCheckpoint()
    {
        if (!checkpointSet)
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
            return;
        }

        ResetHPToFull();
        pendingRespawn = true;
        SceneManager.LoadScene(checkpointSceneIndex);
    }

    public void RestartGame()
    {
        collectedCoinIds.Clear();
        ResetHPToFull();
        playerHPSet = false;
        pendingRespawn = false;

        SceneManager.LoadScene(0);
    }
}