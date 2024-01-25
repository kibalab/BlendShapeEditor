using System;
using UnityEditor;
using UnityEngine;
using UnityEngine.Serialization;

namespace K13A.BlendShapeEditor
{
    [Flags]
    public enum PropertyState
    {
        Editing,
        WillRemove,
        Removed,
        None
    }
    
    public class PropertyEditor : Editor
    {
        public int BlendShapeIndex;
        public PropertyState StateFlag;

        public string Name;
        public bool isSelected = false;
        
        public void Draw()
        {
            UpdateStateColor();

            EditorGUILayout.BeginVertical(GUILayout.Height(17));
            EditorGUILayout.BeginHorizontal(GUI.skin.box);
            {
                DrawQuickButton();

                EditorGUI.BeginChangeCheck();
                {
                    PathField();
                }
                if (EditorGUI.EndChangeCheck()) StateFlag = PropertyState.Editing;
                MenuButton(ref isSelected, new Vector2(17, 17));
            }
            EditorGUILayout.EndHorizontal();
            
            if (isSelected) DrawMenu();
            
            EditorGUILayout.EndVertical();
        }

        public void DrawMenu()
        {
            GUI.skin.box.normal.background = EditorStyle.lineColor;

            EditorGUILayout.BeginHorizontal(GUI.skin.box);

            UpdateStateColor();
            GUILayout.Box("", GUILayout.ExpandHeight(true), GUILayout.Width(5));
            
            
            GUILayout.FlexibleSpace();
            
            
            EditorGUILayout.EndHorizontal();

            UpdateStateColor();
        }

        public void PathField()
        {
            Name = EditorGUILayout.TextField(Name);
        }

        public void DrawQuickButton()
        {
            var b = GUILayout.Button(StateFlag != PropertyState.None ? "Undo" : "Delete", GUILayout.Width(50));
            
            if(!b)return;
            GUI.FocusControl(null);
            switch (StateFlag)
            {
                case PropertyState.None :
                    StateFlag = PropertyState.WillRemove | PropertyState.Editing;
                    break;
                
                default:
                    Name = BlendShapeEditor.CurrentMesh.sharedMesh.GetBlendShapeName(BlendShapeIndex);
                    StateFlag = PropertyState.None;
                    break;
            }
        }
        
        public bool MenuButton(ref bool b, Vector2 size) =>
            GUILayout.Button(EditorGUIUtility.IconContent("d_MoreOptions@2x"), EditorStyle.transskin.box,
                GUILayout.Width(size.x), GUILayout.Height(size.y))
                ? b = !b
                : b;
        
        
        void UpdateStateColor()
        {
            switch (StateFlag)
            {
                case PropertyState.WillRemove :
                    GUI.skin.box.normal.background = EditorStyle.removelineColor;
                    break;
                
                case PropertyState.Editing :
                    GUI.skin.box.normal.background = EditorStyle.edittedlineColor;
                    break;
                
                case PropertyState.None :
                    GUI.skin.box.normal.background = EditorStyle.lineColor;
                    break;
            }
        }
    }
}