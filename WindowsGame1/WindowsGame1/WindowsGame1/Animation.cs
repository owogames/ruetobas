using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace Ruetobas
{
    public class Animation
    {
        public List<AnimationCurve> curves;
        public Action endEvent;
        public Animation(params AnimationCurve[] curves)
        {
            foreach (AnimationCurve curve in curves)
                this.curves.Add(curve);
            endEvent = null;
        }

        public Animation(Action endEvent, params AnimationCurve[] curves)
        {
            this.curves = new List<AnimationCurve>();
            foreach (AnimationCurve curve in curves)
                this.curves.Add(curve);
            this.endEvent = endEvent;
        }

        private float time = 0.0f;

        public bool Update(float deltaTime)
        {
            time += deltaTime;
            for (int i = curves.Count - 1; i >= 0; i--)
            {
                if (curves[i].Update(deltaTime))
                {
                    curves.RemoveAt(i);
                }
            }

            if (curves.Count == 0)
            {
                if (endEvent != null)
                    endEvent();
                return true;
            }
            return false;
        }
    }

    public class AnimationCurve
    {
        public enum Type { Null, RawImageOpacity, RawImageRotation, RawImageX, RawImageY, ButtonX, ButtonY, TextBoxX, TextBoxY, InputBoxX, InputBoxY };

        private object reference;
        private List<AnimationKeyframe> keyframes;
        private float startingValue;
        private Type type;

        public AnimationCurve(Type type, object reference)
        {
            this.type = type;
            this.reference = reference;
            switch (type)
            {
                case Type.Null:
                    startingValue = 0.0f;
                    break;
                case Type.RawImageOpacity:
                    startingValue = ((RawImage)reference).opacity;
                    break;
                case Type.RawImageRotation:
                    startingValue = ((RawImage)reference).rotation;
                    break;
                case Type.RawImageX:
                    startingValue = ((RawImage)reference).location.X;
                    break;
                case Type.RawImageY:
                    startingValue = ((RawImage)reference).location.Y;
                    break;
                case Type.ButtonX:
                    startingValue = ((Button)reference).location.X;
                    break;
                case Type.ButtonY:
                    startingValue = ((Button)reference).location.Y;
                    break;
                case Type.InputBoxX:
                    startingValue = ((InputBox)reference).location.X;
                    break;
                case Type.InputBoxY:
                    startingValue = ((InputBox)reference).location.Y;
                    break;
                case Type.TextBoxX:
                    startingValue = ((TextBox)reference).location.X;
                    break;
                case Type.TextBoxY:
                    startingValue = ((TextBox)reference).location.Y;
                    break;
            }
        }

        public void SetKeyframes(params AnimationKeyframe[] keyframes)
        {
            this.keyframes = new List<AnimationKeyframe>();
            foreach (AnimationKeyframe keyframe in keyframes)
                this.keyframes.Add(keyframe);
            this.keyframes.Add(new AnimationKeyframe(startingValue, 0));
            this.keyframes.Sort((AnimationKeyframe l, AnimationKeyframe r) => { return r.time.CompareTo(l.time); });
            //Pierwszy keyframe będzie na końcu listy
        }

        private float time = 0.0f;

        public bool Update(float deltaTime)
        {
            time += deltaTime;
            if (keyframes.Count <= 1) return true;
            if (time >= keyframes.ElementAt(keyframes.Count - 2).time)
                keyframes.RemoveAt(keyframes.Count - 1);
            if (keyframes.Count <= 1) return true;
            float value = AnimationKeyframe.Lerp(keyframes[keyframes.Count - 1], keyframes[keyframes.Count - 2], time);
            switch (type)
            {
                case Type.RawImageOpacity:
                    ((RawImage)reference).opacity = value;
                    break;
                case Type.RawImageRotation:
                    ((RawImage)reference).rotation = value;
                    break;
                case Type.RawImageX:
                    ((RawImage)reference).location.X = (int)value;
                    break;
                case Type.RawImageY:
                    ((RawImage)reference).location.Y = (int)value;
                    break;
                case Type.TextBoxX:
                    ((TextBox)reference).location.X = (int)value;
                    break;
                case Type.TextBoxY:
                    ((TextBox)reference).location.Y = (int)value;
                    break;
                case Type.InputBoxX:
                    ((InputBox)reference).location.X = (int)value;
                    break;
                case Type.InputBoxY:
                    ((InputBox)reference).location.Y = (int)value;
                    break;
            }
            return false;
        }
    }

    public class AnimationKeyframe
    {
        public float value;
        public float time;
        public AnimationKeyframe(float value, float time)
        {
            this.value = value;
            this.time = time;
        }

        public static float Lerp(AnimationKeyframe a, AnimationKeyframe b, float t)
        {
            return a.value + (b.value - a.value) * (t - a.time) / (b.time - a.time);
        }
    }
}
