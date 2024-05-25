using System.Collections;

using UnityEngine;

[RequireComponent(typeof(GeneralResources))]
public class InGameManager : MonoBehaviour, ISetup
{
    public static Vector2 ScreenRatio = new Vector2(9f, 16f);

    [Header("Levels")]
    [SerializeField] LevelPack[] levelPacks;
    [SerializeField] float endOfLevelDelay = 2f;

    public class LevelConfig
    {
        public TextPack ExpositionPack { get; set; }
        public SpriteAssetPack SpritePack { get; set; }
        public SurfacePack BackgroundSurfacePack { get; set; }
        public SurfacePack GameplaySurfacePack { get; set; }
        public SurfacePack ForegroundSurfacePack { get; set; }
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
    private CanvasSurfaceManager canvasSurfaceManager;
    private TrackingSurfaceManager backgroundSurfaceManager;
    private TrackingSurfaceManager gameplaySurfaceManager;
    private TrackingSurfaceManager foregroundSurfaceManager;
    private EnemyWaveManager enemyWaveManager;
    private LevelPack levelPack;
    private LevelConfig levelConfig;
    private int levelIndex;
    private bool paused;

    void Awake()
    {
        ResolveComponents();

        Cursor.visible = false;
        levelIndex = 0;
        paused = false;

        SetupHelper.SetSetupManager(this);
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
        canvasSurfaceManager = FindObjectOfType<CanvasSurfaceManager>();
        backgroundSurfaceManager = FindObjectOfType<TrackingSurfaceManager>();
        gameplaySurfaceManager = FindObjectOfType<TrackingSurfaceManager>();
        foregroundSurfaceManager = FindObjectOfType<TrackingSurfaceManager>();
        enemyWaveManager = FindObjectOfType<EnemyWaveManager>();
    }

    // Start is called before the first frame update
    void Start()
    {
        PrepareInitLevelPack();
        RegisterDelegates();
        ScheduleLevel();
    }

    private void PrepareInitLevelPack()
    {
        levelPack = ResolveLevel(levelIndex);
        levelConfig = ResolveLevelConfiguration(levelPack);
    }

    private void RegisterDelegates()
    {
        playerController.RegisterDelegates(new DynamicPlayerController.Delegates
        {
            OnShipEngagedDelegate = OnShipEngaged,
            OnShipDisengagedDelegate = OnShipDisengaged
        });

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

    private LevelPack ResolveLevel(int levelIndex)
    {
        LevelPack levelPack = null;

        if (levelIndex < levelPacks.Length)
        {
            levelPack = levelPacks[levelIndex];
        }

        return levelPack;
    }

    private LevelConfig ResolveLevelConfiguration(LevelPack levelPack)
    {
        return new LevelConfig
        {
            SpritePack = levelPack.SpritePack,
            BackgroundSurfacePack = levelPack.BackgroundPack,
            GameplaySurfacePack = levelPack.GameplayPack,
            ForegroundSurfacePack = levelPack.ForegroundPack,
            WaveConfigPack = levelPack.WaveConfigPack,
            AudioClip = levelPack.AudioClip,
            ExpositionPack = levelPack.ExpositionPack
        };
    }

    private void ScheduleLevel()
    {
        ResetScore();
        AssignCanvas(levelConfig.SpritePack);
        AssignBackground(levelConfig.BackgroundSurfacePack);
        AssignGameplay(levelConfig.GameplaySurfacePack);
        AssignForeground(levelConfig.ForegroundSurfacePack);
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
            backgroundSurfaceManager.Actuate(new TrackingSurfaceManager.Configuration
            {
                SurfacePack = surfacePack
            });
        }
    }

    private void AssignGameplay(SurfacePack surfacePack)
    {
        if (surfacePack != null)
        {
            gameplaySurfaceManager.Actuate(new TrackingSurfaceManager.Configuration
            {
                SurfacePack = surfacePack
            });
        }
    }

    private void AssignForeground(SurfacePack surfacePack)
    {
        if (surfacePack != null)
        {
            foregroundSurfaceManager.Actuate(new TrackingSurfaceManager.Configuration
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

    public void OnShipEngaged() => StartCountdown();

    private void EngageShip() => playerController.EngageShip();

    private void StartCountdown()
    {
        countdownUIManager.RegisterDelegate(OnCountdownComplete);
        countdownUIManager.Actuate();
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

    private void EnablePlayerControls() => playerController.EnableControls();

    private void ApplyWaveConfiguration(WaveConfigPack waveConfigPack)
    {
        enemyWaveManager.AssignWaveConfigPack(waveConfigPack);
        enemyWaveManager.SpawnAllWaves();
    }

    private void PlayAudio() => audioSource.Play();

    private void PauseAudio() => audioSource.Pause();

    private void ResumeAudio() => audioSource.UnPause();

    private void SupplementScoreScore(int score) => scoreUIManager.SupplementScore(score);

    private void ResetScore() => scoreUIManager.ResetScore();

    private void RevokeLife() => livesUIManager.RemoveLife();

    private bool HasLives() => livesUIManager.HasLives();

    private void ResetLives() => livesUIManager.ResetLives();

    public void OnExpositionComplete() { }

    public void OnShipDisengaged()
    {
        playerController.Reset();
        audioSource.Stop();
        levelCompleteUIManager.Actuate();
        enemyWaveManager.SuspendFutureWaves();

        StartCoroutine(Co_PrepareNextLevel());
    }

    private bool HasNextLevel() => levelIndex + 1 < levelPacks.Length;

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

    private IEnumerator Co_PrepareNextLevel()
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