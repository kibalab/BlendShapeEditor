using System;
using UnityEditor;
using UnityEngine;

namespace K13A.BlendShapeEditor
{
    class ApplyWindow : EditorWindow
    {
        string Message;
        Action method;

        public ApplyWindow(string Message, Action method)
        {
            this.method = method;
            this.Message = Message;
            OnWindow();
        }

        public static void OnWindow()
        {
            var win = EditorWindow.GetWindow(typeof(ApplyWindow));
            win.minSize = win.maxSize = new Vector2(300, 100);
        }

        private void OnGUI()
        {
            var removeCount = 0;
            var editCount = 0;

            EditorGUILayout.LabelField(Message, GUILayout.ExpandWidth(true));
            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("Yes", GUILayout.Height(30)))
            {
                method();
                Close();
            }
            if (GUILayout.Button("No", GUILayout.Height(30)))
            {
                Close();
            }
            EditorGUILayout.EndHorizontal();
        }
    }
}