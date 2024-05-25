using System.Collections;
using System.Collections.Generic;

using UnityEngine;

public class EnemySandbox : MonoBehaviour
{
   [SerializeField] GameObject geometry;
   [SerializeField] WaveConfig waveConfig;

   [Range(0.0f, 1.0f)]
   [SerializeField] float fractionComplete = 0.0f;

   //[SerializeField] float transformSpeed;
   //[SerializeField] float duration;
   [SerializeField] bool shouldAnimate = false;
   [SerializeField] bool shouldLoop = false;
   [SerializeField] bool enableBuffeting = false;

   public class PositionVector
   {
       public Vector2 Position { get; set; }
       public Vector2 ProjectedPosition { get; set; }
       public Vector2 Direction { get; set; }
   }

   private GameObject enemyPrefab, enemy;
   private AnimationClip animationClip;
   private int waveSize, enemyCount;
   private float timeBetweenSpawns, speed;
   private IDictionary<string, AnimationCurve> animationCurves;
   private AnimationCurve xPositionCurve, yPositionCurve;
   private float? lastFractionComplete = null;
   private float startTransformTime;
   private bool animate = false;

   // Start is called before the first frame update
   void Start()
   {
       enemyPrefab = waveConfig.GetEnemyPrefab();
       animationClip = waveConfig.GetAnimationClip();
       enemyCount = waveSize = waveConfig.GetWaveSize();
       timeBetweenSpawns = waveConfig.GetTimeBetweenSpawns();
       speed = waveConfig.GetSpeed();
       animationCurves = UnityEditorAnimationFunctions.ExtractCurves(animationClip);

       foreach (KeyValuePair<string, AnimationCurve> entry in animationCurves)
       {
           if (entry.Key.Equals("m_LocalPosition.x"))
           {
               xPositionCurve = entry.Value;
           }
           else if (entry.Key.Equals("m_LocalPosition.y"))
           {
               yPositionCurve = entry.Value;
           }
       }

    //    CreateEnemy();
       StartCoroutine(Co_CreateEmemies(speed));
   }

   private void CreateEnemy()
   {
       enemy = Instantiate(enemyPrefab, new Vector3(0.0f, 0.0f, 1.0f), Quaternion.identity) as GameObject;
       enemy.transform.parent = geometry.transform;

       if (shouldAnimate)
       {
           Animate(Time.time, speed/*transformSpeed * duration*/);
       }
   }

   private IEnumerator Co_CreateEmemies(float speed)
   {
      for (int itr = 0; itr < waveSize; ++itr)
      {
          enemy = Instantiate(enemyPrefab, new Vector3(0.0f, 0.0f, 1.0f), Quaternion.identity) as GameObject;
          enemy.transform.parent = geometry.transform;

          if (shouldAnimate)
          {
              Animate(Time.time, speed/*transformSpeed * duration*/);
          }

          yield return new WaitForSeconds(timeBetweenSpawns);
      }
   }

   // Update is called once per frame
   void Update()
   {
       if ((lastFractionComplete == null) || (fractionComplete != (float)lastFractionComplete))
       {
           lastFractionComplete = fractionComplete;

           PositionVector positionVector = GetPositionPointData(fractionComplete);
           enemy.transform.position = positionVector.Position;
           float angle = MathFunctions.GetAngle(positionVector.Position, positionVector.ProjectedPosition, 90);
           enemy.transform.rotation = Quaternion.Euler(enemy.transform.rotation.eulerAngles.x, enemy.transform.rotation.eulerAngles.y, angle);
       }

       if (animate)
       {
           float animatedFractionComplete = (Time.time - startTransformTime) * speed;

           if (animatedFractionComplete >= 0.0f)
           {
               bool complete = OnAnimate(animatedFractionComplete);

               if (complete)
               {
                   OnComplete(enemy);
               }
           }
       }
   }

   private PositionVector GetPositionPointData(float fractionComplete)
   {
       Vector2 position = CalculatePosition(fractionComplete);
       Vector2 projectedPosition = CalculatePosition(fractionComplete + 0.1f);

       if (enableBuffeting)
       {
           Vector2 buffeting = new Vector2
           {
               x = Random.Range(0.0f, 0.25f),
               y = Random.Range(0.0f, 0.25f)
           };

           position += buffeting;
           projectedPosition += buffeting;
       }

       Vector2 direction = projectedPosition - position;

       return new PositionVector
       {
           Position = position,
           ProjectedPosition = projectedPosition,
           Direction = direction
       };
   }

   public bool OnAnimate(float fractionComplete)
   {
       PositionVector positionVector = GetPositionPointData(fractionComplete);
       enemy.transform.position = positionVector.Position;
       float angle = MathFunctions.GetAngle(positionVector.Position, positionVector.ProjectedPosition, 90);
       enemy.transform.rotation = Quaternion.Euler(enemy.transform.rotation.eulerAngles.x, enemy.transform.rotation.eulerAngles.y, angle);

       return (fractionComplete >= 1.0f);
   }

   private Vector2 CalculatePosition(float fractionComplete)
   {
       return new Vector2(
           xPositionCurve.Evaluate((float)fractionComplete),
           yPositionCurve.Evaluate((float)fractionComplete));
   }

   private void OnComplete(GameObject gameObject)
   {
       animate = false;

       Destroy(enemy);
       //--enemyCount;

       if (/*(enemyCount == 0) && */(shouldLoop))
       {
           CreateEnemy();
           //StartCoroutine(CreateEmemies(speed));
       }
   }

   //private void Animate(float startTransformTime, float transformSpeed, float duration)
   //{
   //    Animate(startTransformTime, transformSpeed * duration);
   //}

   private void Animate(float startTransformTime, float speed)
   {
       this.startTransformTime = startTransformTime;
       this.speed = speed;

       animate = true;
   }
}