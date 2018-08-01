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
        public float Variable;
        public List<AnimationKeyframe> keyframes;
        public AnimationCurve(ref float Variable, params AnimationKeyframe[] keyframes)
        {
            this.Variable = Variable;
            this.keyframes = new List<AnimationKeyframe>();
            foreach (AnimationKeyframe keyframe in keyframes)
                this.keyframes.Add(keyframe);
            this.keyframes.Add(new AnimationKeyframe(Variable, 0));
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
            Variable = AnimationKeyframe.Lerp(keyframes[keyframes.Count - 1], keyframes[keyframes.Count - 2], time);
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
