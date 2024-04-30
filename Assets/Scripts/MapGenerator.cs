using UnityEngine;

public class MapGenerator : MonoBehaviour
{
    [SerializeField] private enum DrawMode { NoiseMap, ColourMap };
    [SerializeField] private DrawMode _drawMode;

    [SerializeField] private int _mapWidth;
    [SerializeField] private int _mapHeight;
    [SerializeField] private float _noiseScale;

    [SerializeField] private int _octaves;

    [Range(0, 1)]
    [SerializeField] private float _persistance;
    [SerializeField] private float _lacunarity;

    [SerializeField] private int _seed;
    [SerializeField] private Vector2 _offset;

    [SerializeField] private TerrainType[] _regions;

    private void Start()
    {
        GenerateRandomSeed();
    }

    public void GenerateMap()
    {
        float[,] noiseMap = Noise.GenerateNoiseMap(_mapWidth, _mapHeight, _seed, _noiseScale, _octaves, _persistance, _lacunarity, _offset);

        Color[] colourMap = new Color[_mapWidth * _mapHeight];
        for (int y = 0; y < _mapHeight; y++)
        {
            for (int x = 0; x < _mapWidth; x++)
            {
                float currentHeight = noiseMap[x, y];
                for (int i = 0; i < _regions.Length; i++)
                {
                    if (currentHeight <= _regions[i].height)
                    {
                        colourMap[y * _mapWidth + x] = _regions[i].colour;
                        break;
                    }
                }
            }
        }

        MapDisplay display = FindObjectOfType<MapDisplay>();
        if (_drawMode == DrawMode.NoiseMap)
        {
            display.DrawTexture(TextureGenerator.TextureFromHeightMap(noiseMap));
        }
        else if (_drawMode == DrawMode.ColourMap)
        {
            display.DrawTexture(TextureGenerator.TextureFromColourMap(colourMap, _mapWidth, _mapHeight));
        }
    }

    public void GenerateRandomSeed()
    {
        _seed = Random.Range(1, 10000000);
        GenerateMap();
    }

    [System.Serializable]
    public struct TerrainType
    {
        public string name;
        public float height;
        public Color colour;
    }
}
