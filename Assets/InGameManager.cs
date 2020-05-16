using System.Collections;

using UnityEngine;
using UnityEngine.SceneManagement;
using static BaseMonoBehaviour;

public class InGameManager : MonoBehaviour, ISetup
{
    public static float ScreenWidthInUnits = 9.0f;
    public static float ScreenHeightInUnits = 16.0f;

    //[Header("Surfaces")]
    //[SerializeField] GameObject canvas;
    //[SerializeField] GameObject background;
    //[SerializeField] GameObject gameplay;
    //[SerializeField] GameObject foreground;

    [Header("Levels")]
    [SerializeField] LevelPack[] levelPacks;
    [SerializeField] float endOfLevelDelay = 2.0f;

    public class LevelConfig
    {
        public TextPack ExpositionPack { get; set; }
        public SpriteAssetPack CanvasAssetPack { get; set; }
        public SurfacePack BackgroundPack { get; set; }
        public GameplaySurfacePack GameplayPack { get; set; }
        public SurfacePack ForegroundPack { get; set; }
        public WaveConfigPack WaveConfigPack { get; set; }
        public AudioClip AudioClip { get; set; }
    }

    private GeneralResources generalResources;
    private DynamicPlayerController playerController;
    private AudioSource audioSource;
    private CountdownUIManager countdownUIManager;
    private LevelCompleteUIManager levelCompleteUIManager;
    private LivesUIManager livesUIManager;
    private ScoreUIManager scoreUIManager;
    private CanvasSurfaceManager canvasSurfaceManager;
    private BackgroundSurfaceManager backgroundSurfaceManager;
    private GameplaySurfaceManager gameplaySurfaceManager;
    private ForegroundSurfaceManager foregroundSurfaceManager;
    private EnemyWaveManager enemyWaveManager;
    private LevelPack levelPack;
    private LevelConfig levelConfig;
    private int levelIndex;
    private bool paused;
    private RenderLayer activeLayer;

    void Awake()
    {
        ResolveComponents();

        Cursor.visible = false;
        levelIndex = 0;
        paused = false;
        activeLayer = RenderLayer.SURFACE;

        SetupHelper.SetSetupManager(this);
    }

    // Start is called before the first frame update
    void Start()
    {
        levelPack = ResolveLevel(levelIndex);
        levelConfig = ResolveLevelConfiguration(levelPack);
        ScheduleLevel();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Pause))
        {
            Time.timeScale = (paused) ? 1 : 0;
            paused = !paused;
        }

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Application.Quit();
        }
    }

    private void ResolveComponents()
    {
        generalResources = FindObjectOfType<GeneralResources>();
        playerController = FindObjectOfType<DynamicPlayerController>();
        playerController.RegisterDelegates(new DynamicPlayerController.Delegates
        {
            OnShipEngagedDelegate = OnShipEngaged,
            OnShipDisengagedDelegate = OnShipDisengaged,
            //OnShipLayerChangedDelegate = OnShipLayerChanged
            OnRequisitionLayerChangeDelegate = OnRequisitionLayerChange
        });

        audioSource = Camera.main.gameObject.GetComponent<AudioSource>() as AudioSource;
        countdownUIManager = FindObjectOfType<CountdownUIManager>() as CountdownUIManager;
        levelCompleteUIManager = FindObjectOfType<LevelCompleteUIManager>() as LevelCompleteUIManager;
        livesUIManager = FindObjectOfType<LivesUIManager>() as LivesUIManager;
        scoreUIManager = FindObjectOfType<ScoreUIManager>() as ScoreUIManager;

        canvasSurfaceManager = FindObjectOfType<CanvasSurfaceManager>() as CanvasSurfaceManager;
        backgroundSurfaceManager = FindObjectOfType<BackgroundSurfaceManager>() as BackgroundSurfaceManager;
        gameplaySurfaceManager = FindObjectOfType<GameplaySurfaceManager>() as GameplaySurfaceManager;
        foregroundSurfaceManager = FindObjectOfType<ForegroundSurfaceManager>() as ForegroundSurfaceManager;

        enemyWaveManager = FindObjectOfType<EnemyWaveManager>();
        enemyWaveManager?.RegisterDelegate(OnScoreAchieved);
    }

    private LevelPack ResolveLevel(int levelIndex)
    {
        LevelPack levelPack = null;

        if (levelIndex < levelPacks.Length)
        {
            levelPack = levelPacks[levelIndex];
        }

        return levelPack;
    }

    private bool HasNextLevel()
    {
        return (levelIndex + 1 < levelPacks.Length);
    }

    private LevelPack ResolveNextLevel()
    {
        if (HasNextLevel())
        {
            ++levelIndex;
        }
        else
        {
            levelIndex = 0;
        }

        return ResolveLevel(levelIndex);
    }

    private LevelConfig ResolveLevelConfiguration(LevelPack levelPack)
    {
        return new LevelConfig
        {
            CanvasAssetPack = levelPack.GetCanvasAssetPack(),
            BackgroundPack = levelPack.GetBackgroundPack(),
            GameplayPack = levelPack.GetGameplayPack(),
            ForegroundPack = levelPack.GetForegroundPack(),
            WaveConfigPack = levelPack.GetWaveConfigPack(),
            AudioClip = levelPack.GetAudioClip(),
            ExpositionPack = levelPack.GetExpositionPack()
        };
    }

    private void ScheduleLevel()
    {
        ResetScore();
        AssignCanvas(levelConfig.CanvasAssetPack);
        AssignBackground(levelConfig.BackgroundPack);
        AssignGameplay(levelConfig.GameplayPack);
        AssignForeground(levelConfig.ForegroundPack);
        AssignAudioSource(levelConfig.AudioClip);
        EngageShip();
    }

    private void AssignCanvas(SpriteAssetPack spriteAssetPack)
    {
        if (spriteAssetPack != null)
        {
            canvasSurfaceManager.Actuate(new CanvasSurfaceManager.Configuration
            {
                SpriteAssetPack = spriteAssetPack
            });
        }
    }

    private void AssignBackground(SurfacePack surfacePack)
    {
        if (surfacePack != null)
        {
            backgroundSurfaceManager.Actuate(new BackgroundSurfaceManager.Configuration
            {
                SurfacePack = surfacePack
            });
        }
    }

    private void AssignGameplay(GameplaySurfacePack surfacePack)
    {
        if (surfacePack != null)
        {
            gameplaySurfaceManager.Actuate(new GameplaySurfaceManager.Configuration
            {
                SurfacePack = surfacePack
            });
        }
    }

    private void AssignForeground(SurfacePack surfacePack)
    {
        if (surfacePack != null)
        {
            foregroundSurfaceManager.Actuate(new ForegroundSurfaceManager.Configuration
            {
                SurfacePack = surfacePack
            });
        }
    }

    private void AssignAudioSource(AudioClip audioClip)
    {
        audioSource.clip = audioClip;
        audioSource.loop = true;

        PlayAudio();
    }

    public void OnShipEngaged()
    {
        StartCountdown();
    }

    private void EngageShip() => playerController.EngageShip();

    private void StartCountdown()
    {
        countdownUIManager.RegisterDelegate(OnCountdownComplete);
        countdownUIManager.Actuate();
    }

    public void OnCountdownComplete()
    {
        EnablePlayerControls();
        ApplyWaveConfiguration(levelConfig.WaveConfigPack);

        var expositionUIManager = generalResources.GetExpositionPanel().GetComponent<ExpositionUIManager>() as ExpositionUIManager;
        expositionUIManager.RegisterDelegate(OnExpositionComplete);
        expositionUIManager.Actuate(new ExpositionUIManager.Configuration
        {
            TextPack = levelConfig.ExpositionPack
        });
    }

    private void EnablePlayerControls() => playerController.EnableControls();

    private void ApplyWaveConfiguration(WaveConfigPack waveConfigPack)
    {
        enemyWaveManager?.AssignWaveConfigPack(waveConfigPack);
        enemyWaveManager?.SpawnAllWaves();
    }

    private void PlayAudio() => audioSource.Play();

    private void PauseAudio() => audioSource.Pause();

    private void UnpauseAudio() => audioSource.UnPause();

    private void SupplementScoreScore(int score) => scoreUIManager.SupplementScore(score);

    private void ResetScore() => scoreUIManager.ResetScore();

    private void RevokeLife() => livesUIManager.RemoveLife();

    private bool HasLives()
    {
        return livesUIManager.HasLives();
    }

    private void ResetLives() => livesUIManager.ResetLives();

    public void OnExpositionComplete() { }

    public void OnShipDisengaged()
    {
        playerController.Reset();
        audioSource.Stop();
        levelCompleteUIManager.Actuate();
        enemyWaveManager.SuspendFutureWaves();

        StartCoroutine(PrepareNextLevel());
    }

    public RenderLayer GetActiveLayer()
    {
        return activeLayer;
    }

    public void OnRequisitionLayerChange(RenderLayer layer)
    {
        StartCoroutine(TransitionGameplayObjectsCoroutine(layer));
    }

    private IEnumerator TransitionGameplayObjectsCoroutine(RenderLayer layer)
    {
        GameObject player = playerController.gameObject;
        int activeLayer = player.layer;

        float duration = 0.25f;
        float startTime = Time.time;

        bool complete = false;

        while (!complete)
        {
            float fractionComplete = (Time.time - startTime) / duration;

            if (fractionComplete >= 0.0f)
            {
                gameplaySurfaceManager.ScaleObjects(activeLayer, layer, fractionComplete);
                complete = (fractionComplete >= 1.0f);
            }

            if (complete)
            {
                OnTransitionGameplayLayerComplete(layer);
            }

            yield return null;
        }
    }
    public void OnTransitionGameplayLayerComplete(RenderLayer layer)
    {
        activeLayer = layer;
        NotifyOnLayerChange(layer);
    }

    //public void OnShipLayerChanged(RenderLayer layer)
    //{
    //    activeLayer = layer;
    //    NotifyOnLayerChange(layer);
    //}

    private void NotifyOnLayerChange(RenderLayer layer)
    {
        GameObject[] rootObjects = SceneManager.GetActiveScene().GetRootGameObjects();

        foreach (GameObject gameObject in rootObjects)
        {
            if (gameObject.GetComponentsInChildren<INotify>() is INotify[] iNotifys)
            {
                foreach (INotify iNotify in iNotifys)
                {
                    iNotify.OnLayerChange((int) layer);
                }
            }
        }
    }

    private IEnumerator PrepareNextLevel()
    {
        yield return new WaitForSeconds(endOfLevelDelay);

        levelPack = ResolveNextLevel();
        levelConfig = ResolveLevelConfiguration(levelPack);
        ScheduleLevel();
    }

    public void OnScoreAchieved(int score)
    {
        playerController.DisableControls();
        playerController.DisengageShip();
    }
}