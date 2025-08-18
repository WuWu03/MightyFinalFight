using UnityEngine;

namespace GameFrameWork.Map
{
    public class MapUtil
    {
        /// <summary>
        /// 地图坐标转世界坐标
        /// </summary>
        /// <returns></returns>
        public static Vector2 MapPosToWorldPos(Vector2Int map, float scaleX, float scaleY)
        {
            return new Vector2(map.x * scaleX, map.y * scaleY);
        }

        /// <summary>
        /// 四边形坐标转六边形坐标
        /// </summary>
        public static Vector2Int SquarePosToHexagonPos(int x, int y)
        {
            return new Vector2Int(2 * x + y % 2, y);
        }

        /// <summary>
        /// 六边形坐标转四边形坐标
        /// </summary>>
        public static Vector2Int HexagonPosToSquarePos(int x, int y)
        {
            return new Vector2Int((x - y % 2) / 2, y);
        }

        /// <summary>
        /// 某点是否在多边形内
        /// </summary>
        public static bool PolygonContainsPoint(Vector2Int[] polyPoints, Vector2Int pos)
        {
            int j = polyPoints.Length - 1;
            bool inside = false;

            for (int i = 0; i < polyPoints.Length; j = i++)
            {
                Vector2Int pi = polyPoints[i];
                Vector2Int pj = polyPoints[j];

                if (((pi.y >= pos.y && pos.y > pj.y) || (pj.y >= pos.y && pos.y > pi.y)) && (pos.x < (pj.x - pi.x) * (pos.y - pi.y) / (pj.y - pi.y) + pi.x))
                    inside = !inside;
            }

            return inside;
        }

        /// <summary>
        /// 在多边形内随机一点
        /// </summary>
        public Vector2Int PolygonRandomPoints(Vector2Int[] polygonPoints)
        {
            return PolygonRandomPoints(polygonPoints, Rect.zero);
        }

        /// <summary>
        /// 在多边形内随机一点
        /// </summary>
        public static Vector2Int PolygonRandomPoints(Vector2Int[] polygonPoints, Rect vision)
        {
            int minX = polygonPoints[0].x, minY = polygonPoints[0].y;
            int maxX = polygonPoints[0].x, maxY = polygonPoints[0].y;

            for (int i = 0; i < polygonPoints.Length; i++)
            {
                if (vision == Rect.zero)
                {
                    minX = Mathf.Min(minX, polygonPoints[i].x);
                    minY = Mathf.Min(minY, polygonPoints[i].y);
                    maxX = Mathf.Max(maxX, polygonPoints[i].x);
                    maxY = Mathf.Max(maxY, polygonPoints[i].y);
                }
                else
                {
                    minX = Mathf.Min(minX, polygonPoints[i].x, (int)vision.xMin * 100);
                    minY = Mathf.Min(minY, polygonPoints[i].y, (int)vision.yMin * 100);
                    maxX = Mathf.Max(maxX, polygonPoints[i].x, (int)vision.xMax * 100);
                    maxY = Mathf.Max(maxY, polygonPoints[i].y, (int)vision.yMax * 100);
                }
            }

            Vector2Int randomPoint = Vector2Int.zero;
            int currTime = 0;

            do
            {
                randomPoint.x = UnityEngine.Random.Range(minX, maxX);
                randomPoint.y = UnityEngine.Random.Range(minY, maxY);
                currTime++;
            }
            while (!PolygonContainsPoint(polygonPoints, randomPoint) && currTime < 100);

            return randomPoint;
        }
    }
}