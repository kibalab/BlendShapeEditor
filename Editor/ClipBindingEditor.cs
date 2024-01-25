using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace K13A.BlendShapeEditor
{
    public class ClipBindingEditor
    {
        //blendShape.
        public static void ChangeBlendShape(string originName, string DestName)
        {
            var clips = CollectUsedClips();

            var i = 0;
            foreach (var clip in clips)
            {
                foreach (var binding in AnimationUtility.GetCurveBindings(clip))
                {
                    if (binding.propertyName == $"blendShape.{originName}")
                    {
                        AnimationCurve curve = AnimationUtility.GetEditorCurve(clip, binding);
            
                        AnimationUtility.SetEditorCurve(clip, binding, null);

                        var newBinding = binding;
                        newBinding.propertyName = $"blendShape.{DestName}";
            
                        AnimationUtility.SetEditorCurve(clip, newBinding, curve);
                    }
                }

                i++;
            }
        }

        public static List<AnimationClip> CollectUsedClips()
        {
            var clips = new List<AnimationClip>();
            foreach (var layer in BlendShapeEditor.CurrentAvatar.baseAnimationLayers)
            {
                if(layer.isDefault) continue;
                clips.AddRange(layer.animatorController.animationClips);
            }
            foreach (var layer in BlendShapeEditor.CurrentAvatar.specialAnimationLayers)
            {
                if(layer.isDefault) continue;
                clips.AddRange(layer.animatorController.animationClips);
            }

            return clips;
        }
    }
}