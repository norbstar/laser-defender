﻿using System.Collections;

using UnityEngine;

public class IssueOrdersSequenceActuationManager : MonoBehaviour, IActuate
{
    [Header("Configuration")]
    [SerializeField] IssueOrdersSequenceConfig issueOrdersSequenceConfig;

    private CockpitViewUIManager cockpitViewUIManager;
    private GameObject issueOrdersSequencePrefab;
    private CockpitUIManager cockpitUIManager;
    private IssueOrdersSequenceManager issueOrdersSequenceManager;
    private GeneralResources generalResources;

    void Awake()
    {
        ResolveComponents();
    }

    private void ResolveComponents()
    {
        generalResources = FindObjectOfType<GeneralResources>();
    }

    public void Actuate(IConfiguration configuration = null)
    {
        if (gameObject.activeSelf)
        {
            StartCoroutine(ActuateCoroutine());
        }
    }

    private void ResolveDependencies()
    {
        issueOrdersSequencePrefab = issueOrdersSequenceConfig.GetIssueOrdersSequencePrefab();

        GameObject cockpitPanel = generalResources.GetCockpitPanel();
        cockpitUIManager = cockpitPanel.GetComponent<CockpitUIManager>() as CockpitUIManager;
        cockpitViewUIManager = cockpitUIManager.GetCockpitViewUIManager();
    }

    private IEnumerator ActuateCoroutine()
    {
        ResolveDependencies();

        cockpitUIManager.SetAsset(issueOrdersSequenceConfig.GetCockpitAsset());
        cockpitViewUIManager.RegisterDelegates(new CockpitViewUIManager.Delegates
        {
            OnCockpitShownDelegate = OnCockpitShown,
            OnCockpitHiddenDelegate = OnCockpitHidden
        });

        GameObject prefab = Instantiate(issueOrdersSequencePrefab) as GameObject;
        prefab.transform.parent = generalResources.GetActuatorFolder().transform;

        issueOrdersSequenceManager = prefab.GetComponent<IssueOrdersSequenceManager>() as IssueOrdersSequenceManager;
        issueOrdersSequenceManager.RegisterDelegates(new IssueOrdersSequenceManager.Delegates
        {
            OnStartAnimationDelegate = OnStartAnimation,
            OnEndAnimationDelegate = OnEndAnimation
        });

        issueOrdersSequenceManager.Actuate();

        yield return null;
    }

    private void OnStartAnimation()
    {
        cockpitUIManager.ShowCockpit();
    }

    private void OnEndAnimation()
    {
        cockpitUIManager.HideCockpit();
    }

    private void OnCockpitShown() { }

    private void OnCockpitHidden() { }
}