using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Linq;
using System.Collections.Specialized;
using Debug = UnityEngine.Debug;

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
        if (keptPerlinParameters.Count == 0) // Don't want to keep any
        {
            keptPerlinParameters.Add(perlinParameters[0]); // Add at least 1
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