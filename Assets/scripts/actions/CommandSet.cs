using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class CommandSet {
    public string Id;
    public string PSXName;
    public string PSPName;

    public string GetName()
    {
        return SettingsManager.PSXTranslation ? PSXName : PSPName;
    }
}
