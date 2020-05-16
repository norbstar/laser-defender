using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;

public class EnemyWaveManager : GUIMonoBehaviour
{
    public delegate void OnScoreAchieved(int score);

    //[SerializeField] WaveConfig[] waveConfigs;
    //[SerializeField] WaveConfig activeWaveConfig;
    [SerializeField] GameObject geometry;
    [SerializeField] ScoreUIManager scoreUIManager;
    [SerializeField] GameObject explosionPrefab;

    private GameObject enemyPrefab;
    private WaveConfig[] waveConfigs;
    private int waveSize, enemyCount, allocatedEnemyCount, waveSequence, waveIndex;
    private float timeBetweenSpawns, timeBetweenWaves;
    //private bool waveDeployed, waveDepleted;
    private WaveConfig waveConfig;
    //private float? lastTimestamp;
    private PlayerController playerController;
    private OnScoreAchieved onScoreAchievedDelegate;
    private bool scoreNotified;
    private bool active;

    void Awake()
    {
        ResolveComponents();
        ConfigGUIStyle();

        scoreNotified = false;
        active = true;
    }

    // Start is called before the first frame update

    //IEnumerator Start()
    //{
    //    ConfigGUIStyle();

    //    do
    //    {
    //        yield return StartCoroutine(SpawnAllWaves());
    //    }
    //    while (true);
    //}

    public void RegisterDelegate(OnScoreAchieved onScoreAchievedDelegate)
    {
        this.onScoreAchievedDelegate = onScoreAchievedDelegate;
    }

    public void AssignWaveConfigPack(WaveConfigPack waveConfigPack)
    {
        waveConfigs = waveConfigPack.GetPack();
    }

    //public void LaunchWaves()
    //{
    //    StartCoroutine(SpawnAllWaves()/*SpawnAllEmemiesInWave()*/);
    //}

    public void SuspendFutureWaves()
    {
        active = false;
    }

    private void ResolveComponents()
    {
        scoreUIManager = FindObjectOfType<ScoreUIManager>();
        playerController = FindObjectOfType<PlayerController>();
    }

    private void ResolveDependencies(WaveConfig waveConfig)
    {
        enemyPrefab = waveConfig.GetEnemyPrefab();
        timeBetweenSpawns = waveConfig.GetTimeBetweenSpawns();
        timeBetweenWaves = waveConfig.GetTimeBetweenWaves();
        waveSize = waveConfig.GetWaveSize();
    }

    // Update is called once per frame
    //void Update()
    //{
    //    if (waveDeployed && waveDepleted)
    //    {
    //        StartCoroutine(SpawnAllEmemiesInWave(timeBetweenWaves));
    //    }
    //}

    public void SpawnAllWaves()
    {
        StartCoroutine(SpawnAllWavesCoroutine());
    }

    private IEnumerator SpawnAllWavesCoroutine()
    {
        active = true;
        waveIndex = 0;
        scoreNotified = false;

        while (active)
        {
            waveConfig = waveConfigs[waveIndex];
            yield return StartCoroutine(SpawnAllEmemiesInWave(waveConfig));

            waveIndex = ((waveIndex + 1) <= (waveConfigs.Length - 1)) ? waveIndex + 1 : 0;
            yield return new WaitForSeconds(timeBetweenWaves);
        }
    }

    //IEnumerator SpawnAllWaves(float delay = 0.0f)
    //{
    //    waveConfig = waveConfigs[waveIndex];
    //    yield return StartCoroutine(SpawnAllEmemiesInWave(waveConfig));

    //    waveIndex = ((waveIndex + 1) <= (waveConfigs.Length - 1)) ? waveIndex + 1 : 0;
    //    yield return new WaitForSeconds(delay);
    //}

    private IEnumerator SpawnAllEmemiesInWave(WaveConfig waveConfig/*, float delay = 0.0f*/)
    {
        //activeWaveConfig = waveConfig;
        ResolveDependencies(waveConfig);

        //waveDeployed = waveDepleted = false;
        allocatedEnemyCount = 0;
        ++waveSequence;

        //wave = new Wave
        //{
        //    id = waveSequence,
        //    spawnCount = waveSize
        //};

        //waveData.waves.Add(wave);

        //AddLogEntry(wave, "CreateEmemies Begin");

        //yield return new WaitForSeconds(delay);

        for (int itr = 0; itr < waveSize; ++itr)
        {
            GameObject prefab = Instantiate(enemyPrefab, new Vector3(-1.0f, -1.0f, 1.0f), Quaternion.identity) as GameObject;
            prefab.transform.parent = geometry.transform;
            prefab.name = $"Wave{waveSequence}-Enemy{itr + 1}";

            EnemyController enemyController = prefab.GetComponent<EnemyController>();
            enemyController.Configure(waveConfig, waveSequence);
            enemyController.RegisterDelegates(new EnemyController.Delegates
            {
                OnEnemyDamagedDelegate = OnEnemyDamaged,
                OnEnemyDestroyedDelegate = OnEnemyDestroyed
            });

            enemyController.Actuate(new EnemyController.Configuration { });

            ++allocatedEnemyCount;
            ++enemyCount;
        
            //AddLogEntry(wave, $"CreateEmemies Game Object: {enemy.name} Enemies: {enemyCount}");

            yield return new WaitForSeconds(timeBetweenSpawns);
        }

        //AddLogEntry(wave, "CreateEmemies End");

        //waveIndex = ((waveIndex + 1) <= (waveConfigs.Length - 1)) ? waveIndex + 1 : 0;
        //waveDeployed = true;
    }

    //private void AddLogEntry(Wave wave, string message)
    //{
    //    Log log = new Log
    //    {
    //        timestamp = Time.time.ToString(),
    //        message = message
    //    };

    //    if (lastTimestamp == null)
    //    {
    //        log.delta = null;
    //    }
    //    else
    //    {
    //        log.delta = (Time.time - (float) lastTimestamp).ToString();
    //    }

    //    wave.logs.Add(log);
    //}

    //private Wave RetrieveWave(int waveSequence)
    //{
    //    foreach (Wave wave in waveData.waves)
    //    {
    //        if (wave.id == waveSequence)
    //        {
    //            return wave;
    //        }
    //    }

    //    return null;
    //}

    //void OnDestroy()
    //{
    //    string json = JsonUtility.ToJson(waveData);
    //    LogFunctions.LogRawToFile($"waveData.json", json);
    //}

    public void OnEnemyDamaged(GameObject gameObject, int waveSequence, HealthAttributes healthAttributes)
    {
        // TODO
    }

    public void OnEnemyDestroyed(GameObject gameObject, int waveSequence, bool selfAdministered)
    {
        --enemyCount;

        //if ((allocatedEnemyCount == waveSize) && (enemyCount == 0))
        //{
        //    waveDepleted = true;
        //}

        //Wave wave = RetrieveWave(waveSequence);

        //if (wave != null)
        //{
        //    AddLogEntry(wave, $"OnDestroyed Game Object: {gameObject.name} Self Admin: {selfAdministered} Enemies: {enemyCount} Wave Depleted: {waveDepleted}");
        //}

        if (!selfAdministered)
        {
            Destroy(Instantiate(explosionPrefab, gameObject.transform.position, Quaternion.identity), 0.15f);

            var enemyController = gameObject.GetComponent<EnemyController>();
            int pointsAwarded = enemyController.GetPointsAwarded();
            scoreUIManager.SupplementScore(pointsAwarded);

            int score = scoreUIManager.GetScore();

            if ((scoreUIManager.GetScore() > 250) && !scoreNotified)
            {
                scoreNotified = true;
                onScoreAchievedDelegate?.Invoke(score);
            }
        }

        Destroy(gameObject);
    }

    public override void SetGUIAttributes(GUIStyle guiStyle)
    {
        //GUI.Label(new Rect(20, 75, 200, 40), $"Wave: {waveSequence}", guiStyle);
        //GUI.Label(new Rect(20, 125, 200, 40), $"Enemy Count: {enemyCount}", guiStyle);
    }
}