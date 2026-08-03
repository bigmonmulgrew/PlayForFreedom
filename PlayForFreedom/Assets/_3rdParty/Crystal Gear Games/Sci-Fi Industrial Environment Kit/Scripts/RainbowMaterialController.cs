namespace SciFiKit
{
    using UnityEngine;

    #if UNITY_EDITOR
    using UnityEditor;
    #endif

    [ExecuteAlways]
    public class RainbowMaterialController : MonoBehaviour
    {
        public Material targetMaterial;

        public bool animatePanelColour = true;
        public bool animateLightColour = false;

        public float speed = 1f;

        private float hue;


        void OnEnable()
        {
    #if UNITY_EDITOR
        EditorApplication.update += EditorUpdate;
    #endif
        }


        void OnDisable()
        {
    #if UNITY_EDITOR
        EditorApplication.update -= EditorUpdate;
    #endif
        }


        void Update()
        {
            if (Application.isPlaying)
            {
                UpdateRainbow(Time.deltaTime);
            }
        }


    #if UNITY_EDITOR
    void EditorUpdate()
    {
        if (!Application.isPlaying)
        {
            UpdateRainbow(0.016f);
        }
    }
    #endif


        void UpdateRainbow(float deltaTime)
        {
            if (targetMaterial == null)
                return;


            if (!animatePanelColour && !animateLightColour)
                return;


            // Advance rainbow
            hue += deltaTime * speed;

            if (hue >= 1f)
                hue -= 1f;


            // Generate ONE colour
            Color rainbowColour = Color.HSVToRGB(hue, 1f, 1f);


            // Apply the SAME colour logic to both properties
            ApplyColour("_Panel_Colour", animatePanelColour, rainbowColour);
            ApplyColour("_Light_Colour", animateLightColour, rainbowColour);


    #if UNITY_EDITOR
        EditorUtility.SetDirty(targetMaterial);
    #endif
        }


        void ApplyColour(string propertyName, bool enabled, Color colour)
        {
            if (!enabled)
                return;


            if (targetMaterial.HasProperty(propertyName))
            {
                targetMaterial.SetColor(propertyName, colour);
            }
        }


        public void ResetColours()
        {
            ApplyColour("_Panel_Colour", true, Color.white);
            ApplyColour("_Light_Colour", true, Color.white);


    #if UNITY_EDITOR
        EditorUtility.SetDirty(targetMaterial);
    #endif
        }
    }
}