using UnityEngine;

namespace GameFrameWork.Input
{
    public class AxisArgs : BaseEventArgs
    {
        public string horizontal { get; set; }
        public string vertical { get; set; }
        public KeyCode keyCodeHorizontalPositive { get; set; }
        public KeyCode keyCodeHorizontalNegative { get; set; }
        public KeyCode keyCodeVerticalPositive { get; set; }
        public KeyCode keyCodeVerticalNegative { get; set; }

        public static AxisArgs Create(string horizontal, string vertical, KeyCode keyCodeHorizontalPositive, KeyCode keyCodeHorizontalNegative, KeyCode keyCodeVerticalPositive, KeyCode keyCodeVerticalNegative)
        {
            AxisArgs args = ReferencePool.Acquire<AxisArgs>();
            args.horizontal = horizontal;
            args.vertical = vertical;
            args.keyCodeHorizontalPositive = keyCodeHorizontalPositive;
            args.keyCodeHorizontalNegative = keyCodeHorizontalNegative;
            args.keyCodeVerticalPositive = keyCodeVerticalPositive;
            args.keyCodeVerticalNegative = keyCodeVerticalNegative;
            return args;
        }

        public override void Clear()
        {
            base.Clear();
            horizontal = null;
            vertical = null;
            keyCodeHorizontalPositive = KeyCode.None;
            keyCodeHorizontalNegative = KeyCode.None;
            keyCodeVerticalPositive = KeyCode.None;
            keyCodeVerticalNegative = KeyCode.None;
        }
    }
}