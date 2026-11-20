using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.XR;


public class GameManager : MonoBehaviour
{
    private static GameManager instance;
    public static GameManager Instance => instance;

    private const string ComfortPrefsKey = "vr_comfort_settings";

    [Header("Estado")]
    [SerializeField] private PlayerStats stats = new PlayerStats();
    public PlayerStats Stats => stats;

    [Header("VR")]
    [SerializeField] private ComfortSettings comfort = new ComfortSettings();
    public ComfortSettings Comfort => comfort;

    [SerializeField, Min(0f)] private float fadeDuration = 0.4f;

    private ScreenFader fader;

    private bool isPaused;
    public bool IsPaused => isPaused;

    private bool pausedByPresence;

    private bool isLoading;
    public bool IsLoading => isLoading;

    // Eventos para que UI y otros sistemas reaccionen sin acoplarse
    public event Action<PlayerStats> OnStatsChanged;
    public event Action<bool> OnPauseChanged;
    public event Action<ComfortSettings> OnComfortChanged;

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;
        DontDestroyOnLoad(gameObject); // el GameObject debe ser raiz en la jerarquia

        LoadComfort();
        fader = ScreenFader.Create(transform);
    }

    private void Update()
    {
        CheckHeadsetPresence();

        if (isPaused) return;

        stats.timePlayed += Time.deltaTime;
    }

    // ---------- Stats ----------

    public void RegisterRepair()
    {
        stats.repaired++;
        NotifyStatsChanged();
    }

    public void RegisterFailure()
    {
        stats.failed++;
        NotifyStatsChanged();
    }

    public void AddMoneyGranted(int amount)
    {
        if (amount <= 0) return;
        stats.moneyGranted += amount;
        NotifyStatsChanged();
    }

    public void AddMoneyWasted(int amount)
    {
        if (amount <= 0) return;
        stats.moneyWasted += amount;
        NotifyStatsChanged();
    }

    public void ResetStats()
    {
        stats = new PlayerStats();
        NotifyStatsChanged();
    }

    private void NotifyStatsChanged() => OnStatsChanged?.Invoke(stats);

    // ---------- Pausa ----------

    public void SetPaused(bool paused) => ApplyPause(paused, false);

    public void TogglePause() => SetPaused(!isPaused);

    private void ApplyPause(bool paused, bool byPresence)
    {
        pausedByPresence = paused && byPresence;

        if (isPaused == paused) return;

        isPaused = paused;
        Time.timeScale = paused ? 0f : 1f;
        OnPauseChanged?.Invoke(paused);
    }

    // Pausa sola al quitarse el visor y reanuda al ponerselo.
    // Si el visor no soporta sensor de presencia, no hace nada.
    private void CheckHeadsetPresence()
    {
        InputDevice head = InputDevices.GetDeviceAtXRNode(XRNode.Head);
        if (!head.isValid) return;
        if (!head.TryGetFeatureValue(CommonUsages.userPresence, out bool present)) return;

        if (!present && !isPaused)
        {
            ApplyPause(true, true);
        }
        else if (present && pausedByPresence)
        {
            ApplyPause(false, false);
        }
    }

    // ---------- Escenas ----------

    // Cambio de escena con fade a negro y carga asincrona, para no marear ni congelar el visor.
    public void LoadScene(string sceneName)
    {
        if (isLoading) return;

        if (!Application.CanStreamedLevelBeLoaded(sceneName))
        {
            Debug.LogWarning($"[GameManager] La escena '{sceneName}' no esta en Build Settings.");
            return;
        }

        StartCoroutine(LoadSceneRoutine(sceneName));
    }

    private IEnumerator LoadSceneRoutine(string sceneName)
    {
        isLoading = true;
        SetPaused(false);

        yield return fader.Fade(1f, fadeDuration);

        AsyncOperation operation = SceneManager.LoadSceneAsync(sceneName);
        while (!operation.isDone)
        {
            yield return null;
        }

        yield return null; // un frame para que la escena nueva inicialice

        yield return fader.Fade(0f, fadeDuration);
        isLoading = false;
    }

    // ---------- Confort VR ----------

    public void SetSnapTurn(bool enabled, float degrees)
    {
        comfort.useSnapTurn = enabled;
        comfort.snapTurnDegrees = Mathf.Clamp(degrees, 15f, 90f);
        CommitComfort();
    }

    public void SetVignette(bool enabled)
    {
        comfort.useVignette = enabled;
        CommitComfort();
    }

    public void SetSeatedMode(bool enabled)
    {
        comfort.seatedMode = enabled;
        CommitComfort();
    }

    private void CommitComfort()
    {
        PlayerPrefs.SetString(ComfortPrefsKey, JsonUtility.ToJson(comfort));
        PlayerPrefs.Save();
        OnComfortChanged?.Invoke(comfort);
    }

    private void LoadComfort()
    {
        if (!PlayerPrefs.HasKey(ComfortPrefsKey)) return;

        JsonUtility.FromJsonOverwrite(PlayerPrefs.GetString(ComfortPrefsKey), comfort);
    }

    private void OnDestroy()
    {
        if (instance == this)
        {
            Time.timeScale = 1f;
            instance = null;
        }
    }
}
[Serializable]
public class PlayerStats
{
    public int repaired;
    public int failed;
    public float timePlayed;
    public int moneyWasted;
    public int moneyGranted;
}
