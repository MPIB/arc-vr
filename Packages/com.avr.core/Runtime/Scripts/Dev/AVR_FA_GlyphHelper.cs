using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEditor;

//Head to fontawesome.io/icons/ to get unicode values for symbols

/// <summary>
/// Inspector class that can be used to insert font-awesome symbols into text fields by using their unicode value.
/// For usage: 
/// - Add a script-component of this type to an object which has a Text-component intended as the desired symbol.
/// - Make sure the Text component has the font-awesome selected as the font. It should be located in: Packages\com.avr.core\Runtime\Resources\fa-solid-900.ttf. Alternatively download the newest verison from fontawesome.io
/// - Head on over to fontawesome.io/icons/ and find the desired icond. Once found click on it and copy the respective "unicode" string.
/// - Paste the unicode string into the "unicode" field of this component.
/// - Once the icon appears as desired, remove this component.
/// </summary>
[ExecuteInEditMode]
[RequireComponent(typeof(Text))]
public class AVR_FA_GlyphHelper : MonoBehaviour
{
    Text t;

    public string unicode = "f047";

    void Awake() {
        if(!Application.isEditor) {
            AVR.Core.AVR_DevConsole.cwarn("There is a redundant GlyphHelper object at: "+AVR.Core.Utils.Misc.GetHierarchyPath(transform)+". Consider removing it.", this);
            GameObject.Destroy(this);
        }
    }

    void Start()
    {
        t = GetComponent<Text>();
        char num = (char)int.Parse(unicode, System.Globalization.NumberStyles.HexNumber);
        t.text = num.ToString();
    }

    void OnValidate()
    {
        t = GetComponent<Text>();
        char num = (char)int.Parse(unicode, System.Globalization.NumberStyles.HexNumber);
        t.text = num.ToString();
    }


}
