using System;
using UnityEngine;
using UnityEngine.EventSystems;

namespace GameFrameWork.UI
{
    public class UIEventArg<T> : BaseEventArgs where T : BaseEventData
    {
        public Type argType { get; private set; }
        public object call { get; private set; }
        public byte byteArg { get; private set; }
        public short shortArg { get; private set; }
        public int intArg { get; private set; }
        public long longArg { get; private set; }
        public float floatArg { get; private set; }
        public double doubleArg { get; private set; }
        public bool boolArg { get; private set; }
        public string stringArg { get; private set; }
        public object objectArg { get; private set; }

        public static UIEventArg<T> Create(GameFrameWorkAction<GameObject, T> call)
        {
            UIEventArg<T> uiEventArg = ReferencePool.Acquire<UIEventArg<T>>();
            uiEventArg.argType = null;
            uiEventArg.call = call;
            return uiEventArg;
        }

        public static UIEventArg<T> Create(GameFrameWorkAction<GameObject, T, byte> call, byte arg)
        {
            UIEventArg<T> uiEventArg = ReferencePool.Acquire<UIEventArg<T>>();
            uiEventArg.argType = typeof(byte);
            uiEventArg.call = call;
            uiEventArg.byteArg = arg;
            return uiEventArg;
        }

        public static UIEventArg<T> Create(GameFrameWorkAction<GameObject, T, short> call, short arg)
        {
            UIEventArg<T> uiEventArg = ReferencePool.Acquire<UIEventArg<T>>();
            uiEventArg.argType = typeof(short);
            uiEventArg.call = call;
            uiEventArg.shortArg = arg;
            return uiEventArg;
        }

        public static UIEventArg<T> Create(GameFrameWorkAction<GameObject, T, int> call, int arg)
        {
            UIEventArg<T> uiEventArg = ReferencePool.Acquire<UIEventArg<T>>();
            uiEventArg.argType = typeof(int);
            uiEventArg.call = call;
            uiEventArg.intArg = arg;
            return uiEventArg;
        }

        public static UIEventArg<T> Create(GameFrameWorkAction<GameObject, T, long> call, long arg)
        {
            UIEventArg<T> uiEventArg = ReferencePool.Acquire<UIEventArg<T>>();
            uiEventArg.argType = typeof(long);
            uiEventArg.call = call;
            uiEventArg.longArg = arg;
            return uiEventArg;
        }

        public static UIEventArg<T> Create(GameFrameWorkAction<GameObject, T, float> call, float arg)
        {
            UIEventArg<T> uiEventArg = ReferencePool.Acquire<UIEventArg<T>>();
            uiEventArg.argType = typeof(float);
            uiEventArg.call = call;
            uiEventArg.floatArg = arg;
            return uiEventArg;
        }

        public static UIEventArg<T> Create(GameFrameWorkAction<GameObject, T, double> call, double arg)
        {
            UIEventArg<T> uiEventArg = ReferencePool.Acquire<UIEventArg<T>>();
            uiEventArg.argType = typeof(double);
            uiEventArg.call = call;
            uiEventArg.doubleArg = arg;
            return uiEventArg;
        }

        public static UIEventArg<T> Create(GameFrameWorkAction<GameObject, T, bool> call, bool arg)
        {
            UIEventArg<T> uiEventArg = ReferencePool.Acquire<UIEventArg<T>>();
            uiEventArg.argType = typeof(bool);
            uiEventArg.call = call;
            uiEventArg.boolArg = arg;
            return uiEventArg;
        }

        public static UIEventArg<T> Create(GameFrameWorkAction<GameObject, T, string> call, string arg)
        {
            UIEventArg<T> uiEventArg = ReferencePool.Acquire<UIEventArg<T>>();
            uiEventArg.argType = typeof(string);
            uiEventArg.call = call;
            uiEventArg.stringArg = arg;
            return uiEventArg;
        }

        public static UIEventArg<T> Create(GameFrameWorkAction<GameObject, T, object> call, object arg)
        {
            UIEventArg<T> uiEventArg = ReferencePool.Acquire<UIEventArg<T>>();
            uiEventArg.argType = typeof(object);
            uiEventArg.call = call;
            uiEventArg.objectArg = arg;
            return uiEventArg;
        }

        public override void Clear()
        {
            call = null;
            byteArg = 0;
            shortArg = 0;
            intArg = 0;
            longArg = 0;
            floatArg = 0;
            doubleArg = 0;
            boolArg = false;
            stringArg = null;
            objectArg = null;
        }
    }
}