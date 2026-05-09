using System;
using System.Collections.Generic;

namespace RottenNoble.Core
{
    /// <summary>
    /// MainMenu → InGame 씬 전환 시 전달되는 세션 데이터
    /// </summary>
    public class SessionData
    {
        public static SessionData Current { get; set; }

        public int                              StageId        { get; set; }
        public List<HeroId>                     SelectedHeroes { get; set; } = new();
        public Dictionary<HeroId, HeroSkillLoadout> SkillLoadouts  { get; set; } = new();
    }

    public enum HeroId
    {
        Apolonia,   // Knight
        Lan,        // Archer
        Sybil,      // Thief
        Hina,       // SoulMaster
        Hilda,      // SpellCaster
        Gladis,     // Buffer
    }

    public class HeroSkillLoadout
    {
        public SkillId CW  { get; set; }
        public SkillId CCW { get; set; }
    }

    public enum SkillId
    {
        None,

        // Apolonia
        ShieldBash, IronWall,

        // Lan
        FocusShot, WideShot,

        // Sybil
        CriticalStrike, Smokescreen,

        // Hina
        SoulBlast, SoulAbsorb,

        // Hilda
        FlameBurst, MagicCircle,

        // Gladis
        FocusBuff, PartyBuff,
    }
}
