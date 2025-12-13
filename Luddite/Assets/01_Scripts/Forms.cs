using UnityEngine;

public static class PersonalColors
{
    public static Color GetColors(PlayerClassType _class)
    {
        switch (_class)
        {
            default:
            case PlayerClassType.Knight:
                return new Color(211f/256f, 203f/256f, 11f/256f);
            case PlayerClassType.Archer:
                return new Color(243f / 256f, 146f / 256f, 0f);
            case PlayerClassType.SpellMaster:
                return new Color(0f, 215f / 256f, 47f / 256f);
            case PlayerClassType.Buffer:
                return new Color(0f, 214f / 256f, 50f / 256f);
            case PlayerClassType.SoulMaster:
                return new Color(127f / 256f, 0f, 63f / 256f);
            case PlayerClassType.Thief:
                return new Color(243f / 256f, 0f, 65f / 256f);
        }
    }
    // Silouthette
    public static Color GetCompColors(PlayerClassType _class)
    {
        switch (_class)
        {
            default:
            case PlayerClassType.Knight:
                return new Color(211f / 256f, 203f / 256f, 11f / 256f);
            case PlayerClassType.Archer:
                return new Color(243f / 256f, 146f / 256f, 0f);
            case PlayerClassType.SpellMaster:
                return new Color(0f, 215f / 256f, 47f / 256f);
            case PlayerClassType.Buffer:
                return new Color(0f, 214f / 256f, 50f / 256f);
            case PlayerClassType.SoulMaster:
                return new Color(127f / 256f, 0f, 63f / 256f);
            case PlayerClassType.Thief:
                return new Color(243f / 256f, 0f, 65f / 256f);
        }
    }

    public static Color GetDifficultyColors(Difficulty _diff)
    {
        switch (_diff)
        {
            default:
            case Difficulty.Easy:
                return Color.yellow;
            case Difficulty.Normal:
                return Color.white;
            case Difficulty.Hard:
                return new Color(200f/256f, 3f/256f, 0);
            case Difficulty.Nightmare:
                return new Color(100f/256f, 0f, 145f/256f);
        }

    }
}