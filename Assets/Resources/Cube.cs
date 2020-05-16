using UnityEngine;

public class Cube : MonoBehaviour
{
    [Range(1.0f, 10.0f)]
    [SerializeField] float range = 1.0f;

    // Start is called before the first frame update
    void Start()
    {
        GenerateColor();
    }

    public void GenerateColor()
    {
        GetComponent<Renderer>().sharedMaterial.color = Random.ColorHSV();
    }

    public void ResetColor()
    {
        GetComponent<Renderer>().sharedMaterial.color = Color.white;
    }
}