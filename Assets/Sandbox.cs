using System.Collections.Generic;

using UnityEngine;

public class Sandbox : GUIMonoBehaviour
{
    private IList<GameObject> potentialCandidates;
    private GameObject closestCandidate;
    private GameObject player;

    void Awake()
    {
        ResolveComponents();
        ConfigGUIStyle();

        guiStyle.fontSize = 8;
        guiStyle.font = (Font) Resources.Load("Fonts/Block Stock");
        guiStyle.normal.textColor = Color.white;
    }

    // Update is called once per frame
    void Update()
    {
        potentialCandidates = ResolvePotentialCandidates();
    }

    private void ResolveComponents()
    {
        player = GameObject.FindGameObjectWithTag("Player") as GameObject;
    }

    private IList<GameObject> ResolvePotentialCandidates()
    {
        IList<GameObject> potentialCandidates = new List<GameObject>();
        var instances = Object.FindObjectsOfType<HealthAttributes>() as HealthAttributes[];

        foreach (MonoBehaviour instance in instances)
        {
            if (instance.transform.root.tag != "Player")
            {
                float distance = (instance.transform.position - player.transform.position).magnitude;

                if (closestCandidate == null)
                {
                    closestCandidate = instance.transform.root.gameObject;
                }
                else if (distance < ((closestCandidate.transform.position - player.transform.position).magnitude))
                {
                    closestCandidate = instance.transform.root.gameObject;
                }

                potentialCandidates.Add(instance.transform.root.gameObject);
            }
        }

        return potentialCandidates;
    }
        
    private int labelIndex;

    public override void SetGUIAttributes(GUIStyle guiStyle)
    {
        labelIndex = 0;

        if (potentialCandidates != null)
        {
            PublishLabel($"Closest Candidate: {closestCandidate.name}", Color.white);

            foreach (GameObject potentialCandidate in potentialCandidates)
            {
                PublishLabel($"{potentialCandidate.name}", Color.yellow);
            }
        }
    }

    private void PublishLabel(string text, Color color)
    {
        guiStyle.normal.textColor = color;
        GUI.Label(new Rect(15, 15 + (15 * labelIndex + 1), 200, 40), text, guiStyle);
        ++labelIndex;
    }
}