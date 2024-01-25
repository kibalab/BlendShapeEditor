using UnityEditor;
using UnityEngine;

namespace K13A.BlendShapeEditor
{
    [InitializeOnLoad]
    public static class EditorStyle
    {
        public static Texture2D lineColor;
        public static Texture2D edittedlineColor;
        public static Texture2D removelineColor;
        public static Texture2D transColor;

        public static GUISkin transskin;

        static EditorStyle()
        {
            transColor = MakeTex(2, 2, new Color(0, 0, 0, 0));
            lineColor = MakeTex(2, 2, new Color(0, 0, 0, 0.2f));
            edittedlineColor = MakeTex(2, 2, new Color(0.2f, 1, 0.95f, 0.3f));
            removelineColor = MakeTex(2, 2, new Color(1f, 0.2f, 0.2f, 0.3f));


            transskin = new GUISkin();
            transskin.box.normal.background = transColor;
        }
        
        private static Texture2D MakeTex(int width, int height, Color col)
        {
            Color[] pix = new Color[width * height];
            for (int i = 0; i < pix.Length; ++i)
            {
                pix[i] = col;
            }
            Texture2D result = new Texture2D(width, height);
            result.SetPixels(pix);
            result.Apply();
            return result;
        }
    }
}