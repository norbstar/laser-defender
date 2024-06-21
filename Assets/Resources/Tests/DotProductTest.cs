using System;
using System.Collections.Generic;
using UnityEngine;

namespace Tests
{
    public class DotProductTest : MonoBehaviour
    {
        [Serializable]
        public class Info
        {
            public Collider2D collider;
            public float dotProduct;
        }

        [Header("Analytics")]
        [SerializeField] List<Info> infos;

        private int targetLayerMask;

        void Awake() => targetLayerMask = LayerMask.GetMask("Gameplay Subsurface Layer");

        // Start is called before the first frame update
        void Start() => infos = new List<Info>();

        // Update is called once per frame
        void Update()
        {
            var colliders = Physics2D.OverlapCircleAll(VectorFunctions.ToVector2(transform.position), 5f, targetLayerMask);

            infos.Clear();
            
            foreach (var collider in colliders)
            {
                infos.Add(new Info
                {
                    collider = collider,
                    dotProduct = Vector2.Dot(Vector2.up, (collider.transform.position - transform.position).normalized)
                });
            }
        }
    }
}
