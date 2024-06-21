using System.Collections.Generic;

using UnityEngine;

namespace Tests
{
    public class OverlapAreaAllTest : BaseMoveTest
    {
        [Header("Analytics")]
        [SerializeField] List<Collider2D> colliders;

        private int targetLayerMask;
        private float shortestDistance;
        // private float narrowestAngle;
        private IFocus closestTarget;

        public override void Awake()
        {
            base.Awake();
            targetLayerMask = LayerMask.GetMask("Gameplay Layer");
        }

        // Update is called once per frame
        public override void Update()
        {
            base.Update();

            if (closestTarget != null)
            {
                closestTarget.ShowCue(false);
            }

            var colliders = Physics2D.OverlapAreaAll(new Vector2(boundary.XMin,boundary.YMin), new Vector2(boundary.XMax,boundary.YMax), targetLayerMask);
            
            shortestDistance = float.MaxValue;
            // narrowestAngle = float.MaxValue;
            closestTarget = null;

            foreach (var collider in colliders)
            {
                var distanceToTarget = Vector3.Distance(transform.position, collider.transform.position);
                var dotProduct = Vector2.Dot(Vector2.up, (collider.transform.position - transform.position).normalized);

                if (collider.TryGetComponent<IFocus>(out var focus) && dotProduct >= 0.7f && dotProduct <= 1f)
                {
                    if (distanceToTarget < shortestDistance/* && dotProduct < narrowestAngle*/)
                    {
                        shortestDistance = distanceToTarget;
                        // narrowestAngle = dotProduct;
                        closestTarget = focus;
                    }
                }

            }

            if (closestTarget != null)
            {
                closestTarget.ShowCue();
            }
        }
    }
}
