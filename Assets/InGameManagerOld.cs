using System.Collections;

using UnityEngine;

[RequireComponent(typeof(GeneralResources))]
public class InGameManagerOld : GUIMonoBehaviour
{
    public static Vector2 ScreenRatio = new Vector2(9f, 16f);

    [Header("Levels")]
    [SerializeField] LevelOld[] levels;
    [SerializeField] float endOfLevelDelay = 2f;

    public class LevelConfig
    {
        public TextPack ExpositionPack { get; set; }
        public SpriteAssetPack SpritePack { get; set; }
        public LayerPack BackgroundLayerPack { get; set; }
        public LayerPack GameplayLayerPack { get; set; }
        public LayerPack ForegroundLayerPack { get; set; }
        public WaveConfigPack WaveConfigPack { get; set; }
        public AudioClip AudioClip { get; set; }
    }

    private GeneralResources generalResources;
    private ScoreUIManager scoreUIManager;
    private LivesUIManager livesUIManager;
    private CountdownUIManager countdownUIManager;
    private ExpositionUIManager expositionUIManager;
    private LevelCompleteUIManager levelCompleteUIManager;
    private CockpitUIManager cockpitManager;

    private DynamicPlayerController playerController;
    private AudioSource audioSource;
    private BackdropManager backdropManager;
    //private CanvasSurfaceManager canvasSurfaceManager;
    private LayersManager backgroundLayersManager;
    private LayersManager gameplayLayersManager;
    private LayersManager foregroundLayersManager;
    private EnemyWaveManager enemyWaveManager;
    private LevelOld level;
    private LevelConfig levelConfig;
    private int levelIndex;
    private bool paused;

    void Awake()
    {
        ResolveComponents();
        ConfigGUIStyle();

        GUIStyle.fontSize = 14;
        Cursor.visible = false;
        levelIndex = 0;
        paused = false;
    }

    private void ResolveComponents()
    {
        generalResources = GetComponent<GeneralResources>();
        scoreUIManager = generalResources.ScoreManager;
        livesUIManager = generalResources.LivesManager;
        countdownUIManager = generalResources.CountdownManager;
        expositionUIManager = generalResources.ExpositionManager;
        levelCompleteUIManager = generalResources.LevelCompleteManager;
        cockpitManager = generalResources.CockpitManager;

        playerController = FindObjectOfType<DynamicPlayerController>();
        audioSource = Camera.main.gameObject.GetComponent<AudioSource>();
        backdropManager = FindObjectOfType<BackdropManager>();
        //canvasSurfaceManager = FindObjectOfType<CanvasSurfaceManager>();
        
        backgroundLayersManager = generalResources.BackgroundLayers;
        gameplayLayersManager = generalResources.GameplayLayers;
        foregroundLayersManager = generalResources.ForegroundLayers;
        enemyWaveManager = FindObjectOfType<EnemyWaveManager>();
    }

    // Start is called before the first frame update
    void Start()
    {
        PrepareInitLevel();
        RegisterDelegates();
        ScheduleLevel();
    }

    private void PrepareInitLevel()
    {
        level = ResolveLevel(levelIndex);
        levelConfig = ResolveLevelConfiguration(level);
    }

    private void RegisterDelegates()
    {
        playerController.RegisterDelegates(new DynamicPlayerController.Delegates
        {
            OnShipEngagedDelegate = OnShipEngaged,
            OnShipDisengagedDelegate = OnShipDisengaged
        });

        //backdropManager.RegisterDelegate(OnBackdropDeactivated);
        enemyWaveManager?.RegisterDelegate(OnScoreAchieved);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Pause))
        {
            Time.timeScale = paused ? 1 : 0;
            paused = !paused;
        }

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Application.Quit();
        }
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

    private LevelConfig ResolveLevelConfiguration(LevelOld level)
    {
        return new LevelConfig
        {
            SpritePack = level.SpritePack,
            BackgroundLayerPack = level.BackgroundLayerPack,
            GameplayLayerPack = level.GameplayLayerPack,
            ForegroundLayerPack = level.ForegroundLayerPack,
            WaveConfigPack = level.WaveConfigPack,
            AudioClip = level.AudioClip,
            ExpositionPack = level.ExpositionPack
        };
    }

    private void ScheduleLevel()
    {
        ResetScore();
        ApplyBackground(levelConfig.SpritePack);
        ApplyBackgroundLayers(levelConfig.BackgroundLayerPack);
        ApplyGameplayLayers(levelConfig.GameplayLayerPack);
        ApplyForegroundLayers(levelConfig.ForegroundLayerPack);
        ApplyAudioSource(levelConfig.AudioClip);
        EngageShip();
    }

    private void ApplyBackground(SpriteAssetPack spriteAssetPack)
    {
        if (backdropManager.IsActive())
        {
            backdropManager.Deactivate();
        }
        else
        {
            backdropManager.Activate(spriteAssetPack);
        }

        //canvasSurfaceManager.Actuate(new CanvasSurfaceManager.Configuration
        //{
        //    SpriteAssetPack = spriteAssetPack
        //});
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

    private void ResumeAudio() => audioSource.UnPause();

    private void SupplementScoreScore(int score) => scoreUIManager.SupplementScore(score);

    private void ResetScore() => scoreUIManager.ResetScore();

    private void RevokeLife() => livesUIManager.RemoveLife();

    private bool HasLives() => livesUIManager.HasLives();

    private void ResetLives() => livesUIManager.ResetLives();

    public void OnScoreAchieved(int score)
    {
        playerController.DisableControls();
        playerController.DisengageShip();
    }

    public void OnCountdownComplete()
    {
        EnablePlayerControls();
        
        if (levelConfig.WaveConfigPack != null)
        {
            ApplyWaveConfiguration(levelConfig.WaveConfigPack);
        }

        expositionUIManager.RegisterDelegate(OnExpositionComplete);
        expositionUIManager.Actuate(new ExpositionUIManager.Configuration
        {
            TextPack = levelConfig.ExpositionPack
        });
    }

    public void OnExpositionComplete() { }

    public void OnShipEngaged() => StartCountdown();

    public void OnShipDisengaged()
    {
        playerController.Reset();
        audioSource.Stop();
        levelCompleteUIManager.Actuate();
        enemyWaveManager.SuspendFutureWaves();

        StartCoroutine(Co_PrepareNextLevel());
    }

    //public void OnBackdropDeactivated()
    //{
    //    SpriteAssetPack spriteAssetPack = levelConfig.BackdropAssetPack;
    //    backdropManager.Activate(spriteAssetPack);
    //}

    private bool HasNextLevel() => levelIndex + 1 < levels.Length;

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

    private IEnumerator Co_PrepareNextLevel()
    {
        yield return new WaitForSeconds(endOfLevelDelay);

        level = ResolveNextLevel();
        levelConfig = ResolveLevelConfiguration(level);
        ScheduleLevel();
    }

    public override void SetGUIAttributes(GUIStyle guiStyle)
    {
        // GUI.Label(new Rect(20, 75, 200, 40), $"Last Position: {backgroundCanvasManager.GetLastPosition()}", guiStyle);
    }
}