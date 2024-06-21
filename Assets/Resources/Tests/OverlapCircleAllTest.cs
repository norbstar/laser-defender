using System.Collections.Generic;

using UnityEngine;

namespace Tests
{
    public class OverlapCircleAllTest : BaseMoveTest
    {
        [Header("Config")]
        [SerializeField] float targetRadius = 5f;

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

        private bool InBounds(Vector3 position) => position.x >= boundary.XMin && position.x <= boundary.XMax && position.y >= boundary.YMin && position.y <= boundary.YMax;

#if false
        // Update is called once per frame
        public override void Update()
        {
            base.Update();

            foreach (var collider in this.colliders)
            {
                if (collider.TryGetComponent<IFocus>(out var focus))
                {
                    focus.ShowCue(false);
                }
            }
            
            this.colliders.Clear();

            var colliders = Physics2D.OverlapCircleAll(VectorFunctions.ToVector2(transform.position), targetRadius, targetLayerMask);
            
            foreach (var collider in colliders)
            {
                var inBounds = InBounds(collider.transform.position);
                var distanceToTarget = Vector3.Distance(transform.position, collider.transform.position);
                var dotProduct = Vector2.Dot(Vector2.up, (collider.transform.position - transform.position).normalized);

                if (collider.TryGetComponent<IFocus>(out var focus) && inBounds && dotProduct >= 0.7f && dotProduct <= 1f)
                {
                    focus.ShowCue();
                    this.colliders.Add(collider);
                }
            }
        }
#endif

#if true
        // Update is called once per frame
        public override void Update()
        {
            base.Update();

            if (closestTarget != null)
            {
                closestTarget.ShowCue(false);
            }

            var colliders = Physics2D.OverlapCircleAll(VectorFunctions.ToVector2(transform.position), targetRadius, targetLayerMask);
            
            shortestDistance = float.MaxValue;
            // narrowestAngle = float.MaxValue;
            closestTarget = null;

            foreach (var collider in colliders)
            {
                var inBounds = InBounds(collider.transform.position);
                var distanceToTarget = Vector3.Distance(transform.position, collider.transform.position);
                var dotProduct = Vector2.Dot(Vector2.up, (collider.transform.position - transform.position).normalized);

                if (collider.TryGetComponent<IFocus>(out var focus) && inBounds && dotProduct >= 0.7f && dotProduct <= 1f)
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
#endif
    }
}
