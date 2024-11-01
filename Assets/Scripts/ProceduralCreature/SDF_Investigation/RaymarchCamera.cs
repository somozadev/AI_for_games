using UnityEngine;

[RequireComponent(typeof(Camera))]
public class RaymarchingAndNormalRendering : MonoBehaviour
{
    [SerializeField] private Shader _raymarchShader;
    [SerializeField] private float _maxDistance = 100f;

    private Material _material;
    private Camera _camera;

    public Material Material
    {
        get
        {
            if (_material == null && _raymarchShader != null)
            {
                _material = new Material(_raymarchShader);
                _material.hideFlags = HideFlags.HideAndDontSave;
            }
            return _material;
        }
    }

    private void OnRenderImage(RenderTexture source, RenderTexture destination)
    {
        if (!Material)
        {
            Graphics.Blit(source, destination);
            return;
        }

        // Recoger las esferas de la escena
        GameObject[] sphereObjects = GameObject.FindGameObjectsWithTag("Sphere");

        // Pasar las posiciones y radios de las esferas al shader
        for (int i = 0; i < sphereObjects.Length && i < 2; i++) // Cambiar el límite según tus necesidades
        {
            Transform sphereTransform = sphereObjects[i].transform;
            Vector4 sphereData = new Vector4(sphereTransform.position.x, sphereTransform.position.y, sphereTransform.position.z, sphereTransform.localScale.x / 2);
            Material.SetVector($"_Sphere{i + 1}", sphereData);
        }

        // Establecer el contador de esferas
        Material.SetFloat("_MaxDistance", _maxDistance);

        // Blit para mezclar el raymarching con el resto de la escena
        Graphics.Blit(source, destination, Material);
    }
}