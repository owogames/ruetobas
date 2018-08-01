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
            this.curves = new List<AnimationCurve>();
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

        public static Animation GenerateTimer(float duration, Action timerEvent)
        {
            AnimationCurve curve = new AnimationCurve(AnimationCurve.Type.Null, null);
            curve.SetKeyframes(new AnimationKeyframe(0.0f, duration));
            return new Animation(timerEvent, curve);
        }

        public static Animation GenerateFadeIn(float duration, RawImage reference)
        {
            AnimationCurve curve = new AnimationCurve(AnimationCurve.Type.RawImageOpacity, reference);
            curve.SetKeyframes(new AnimationKeyframe(0.0f, 0.0f), new AnimationKeyframe(1.0f, duration));
            return new Animation(curve);
        }

        public static Animation GenerateFadeOut(float duration, RawImage reference)
        {
            AnimationCurve curve = new AnimationCurve(AnimationCurve.Type.RawImageOpacity, reference);
            curve.SetKeyframes(new AnimationKeyframe(1.0f, 0.0f), new AnimationKeyframe(0.0f, duration));
            return new Animation(curve);
        }

        public static Animation GenerateFadeInOut(float durationIn, float durationStay, float durationOut, RawImage reference)
        {
            AnimationCurve curve = new AnimationCurve(AnimationCurve.Type.RawImageOpacity, reference);
            curve.SetKeyframes(new AnimationKeyframe(0.0f, 0.0f), new AnimationKeyframe(1.0f, durationIn), new AnimationKeyframe(1.0f, durationIn + durationStay), new AnimationKeyframe(0.0f, durationIn + durationStay + durationOut));
            return new Animation(curve);
        }

        public static Animation GenerateFadeOutIn(float durationIn, float durationStay, float durationOut, RawImage reference)
        {
            AnimationCurve curve = new AnimationCurve(AnimationCurve.Type.RawImageOpacity, reference);
            curve.SetKeyframes(new AnimationKeyframe(1.0f, 0.0f), new AnimationKeyframe(0.0f, durationIn), new AnimationKeyframe(0.0f, durationIn + durationStay), new AnimationKeyframe(1.0f, durationIn + durationStay + durationOut));
            return new Animation(curve);
        }

        public static Animation GenerateTranslate(float duration, Vector2 start, Vector2 end, Button reference)
        {
            AnimationCurve curvex = new AnimationCurve(AnimationCurve.Type.ButtonX, reference);
            AnimationCurve curvey = new AnimationCurve(AnimationCurve.Type.ButtonY, reference);
            curvex.SetKeyframes(new AnimationKeyframe(start.X, 0.0f), new AnimationKeyframe(end.X, duration));
            curvey.SetKeyframes(new AnimationKeyframe(start.Y, 0.0f), new AnimationKeyframe(end.Y, duration));
            return new Animation(curvex, curvey);
        }

        public static Animation GenerateTranslate(float duration, Vector2 start, Vector2 end, InputBox reference)
        {
            AnimationCurve curvex = new AnimationCurve(AnimationCurve.Type.InputBoxX, reference);
            AnimationCurve curvey = new AnimationCurve(AnimationCurve.Type.InputBoxY, reference);
            curvex.SetKeyframes(new AnimationKeyframe(start.X, 0.0f), new AnimationKeyframe(end.X, duration));
            curvey.SetKeyframes(new AnimationKeyframe(start.Y, 0.0f), new AnimationKeyframe(end.Y, duration));
            return new Animation(curvex, curvey);
        }

        public static Animation GenerateTranslate(float duration, Vector2 start, Vector2 end, TextBox reference)
        {
            AnimationCurve curvex = new AnimationCurve(AnimationCurve.Type.TextBoxX, reference);
            AnimationCurve curvey = new AnimationCurve(AnimationCurve.Type.TextBoxY, reference);
            curvex.SetKeyframes(new AnimationKeyframe(start.X, 0.0f), new AnimationKeyframe(end.X, duration));
            curvey.SetKeyframes(new AnimationKeyframe(start.Y, 0.0f), new AnimationKeyframe(end.Y, duration));
            return new Animation(curvex, curvey);
        }

        public static Animation GenerateTranslate(float duration, Vector2 start, Vector2 end, RawImage reference)
        {
            AnimationCurve curvex = new AnimationCurve(AnimationCurve.Type.RawImageX, reference);
            AnimationCurve curvey = new AnimationCurve(AnimationCurve.Type.RawImageY, reference);
            curvex.SetKeyframes(new AnimationKeyframe(start.X, 0.0f), new AnimationKeyframe(end.X, duration));
            curvey.SetKeyframes(new AnimationKeyframe(start.Y, 0.0f), new AnimationKeyframe(end.Y, duration));
            return new Animation(curvex, curvey);
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
            this.keyframes.Sort((AnimationKeyframe l, AnimationKeyframe r) => { return r.time.CompareTo(l.time); });
            if (keyframes.Last().time >= 0.001f)
                this.keyframes.Add(new AnimationKeyframe(startingValue, 0.0f));
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
