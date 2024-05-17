using System;
//using System.Collections;
using System.Reflection;

using UnityEngine;

namespace UnityEditorInternal
{
    public class LayersSandbox : MonoBehaviour
    {
        [SerializeField] LayerPack layerPack;

        private LayersManager layersManager;

        void Awake()
        {
            ResolveComponents();

            string[] names = GetSortingLayerNames();

            foreach (string name in names)
            {
                Debug.Log($"Name: {name}");
            }

            int[] ids = GetSortingLayerUniqueIDs();

            foreach (int id in ids)
            {
                Debug.Log($"Id: {id}");
            }

            int canvasSurfaceId = SortingLayer.NameToID("Canvas Surface");
            Debug.Log($"Id for Canvas Surface: {canvasSurfaceId}");

            int backgroundSurfaceId = SortingLayer.NameToID("Background Surface");
            Debug.Log($"Id for Background Surface: {backgroundSurfaceId}");

            int gameplaySubsurfaceId = SortingLayer.NameToID("Gameplay Subsurface");
            Debug.Log($"Id for Gameplay Subsurface: {gameplaySubsurfaceId}");

            int gameplaySurfaceId = SortingLayer.NameToID("Gameplay Surface");
            Debug.Log($"Id for Gameplay Surface: {gameplaySurfaceId}");

            int foregroundSurfaceId = SortingLayer.NameToID("Foreground Surface");
            Debug.Log($"Id for Foreground Surface: {foregroundSurfaceId}");
        }

        // Start is called before the first frame update
        void Start()
        {
            //ApplyBackgroundLayers(layerPack);
        }

        // Update is called once per frame
        //void Update()
        //{
        //    if (Input.GetKeyDown(KeyCode.Escape))
        //    {
        //        layersManager.DestroyLayers();
        //        ApplyBackgroundLayers(layerPack);
        //    }
        //}

        private void ResolveComponents()
        {
            //layersManager = FindObjectOfType<LayersManager>();
        }

        //private void ApplyBackgroundLayers(LayerPack layerPack)
        //{
        //    layersManager.Initiate(layerPack);
        //}

        public string[] GetSortingLayerNames()
        {
            Type internalEditorUtilityType = typeof(InternalEditorUtility);
            PropertyInfo sortingLayersProperty = internalEditorUtilityType.GetProperty("sortingLayerNames", BindingFlags.Static | BindingFlags.NonPublic);
            return (string[])sortingLayersProperty.GetValue(null, new object[0]);
        }

        public int[] GetSortingLayerUniqueIDs()
        {
            Type internalEditorUtilityType = typeof(InternalEditorUtility);
            PropertyInfo sortingLayerUniqueIDsProperty = internalEditorUtilityType.GetProperty("sortingLayerUniqueIDs", BindingFlags.Static | BindingFlags.NonPublic);
            return (int[])sortingLayerUniqueIDsProperty.GetValue(null, new object[0]);
        }
    }
}