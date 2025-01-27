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
    public static readonly Vector2 ShadowOffset = new Vector2(0.15f, -0.15f);
    public static readonly int ShadowLayer = -2;
}

public enum AttachmentEffect
{
    NONE,
    PERFECT_ACCURACY,
    MORE_FIREPOWER,
    SILENCED,
    RUBBER_CHICKEN,
    SUBMARINE,
    MORE_AMMO,
    PIGGY_BANK
}
