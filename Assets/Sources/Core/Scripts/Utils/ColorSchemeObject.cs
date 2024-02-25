using System;
using UnityEngine;

namespace Deslab.Level
{
    [Serializable]
    public class ColorScheme
    {
        //public Material skyMaterial;
        //public Material fogMaterial;
        //public Material fogMaterial1;
        public Material buildingsMaterial;

        //[Header("Sky colors")]
        //public Color32 skyColor1;
        //public Color32 skyColor2;

        //[Header("Fog color")]
        //public Color32 fogColor;
        //public Color32 fogColor1;

        [Header("Buildings color")]
        public Color32 buildingsColor;
        public Color32 highlightColor;

        public void ApplyScheme()
        {
            //skyMaterial?.SetColor("_Color1", skyColor1);
            //skyMaterial?.SetColor("_Color2", skyColor2);

            //fogMaterial?.SetColor("_Color", fogColor);
            //fogMaterial1?.SetColor("_Color", fogColor1);

            buildingsMaterial.SetColor("_Color", buildingsColor);
            buildingsMaterial.SetColor("_HColor", highlightColor);
        }
    }


    [CreateAssetMenu(fileName = "ColorScheme", menuName = "Deslab/ScriptableObjects/ColorScheme")]
    public class ColorSchemeObject : ScriptableObject
    {
        public ColorScheme colorScheme;

        public void ApplyScheme()
        {
            colorScheme.ApplyScheme();
        }
    }
}