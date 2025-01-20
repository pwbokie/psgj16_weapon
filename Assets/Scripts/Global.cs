using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum AttachmentType {
    NONE,
    STOCK,
    UNDERBARREL,
    SCOPE,
    MUZZLE,
    MAGAZINE,
    GRIP
}

public static class Global
{
    public static readonly Color ShadowColor = new Color(96, 209, 255, 1f);
    public static readonly Vector2 ShadowOffset = new Vector3(0.1f, -0.1f, 0f);
    public static readonly int ShadowLayer = -2;
}
