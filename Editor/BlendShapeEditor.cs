using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEngine;
using UnityEngine.Serialization;
using VRC.SDK3.Avatars.Components;
using Object = UnityEngine.Object;

namespace K13A.BlendShapeEditor {
    class BlendShapeEditor : EditorWindow
    {
        public Object Mesh;
        public static VRCAvatarDescriptor CurrentAvatar;
        public static SkinnedMeshRenderer CurrentMesh;

        private List<PropertyEditor> PropertyEditors = new List<PropertyEditor>();

        public Vector2 ScrollPos = new Vector2(0, 0);

        static EditorWindow self;


        bool editting = false;

        ApplyWindow applyWindow;

        [MenuItem("K13A Labs/BlendShape Editor")]
        public static void OnWindow()
        {
            self = EditorWindow.GetWindow(typeof(BlendShapeEditor));
            self.title = "K13A_Property Cutter";
            self.minSize = new Vector2(450, 300);
        }
        
        private void OnGUI()
        {

            EditorGUILayout.LabelField("Mesh");

            Mesh = EditorGUILayout.ObjectField(Mesh, typeof(SkinnedMeshRenderer));

            if ((SkinnedMeshRenderer)Mesh != CurrentMesh) UpdatePropertyEditors();

            if (CurrentMesh == null) return;

            EditorGUILayout.Space(15);

            EditorGUILayout.BeginHorizontal(GUI.skin.box);
                EditorGUILayout.LabelField("Property Path");
                EditorGUILayout.LabelField("Property Name");
            EditorGUILayout.EndHorizontal();
            

            ScrollPos = GUILayout.BeginScrollView(ScrollPos, GUILayout.ExpandWidth(true));
            {
                var origin = GUI.skin.box.normal.background;

                foreach (var drawer in PropertyEditors)
                {
                    if(drawer.StateFlag == PropertyState.Removed) continue;
                    switch (drawer.StateFlag)
                    {
                        case PropertyState.WillRemove:
                            GUI.skin.box.normal.background = Texture2D.redTexture;
                            break;
                        case PropertyState.Editing:
                            GUI.skin.box.normal.background = Texture2D.whiteTexture;
                            break;
                        default:
                            break;
                    }
                    drawer.Draw();
                }

                GUI.skin.box.normal.background = origin;
            }
            GUILayout.EndScrollView();
            
            GUILayout.FlexibleSpace();

            if (GUILayout.Button("Apply All Changes"))
            {
                EditorCoroutine.Start(ApplyAllChanges());
            }
        }
        
        public struct BlendShapeData
        {
            public string Name;
            public float Weight;
            public Vector3[] DeltaVertices;
            public Vector3[] DeltaNormals;
            public Vector3[] DeltaTangents;
        }
        
        private IEnumerator ApplyAllChanges()
        {
            GUI.FocusControl(null);
            var frames = new List<BlendShapeData>();
            
            for (var i = 0; i < CurrentMesh.sharedMesh.blendShapeCount; i++)
            {
                EditorUtility.DisplayProgressBar("BlendShape Editor", "Read&Write BlendShapes", i/(float)CurrentMesh.sharedMesh.blendShapeCount);
                int frameCount = CurrentMesh.sharedMesh.GetBlendShapeFrameCount(i);
                for (var j = 0; j < frameCount; j++)
                {
                    if(PropertyEditors[i].StateFlag == PropertyState.WillRemove) continue; 
                    var frame = new BlendShapeData()
                    {
                        Name = PropertyEditors[i].Name,
                        Weight = CurrentMesh.sharedMesh.GetBlendShapeFrameWeight(i, j),
                        DeltaVertices = new Vector3[CurrentMesh.sharedMesh.vertexCount],
                        DeltaNormals = new Vector3[CurrentMesh.sharedMesh.vertexCount],
                        DeltaTangents = new Vector3[CurrentMesh.sharedMesh.vertexCount],
                    }; 
                    frames.Add(frame);
                    CurrentMesh.sharedMesh.GetBlendShapeFrameVertices(i, j, frame.DeltaVertices, frame.DeltaNormals, frame.DeltaTangents);
                    ClipBindingEditor.ChangeBlendShape(CurrentMesh.sharedMesh.GetBlendShapeName(i), PropertyEditors[i].Name);
                    PropertyEditors[i].StateFlag = PropertyState.None;
                }
                yield return null;
            }
            EditorUtility.ClearProgressBar();
            CurrentMesh.sharedMesh.ClearBlendShapes();
            foreach (var frame in frames)
            {
                CurrentMesh.sharedMesh.AddBlendShapeFrame(frame.Name, frame.Weight, frame.DeltaVertices, frame.DeltaNormals, frame.DeltaTangents);
            }
        }

        private void UpdatePropertyEditors()
        {
            PropertyEditors.Clear();

            CurrentMesh = (SkinnedMeshRenderer)Mesh;
            
            if (CurrentMesh == null) return;

            CurrentAvatar = CurrentMesh.GetComponentInParent<VRCAvatarDescriptor>();

            for (var i = 0; i < CurrentMesh.sharedMesh.blendShapeCount; i++)
            {
                PropertyEditors.Add(new PropertyEditor()
                {
                    BlendShapeIndex = i,
                    StateFlag = PropertyState.None,
                    Name = CurrentMesh.sharedMesh.GetBlendShapeName(i)
                });
            }
        }
    }
}