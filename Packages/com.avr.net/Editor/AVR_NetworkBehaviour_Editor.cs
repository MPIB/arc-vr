using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

using AVR.Core;
using AVR.Net;

namespace AVR.UEditor.Net
{
    [CustomEditor(typeof(AVR_NetworkBehaviour), true)]
    [CanEditMultipleObjects]
    public class AVR_NetworkBehaviour_Editor : AVR.UEditor.Core.AVR_Behaviour_Editor
    {

    }
}
