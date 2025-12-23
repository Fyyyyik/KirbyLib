using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using static KirbyLib.Mapping.Map3DRumble;

namespace KirbyLib.Mapping
{
    public class MapKBB
    {
        #region Enums

        public enum BinFoodKind : uint
        {
            DinnerCurry,
            DinnerOmelet,
            UNUSED_0,
            UNUSED_1,
            DrinkCreamSoda,
            UNUSED_2,
            FruitCherry,
            UNUSED_3,
            FruitPineApple,
            UNUSED_4,
            UNUSED_5,
            UNUSED_6,
            UNUSED_7,
            UNUSED_8,
            UNUSED_9,
            UNUSED_10,
            UNUSED_11,
            UNUSED_12,
            UNUSED_13,
            UNUSED_14,
            UNUSED_15,
            SweetsPudding,
            SweetsShortCake,
            UNUSED_16,
            VegetableCarrot,
            UNUSED_17,
            SweetsChocolate
        }

        public enum BinItemKind : uint
        {
            PointCoinG,
            PointCoinS,
            PointCoinB,
            Food
        }

        public enum BinObjKind : uint
        {
            Kirby,
            Waddledee,
            Brontoburt,
            Broomhatter,
            Bouncy,
            SpearWaddledee,
            Grizzo,
            Masher,
            Gordo,
            Kabu,
            Scarfy,
            Dedede,
            Nruff,
            Cappy,
            Mumbies,
            Wonkey,
            Lololo,
            Lalala,
            Glunk,
            Kracko,
            Soarar,
            Chip,
            Babut,
            Squishy,
            Sectledee,
            Sectraburt,
            Shotzo,
            KrackoJr,
            SpearSectledee
        }

        public enum BinObjType : uint
        {
            Wait,
            RoundTrip,
            Around,
            Circle,
            Tackle,
            Warp,
            WarpAttack,
            SleepWait,
            HappyWait,
            AngryAppear,
            WaitTackle,
            StraightTackle,
            AlwaysPursuit,
            Escape,
            EscapeStraight,
            EscapeRange,
            EscapeStraightRange,
            JumpPursuit,
            JumpRoundTrip
        }

        #endregion

        #region Structs

        /// <summary>
        /// A decorative object that can be placed in the level.
        /// </summary>
        public struct Gimmick
        {
            public uint Kind;
            public uint Unknown0x4;
            public uint Unknown0x8;
            public float Angle;
            public Vector3 Position;
        }

        /// <summary>
        /// The starting position for Kirby.
        /// </summary>
        public struct StartPortal
        {
            public uint Unknown0x0;
            public uint Unknown0x4;
            public uint Unknown0x8;
            public float Angle;
            public Vector3 Position;
        }

        /// <summary>
        /// A destructible block.
        /// </summary>
        public struct Block
        {
            public uint Kind;
            public GridPos3D GridPosition;
        }

        /// <summary>
        /// A Warp Star that appears after all the wave of enemies have been cleared. Unused in Blowout Blast.
        /// </summary>
        public struct WarpStar
        {
            public uint Unknown0x0;
            public Vector3 Position;
        }

        /// <summary>
        /// An item that can be picked up by running into it.
        /// </summary>
        public struct Item
        {
            public BinItemKind Kind;
            public uint Variation;
            public Vector3 Position;
        }

        #endregion
    }
}
