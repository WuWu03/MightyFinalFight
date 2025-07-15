namespace GameFrameWork.Input
{
    public class KeyArgs : BaseEventArgs
    {
        public string keyName { get; set; }
        public KeyType keyType { get; set; }
        public KeyType replaceKeyType { get; set; }
        public UnityEngine.KeyCode keyCode { get; set; }
        public bool isTurbo { get; set; }
        public bool isCheckCombo { get; set; }

        public static KeyArgs Create(string keyName, KeyType keyType, KeyType replaceKeyType, UnityEngine.KeyCode keyCode, bool isTurbo, bool isChcekCombo)
        {
            KeyArgs args = ReferencePool.Acquire<KeyArgs>();
            args.keyName = keyName;
            args.keyType = keyType;
            args.replaceKeyType = replaceKeyType;
            args.keyCode = keyCode;
            args.isTurbo = isTurbo;
            args.isCheckCombo = isChcekCombo;
            return args;
        }

        public override void Clear()
        {
            keyName = null;
            keyType = KeyType.None;
            replaceKeyType = KeyType.None;
            keyCode = UnityEngine.KeyCode.None;
            isTurbo = false;
            isCheckCombo = false;
        }
    }
}