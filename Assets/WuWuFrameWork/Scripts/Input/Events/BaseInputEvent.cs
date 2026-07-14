using System;
using UnityEngine;

namespace WuWuFramework.Input
{
    public abstract class BaseInputEvent
    {
        public abstract Type inputValueType { get; }
        public abstract void Add(InputEventCallType inputEventCallType, object action);
        public abstract void Remove(InputEventCallType inputEventCallType, object action);
        public virtual void Call(InputEventCallType inputEventCallType) { }
        public virtual void Call(InputEventCallType inputEventCallType, Vector2 inputValue) { }
        public virtual void Call(InputEventCallType inputEventCallType, float inputValue) { }
    }
}