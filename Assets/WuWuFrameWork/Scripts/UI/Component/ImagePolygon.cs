using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Sprites;
using UnityEngine.UI;

namespace WuWuFramework.UI
{
    [AddComponentMenu("UI/ImagePolygon")]
    public class ImagePolygon : ImageEx
    {
        protected override void Awake()
        {
            innerVertices = new List<Vector3>();
            outterVertices = new List<Vector3>();
        }

        void Update()
        {
            this.thickness = Mathf.Clamp(this.thickness, 0, rectTransform.rect.width / 2);
        }
        
        [Range(0, 1)]
        public float fillPercent = 1f;
        public bool useRadar;
        public float thickness = 5;
        [Range(3, 50)]
        public int segements = 20;
        public int maxScore = 100;
        public int[] scores;
        private List<Vector3> innerVertices;
        private List<Vector3> outterVertices;

        protected override void OnPopulateMesh(VertexHelper vh)
        {
            vh.Clear();
            innerVertices.Clear();
            outterVertices.Clear();
            int curSegements = (int)(segements * fillPercent);
            float degreeDelta = 2 * Mathf.PI / segements;
            float tw = rectTransform.rect.width;
            float th = rectTransform.rect.height;
            float outerRadius = rectTransform.pivot.x * tw;
            float innerRadius = rectTransform.pivot.x * tw - thickness;
            Vector4 uv = overrideSprite != null ? DataUtility.GetOuterUV(overrideSprite) : Vector4.zero;
            float uvCenterX = (uv.x + uv.z) * 0.5f;
            float uvCenterY = (uv.y + uv.w) * 0.5f;
            float uvScaleX = (uv.z - uv.x) / tw;
            float uvScaleY = (uv.w - uv.y) / th;
            float curDegree = 0;
            UIVertex uiVertex;
            int verticeCount;
            int triangleCount;
            Vector2 curVertice;

            if (fillCenter) //圆形
            {
                curVertice = Vector2.zero;
                verticeCount = curSegements + 1;
                uiVertex = new UIVertex();
                uiVertex.color = color;
                uiVertex.position = curVertice;
                uiVertex.uv0 = new Vector2(curVertice.x * uvScaleX + uvCenterX, curVertice.y * uvScaleY + uvCenterY);
                vh.AddVert(uiVertex);

                for (int i = 1; i < verticeCount; i++)
                {
                    float cosA = Mathf.Cos(curDegree);
                    float sinA = Mathf.Sin(curDegree);
                    curVertice = new Vector2(cosA * outerRadius, sinA * outerRadius);
                    curDegree += degreeDelta;
                    uiVertex = new UIVertex();
                    uiVertex.color = color;
                    uiVertex.position = curVertice * GetScorePercent(i - 1);
                    uiVertex.uv0 = new Vector2(curVertice.x * uvScaleX + uvCenterX, curVertice.y * uvScaleY + uvCenterY);
                    vh.AddVert(uiVertex);
                    outterVertices.Add(curVertice);
                }

                triangleCount = curSegements * 3;
                
                for (int i = 0, vIdx = 1; i < triangleCount - 3; i += 3, vIdx++)
                {
                    vh.AddTriangle(vIdx, 0, vIdx + 1);
                }
                
                if (Mathf.Approximately(fillPercent, 1))
                {
                    //首尾顶点相连
                    vh.AddTriangle(verticeCount - 1, 0, 1);
                }
            }
            else//圆环
            {
                verticeCount = curSegements * 2;
                for (int i = 0; i < verticeCount; i += 2)
                {
                    float cosA = Mathf.Cos(curDegree);
                    float sinA = Mathf.Sin(curDegree);
                    curDegree += degreeDelta;

                    curVertice = new Vector3(cosA * innerRadius, sinA * innerRadius);
                    uiVertex = new UIVertex();
                    uiVertex.color = color;
                    uiVertex.position = curVertice * GetScorePercent(i);
                    uiVertex.uv0 = new Vector2(curVertice.x * uvScaleX + uvCenterX, curVertice.y * uvScaleY + uvCenterY);
                    vh.AddVert(uiVertex);
                    innerVertices.Add(curVertice);

                    curVertice = new Vector3(cosA * outerRadius, sinA * outerRadius);
                    uiVertex = new UIVertex();
                    uiVertex.color = color;
                    uiVertex.position = curVertice;
                    uiVertex.uv0 = new Vector2(curVertice.x * uvScaleX + uvCenterX, curVertice.y * uvScaleY + uvCenterY);
                    vh.AddVert(uiVertex);
                    outterVertices.Add(curVertice);
                }

                triangleCount = curSegements * 3 * 2;
                
                for (int i = 0, vIdx = 0; i < triangleCount - 6; i += 6, vIdx += 2)
                {
                    vh.AddTriangle(vIdx + 1, vIdx, vIdx + 3);
                    vh.AddTriangle(vIdx, vIdx + 2, vIdx + 3);
                }
                if (Mathf.Approximately(fillPercent, 1))
                {
                    //首尾顶点相连
                    vh.AddTriangle(verticeCount - 1, verticeCount - 2, 1);
                    vh.AddTriangle(verticeCount - 2, 0, 1);
                }
            }
        }

        private float GetScorePercent(int index)
        {
            float score = scores != null && scores.Length > index ? scores[index] : maxScore;
            return Mathf.Clamp(score, 0, maxScore) / maxScore;
        }

        public override bool IsRaycastLocationValid(Vector2 screenPoint, UnityEngine.Camera eventCamera)
        {
            if (overrideSprite == null)
                return true;

            RectTransformUtility.ScreenPointToLocalPointInRectangle(rectTransform, screenPoint, eventCamera, out Vector2 local);
            return Contains(local, outterVertices, innerVertices);
        }

        private bool Contains(Vector2 p, List<Vector3> outterVertices, List<Vector3> innerVertices)
        {
            var crossNumber = 0;
            RayCrossing(p, innerVertices, ref crossNumber);//检测内环
            RayCrossing(p, outterVertices, ref crossNumber);//检测外环
            return (crossNumber & 1) == 1;
        }

        /// <summary>
        /// 使用RayCrossing算法判断点击点是否在封闭多边形里
        /// </summary>
        private void RayCrossing(Vector2 p, List<Vector3> vertices, ref int crossNumber)
        {
            for (int i = 0, count = vertices.Count; i < count; i++)
            {
                var v1 = vertices[i];
                var v2 = vertices[(i + 1) % count];

                //点击点水平线必须与两顶点线段相交
                if (((v1.y <= p.y) && (v2.y > p.y))
                    || ((v1.y > p.y) && (v2.y <= p.y)))
                {
                    //只考虑点击点右侧方向，点击点水平线与线段相交，且交点x > 点击点x，则crossNumber+1
                    if (p.x < v1.x + (p.y - v1.y) / (v2.y - v1.y) * (v2.x - v1.x))
                    {
                        crossNumber += 1;
                    }
                }
            }
        }
    }
}