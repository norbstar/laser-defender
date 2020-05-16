using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;

[RequireComponent(typeof(HealthAttributes))]
public class ScriptedShipController : MonoBehaviour, IActuate
{
    public delegate void OnShipEnaged();
    public delegate void OnShipDisengaged();

    public class Delegates
    {
        public OnShipEnaged OnShipEngagedDelegate { get; set; }
        public OnShipDisengaged OnShipDisengagedDelegate { get; set; }
    }

    [SerializeField] GameObject ship;
    [SerializeField] GameObject[] exhausts;
    [SerializeField] GameObject reverseLeftTrust;
    [SerializeField] GameObject reverseRightTrust;
    [SerializeField] float speed = 10.0f;

    private class ExhaustComponent
    {
        public DynamicExhaustController ExhaustController { get; set; }
    }

    private Animator animator;
    private IList<ExhaustComponent> exhaustComponents;
    //private Vector3 lastPosition;
    private Queue<Vector2> inputs;

    void Awake()
    {
        ResolveComponents();

        inputs = new Queue<Vector2>();
    }

    public void Actuate(IConfiguration configuration = null)
    {
        if (gameObject.activeSelf)
        {
            StartCoroutine(ActuateCoroutine());
        }
    }

    private IEnumerator ActuateCoroutine()
    {
        ResolveDependencies();

        //lastPosition = transform.position;

        animator.SetBool("actuate", true);

        while (true)
        {
            //Vector2 input = (transform.position - lastPosition) * speed * Time.deltaTime;

            if (inputs.Count > 0)
            { 
                Vector2 input = inputs.Dequeue();

                //Debug.Log($"Input X: {input.x} Y: {input.y}");

                animator.SetFloat("speed", input.x);

                SetExhaustThrust(input.y);

                reverseLeftTrust.SetActive(input.y < 0.0f);
                reverseRightTrust.SetActive(input.y < 0.0f);

                //if (input.x != 0.0f)
                //{
                //    animator.SetFloat("speed", input.x);
                //}

                //if (input.y != 0.0f)
                //{
                //    SetExhaustThrust(input.y);

                //    reverseLeftTrust.SetActive(input.y < 0.0f);
                //    reverseRightTrust.SetActive(input.y < 0.0f);
                //}
                //else
                //{
                //    reverseLeftTrust.SetActive(false);
                //    reverseRightTrust.SetActive(false);
                //}

                //lastPosition = transform.position;
            }

            yield return null;
        }
    }

    public void EnqueueInput(Vector2 input)
    {
        inputs.Enqueue(input);
    }

    private void ResolveDependencies() { }

    public GameObject[] GetExhausts()
    {
        return exhausts;
    }

    public GameObject GetReverseLeftTrust()
    {
        return reverseLeftTrust;
    }

    public GameObject GetReverseRightTrust()
    {
        return reverseRightTrust;
    }

    private void SetExhaustThrust(float thrust)
    {
        foreach (ExhaustComponent exhaustComponent in exhaustComponents)
        {
            exhaustComponent.ExhaustController.SetThrust(thrust);
        }
    }

    private void ResolveComponents()
    {
        animator = ship.GetComponent<Animator>() as Animator;
        exhaustComponents = new List<ExhaustComponent>();

        foreach (GameObject exhaust in exhausts)
        {
            exhaustComponents.Add(new ExhaustComponent
            {
                ExhaustController = exhaust.GetComponent<DynamicExhaustController>() as DynamicExhaustController
            });
        }
    }
}