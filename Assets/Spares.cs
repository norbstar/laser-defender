namespace Assets
{
    class Spares
    {
#if (false)
        private void InitLayerObjects(int activeLayer)
        {
            GameObject[] rootObjects = SceneManager.GetActiveScene().GetRootGameObjects();

            foreach (GameObject gameObject in rootObjects)
            {
                if (gameObject.GetComponentsInChildren<IModify>() is IModify[] iModifys)
                {
                    foreach (IModify iModify in iModifys)
                    {
                        GameObject childGameObject = iModify.GetDefaults().Transform.gameObject;

                        if (childGameObject.activeSelf)
                        {
                            ScaleLayerObject(childGameObject, activeLayer);
                        }
                    }
                }
            }
        }

        private void ScaleLayerObjects(int activeLayer, RenderLayer targetLayer, float fractionComplete)
        {
            GameObject[] rootObjects = SceneManager.GetActiveScene().GetRootGameObjects();

            foreach (GameObject gameObject in rootObjects)
            {
                if (gameObject.GetComponentsInChildren<IModify>() is IModify[] iModifys)
                {
                    foreach (IModify iModify in iModifys)
                    {
                        GameObject childGameObject = iModify.GetDefaults().Transform.gameObject;

                        if (childGameObject.activeSelf)
                        {
                            ScaleLayerObject(childGameObject, activeLayer, targetLayer, fractionComplete);
                        }
                    }
                }
            }
        }

        public float GetMultiplier(GameObject gameObject, int layer)
        {
            float multiplier = 1.0f;

            int childLayer = gameObject.layer;

            if (layer == (int)RenderLayer.SURFACE)
            {
                if (childLayer == (int)RenderLayer.SUB_SURFACE)
                {
                    return 0.9f;
                }
            }
            else if (layer == (int)RenderLayer.SUB_SURFACE)
            {
                if (childLayer == (int)RenderLayer.SURFACE)
                {
                    return 1.1f;
                }
            }

            return multiplier;
        }

        private void ScaleLayerObject(GameObject gameObject, int activeLayer)
        {
            var iModify = gameObject.GetComponent<IModify>() as IModify;

            if (iModify != null)
            {
                float multiplier = GetMultiplier(gameObject, activeLayer);

                iModify.SetScale(multiplier);
            }
        }

        private void ScaleLayerObject(GameObject gameObject, int activeLayer, RenderLayer layer, float fractionComplete)
        {
            var iModify = gameObject.GetComponent<IModify>() as IModify;

            if (iModify != null)
            {
                float originMultiplier = GetMultiplier(gameObject, activeLayer);
                float targetMultiplier = GetMultiplier(gameObject, (int)layer);
                float multiplier = Mathf.Lerp(originMultiplier, targetMultiplier, fractionComplete);

                iModify.SetScale(multiplier);
            }
        }
#endif
    }
}