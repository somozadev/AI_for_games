using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Linq;
using System.Collections.Specialized;
using Debug = UnityEngine.Debug;
using System.ComponentModel.Design;

[ExecuteInEditMode]

public class CustomTerrain : MonoBehaviour
{
    
    public Vector2 randomHeightRange = new Vector2 (0, 0.1f);
    public Texture2D heightMapImage;
    public Vector3 heightMapScale = new Vector3(1, 1, 1);

    public bool resetTerrain = true;

    //Perlin Noise #######
    public float perlinXScale = 0.001f;
    public float perlinYScale = 0.001f;
    public int perlinXOffset = 0;
    public int perlinYOffset = 0;
    public int perlinOctaves = 3;
    public float perlinPersistance = 8;
    public float perlinHeightScale = 0.09f;

    // SplatMaps #######    
    [System.Serializable]
    public class SplatHeights
    {
        public Texture2D texture = null;
        public Texture2D textureNormalMap = null;
        //public float tilesize;
        public float minHeight = 0.1f;
        public float maxHeight = 0.2f;
        public float splatOffset = 000000.10f;
        public float splatNoiseXScale = 0.01f;
        public float splatNoiseYScale = 0.01f;
        public float splatNoiseZScale = 0.10f;
        public float minSlope = 0.0f;
        public float maxSlope = 90.0f;

        public Vector2 tileOffset = Vector2.zero;
        public Vector2 tileSize = new Vector2(50.0f, 50.0f);
        public bool remove = false;
    }

    public List<SplatHeights> splatHeights = new List<SplatHeights>()
    {
        new SplatHeights()

    };

    public void SplatMaps()
    {

        int tah = terrainData.alphamapHeight;
        int taw = terrainData.alphamapWidth;
        int aml = terrainData.alphamapLayers;

        TerrainLayer[] newSplatPrototypes;

        newSplatPrototypes = new TerrainLayer[splatHeights.Count];
        int spIndex = 0;

        foreach (SplatHeights sh in splatHeights)
        {

            newSplatPrototypes[spIndex] = new TerrainLayer
            {

                diffuseTexture = sh.texture,
                normalMapTexture = sh.textureNormalMap,
                tileOffset = sh.tileOffset,
                tileSize = sh.tileSize
            };

            newSplatPrototypes[spIndex].diffuseTexture.Apply(true);
            string path = "Assets/New Terrain Layer " + spIndex + ".terrainlayer";
            AssetDatabase.CreateAsset(newSplatPrototypes[spIndex], path);
            spIndex++;
            Selection.activeObject = this.gameObject;
        }
        terrainData.terrainLayers = newSplatPrototypes;

        float[,] heightMap = terrainData.GetHeights(0, 0, hmres, hmres);

        float[,,] splatmapData = new float[taw, tah, aml];

        for (int y = 0; y < tah; ++y)
        {

            for (int x = 0; x < taw; ++x)
            {

                float[] splat = new float[aml];
                bool emptySplat = true;

                for (int i = 0; i < splatHeights.Count; ++i)
                {

                    float noise = Mathf.PerlinNoise(x * splatHeights[i].splatNoiseXScale,
                                                    y * splatHeights[i].splatNoiseYScale) *
                                                    splatHeights[i].splatNoiseZScale;

                    float offset = splatHeights[i].splatOffset + noise;
                    float thisHeightStart = splatHeights[i].minHeight - offset;
                    float thisHeightStop = splatHeights[i].maxHeight + offset;

                    //SCALE FOR RESOLUTION DIFFERENCES
                    //Scale between the heightmap resolution and the splatmap resolution
                    int hmX = x * ((hmres - 1) / taw);
                    int hmY = y * ((hmres - 1) / tah);

                    float normX = x * 1.0f / (terrainData.alphamapWidth - 1);
                    float normY = y * 1.0f / (terrainData.alphamapHeight - 1);

                    var steepness = terrainData.GetSteepness(normX, normY);

                    if ((heightMap[hmX, hmY] >= thisHeightStart && heightMap[hmX, hmY] <= thisHeightStop) &&
                         (steepness >= splatHeights[i].minSlope && steepness <= splatHeights[i].maxSlope))
                    {

                        if (heightMap[hmX, hmY] <= splatHeights[i].minHeight)
                            splat[i] = 1 - Mathf.Abs(heightMap[hmX, hmY] - splatHeights[i].minHeight) / offset;
                        else if (heightMap[hmX, hmY] >= splatHeights[i].maxHeight)
                            splat[i] = 1 - Mathf.Abs(heightMap[hmX, hmY] - splatHeights[i].maxHeight) / offset;
                        else
                            splat[i] = 1;
                        emptySplat = false;
                    }
                }

                NormalizeVector(ref splat);

                if (emptySplat)
                {

                    splatmapData[x, y, 0] = 1;
                }
                else
                {

                    for (int j = 0; j < splatHeights.Count; j++)
                    {
                        splatmapData[x, y, j] = splat[j];
                    }
                }
            }
        }
        terrainData.SetAlphamaps(0, 0, splatmapData);
    }

    void NormalizeVector(ref float[] v)
    {

        float total = 0.0f;
        for (int i = 0; i < v.Length; ++i)
        {

            total += v[i];
        }
        if (total == 0) return;

        for (int i = 0; i < v.Length; ++i)
        {

            v[i] /= total;
        }
    }

    public void AddNewSplatHeights()
        {
            splatHeights.Add(new SplatHeights());
        }

        public void RemoveSplatHeights()
        {
            List<SplatHeights> keptSplatHeights = new List<SplatHeights>();
            for (int i = 0; i < splatHeights.Count; i++)
            {
                if (!splatHeights[i].remove)
                {
                    keptSplatHeights.Add(splatHeights[i]);
                }
            }
            if (keptSplatHeights.Count == 0)
            {
                keptSplatHeights.Add(splatHeights[0]); 
            }
            splatHeights = keptSplatHeights;
        }

    

    //Multiple Perlin Noise #######
    [System.Serializable]
    public class PerlinParameters
    {
        public float mPerlinXScale = 0.01f;
        public float mPerlinYScale = 0.01f;
        public int mPerlinOctaves = 3;
        public float mPerlinPersistance = 8;
        public float mPerlinHeightScale = 0.09f;
        public int mPerlinOffsetX = 0;
        public int mPerlinOffsetY = 0;
        public bool remove = false;
    }

    public List<PerlinParameters> perlinParameters = new List<PerlinParameters>()
    {
    new PerlinParameters()
    };

    //VORONOI #######
    public float voronoiFallOff = 0.2f;
    public float voronoiDropOff = 0.6f;
    public float voronoiMinHeight = 0.1f;
    public float voronoiMaxHeight = 0.5f;
    public int voronoiPeaks = 5;
    public enum VoronoiType { Linear = 0, Power = 1, Combined = 2, Blob = 3, Perlin = 4 };
    public VoronoiType voronoiType = VoronoiType.Linear;



    // Midpoint Displacement ########
    public float MPDHeightMin = -2.0f;
    public float MPDHeightMax = 2.0f;
    public float MPDHeightDampner = 2.0f;
    public float MPDRoughness = 2.0f;

    public int smoothAmount = 1;

    public Terrain terrain;
    public TerrainData terrainData;

    int hmres { get {  return terrainData.heightmapResolution; } }


    float[,] GetHeightMap()
    {
        if (!resetTerrain)
        {
            return terrainData.GetHeights(0, 0, hmres, hmres);
        }
        else
        {
            return new float[hmres, hmres];
        }
    }

    /* public void Voronoi()
      {
          float[,] heightMap = GetHeightMap();
          float falloff = 1.8f;
          float dropOff = 0.5f;
          Vector3 peak = new Vector3(256, 0.2f, 256);
          // Vector3 peak = new Vector3(UnityEngine.Random.Range(0, hmres),
           //   UnityEngine.Random.Range(0.0f, 1.0f),
           //   UnityEngine.Random.Range(0, hmres));

          heightMap[(int)peak.x, (int)peak.z] = peak.y;

          // Store the peak's location in a Vector2 for easier distance calculations
          Vector2 peakLocation = new Vector2(peak.x, peak.z);

          // Calculate the maximum possible distance within the heightmap
          float maxDistance = Vector2.Distance(
              new Vector2(0, 0),
              new Vector2(hmres, hmres)
          );
          // Iterate through each point in the heightmap
          for (int y = 0; y < hmres; y++)
          {
              for (int x = 0; x < hmres; x++)
              {
                 if (!(x == peak.x && y == peak.z))
                 {

                  // Calculate the distance from the current point to the peak
                 // float distanceToPeak = Vector2.Distance(peakLocation, new Vector2(x, y)) * falloff;
                  // Calculate the height based on the distance to the peak
                  //heightMap[x, y] = peak.y - (distanceToPeak / maxDistance); // Adjust height based on distance 

                      // Calculate Gradual slope from the current point to the peak
                 float distanceToPeak = Vector2.Distance(peakLocation, new Vector2(x, y)) / maxDistance;
                      // float h = peak.y - distanceToPeak * falloff - Mathf.Pow(distanceToPeak, dropOff);
                      // float h = peak.y - Mathf.Sin(distanceToPeak * Mathf.PI) * falloff;
                      float h = peak.y - Mathf.Sin(distanceToPeak * 100); 
                      heightMap[x, y] = h;
                  }
              }
          }
          terrainData.SetHeights(0, 0, heightMap);
      } */

    public void Voronoi()
    {

        float[,] heightMap = GetHeightMap();

        for (int p = 0; p < voronoiPeaks; ++p)
        {

            Vector3 peak = new Vector3(UnityEngine.Random.Range(0, hmres),
                                       UnityEngine.Random.Range(voronoiMinHeight, voronoiMaxHeight),
                                       UnityEngine.Random.Range(0, hmres));

            if (heightMap[(int)peak.x, (int)peak.z] < peak.y)
            {

                heightMap[(int)peak.x, (int)peak.z] = peak.y;
            }
            else
            {

                continue;
            }

            Vector2 peakLocation = new Vector2(peak.x, peak.z);
            float maxDistance = Vector2.Distance(new Vector2(0.0f, 0.0f), new Vector2(hmres, hmres));

            for (int y = 0; y < hmres; ++y)
            {

                for (int x = 0; x < hmres; ++x)
                {

                    if (!(x == peak.x && y == peak.z))
                    {

                        float distanceToPeak = Vector2.Distance(peakLocation, new Vector2(x, y)) / maxDistance;
                        float h;  

                        if (voronoiType == VoronoiType.Combined)
                        {
                            h = peak.y - distanceToPeak * voronoiFallOff - Mathf.Pow(distanceToPeak, voronoiDropOff); //combined
                        }
                        else if (voronoiType == VoronoiType.Power)
                        {
                            h = peak.y - Mathf.Pow(distanceToPeak, voronoiDropOff) * voronoiFallOff; //power
                        }
                        else if (voronoiType == VoronoiType.Blob)
                        {
                            // h = peak.y - Mathf.Sin(distanceToPeak) - Mathf.Pow(peakLocation,voronoiFallOff) -  Mathf.Sin(peakLocation * 2 * Mathf.PI)/voronoiDropOff; //blob
                            h = peak.y - Mathf.Pow(distanceToPeak * 3, voronoiFallOff) - Mathf.Sin(distanceToPeak * 2 * Mathf.PI) / voronoiDropOff;
                        }
                        else if (voronoiType == VoronoiType.Perlin)
                        {
                            h = (peak.y - distanceToPeak * voronoiFallOff) + Utils.fBM((x + perlinXOffset) * perlinXScale,
                                             (y + perlinYOffset) * perlinYScale,
                                             perlinOctaves,
                                             perlinPersistance) * perlinHeightScale; ; //Perlin
                        }
                        else
                        {
                            h = peak.y - distanceToPeak * voronoiFallOff; //linear
                        }

                        if (heightMap[x, y] < h)
                        {

                            heightMap[x, y] = h;
                        }
                    }
                }
            }
        }

        terrainData.SetHeights(0, 0, heightMap);
    }

    public void Perlin()
    {
        float[,] heightMap = GetHeightMap();

        for (int y = 0; y < hmres; y++)
        {
            for (int x = 0; x < hmres; x++)
            {
                heightMap[x, y] += Utils.fBM( (x + perlinXOffset) * perlinXScale,
                                             (y + perlinYOffset) * perlinYScale,
                                             perlinOctaves,
                                             perlinPersistance) * perlinHeightScale;
            }
        }

        terrainData.SetHeights(0, 0, heightMap);
    }

    public void MultiplePerlinTerrain()
    {
        float[,] heightMap = GetHeightMap();

        for (int y = 0; y < hmres; ++y)
        {
            for (int x = 0; x < hmres; ++x)
            {
                foreach (PerlinParameters p in perlinParameters)
                {
                    heightMap[x, y] += Utils.fBM((x + p.mPerlinOffsetX) * p.mPerlinXScale,
                                                (y + p.mPerlinOffsetY) * p.mPerlinYScale,
                                                p.mPerlinOctaves,
                                                p.mPerlinPersistance) * p.mPerlinHeightScale;
                }
            }
        }

        terrainData.SetHeights(0, 0, heightMap);
    }

    public void MidPointDisplacement()
    {

        float[,] heightMap = GetHeightMap();
        int width = hmres - 1;
        int squareSize = width;
        float heightMin = MPDHeightMin;
        float heightMax = MPDHeightMax;
        float heightDampener = (float)Mathf.Pow(MPDHeightDampner, -1 * MPDRoughness);

        int cornerX, cornerY;
        int midX, midY;
        int pmidXL, pmidXR, pmidYU, pmidYD;

        //heightMap[0, 0] = UnityEngine.Random.Range(0.0f, 0.2f);
        //heightMap[0, hmres - 2] = UnityEngine.Random.Range(0.0f, 0.2f);
        //heightMap[hmres - 2, 0] = UnityEngine.Random.Range(0.0f, 0.2f);
        //heightMap[hmres - 2, hmres - 2] = UnityEngine.Random.Range(0.0f, 0.2f);

        while (squareSize > 0)
        {

            for (int x = 0; x < width; x += squareSize)
            {

                for (int y = 0; y < width; y += squareSize)
                {

                    cornerX = (x + squareSize);
                    cornerY = (y + squareSize);

                    midX = (int)(x + squareSize / 2.0f);
                    midY = (int)(y + squareSize / 2.0f);

                    heightMap[midX, midY] = (float)((heightMap[x, y] +
                                                     heightMap[cornerX, y] +
                                                     heightMap[x, cornerY] +
                                                     heightMap[cornerX, cornerY]) / 4.0f +
                                                     UnityEngine.Random.Range(heightMin, heightMax));
                }
            }

            for (int x = 0; x < width; x += squareSize)
            {

                for (int y = 0; y < width; y += squareSize)
                {

                    cornerX = (x + squareSize);
                    cornerY = (y + squareSize);

                    midX = (int)(x + squareSize / 2.0f);
                    midY = (int)(y + squareSize / 2.0f);

                    pmidXR = (int)(midX + squareSize);
                    pmidYU = (int)(midY + squareSize);
                    pmidXL = (int)(midX - squareSize);
                    pmidYD = (int)(midY - squareSize);

                    if (pmidXL <= 0 || pmidYD <= 0
                        || pmidXR >= width - 1 || pmidYU >= width - 1) continue;

                    // Calculate the square value for the bottom right
                    heightMap[midX, y] = (float)((heightMap[midX, midY] +
                                                  heightMap[x, y] +
                                                  heightMap[midX, pmidYD] +
                                                  heightMap[cornerX, y]) / 4.0f +
                                                  UnityEngine.Random.Range(heightMin, heightMax));

                    // Calculate the square value for the top side
                    heightMap[midX, cornerY] = (float)((heightMap[x, cornerY] +
                                                  heightMap[midX, midY] +
                                                  heightMap[cornerX, cornerY] +
                                                  heightMap[midX, pmidYU]) / 4.0f +
                                                  UnityEngine.Random.Range(heightMin, heightMax));

                    // Calculate the square value for the left side
                    heightMap[x, midY] = (float)((heightMap[x, y] +
                                                  heightMap[pmidXL, midY] +
                                                  heightMap[x, cornerY] +
                                                  heightMap[midX, midY]) / 4.0f +
                                                  UnityEngine.Random.Range(heightMin, heightMax));

                    // Calculate the square value for the right side
                    heightMap[cornerX, midY] = (float)((heightMap[midX, y] +
                                                  heightMap[midX, midY] +
                                                  heightMap[cornerX, cornerY] +
                                                  heightMap[pmidXR, midY]) / 4.0f +
                                                  UnityEngine.Random.Range(heightMin, heightMax));
                }
            }

            squareSize = (int)(squareSize / 2.0f);
            heightMin *= heightDampener;
            heightMax *= heightDampener;
        }
        terrainData.SetHeights(0, 0, heightMap);
    }

    List<Vector2> GenerateNeighbours(Vector2 pos, int width, int height)
    {

        List<Vector2> neighbours = new List<Vector2>();

        for (int y = -1; y < 2; ++y)
        {

            for (int x = -1; x < 2; ++x)
            {

                if (!(x == 0 && y == 0))
                {

                    Vector2 nPos = new Vector2(
                        Mathf.Clamp(pos.x + x, 0.0f, width - 1),
                        Mathf.Clamp(pos.y + y, 0.0f, height - 1));

                    if (!neighbours.Contains(nPos))
                        neighbours.Add(nPos);
                }
            }
        }
        return neighbours;
    }

    public void Smooth()
    {

        float[,] heightMap = terrainData.GetHeights(0, 0, hmres, hmres);
        float smoothProgress = 0.0f;
        EditorUtility.DisplayProgressBar("Smoothing Terrain", "Progress", smoothProgress);

        for (int s = 0; s < smoothAmount; ++s)
        {

            for (int y = 0; y < hmres; ++y)
            {

                for (int x = 0; x < hmres; ++x)
                {

                    float avgHeight = heightMap[x, y];
                    List<Vector2> neighbours = GenerateNeighbours(new Vector2(x, y), hmres, hmres);

                    foreach (Vector2 n in neighbours)
                    {

                        avgHeight += heightMap[(int)n.x, (int)n.y];
                    }
                    heightMap[x, y] = avgHeight / ((float)neighbours.Count + 1);
                }
            }
            smoothProgress++;
            EditorUtility.DisplayProgressBar("Smoothin Terrain", "Progress", smoothProgress / smoothAmount);
        }
        terrainData.SetHeights(0, 0, heightMap);
        EditorUtility.ClearProgressBar();
    }

    public void RidgeTerrain()
        {
            ResetTerrain();
            MultiplePerlinTerrain();
            float[,] heightMap = GetHeightMap();
            for (int y = 0; y < hmres; ++y)
            {
               for (int x = 0; x < hmres; ++x)
               {
                        heightMap[x, y] = 1 - Mathf.Abs(heightMap[x, y] - 0.5f);
                  }
            }
            terrainData.SetHeights(0, 0, heightMap);
        }


    public void AddNewPerlin()
    {
        perlinParameters.Add(new PerlinParameters());
    }

    public void RemovePerlin()
    {
        List<PerlinParameters> keptPerlinParameters = new List<PerlinParameters>();
        for (int i = 0; i < perlinParameters.Count; i++)
        {
            if (!perlinParameters[i].remove)
            {
                keptPerlinParameters.Add(perlinParameters[i]);
            }
        }
        if (keptPerlinParameters.Count == 0) 
        {
            keptPerlinParameters.Add(perlinParameters[0]); 
        }
        perlinParameters = keptPerlinParameters;
    }



    public void RandomTerrain()
    {
        float[,] heightMap = GetHeightMap();
        for (int x = 0; x < hmres; ++x)
        {
            for (int z = 0; z < hmres; ++z)
            {
                heightMap[x, z] += UnityEngine.Random.Range(randomHeightRange.x, randomHeightRange.y);
            }
        }
        terrainData.SetHeights(0, 0, heightMap);
    }


    public void LoadTextureAddHeightd()
    {
        float[,] heightMap = GetHeightMap();
        //heightMap = new float[hmres, hmres];
        for (int x = 0; x < hmres; ++x)
        {
            for(int z = 0;z < hmres; ++z)
            {        
              heightMap[x, z] += heightMapImage.GetPixel((int)(x * heightMapScale.x), (int)(z * heightMapScale.z)).grayscale * heightMapScale.y;           
            }

        }
        terrainData.SetHeights(0, 0, heightMap);
    }

    public void LoadTexture()
    {
        float[,] heightMap = GetHeightMap();
        heightMap = new float[hmres, hmres];
        for (int x = 0; x < hmres; ++x)
        {
            for (int z = 0; z < hmres; ++z)
            {
                heightMap[x, z] = heightMapImage.GetPixel((int)(x * heightMapScale.x), (int)(z * heightMapScale.z)).grayscale * heightMapScale.y;
            }

        }
        terrainData.SetHeights(0, 0, heightMap);
    }

    public void ResetTerrain()
    {
        float[,] heightMap = new float[hmres, hmres];
        /* for (int x = 0; x < hmres; ++x)
         {
             for (int z = 0; z < hmres; ++z)
             {
                 heightMap[x, z] = 0;
             }
         }*/
        terrainData.SetHeights(0, 0, heightMap);
    }

    void OnEnable()
    {
        Debug.Log("Initialising Terrain Data");
        terrain = this.GetComponent<Terrain>();
        terrainData = Terrain.activeTerrain.terrainData;
    }

    // Start is called before the first frame update
    void Start()
    {
        SerializedObject tagManager = new SerializedObject(AssetDatabase.LoadAllAssetsAtPath("ProjectSettings/TagManager.asset")[0]);

        SerializedProperty tagsProp = tagManager.FindProperty("tags");

        AddTag(tagsProp, "Terrain");
        AddTag(tagsProp, "Cloud");
        AddTag(tagsProp, "Shore");

        tagManager.ApplyModifiedProperties();
        this.gameObject.tag = "Terrain";
    }

    void AddTag(SerializedProperty tagsProp, string newTag)
    {
        bool found = false;
        for (int i = 0; i < tagsProp.arraySize; i++)
        {
            SerializedProperty t = tagsProp.GetArrayElementAtIndex(i);
            if (t.stringValue.Equals(newTag))
            {
                found = true; break;
            }
        }

        if (!found)
        {
            tagsProp.InsertArrayElementAtIndex(0);
            SerializedProperty newTagProp = tagsProp.GetArrayElementAtIndex(0);
            newTagProp.stringValue = newTag;
        }

    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
