using UnityEngine;

namespace GameFrameWork.Utils
{
    public class TweenUtil
    {
        public enum TweenType
        {
            None,
            Linear,
            Spring,
            InQuad,
            OutQuad,
            InOutQuad,
            InCubic,
            OutCubic,
            InOutCubic,
            InQuart,
            OutQuart,
            InOutQuart,
            InQuint,
            OutQuint,
            InOutQuint,
            InSine,
            OutSine,
            InOutSine,
            InExpo,
            OutExpo,
            InOutExpo,
            InCirc,
            OutCirc,
            InOutCirc,
            InBounce,
            OutBounce,
            InOutBounce,
            InBack,
            OutBack,
            InOutBack,
            InElastic,
            OutElastic,
            InOutElastic
        }

        public static float Tween(TweenType tweenType,float start,float end,float val)
        {
            if(tweenType == TweenType.None)
            {
                return 0f;
            }

            float newPosition = 0f;

            switch (tweenType)
            {
                case TweenType.Linear: newPosition = Linear(start, end, val); break;
                case TweenType.Spring: newPosition = Spring(start, end, val); break;
                case TweenType.InQuad: newPosition = InQuad(start, end, val); break;
                case TweenType.OutQuad: newPosition = OutQuad(start, end, val); break;
                case TweenType.InOutQuad: newPosition = InOutQuad(start, end, val); break;
                case TweenType.InCubic: newPosition = InCubic(start, end, val); break;
                case TweenType.OutCubic: newPosition = OutCubic(start, end, val); break;
                case TweenType.InOutCubic: newPosition = InOutCubic(start, end, val); break;
                case TweenType.InQuart: newPosition = InQuart(start, end, val); break;
                case TweenType.OutQuart: newPosition = OutQuart(start, end, val); break;
                case TweenType.InOutQuart: newPosition = InOutQuart(start, end, val); break;
                case TweenType.InQuint: newPosition = InQuint(start, end, val); break;
                case TweenType.OutQuint: newPosition = OutQuint(start, end, val); break;
                case TweenType.InOutQuint: newPosition = InOutQuint(start, end, val); break;
                case TweenType.InSine: newPosition = InSine(start, end, val); break;
                case TweenType.OutSine: newPosition = OutSine(start, end, val); break;
                case TweenType.InOutSine: newPosition = InOutSine(start, end, val); break;
                case TweenType.InExpo: newPosition = InExpo(start, end, val); break;
                case TweenType.OutExpo: newPosition = OutExpo(start, end, val); break;
                case TweenType.InOutExpo: newPosition = InOutExpo(start, end, val); break;
                case TweenType.InCirc: newPosition = InCirc(start, end, val); break;
                case TweenType.OutCirc: newPosition = OutCirc(start, end, val); break;
                case TweenType.InOutCirc: newPosition = InOutCirc(start, end, val); break;
                case TweenType.InBounce: newPosition = InBounce(start, end, val); break;
                case TweenType.OutBounce: newPosition = OutBounce(start, end, val); break;
                case TweenType.InOutBounce: newPosition = InOutBounce(start, end, val); break;
                case TweenType.InBack: newPosition = InBack(start, end, val); break;
                case TweenType.OutBack: newPosition = OutBack(start, end, val); break;
                case TweenType.InOutBack: newPosition = InOutBack(start, end, val); break;
                case TweenType.InElastic: newPosition = InElastic(start, end, val); break;
                case TweenType.OutElastic: newPosition = OutElastic(start, end, val); break;
                case TweenType.InOutElastic: newPosition = InOutElastic(start, end, val); break;
            }

            return newPosition;
        }

        private static float Linear(float start, float end, float val)
        {
            return Mathf.Lerp(start, end, val);
        }

        private static float Spring(float start, float end, float val)
        {
            val = Mathf.Clamp01(val);
            val = (Mathf.Sin(val * Mathf.PI * (0.2f + 2.5f * val * val * val)) * Mathf.Pow(1f - val, 2.2f) + val) * (1f + (1.2f * (1f - val)));
            return start + (end - start) * val;
        }

        private static float InQuad(float start, float end, float val)
        {
            end -= start;
            return end * val * val + start;
        }

        private static float OutQuad(float start, float end, float val)
        {
            end -= start;
            return -end * val * (val - 2) + start;
        }

        private static float InOutQuad(float start, float end, float val)
        {
            val /= .5f;
            end -= start;
            if (val < 1) return end / 2 * val * val + start;
            val--;
            return -end / 2 * (val * (val - 2) - 1) + start;
        }

        private static float InCubic(float start, float end, float val)
        {
            end -= start;
            return end * val * val * val + start;
        }

        private static float OutCubic(float start, float end, float val)
        {
            val--;
            end -= start;
            return end * (val * val * val + 1) + start;
        }

        private static float InOutCubic(float start, float end, float val)
        {
            val /= .5f;
            end -= start;
            if (val < 1) return end / 2 * val * val * val + start;
            val -= 2;
            return end / 2 * (val * val * val + 2) + start;
        }

        private static float InQuart(float start, float end, float val)
        {
            end -= start;
            return end * val * val * val * val + start;
        }

        private static float OutQuart(float start, float end, float val)
        {
            val--;
            end -= start;
            return -end * (val * val * val * val - 1) + start;
        }

        private static float InOutQuart(float start, float end, float val)
        {
            val /= .5f;
            end -= start;
            if (val < 1) return end / 2 * val * val * val * val + start;
            val -= 2;
            return -end / 2 * (val * val * val * val - 2) + start;
        }

        private static float InQuint(float start, float end, float val)
        {
            end -= start;
            return end * val * val * val * val * val + start;
        }

        private static float OutQuint(float start, float end, float val)
        {
            val--;
            end -= start;
            return end * (val * val * val * val * val + 1) + start;
        }

        private static float InOutQuint(float start, float end, float val)
        {
            val /= .5f;
            end -= start;
            if (val < 1) return end / 2 * val * val * val * val * val + start;
            val -= 2;
            return end / 2 * (val * val * val * val * val + 2) + start;
        }

        private static float InSine(float start, float end, float val)
        {
            end -= start;
            return -end * Mathf.Cos(val / 1 * (Mathf.PI / 2)) + end + start;
        }

        private static float OutSine(float start, float end, float val)
        {
            end -= start;
            return end * Mathf.Sin(val / 1 * (Mathf.PI / 2)) + start;
        }

        private static float InOutSine(float start, float end, float val)
        {
            end -= start;
            return -end / 2 * (Mathf.Cos(Mathf.PI * val / 1) - 1) + start;
        }

        private static float InExpo(float start, float end, float val)
        {
            end -= start;
            return end * Mathf.Pow(2, 10 * (val / 1 - 1)) + start;
        }

        private static float OutExpo(float start, float end, float val)
        {
            end -= start;
            return end * (-Mathf.Pow(2, -10 * val / 1) + 1) + start;
        }

        private static float InOutExpo(float start, float end, float val)
        {
            val /= .5f;
            end -= start;
            if (val < 1) return end / 2 * Mathf.Pow(2, 10 * (val - 1)) + start;
            val--;
            return end / 2 * (-Mathf.Pow(2, -10 * val) + 2) + start;
        }

        private static float InCirc(float start, float end, float val)
        {
            end -= start;
            return -end * (Mathf.Sqrt(1 - val * val) - 1) + start;
        }

        private static float OutCirc(float start, float end, float val)
        {
            val--;
            end -= start;
            return end * Mathf.Sqrt(1 - val * val) + start;
        }

        private static float InOutCirc(float start, float end, float val)
        {
            val /= .5f;
            end -= start;
            if (val < 1) return -end / 2 * (Mathf.Sqrt(1 - val * val) - 1) + start;
            val -= 2;
            return end / 2 * (Mathf.Sqrt(1 - val * val) + 1) + start;
        }

        private static float InBounce(float start, float end, float val)
        {
            end -= start;
            float d = 1f;
            return end - OutBounce(0, end, d - val) + start;
        }

        private static float OutBounce(float start, float end, float val)
        {
            val /= 1f;
            end -= start;
            if (val < (1 / 2.75f))
            {
                return end * (7.5625f * val * val) + start;
            }
            else if (val < (2 / 2.75f))
            {
                val -= (1.5f / 2.75f);
                return end * (7.5625f * (val) * val + .75f) + start;
            }
            else if (val < (2.5 / 2.75))
            {
                val -= (2.25f / 2.75f);
                return end * (7.5625f * (val) * val + .9375f) + start;
            }
            else
            {
                val -= (2.625f / 2.75f);
                return end * (7.5625f * (val) * val + .984375f) + start;
            }
        }

        private static float InOutBounce(float start, float end, float val)
        {
            end -= start;
            float d = 1f;
            if (val < d / 2) return InBounce(0, end, val * 2) * 0.5f + start;
            else return OutBounce(0, end, val * 2 - d) * 0.5f + end * 0.5f + start;
        }

        private static float InBack(float start, float end, float val)
        {
            end -= start;
            val /= 1;
            float s = 1.70158f;
            return end * (val) * val * ((s + 1) * val - s) + start;
        }

        private static float OutBack(float start, float end, float val)
        {
            float s = 1.70158f;
            end -= start;
            val = (val / 1) - 1;
            return end * ((val) * val * ((s + 1) * val + s) + 1) + start;
        }

        private static float InOutBack(float start, float end, float val)
        {
            float s = 1.70158f;
            end -= start;
            val /= .5f;
            if ((val) < 1)
            {
                s *= (1.525f);
                return end / 2 * (val * val * (((s) + 1) * val - s)) + start;
            }
            val -= 2;
            s *= (1.525f);
            return end / 2 * ((val) * val * (((s) + 1) * val + s) + 2) + start;
        }

        private static float InElastic(float start, float end, float val)
        {
            end -= start;

            float d = 1f;
            float p = d * .3f;
            float s = 0;
            float a = 0;

            if (val == 0) return start;
            val = val / d;
            if (val == 1) return start + end;

            if (a == 0f || a < Mathf.Abs(end))
            {
                a = end;
                s = p / 4;
            }
            else
            {
                s = p / (2 * Mathf.PI) * Mathf.Asin(end / a);
            }
            val = val - 1;
            return -(a * Mathf.Pow(2, 10 * val) * Mathf.Sin((val * d - s) * (2 * Mathf.PI) / p)) + start;
        }

        private static float OutElastic(float start, float end, float val)
        {
            end -= start;

            float d = 1f;
            float p = d * .3f;
            float s = 0;
            float a = 0;

            if (val == 0) return start;

            val = val / d;
            if (val == 1) return start + end;

            if (a == 0f || a < Mathf.Abs(end))
            {
                a = end;
                s = p / 4;
            }
            else
            {
                s = p / (2 * Mathf.PI) * Mathf.Asin(end / a);
            }

            return (a * Mathf.Pow(2, -10 * val) * Mathf.Sin((val * d - s) * (2 * Mathf.PI) / p) + end + start);
        }

        private static float InOutElastic(float start, float end, float val)
        {
            end -= start;

            float d = 1f;
            float p = d * .3f;
            float s = 0;
            float a = 0;

            if (val == 0) return start;

            val = val / (d / 2);
            if (val == 2) return start + end;

            if (a == 0f || a < Mathf.Abs(end))
            {
                a = end;
                s = p / 4;
            }
            else
            {
                s = p / (2 * Mathf.PI) * Mathf.Asin(end / a);
            }

            if (val < 1)
            {
                val = val - 1;
                return -0.5f * (a * Mathf.Pow(2, 10 * val) * Mathf.Sin((val * d - s) * (2 * Mathf.PI) / p)) + start;
            }
            val = val - 1;
            return a * Mathf.Pow(2, -10 * val) * Mathf.Sin((val * d - s) * (2 * Mathf.PI) / p) * 0.5f + end + start;
        }
    }
}