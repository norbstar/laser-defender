using System.Collections;

using UnityEngine;

public class InGameManagerOld : GUIMonoBehaviour
{
    public static float ScreenWidthInUnits = 9.0f;
    public static float ScreenHeightInUnits = 16.0f;

    [Header("Background")]
    [SerializeField] GameObject backgroundLayers;

    [Header("Gameplay")]
    [SerializeField] GameObject gameplayLayers;

    [Header("Foreground")]
    [SerializeField] GameObject foregroundLayers;

    [Header("Levels")]
    [SerializeField] LevelOld[] levels;
    [SerializeField] float endOfLevelDelay = 2.0f;

    public class LevelConfig
    {
        public TextPack ExpositionPack { get; set; }
        public SpriteAssetPack BackdropAssetPack { get; set; }
        public LayerPack BackgroundLayerPack { get; set; }
        public LayerPack GameplayLayerPack { get; set; }
        public LayerPack ForegroundLayerPack { get; set; }
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
    //private BackdropManager backdropManager;
    private CanvasSurfaceManager canvasSurfaceManager;
    private LayersManager backgroundLayersManager;
    private LayersManager gameplayLayersManager;
    private LayersManager foregroundLayersManager;
    private EnemyWaveManager enemyWaveManager;
    private LevelOld level;
    private LevelConfig levelConfig;
    private int levelIndex;
    private bool paused;

    private Vector2 journeyPosition;

    void Awake()
    {
        ResolveComponents();
        ConfigGUIStyle();

        GetGUIStyle().fontSize = 14;
        Cursor.visible = false;
        levelIndex = 0;
        paused = false;
    }

    // Start is called before the first frame update
    void Start()
    {
        level = ResolveLevel(levelIndex);
        levelConfig = ResolveLevelConfiguration(level);
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
            OnShipDisengagedDelegate = OnShipDisengaged
        });

        audioSource = Camera.main.gameObject.GetComponent<AudioSource>() as AudioSource;
        countdownUIManager = FindObjectOfType<CountdownUIManager>() as CountdownUIManager;
        levelCompleteUIManager = FindObjectOfType<LevelCompleteUIManager>() as LevelCompleteUIManager;
        livesUIManager = FindObjectOfType<LivesUIManager>() as LivesUIManager;
        scoreUIManager = FindObjectOfType<ScoreUIManager>() as ScoreUIManager;

        //backdropManager = FindObjectOfType<BackdropManager>() as BackdropManager;
        //backdropManager.RegisterDelegate(OnBackdropDeactivated);
        canvasSurfaceManager = FindObjectOfType<CanvasSurfaceManager>() as CanvasSurfaceManager;
        
        backgroundLayersManager = backgroundLayers.GetComponent<LayersManager>() as LayersManager;
        gameplayLayersManager = gameplayLayers.GetComponent<LayersManager>() as LayersManager;
        foregroundLayersManager = foregroundLayers.GetComponent<LayersManager>() as LayersManager;

        enemyWaveManager = FindObjectOfType<EnemyWaveManager>();
        enemyWaveManager?.RegisterDelegate(OnScoreAchieved);
    }

    private LevelOld ResolveLevel(int levelIndex)
    {
        LevelOld level = null;

        if (levelIndex < levels.Length)
        {
            level = levels[levelIndex];
        }

        return level;
    }

    private bool HasNextLevel()
    {
        return (levelIndex + 1 < levels.Length);
    }

    private LevelOld ResolveNextLevel()
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

    private LevelConfig ResolveLevelConfiguration(LevelOld level)
    {
        return new LevelConfig
        {
            BackdropAssetPack = level.GetBackdropAssetPack(),
            BackgroundLayerPack = level.GetBackgroundLayerPack(),
            GameplayLayerPack = level.GetGameplayLayerPack(),
            ForegroundLayerPack = level.GetForegroundLayerPack(),
            WaveConfigPack = level.GetWaveConfigPack(),
            AudioClip = level.GetAudioClip(),
            ExpositionPack = level.GetExpositionPack()
        };
    }

    private void ScheduleLevel()
    {
        ResetScore();
        ApplyBackground(levelConfig.BackdropAssetPack);
        ApplyBackgroundLayers(levelConfig.BackgroundLayerPack);
        ApplyGameplayLayers(levelConfig.GameplayLayerPack);
        ApplyForegroundLayers(levelConfig.ForegroundLayerPack);
        ApplyAudioSource(levelConfig.AudioClip);
        EngageShip();
    }

    private void ApplyBackground(SpriteAssetPack spriteAssetPack)
    {
        //if (backdropManager.IsActive())
        //{
        //    backdropManager.Deactivate();
        //}
        //else
        //{
        //    backdropManager.Activate(spriteAssetPack);
        //}

        canvasSurfaceManager.Actuate(new CanvasSurfaceManager.Configuration
        {
            SpriteAssetPack = spriteAssetPack
        });
    }

    private void ApplyBackgroundLayers(LayerPack layerPack)
    {
        backgroundLayersManager.DestroyLayers();
        backgroundLayersManager.Initiate(layerPack);
    }

    private void ApplyGameplayLayers(LayerPack layerPack)
    {
        gameplayLayersManager.DestroyLayers();
        gameplayLayersManager.Initiate(layerPack);
    }

    private void ApplyForegroundLayers(LayerPack layerPack)
    {
        foregroundLayersManager.DestroyLayers();
        foregroundLayersManager.Initiate(layerPack);
    }

    private void ApplyAudioSource(AudioClip audioClip)
    {
        audioSource.clip = audioClip;
        audioSource.loop = true;

        PlayAudio();
    }

    //private void EngageShip() => playerController.StartCoroutine(playerController.EngageShip());
    private void EngageShip() => playerController.EngageShip();

    private void StartCountdown()
    {
        countdownUIManager.RegisterDelegate(OnCountdownComplete);
        countdownUIManager.Actuate();
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

    public void OnScoreAchieved(int score)
    {
        playerController.DisableControls();
        playerController.DisengageShip();
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

    public void OnExpositionComplete() { }

    public void OnShipEngaged()
    {
        StartCountdown();
    }

    public void OnShipDisengaged()
    {
        playerController.Reset();
        audioSource.Stop();
        levelCompleteUIManager.Actuate();
        enemyWaveManager.SuspendFutureWaves();

        StartCoroutine(PrepareNextLevel());
    }

    //public void OnBackdropDeactivated()
    //{
    //    SpriteAssetPack spriteAssetPack = levelConfig.BackdropAssetPack;
    //    backdropManager.Activate(spriteAssetPack);
    //}

    private IEnumerator PrepareNextLevel()
    {
        yield return new WaitForSeconds(endOfLevelDelay);

        level = ResolveNextLevel();
        levelConfig = ResolveLevelConfiguration(level);
        ScheduleLevel();
    }

    public override void SetGUIAttributes(GUIStyle guiStyle)
    {
        //GUI.Label(new Rect(20, 75, 200, 40), $"Last Position: {backgroundCanvasManager.GetLastPosition()}", guiStyle);
    }
}