using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Runtime.InteropServices;


#if UNITY_IOS || UNITY_TVOS
public static class NativeAPI {
    [DllImport("__Internal")]
    public static extern void loadCocos();
}
#endif
