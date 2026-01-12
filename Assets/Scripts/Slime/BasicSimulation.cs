using UnityEngine;

public class BasicSimulation : MonoBehaviour
{
    // Simulation parameters
    [Header("Simulation")]
    public int width = 256;
    public int height = 256;
    public int numAgents = 2000;
    public float speed = 20f;
    [Range(0.8f, 1f)] public float decay = 0.985f;

    [Header("Display")]
    public Material targetMaterial;
    public FilterMode filterMode;


    Texture2D tex;  // the texture we draw to
    Color[] pixels; // the colour data for each pixel
    Agent[] agents; // the dots in the simulation

    struct Agent { 
        public Vector2 pos; 
        public float angle; 
    }

    void Start()
    {
        tex = new Texture2D(width, height, TextureFormat.RGBA32, false);    // create new texture
        tex.filterMode = filterMode;  // makes the visuals crisp or blurry
        pixels = new Color[width * height];     // create pixel array
        ClearPixels();

        InitAgents();

        if (targetMaterial != null) targetMaterial.mainTexture = tex;   // If material already assigned, set its texture
    }

    // Create agents list with randomized positions and angles
    void InitAgents()
    {
        agents = new Agent[numAgents];
        for (int i = 0; i < agents.Length; i++)
        {
            agents[i].pos = new Vector2(Random.Range(0, width), Random.Range(0, height));
            agents[i].angle = Random.value * Mathf.PI * 2f;
        }
    }

    void Update()
    {
        FadePixels();   // fade all pixels each update (movement will not be faded since we redraw them each frame)
        MoveAgents();   // update agent positions along their direction vectors
        DrawAgents();   // make pixels white where agents are

        tex.SetPixels(pixels);  // update texture pixel data
        tex.Apply(false);   // apply changes to texture
    }

    // Set all pixels to black
    void ClearPixels()
    {
        for (int i = 0; i < pixels.Length; i++) pixels[i] = Color.black;
        if (tex != null) { tex.SetPixels(pixels); tex.Apply(false); }
    }

    void FadePixels()
    {
        for (int i = 0; i < pixels.Length; i++)
        {
            var pixel = pixels[i];

            // move pixel colour towards black
            pixel.r *= decay;
            pixel.g *= decay;
            pixel.b *= decay;

            pixels[i] = pixel;
        }
    }

    void MoveAgents()
    {
        for (int i = 0; i < agents.Length; i++)
        {
            Vector2 dir = new Vector2(Mathf.Cos(agents[i].angle), Mathf.Sin(agents[i].angle));  // creates the direction vector
            agents[i].pos += dir * speed * Time.deltaTime;  // moves the agent along the direction vector
            agents[i].angle += (Random.value - 0.5f) * 0.5f; // small random turn

            // wrap around screen edges
            if (agents[i].pos.x < 0) agents[i].pos.x += width;
            if (agents[i].pos.x >= width) agents[i].pos.x -= width;
            if (agents[i].pos.y < 0) agents[i].pos.y += height;
            if (agents[i].pos.y >= height) agents[i].pos.y -= height;
        }
    }

    void DrawAgents()
    {
        for (int i = 0; i < agents.Length; i++)
        {
            // get pixel coords from agent position with flooring since pixel coords are integers
            int x = Mathf.FloorToInt(agents[i].pos.x);
            int y = Mathf.FloorToInt(agents[i].pos.y);

            int index = y * width + x;  // getting the index of the pixel in the pixels array. Note: we multiply y by width since pixels are stored row by row.
            if (index >= 0 && index < pixels.Length)
            {
                pixels[index] = Color.white;    // set the pixel to white where the agent is
            }
        }
    }
}
