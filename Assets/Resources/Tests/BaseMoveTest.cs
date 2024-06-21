using UnityEngine;

namespace Tests
{
    public class BaseMoveTest : MonoBehaviour
    {
        [Header("Config")]
        [SerializeField] float speed = 10.0f;

        public class Boundary
        {
            public float XMin { get; set; }
            public float XMax { get; set; }
            public float YMin { get; set; }
            public float YMax { get; set; }

            public override string ToString() => $"({XMin}, {YMin}, {XMax}, {YMax})";
        }

        private DualInputActions inputActions;
        protected Boundary boundary;
        
        public virtual void Awake() => inputActions = new DualInputActions();

        // Start is called before the first frame update
        void Start()
        {
            var mainCamera = Camera.main;

            boundary = new Boundary
            {
                XMin = mainCamera.ViewportToWorldPoint(new Vector3(0.0f, 0.0f, 0.0f)).x,
                XMax = mainCamera.ViewportToWorldPoint(new Vector3(1.0f, 0.0f, 0.0f)).x,
                YMin = mainCamera.ViewportToWorldPoint(new Vector3(0.0f, 0.0f, 0.0f)).y,
                YMax = mainCamera.ViewportToWorldPoint(new Vector3(0.0f, 1.0f, 0.0f)).y
            };

            // Debug.Log($"Boundary XMin: {boundary.XMin} XMax: {boundary.XMax} YMin: {boundary.YMin} YMax: {boundary.YMax}");
        }

        void OnEnable() => inputActions.Enable();

        void OnDisable() => inputActions.Disable();

        // Update is called once per frame
        public virtual void Update()
        {
            var input = inputActions.Player.Move.ReadValue<Vector2>();
            var regulatedInput = input * Time.deltaTime * speed;

            var positionX = transform.position.x + regulatedInput.x;
            var unitPositionX = Mathf.Clamp(positionX, boundary.XMin + 1.0f, boundary.XMax - 1.0f);

            var positionY = transform.position.y + regulatedInput.y;
            var unitPositionY = Mathf.Clamp(positionY, boundary.YMin + 1.25f, boundary.YMax - 1.0f);

            transform.position = new Vector3(unitPositionX, unitPositionY, transform.position.z);
        }
    }
}
