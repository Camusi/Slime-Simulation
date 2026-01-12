using UnityEngine;

public class Simulation : MonoBehaviour
{
    // simulation parameters
    [Header("Simulation")]
    public int width = 1280;
    public int height = 720;
    public int numAgents = 2000;
    public float speed = 20f;

    [Header("Display")]
    public Material targetMaterial;
    public FilterMode filterMode;

    // key variables
    Texture2D texture;
    Agent[] agents;
    Color[] pixels;

    struct Agent {
        public Vector2 pos;
        public float angle;
    }

    void Start()
    {
        texture = new Texture2D(width, height, TextureFormat.RGBA32, false);
        texture.filterMode = filterMode;
        pixels = new Color[width * height];
        ClearPixels();
        InitAgents();

        targetMaterial.mainTexture = texture;
    }

    void ClearPixels()
    {
        for (int i = 0; i < pixels.Length; i++)
        {
            pixels[i] = Color.black;
        }

        texture.SetPixels(pixels);
        texture.Apply();
    }

    void InitAgents()
    {
        for (int i = 0; i < agents.Length; i++)
        {
            agents[i].angle = 2f * Mathf.PI * Random.value;  // C = 2 * pi * r, Note: .value is range [0.0, 1.0]
            agents[i].pos = new Vector2(Random.Range(0, width), Random.Range(0, height));
        }
    }

    void Update()
    {
        // FadePixels();
        // MoveAgents();
        // DrawAgents();

        // Update texture with pixel values
        // Apply update
    }

}