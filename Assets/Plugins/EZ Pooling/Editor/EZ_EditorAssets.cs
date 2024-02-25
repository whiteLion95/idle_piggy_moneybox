using UnityEngine;
using UnityEditor;
using System.Collections;
using System.IO;

namespace EZ_Pooling
{
    /// <summary>
    /// miscs
    /// </summary>
    /// 
    public class TextureData
    {
        public Texture tex;
        public string path;

        public TextureData(Texture t, string s)
        {
            tex = t;
            path = s;
        }
    }

    public static class EZ_EditorAssets
    {
        public static string assetName = "EZ Pooling";

        public static TextureData poolManagerItemLogo = new TextureData(null, "EZ Pooling Logo.psd");
        public static TextureData missingPrefabIcon = new TextureData(null, "missingPrefabIcon.psd");
        public static TextureData poolItemTop = new TextureData(null, "Pool Item Top Logo.psd");
        public static TextureData poolItemBottom = new TextureData(null, "Pool Item Bottom Logo.psd");

        public static Color addBtnColor = new Color(0, 1f, 0);
        public static Color delBtnColor = new Color(1f, 0, 0);
        public static Color shiftPosColor = new Color(0.5f, 0.5f, 0.5f);
    }

    /// <summary>
    /// class containing some common editor methods
    /// </summary>
    public static class EZ_EditorUtility
    {
        /// <summary>
        /// method to draw a texture in the editor inspector
        /// </summary>
        public static void DrawTexture(Texture tex)
        {
            if (tex == null)
            {
                Debug.LogWarning("GUI texture is missing !");
                return;
            }

            Rect rect = GUILayoutUtility.GetRect(0f, 0f);
            rect.width = tex.width;
            rect.height = tex.height;
            GUILayout.Space(rect.height);
            GUI.DrawTexture(rect, tex);
        }

        public static void DrawTexture(TextureData texData)
        {
            DrawTexture(LoadTexture(texData));
        }

        public static Texture LoadTexture(TextureData texData)
        {
            if (texData.tex == null)
            {
                if (!File.Exists("EZ CSP/" + texData.path))
                {
                    if (!Directory.Exists("Assets/Editor Default Resources/" + EZ_EditorAssets.assetName))
                    {
                        Directory.CreateDirectory("Assets/Editor Default Resources/" + EZ_EditorAssets.assetName);
                    }

                    AssetDatabase.Refresh();

                    FileInfo fInfo = new FileInfo("Assets/Plugins/" + EZ_EditorAssets.assetName + "/Editor/Texture Resources/" + texData.path);
                    fInfo.CopyTo("Assets/Editor Default Resources/" + EZ_EditorAssets.assetName + "/" + texData.path, true);

                    AssetDatabase.Refresh();

                    texData.tex = EditorGUIUtility.LoadRequired(EZ_EditorAssets.assetName + "/" + texData.path) as Texture;
                }
            }

            return texData.tex;
        }

        public static void DrawTexture(Texture tex, float optionalWidth, float optionalHeight)
        {
            if (tex == null)
            {
                Debug.LogWarning("GUI texture is missing !");
                return;
            }

            Rect rect = GUILayoutUtility.GetRect(0f, 0f);
            rect.width = optionalWidth;
            rect.height = optionalHeight;
            GUILayout.Space(rect.height);
            GUI.DrawTexture(rect, tex);
        }

        private static Color oldColor;

        public static void BeginColor(Color col)
        {
            oldColor = GUI.color;
            GUI.color = col;
        }

        public static void EndColor()
        {
            GUI.color = oldColor;
        }
    }
}