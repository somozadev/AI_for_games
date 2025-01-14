using UnityEditor;
using UnityEngine;
using EditorGUITable;

[CustomEditor(typeof(CustomTerrain))]
[CanEditMultipleObjects]
public class CustomTerrainEditor : Editor
{
    //properties
    SerializedProperty randomHeightRange;
    SerializedProperty heightMapScale;
    SerializedProperty heightMapImage;
    SerializedProperty perlinXScale;
    SerializedProperty perlinYScale;
    SerializedProperty perlinXOffset;
    SerializedProperty perlinYOffset;
    SerializedProperty perlinOctaves;
    SerializedProperty perlinPersistance;
    SerializedProperty perlinHeightScale;
    SerializedProperty resetTerrain;
    SerializedProperty voronoiFallOff;
    SerializedProperty voronoiDropOff;
    SerializedProperty voronoiMinHeight;
    SerializedProperty voronoiMaxHeight;
    SerializedProperty voronoiPeaks;
    SerializedProperty voronoiType;
    SerializedProperty MPDHeightMin;
    SerializedProperty MPDHeightMax;
    SerializedProperty MPDHeightDampner;
    SerializedProperty MPDRoughness;
    SerializedProperty smoothAmount;


    GUITableState perlinParameterTable;
    SerializedProperty perlinParameters;

    GUITableState splatMapTable;
    SerializedProperty splatHeights;

    GUITableState vegMapTable;
    SerializedProperty vegetation;
    SerializedProperty maxTrees;
    SerializedProperty treeSpacing;

    GUITableState detailMapTable;
    SerializedProperty details;
    SerializedProperty maxDetails;
    SerializedProperty detailSpacing;

    SerializedProperty waterHeight;
    SerializedProperty waterGO;

    SerializedProperty erosionType;
    SerializedProperty erosionStrength;
    SerializedProperty erosionAmount;
    SerializedProperty springsPerRiver;
    SerializedProperty solubilty;
    SerializedProperty droplets;
    SerializedProperty erosionSmoothAmount;


    //fold outs Hide 
    bool showRandom = false;
    bool showLoadHeights = false;
    bool showPerlinNoise = false;
    bool showMultiplePerlin = false;
    bool showVoronoi = false;
    bool showMPD = false;
    bool showSmooth = false;
    bool showSplatMaps = false;
    bool showVeg = false;
    bool showDetails = false;
    bool showWater = false;
    bool showErosion = false;

    private void OnEnable()
    {
        randomHeightRange = serializedObject.FindProperty("randomHeightRange");
        heightMapScale = serializedObject.FindProperty("heightMapScale");
        heightMapImage = serializedObject.FindProperty("heightMapImage");
        perlinXScale = serializedObject.FindProperty("perlinXScale");
        perlinYScale = serializedObject.FindProperty("perlinYScale");
        perlinXOffset = serializedObject.FindProperty("perlinXOffset");
        perlinYOffset = serializedObject.FindProperty("perlinYOffset");
        perlinOctaves = serializedObject.FindProperty("perlinOctaves");
        perlinPersistance = serializedObject.FindProperty("perlinPersistance");
        perlinHeightScale = serializedObject.FindProperty("perlinHeightScale");
        resetTerrain = serializedObject.FindProperty("resetTerrain");
        perlinParameterTable = new GUITableState("perlinParameterTable");
        perlinParameters = serializedObject.FindProperty("perlinParameters");
        voronoiDropOff = serializedObject.FindProperty("voronoiDropOff");
        voronoiFallOff = serializedObject.FindProperty("voronoiFallOff");
        voronoiMinHeight = serializedObject.FindProperty("voronoiMinHeight");
        voronoiMaxHeight = serializedObject.FindProperty("voronoiMaxHeight");
        voronoiPeaks = serializedObject.FindProperty("voronoiPeaks");
        voronoiType = serializedObject.FindProperty("voronoiType");
        MPDHeightMin = serializedObject.FindProperty("MPDHeightMin");
        MPDHeightMax = serializedObject.FindProperty("MPDHeightMax");
        MPDHeightDampner = serializedObject.FindProperty("MPDHeightDampner");
        MPDRoughness = serializedObject.FindProperty("MPDRoughness");
        smoothAmount = serializedObject.FindProperty("smoothAmount");
        splatMapTable = new GUITableState("splatMapTable");
        splatHeights = serializedObject.FindProperty("splatHeights");
        vegMapTable = new GUITableState("vegMapTable");
        vegetation = serializedObject.FindProperty("vegetation");
        maxTrees = serializedObject.FindProperty("maxTrees");
        treeSpacing = serializedObject.FindProperty("treeSpacing");

        detailMapTable = new GUITableState("detailMapTable");
        details = serializedObject.FindProperty("details");
        maxDetails = serializedObject.FindProperty("maxDetails");
        detailSpacing = serializedObject.FindProperty("detailSpacing");

        waterHeight = serializedObject.FindProperty("waterHeight");
        waterGO = serializedObject.FindProperty("waterGO");

        erosionType = serializedObject.FindProperty("erosionType");
        erosionStrength = serializedObject.FindProperty("erosionStrength");
        erosionAmount = serializedObject.FindProperty("erosionAmount");
        springsPerRiver = serializedObject.FindProperty("springsPerRiver");
        solubilty = serializedObject.FindProperty("solubilty");
        droplets = serializedObject.FindProperty("droplets");
        erosionSmoothAmount = serializedObject.FindProperty("erosionSmoothAmount");

    }
    Vector2 scrollPos;
    public override void OnInspectorGUI()
    {
        serializedObject.Update();
        CustomTerrain terrain = (CustomTerrain) target;

        Rect r = EditorGUILayout.BeginVertical();
        scrollPos = EditorGUILayout.BeginScrollView(scrollPos, GUILayout.Width(r.width), GUILayout.Height(r.height));
        EditorGUI.indentLevel++;

        EditorGUILayout.PropertyField(resetTerrain);

        showRandom = EditorGUILayout.Foldout(showRandom, "Random");
        if (showRandom)
        {
            EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);
            GUILayout.Label("Set Heights Between Random Values", EditorStyles.boldLabel);
            EditorGUILayout.PropertyField(randomHeightRange);
            if (GUILayout.Button("Random Heights"))
            {
                terrain.RandomTerrain();
            }
        }

        showLoadHeights = EditorGUILayout.Foldout(showLoadHeights, "Load Heights");
        if (showLoadHeights)
        {
            EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);
            GUILayout.Label("Load Heights From Texture", EditorStyles.boldLabel);
            EditorGUILayout.PropertyField(heightMapImage);
            EditorGUILayout.PropertyField(heightMapScale);
            if (GUILayout.Button("Load Texture"))
            {
                terrain.LoadTexture();
            }
        }


        showPerlinNoise = EditorGUILayout.Foldout(showPerlinNoise, "Single Perlin Noise");
        if (showPerlinNoise)
        {
            EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);
            GUILayout.Label("Perlin Noise", EditorStyles.boldLabel);
            EditorGUILayout.Slider(perlinXScale, 0, 1, new GUIContent("X Scale"));
            EditorGUILayout.Slider(perlinYScale, 0, 1, new GUIContent("Y Scale"));
            EditorGUILayout.IntSlider(perlinXOffset, 0, 10000, new GUIContent("X Offset"));
            EditorGUILayout.IntSlider(perlinYOffset, 0, 10000, new GUIContent("Y Offset"));
            EditorGUILayout.IntSlider(perlinOctaves, 1, 10, new GUIContent("Octaves"));
            EditorGUILayout.Slider(perlinPersistance, 0.1f, 10, new GUIContent("Persistance"));
            EditorGUILayout.Slider(perlinHeightScale, 0, 1, new GUIContent("Height Scale"));
                if (GUILayout.Button("Generate Terrain"))
            {
                terrain.Perlin();
            }
        }
        
        showMultiplePerlin = EditorGUILayout.Foldout(showMultiplePerlin, "Multiple Perlin Noise");
        if (showMultiplePerlin)
        {
            EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);
            GUILayout.Label("Multiple Perlin Noise", EditorStyles.boldLabel);
            perlinParameterTable = GUITableLayout.DrawTable(perlinParameterTable, serializedObject.FindProperty("perlinParameters"));

            //adds space between the button and the table
            GUILayout.Space(20);

            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("+"))
            {
                terrain.AddNewPerlin();
            }
            if (GUILayout.Button("-"))
            {
                terrain.RemovePerlin();
            }
            EditorGUILayout.EndHorizontal();

            if (GUILayout.Button("Apply Multiple Perlin"))
            {
                terrain.MultiplePerlinTerrain();
 

            }
            GUILayout.Space(20);
            if (GUILayout.Button("Apply Ridge Terrain"))
            {
                terrain.RidgeTerrain();
            }
        }
        showVoronoi = EditorGUILayout.Foldout(showVoronoi, "Voronoi");

        if (showVoronoi)
        {
            EditorGUILayout.IntSlider(voronoiPeaks, 1, 10, new GUIContent("Peak Count"));
            EditorGUILayout.Slider(voronoiFallOff, 0, 10, new GUIContent("Falloff"));
            EditorGUILayout.Slider(voronoiDropOff, 0, 10, new GUIContent("Dropoff"));
            EditorGUILayout.Slider(voronoiMinHeight, 0, 1, new GUIContent("Min Height"));
            EditorGUILayout.Slider(voronoiMaxHeight, 0, 1, new GUIContent("Max Height"));
            EditorGUILayout.PropertyField(voronoiType);

            if (GUILayout.Button("Voronoi"))
            {
                terrain.Voronoi();
            }
        }

        showMPD = EditorGUILayout.Foldout(showMPD, "Mid Point Displacement");
        if (showMPD)
        {

            EditorGUILayout.PropertyField(MPDHeightMin);
            EditorGUILayout.PropertyField(MPDHeightMax);
            EditorGUILayout.PropertyField(MPDHeightDampner);
            EditorGUILayout.PropertyField(MPDRoughness);

            if (GUILayout.Button("MPD"))
            {

                terrain.MidPointDisplacement();
            }
        }


        showSmooth = EditorGUILayout.Foldout(showSmooth, "Smooth Terrain");
        if (showSmooth)
        {

            EditorGUILayout.IntSlider(smoothAmount, 1, 10, new GUIContent("smoothAmount"));
            if (GUILayout.Button("Smooth"))
            {

                terrain.Smooth();
            }
        }

        showSplatMaps = EditorGUILayout.Foldout(showSplatMaps, "Splat Maps");
        if (showSplatMaps) {

            EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);
            GUILayout.Label("Splat Maps", EditorStyles.boldLabel);
            splatMapTable = GUITableLayout.DrawTable(splatMapTable, serializedObject.FindProperty("splatHeights"));

            GUILayout.Space(20);

            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("+"))
            {
                terrain.AddNewSplatHeights();
            }
            if (GUILayout.Button("-"))
            {
                terrain.RemoveSplatHeights();
            }
            EditorGUILayout.EndHorizontal();

            if (GUILayout.Button("Apply SplatMaps"))
            {
                terrain.SplatMaps();
            }
        }
       
        
        showVeg = EditorGUILayout.Foldout(showVeg, "Vegetaion");
        if (showVeg)
        {

            EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);
            GUILayout.Label("Vegetaion", EditorStyles.boldLabel);
            EditorGUILayout.IntSlider(maxTrees, 0, 10000, new GUIContent("Maximum Trees"));
            EditorGUILayout.IntSlider(treeSpacing, 2, 20, new GUIContent("Tree Spacing"));
            vegMapTable = GUITableLayout.DrawTable(vegMapTable, serializedObject.FindProperty("vegetation"));

            GUILayout.Space(20);

            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("+"))
            {

                terrain.AddNewVegetation();
            }

            if (GUILayout.Button("-"))
            {

                terrain.RemoveVegetation();
            }

            EditorGUILayout.EndHorizontal();

            if (GUILayout.Button("Apply Vegetaion"))
            {

                terrain.PlantVegetation();
            }
        }

        showDetails = EditorGUILayout.Foldout(showDetails, "Details");
        if (showDetails)
        {

            EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);
            GUILayout.Label("Details", EditorStyles.boldLabel);
            EditorGUILayout.IntSlider(maxDetails, 0, 10000, new GUIContent("Maximum Details"));
            EditorGUILayout.IntSlider(detailSpacing, 1, 20, new GUIContent("Detail Spacing"));
            detailMapTable = GUITableLayout.DrawTable(detailMapTable, serializedObject.FindProperty("details"));

            terrain.GetComponent<Terrain>().detailObjectDistance = maxDetails.intValue;

            GUILayout.Space(20);
            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("+"))
            {

                terrain.AddNewDetails();
            }
            if (GUILayout.Button("-"))
            {

                terrain.RemoveDetails();
            }
            EditorGUILayout.EndHorizontal();
            if (GUILayout.Button("Apply Details"))
            {

                terrain.AddDetails();
            }
        }

        showWater = EditorGUILayout.Foldout(showWater, "Water");
        if (showWater)
        {

            EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);
            GUILayout.Label("Water", EditorStyles.boldLabel);
            EditorGUILayout.Slider(waterHeight, 0.0f, 1.0f, new GUIContent("Water Height"));
            EditorGUILayout.PropertyField(waterGO);

            if (GUILayout.Button("Add Water"))
            {

                terrain.AddWater();
            }
        }

        showErosion = EditorGUILayout.Foldout(showErosion, "Erosion");
        if (showErosion)
        {

            EditorGUILayout.PropertyField(erosionType);
            EditorGUILayout.Slider(erosionStrength, 0.0f, 1.0f, new GUIContent("Erosion Strength"));
            EditorGUILayout.Slider(erosionAmount, 0.0f, 1.0f, new GUIContent("Erosion Amount"));
            EditorGUILayout.IntSlider(droplets, 0, 500, new GUIContent("Droplets"));
            EditorGUILayout.Slider(solubilty, 0.001f, 1.0f, new GUIContent("Solubility"));
            EditorGUILayout.IntSlider(springsPerRiver, 0, 20, new GUIContent("Springs Per River"));
            EditorGUILayout.IntSlider(erosionSmoothAmount, 0, 10, new GUIContent("Smooth Amount"));

            if (GUILayout.Button("Erode"))
            {

                terrain.Erode();
            }
        }

        EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);
        if (GUILayout.Button("Reset Terrain"))
        {
            terrain.ResetTerrain();
        }

        EditorGUILayout.EndScrollView();
        EditorGUILayout.EndVertical();


        serializedObject.ApplyModifiedProperties();
    }
  
   

// Start is called before the first frame update
void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
