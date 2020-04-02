using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FrameWork.LocalData
{
    public static class DataHelper
    {
        public static T[] LoadData<T>(string fileName) where T : AbstractData, new()
        {
            string path = string.Format(ResDefine.ConfigDataPath + "/{0}", fileName);
            T[] t = null;
            using (GameDataTableParser parser = new GameDataTableParser(path))
            {
                t = new T[parser.m_Row - 3];
                int index = 0;
                while (!parser.Eof)
                {
                    t[index] = new T();
                    t[index].Read(parser);
                    parser.Next();
                    index++;
                }
            }
            return t;
        }
    }
}
