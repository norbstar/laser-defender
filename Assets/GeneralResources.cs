﻿using UnityEngine;

public class GeneralResources : MonoBehaviour
{
    [Header("Canvas")]
    [SerializeField] ScoreUIManager scoreManager;
    [SerializeField] LivesUIManager livesManager;
    [SerializeField] CountdownUIManager countdownManager;
    [SerializeField] ExpositionUIManager expositionManager;
    [SerializeField] LevelCompleteUIManager levelCompleteManager;
    [SerializeField] CockpitUIManager cockpitManager;

    [Header("Layers")]
    [SerializeField] BackdropManager backdropLayer;
    [SerializeField] LayersManager backgroundLayers;
    [SerializeField] LayersManager gameplayLayers;
    [SerializeField] LayersManager foregroundLayers;

    [Header("Player")]
    [SerializeField] DynamicPlayerController player;

    [Header("Prefabs")]
    [SerializeField] GameObject explosionPrefab;

    [Header("Folders")]
    [SerializeField] GameObject actuatorFolder;

    public ScoreUIManager ScoreManager { get => scoreManager; }

    public LivesUIManager LivesManager { get => livesManager; }

    public CountdownUIManager CountdownManager { get => countdownManager; }

    public ExpositionUIManager ExpositionManager { get => expositionManager; }

    public LevelCompleteUIManager LevelCompleteManager { get => levelCompleteManager; }

    public CockpitUIManager CockpitManager { get => cockpitManager; }

    public BackdropManager BackdropLayer { get => backdropLayer; }

    public LayersManager BackgroundLayers { get => backgroundLayers; }

    public LayersManager GameplayLayers { get => gameplayLayers; }

    public LayersManager ForegroundLayers { get => foregroundLayers; }

    public DynamicPlayerController Player { get => player; }

    public GameObject ActuatorFolder { get => actuatorFolder; }

    public GameObject ExplosionPrefab { get => explosionPrefab; }
}