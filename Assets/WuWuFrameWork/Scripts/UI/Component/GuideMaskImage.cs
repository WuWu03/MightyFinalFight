using UnityEngine;
using UnityEngine.UI;

namespace WuWuFramework.UI
{
    [AddComponentMenu("UI/GuideMaskImage")]
    public class GuideMaskImage : MaskableGraphic, ICanvasRaycastFilter
    {
        public enum MaskType
        {
            Circle,
            Rectangle,
        }
        
        protected override void Awake()
        {
            base.Awake();
            m_CanMask = false;
        }

        public void SetMaskBounds(Bounds bounds)
        {
            m_TargetBoundsMin = bounds.min;
            m_TargetBoundsMax = bounds.max;
            m_TargetBoundsCenter = bounds.center;
            m_CanMask = true;
        }

        public void SetTarget(RectTransform target, MaskType maskType = MaskType.Rectangle)
        {
            m_MaskType = maskType;

            if (target == null)
            {
                m_TargetBoundsMin = Vector3.zero;
                m_TargetBoundsMax = Vector3.zero;
                m_TargetBoundsCenter = Vector3.zero;
                m_CanMask = false;
                SetAllDirty();
            }
            else
            {
                Bounds bounds = RectTransformUtility.CalculateRelativeRectTransformBounds(rectTransform, target);
                m_TargetBoundsMin = bounds.min;
                m_TargetBoundsMax = bounds.max;
                m_TargetBoundsCenter = bounds.center;
                m_CanMask = true;
            }
        }

        public void SetTarget(RectTransform target, float width, float height, MaskType maskType = MaskType.Rectangle)
        {
            SetTarget(target, maskType);

            if (target != null)
            {
                m_TargetBoundsMin = new Vector3(m_TargetBoundsCenter.x - width / 2f, m_TargetBoundsCenter.y - height / 2f, 0f);
                m_TargetBoundsMax = new Vector3(m_TargetBoundsCenter.x + width / 2f, m_TargetBoundsCenter.y + height / 2f, 0f);
            }
        }

        protected override void OnPopulateMesh(VertexHelper vh)
        {
            if (!m_CanMask)
            {
                base.OnPopulateMesh(vh);
                return;
            }

            vh.Clear();

            if (m_MaskType == MaskType.Rectangle)
            {
                Rect rect = rectTransform.rect;
                Vector2 pivot = rectTransform.pivot;
                float outerLeftBottomX = -pivot.x * rect.width;
                float outerLeftBottomY = -pivot.y * rect.height;
                float outerRightTopX = (1 - pivot.x) * rect.width;
                float outerRightTopY = (1 - pivot.y) * rect.height;

                // 准备顶点数据
                UIVertex vert = UIVertex.simpleVert;
                // 填充顶点颜色
                vert.color = color;

                // 计算遮罩区域顶点位置
                // 0 outer LeftTop
                vert.position = new Vector3(outerLeftBottomX, outerRightTopY);
                vh.AddVert(vert);
                // 1 outer RightTop
                vert.position = new Vector3(outerRightTopX, outerRightTopY);
                vh.AddVert(vert);
                // 2 outer RightBottom
                vert.position = new Vector3(outerRightTopX, outerLeftBottomY);
                vh.AddVert(vert);
                // 3 outer LeftBottom
                vert.position = new Vector3(outerLeftBottomX, outerLeftBottomY);
                vh.AddVert(vert);

         
                // 计算镂空区域顶点位置
                // 4 outer LeftTop
                vert.position = new Vector3(m_TargetBoundsMin.x, m_TargetBoundsMax.y);
                vh.AddVert(vert);
                // 5 inner RightTop
                vert.position = new Vector3(m_TargetBoundsMax.x, m_TargetBoundsMax.y);
                vh.AddVert(vert);
                // 6 inner RightBottom
                vert.position = new Vector3(m_TargetBoundsMax.x, m_TargetBoundsMin.y);
                vh.AddVert(vert);
                // 7 inner LeftBottom
                vert.position = new Vector3(m_TargetBoundsMin.x, m_TargetBoundsMin.y);
                vh.AddVert(vert);

                // 向缓冲区中添加三角形
                vh.AddTriangle(4, 0, 1);
                vh.AddTriangle(4, 1, 5);
                vh.AddTriangle(5, 1, 2);
                vh.AddTriangle(5, 2, 6);
                vh.AddTriangle(6, 2, 3);
                vh.AddTriangle(6, 3, 7);
                vh.AddTriangle(7, 3, 0);
                vh.AddTriangle(7, 0, 4);
            }
            else
            {
                int segements = 50;
                int verticeCount = segements * 2;
                int triangleCount;
                float curDegree = 0;
                float degreeDelta = (float)(2 * Mathf.PI / segements);

                float tw = Mathf.Abs(m_TargetBoundsMax.x - m_TargetBoundsMin.x);
                float th = Mathf.Abs(m_TargetBoundsMax.y - m_TargetBoundsMin.y);
                float radius = tw/2;

                Vector4 uv = Vector4.zero;
                Bounds bounds = RectTransformUtility.CalculateRelativeRectTransformBounds(rectTransform, rectTransform);

                float uvCenterX = (uv.x + uv.z) * 0.5f;
                float uvCenterY = (uv.y + uv.w) * 0.5f;
                float uvScaleX = (uv.z - uv.x) / tw;
                float uvScaleY = (uv.w - uv.y) / th;

                Vector3 curVertice;
                for (int i = 0; i < verticeCount; i += 2)
                {
                    float cosA = Mathf.Cos(curDegree);
                    float sinA = Mathf.Sin(curDegree);
                    curDegree += degreeDelta;

                    curVertice = new Vector3(cosA * radius + m_TargetBoundsCenter.x, sinA * radius + m_TargetBoundsCenter.y);
                    UIVertex vert = new UIVertex();
                    vert.color = color;
                    vert.position = curVertice;
                    vert.uv0 = new Vector2(curVertice.x * uvScaleX + uvCenterX, curVertice.y * uvScaleY + uvCenterY);
                    vh.AddVert(vert);

                    float width = Mathf.Abs(bounds.max.x - bounds.min.x);
                    float height = Mathf.Abs(bounds.max.y - bounds.min.y);

                    float x = Mathf.Clamp(cosA * width, bounds.min.x, bounds.max.x);
                    float y = Mathf.Clamp(sinA * height, bounds.min.y, bounds.max.y);

                    curVertice = new Vector3(x, y);
                    vert = new UIVertex();
                    vert.color = color;
                    vert.position = curVertice;
                    vert.uv0 = new Vector2(curVertice.x * uvScaleX + uvCenterX, curVertice.y * uvScaleY + uvCenterY);
                    vh.AddVert(vert);
                }

                triangleCount = segements * 3 * 2;

                for (int i = 0, vIdx = 0; i < triangleCount - 6; i += 6, vIdx += 2)
                {
                    vh.AddTriangle(vIdx + 1, vIdx, vIdx + 3);
                    vh.AddTriangle(vIdx, vIdx + 2, vIdx + 3);
                }

                vh.AddTriangle(verticeCount - 1, verticeCount - 2, 1);
                vh.AddTriangle(verticeCount - 2, 0, 1);
            }
        }

        bool ICanvasRaycastFilter.IsRaycastLocationValid(Vector2 screenPoint, UnityEngine.Camera eventCamera)
        {
            if (!m_CanMask)
            {
                return true;
            }

            Vector2 local;
            RectTransformUtility.ScreenPointToLocalPointInRectangle(rectTransform, screenPoint, eventCamera, out local);

            if (m_MaskType == MaskType.Rectangle)
            {
                if(local.x >= m_TargetBoundsMin.x && local.x <= m_TargetBoundsMax.x && local.y >= m_TargetBoundsMin.y && local.y <= m_TargetBoundsMax.y)
                {
                    return false;
                }
            }
            else
            {
                float radius = Mathf.Abs(m_TargetBoundsMax.x - m_TargetBoundsMin.x) / 2f;

                if ((local.x - m_TargetBoundsCenter.x) * (local.x - m_TargetBoundsCenter.x) + (local.y - m_TargetBoundsCenter.y) * (local.y - m_TargetBoundsCenter.y) <= radius * radius)
                {
                    return false;
                }
            }

            return true;
        }


        private MaskType m_MaskType = MaskType.Rectangle;
        private Vector3 m_TargetBoundsCenter;
        private Vector3 m_TargetBoundsMin;
        private Vector3 m_TargetBoundsMax;
        private bool m_CanMask = false;
    }
}