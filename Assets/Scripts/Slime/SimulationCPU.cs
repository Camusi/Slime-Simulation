using UnityEngine;

public class SimulationCPU : MonoBehaviour
{
    // simulation parameters
    [Header("Simulation")]
    public int width = 1280;
    public int height = 720;
    public int numAgents = 2000;
    [Range(0.8f, 1f)] public float decay = 0.985f;

    public SpeciesSettingsSO[] species;

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

        public Color color;
        public float speed;
        public float sensorAngle;
        public float sensorDistance;
        public int sensorRadius;
        public float turnStrength;
        public float randomTurnStrength;
        public bool repel;
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
        agents = new Agent[numAgents];

        Vector2 center = new Vector2(width/2, height/2);
        float spawnRadius = Mathf.Min(width, height) * 0.25f;

        for (int i = 0; i < agents.Length; i++)
        {
            float angle = Random.value * 2f * Mathf.PI;
            float radius = Random.value * spawnRadius;
            
            agents[i].pos = (new Vector2(Mathf.Cos(angle), Mathf.Sin(angle)) * radius) + center;
            agents[i].angle = angle;

            var slot = species[i % species.Length]; // cycle through slots

            agents[i].color = slot.color;
            agents[i].speed = slot.speed;
            agents[i].sensorAngle = slot.sensorAngle;
            agents[i].sensorDistance = slot.sensorDistance;
            agents[i].sensorRadius = slot.sensorRadius;
            agents[i].turnStrength = slot.turnStrength;
            agents[i].randomTurnStrength = slot.randomTurnStrength;
            agents[i].repel = slot.repel;
        }
    }

    void Update()
    {
        FadePixels();
        MoveAgents();
        DrawAgents();

        texture.SetPixels(pixels);
        texture.Apply(false);   // false is saying no mipmaps which is pre-computed, smaller versions of a texture that the GPU uses when an object is far away
    }

    void FadePixels()
    {
        for (int i = 0; i < pixels.Length; i++)
        {
            pixels[i].r *= decay;
            pixels[i].g *= decay;
            pixels[i].b *= decay;
        }
    }

    // returns an average brightness of the pixels in a circular area around the given position
    float SampleSensor(Vector2 pos, int sensorRadius, Color speciesColor, bool repel)
    {
        float sum = 0;

        for (int offsetX = -sensorRadius; offsetX < sensorRadius; offsetX++)
        {
            for (int offsetY = -sensorRadius; offsetY < sensorRadius; offsetY++)
            {
                if (offsetX * offsetX + offsetY * offsetY <= sensorRadius * sensorRadius)
                {
                    int pixelX = (int)pos.x + offsetX;
                    int pixelY = (int)pos.y + offsetY;

                    // wrap
                    if (pixelX < 0) pixelX += width;
                    if (pixelX >= width) pixelX -= width;
                    if (pixelY < 0) pixelY += height;
                    if (pixelY >= height) pixelY -= height;

                    int pixelIndex = pixelY * width + pixelX;

                    if (repel)
                    {
                        if (pixels[pixelIndex] == speciesColor) 
                            sum += 1f;
                        else
                            sum -= 1f;
                    }
                    else
                    {
                        sum += pixels[pixelIndex].r + pixels[pixelIndex].g + pixels[pixelIndex].b;
                    }
                }
            }
        }
        return sum;
    }

    void MoveAgents()
    {
        for (int i = 0; i < agents.Length; i++)
        {
            Agent currentAgent = agents[i];

            Vector2 leftSensor = currentAgent.pos + new Vector2(Mathf.Cos(currentAgent.angle - currentAgent.sensorAngle), Mathf.Sin(currentAgent.angle - currentAgent.sensorAngle)) * currentAgent.sensorDistance;
            Vector2 forwardSensor = currentAgent.pos + new Vector2(Mathf.Cos(currentAgent.angle), Mathf.Sin(currentAgent.angle)) * currentAgent.sensorDistance;
            Vector2 rightSensor = currentAgent.pos + new Vector2(Mathf.Cos(currentAgent.angle + currentAgent.sensorAngle), Mathf.Sin(currentAgent.angle + currentAgent.sensorAngle)) * currentAgent.sensorDistance;

            float leftVal = SampleSensor(leftSensor, currentAgent.sensorRadius, currentAgent.color, currentAgent.repel);
            float forwardVal = SampleSensor(forwardSensor, currentAgent.sensorRadius, currentAgent.color, currentAgent.repel);
            float rightVal = SampleSensor(rightSensor, currentAgent.sensorRadius, currentAgent.color, currentAgent.repel);

            if (leftVal > rightVal && leftVal > forwardVal)
            {
                currentAgent.angle -= currentAgent.turnStrength * Time.deltaTime;
            } 
            else if (rightVal > leftVal && rightVal > forwardVal)
            {
                currentAgent.angle += currentAgent.turnStrength * Time.deltaTime;
            }

            currentAgent.angle += Random.Range(-currentAgent.randomTurnStrength, currentAgent.randomTurnStrength);

            Vector2 dir_vec = new Vector2(Mathf.Cos(currentAgent.angle), Mathf.Sin(currentAgent.angle));  // Angle to vector formula
            currentAgent.pos += dir_vec * currentAgent.speed * Time.deltaTime;  // d = vt. d = dir_vec * speed, t = Time.deltaTime (time since last frame)

            // wrap around
            if (currentAgent.pos.x < 0) {currentAgent.pos.x += width;}
            if (currentAgent.pos.x >= width) {currentAgent.pos.x -= width;}
            if (currentAgent.pos.y < 0) {currentAgent.pos.y += height;}
            if (currentAgent.pos.y >= height) {currentAgent.pos.y -= height;}

            agents[i] = currentAgent;
        }
    }

    void DrawAgents()
    {
        for (int i = 0; i < agents.Length; i++)
        {
            int x = Mathf.FloorToInt(agents[i].pos.x);
            int y = Mathf.FloorToInt(agents[i].pos.y);

            int pixel_index = y * width + x;
            pixels[pixel_index] = agents[i].color;
        }
    }

}