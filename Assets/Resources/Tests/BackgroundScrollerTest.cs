using UnityEngine;

namespace Tests
{
    public class BackgroundScrollerTest : MonoBehaviour
    {
        [Header("Packs")]
        [SerializeField] SpriteAssetPack spritePack;
        [SerializeField] LayerPack layerPack;

        private GeneralResources generalResources;
        private BackdropManager backdropManager;
        private LayersManager layersManager;
        private DynamicPlayerController playerController;

        void Awake() => ResolveComponents();

        private void ResolveComponents()
        {
            generalResources = GetComponent<GeneralResources>();
            layersManager = generalResources.GameplayLayers;
            
            backdropManager = FindObjectOfType<BackdropManager>();
            playerController = FindObjectOfType<DynamicPlayerController>();
        }

        // Start is called before the first frame update
        void Start()
        {
            ApplyBackground(spritePack);
            ApplyLayers(layerPack);

            RegisterDelegates();
            EngageShip();
        }

        private void ApplyBackground(SpriteAssetPack spritePack)
        {
            if (backdropManager.IsActive())
            {
                backdropManager.Deactivate();
            }
            else
            {
                backdropManager.Activate(spritePack);
            }
        }

        private void ApplyLayers(LayerPack layerPack)
        {
            layersManager.DestroyLayers();
            layersManager.Initiate(layerPack);
        }

        private void RegisterDelegates()
        {
            playerController.RegisterDelegates(new DynamicPlayerController.Delegates
            {
                OnShipEngagedDelegate = OnShipEngaged,
                OnShipDisengagedDelegate = OnShipDisengaged
            });
        }

        private void EnablePlayerControls() => playerController.EnableControls();

        public void OnShipEngaged() => EnablePlayerControls();

        private void EngageShip() => playerController.EngageShip();

        public void OnShipDisengaged()
        {
            playerController.Reset();
            EngageShip();
        }
    }
}
