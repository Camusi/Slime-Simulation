using UnityEngine;

public enum Species
{
    Blue,
    Red,
    Green
}

[CreateAssetMenu(fileName = "NewSpeciesSettings", menuName = "Slime/Species Settings")]
public class SpeciesSettingsSO : ScriptableObject
{
    public Color color;
    public float speed;
    public float sensorAngle;
    public int sensorRadius;
    public float sensorDistance;
    public float turnStrength;
    public float randomTurnStrength;
    public bool repel;
}
