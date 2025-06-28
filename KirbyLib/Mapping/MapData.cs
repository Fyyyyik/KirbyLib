using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KirbyLib.Mapping
{
    public struct CollisionTile
    {
        /// <summary>
        /// The collision shape the tile uses.
        /// </summary>
        public LandGridShapeKind Shape;
        /// <summary>
        /// Enables special attributes for the tile.
        /// </summary>
        public LandGridProperty PropertyFlags;
        /// <summary>
        /// Tile material. Typically defines effects when walking and landing.<br/>
        /// <b>Used in:</b>
        /// <list type="bullet">
        ///     <item>Kirby's Return to Dream Land</item>
        ///     <item>Kirby Triple Deluxe</item>
        ///     <item>Kirby Fighters</item>
        ///     <item>Kirby Fighters Deluxe</item>
        ///     <item>Kirby Fighters 2</item>
        /// </list>
        /// </summary>
        public byte Material;
        /// <summary>
        /// The speed at which objects will move while standing on the tile.<br/>Negative values move left.
        /// </summary>
        public sbyte ConveyorSpeed;

        public CollisionTile()
        {
            Shape = LandGridShapeKind.None;
            PropertyFlags = LandGridProperty.None;
            Material = 0;
            ConveyorSpeed = 0;
        }

        public CollisionTile(LandGridShapeKind shape, LandGridProperty prop, sbyte conveyorSpeed)
        {
            Shape = shape;
            PropertyFlags = prop;
            ConveyorSpeed = conveyorSpeed;
        }

        public CollisionTile(LandGridShapeKind shape, LandGridProperty prop, byte material, sbyte conveyorSpeed)
        {
            Shape = shape;
            PropertyFlags = prop;
            Material = material;
            ConveyorSpeed = conveyorSpeed;
        }
    }

    /// <summary>
    /// Representation of a tile within a moving collision group.
    /// </summary>
    public struct MoveGridTile
    {
        /// <summary>
        /// The collision shape the tile uses.
        /// </summary>
        public LandGridShapeKind Shape;
        /// <summary>
        /// Enables special attributes for the tile.
        /// </summary>
        public MoveGridProperty Property;

        public MoveGridTile()
        {
            Shape = LandGridShapeKind.None;
            Property = MoveGridProperty.None;
        }

        public MoveGridTile(LandGridShapeKind shape, MoveGridProperty prop)
        {
            Shape = shape;
            Property = prop;
        }
    }

    public struct DecorationTile
    {
        /// <summary>
        /// The decoration tile ID this tile uses. References bone names in the 3D tileset model.<br/>If -1, no tile is used.
        /// </summary>
        public short Tile;
        public byte Unknown;
        /// <summary>
        /// Binds the tile to a specific moving terrain group.<br/>If -1, the tile is static.
        /// </summary>
        public sbyte Group;

        public DecorationTile()
        {
            Tile = -1;
            Unknown = 0;
            Group = -1;
        }

        public DecorationTile(short tile, byte unk, sbyte group)
        {
            Tile = tile;
            Unknown = unk;
            Group = group;
        }
    }

    /// <summary>
    /// Bitflags that determine special properties for a tile.
    /// </summary>
    [Flags]
    public enum LandGridProperty : ushort
    {
        None = 0x0,
        Unknown_0x1 = 0x1,
        Ladder = 0x2,
        InvisibleLand = 0x4,
        Water = 0x8,
        Spike = 0x10,
        Ice = 0x20,
        Lava = 0x40,
        ItemThroughLand = 0x100,
        NoWallCling = 0x400
    }

    /// <summary>
    /// Enum that determines special properties for a tile within a moving land group.
    /// </summary>
    public enum MoveGridProperty : byte
    {
        None = 0,
        Spike = 4,
        Ice,
        Lava
    }

    /// <summary>
    /// Defines collision shape types.
    /// </summary>
    public enum LandGridShapeKind : byte
    {
        /// <summary>
        /// No collision
        /// </summary>
        None,
        /// <summary>
        /// Square collision
        /// </summary>
        Cube,
        /// <summary>
        /// An ascending 45° grounded slope
        /// </summary>
        FSlopeLS11,
        /// <summary>
        /// The first tile of an ascending 22.5° grounded slope
        /// </summary>
        FSlopeLS21,
        /// <summary>
        /// The last tile of an ascending 22.5° grounded slope
        /// </summary>
        FSlopeLS22,
        /// <summary>
        /// The first tile of an ascending 15° grounded slope
        /// </summary>
        FSlopeLS31,
        /// <summary>
        /// The second tile of an ascending 15° grounded slope
        /// </summary>
        FSlopeLS32,
        /// <summary>
        /// The last tile of an ascending 15° grounded slope
        /// </summary>
        FSlopeLS33,
        /// <summary>
        /// A descending 45° grounded slope
        /// </summary>
        FSlopeRS11,
        /// <summary>
        /// The first tile of a descending 22.5° grounded slope
        /// </summary>
        FSlopeRS21,
        /// <summary>
        /// The second tile of a descending 22.5° grounded slope
        /// </summary>
        FSlopeRS22,
        /// <summary>
        /// The first tile of a descending 15° grounded slope
        /// </summary>
        FSlopeRS31,
        /// <summary>
        /// The second tile of a descending 15° grounded slope
        /// </summary>
        FSlopeRS32,
        /// <summary>
        /// The last tile of a descending 15° grounded slope
        /// </summary>
        FSlopeRS33,
        /// <summary>
        /// A descending 45° ceiling slope
        /// </summary>
        RSlopeLS11,
        /// <summary>
        /// The first tile of a descending 22.5° ceiling slope
        /// </summary>
        RSlopeLS21,
        /// <summary>
        /// The last tile of a descending 22.5° ceiling slope
        /// </summary>
        RSlopeLS22,
        /// <summary>
        /// The first tile of a descending 15° ceiling slope
        /// </summary>
        RSlopeLS31,
        /// <summary>
        /// The second tile of a descending 15° ceiling slope
        /// </summary>
        RSlopeLS32,
        /// <summary>
        /// The last tile of a descending 15° ceiling slope
        /// </summary>
        RSlopeLS33,
        /// <summary>
        /// An ascending 45° ceiling slope
        /// </summary>
        RSlopeRS11,
        /// <summary>
        /// The first tile of an ascending 22.5° ceiling slope
        /// </summary>
        RSlopeRS21,
        /// <summary>
        /// The last tile of an ascending 22.5° ceiling slope
        /// </summary>
        RSlopeRS22,
        /// <summary>
        /// The first tile of an ascending 15° ceiling slope
        /// </summary>
        RSlopeRS31,
        /// <summary>
        /// The second tile of an ascending 15° ceiling slope
        /// </summary>
        RSlopeRS32,
        /// <summary>
        /// The last tile of an ascending 15° ceiling slope
        /// </summary>
        RSlopeRS33,
        /// <summary>
        /// A drop-through platform
        /// </summary>
        BWThroughCT,
        /// <summary>
        /// An ascending 45° drop-through platform
        /// </summary>
        BWThroughLS11,
        /// <summary>
        /// The first tile of an ascending 22.5° drop-through platform
        /// </summary>
        BWThroughLS21,
        /// <summary>
        /// The last tile of an ascending 22.5° drop-through platform
        /// </summary>
        BWThroughLS22,
        /// <summary>
        /// The first tile of an ascending 15° drop-through platform
        /// </summary>
        BWThroughLS31,
        /// <summary>
        /// The second tile of an ascending 15° drop-through platform
        /// </summary>
        BWThroughLS32,
        /// <summary>
        /// The last tile of an ascending 15° drop-through platform
        /// </summary>
        BWThroughLS33,
        /// <summary>
        /// A descending 45° drop-through platform
        /// </summary>
        BWThroughRS11,
        /// <summary>
        /// The first tile of a descending 22.5° drop-through platform
        /// </summary>
        BWThroughRS21,
        /// <summary>
        /// The last tile of a descending 22.5° drop-through platform
        /// </summary>
        BWThroughRS22,
        /// <summary>
        /// The first tile of a descending 15° drop-through platform
        /// </summary>
        BWThroughRS31,
        /// <summary>
        /// The second tile of a descending 15° drop-through platform
        /// </summary>
        BWThroughRS32,
        /// <summary>
        /// The last tile of a descending 15° drop-through platform
        /// </summary>
        BWThroughRS33,
        /// <summary>
        /// A one-way platform
        /// </summary>
        OWThroughCT,
        /// <summary>
        /// An ascending 45° one-way platform
        /// </summary>
        OWThroughLS11,
        /// <summary>
        /// The first tile of an ascending 22.5° one-way platform
        /// </summary>
        OWThroughLS21,
        /// <summary>
        /// The second tile of an ascending 22.5° one-way platform
        /// </summary>
        OWThroughLS22,
        /// <summary>
        /// The first tile of an ascending 15° one-way platform
        /// </summary>
        OWThroughLS31,
        /// <summary>
        /// The second tile of an ascending 15° one-way platform
        /// </summary>
        OWThroughLS32,
        /// <summary>
        /// The last tile of an ascending 15° one-way platform
        /// </summary>
        OWThroughLS33,
        /// <summary>
        /// A descending 45° one-way platform
        /// </summary>
        OWThroughRS11,
        /// <summary>
        /// The first tile of a descending 22.5° one-way platform
        /// </summary>
        OWThroughRS21,
        /// <summary>
        /// The second tile of a descending 22.5° one-way platform
        /// </summary>
        OWThroughRS22,
        /// <summary>
        /// The first tile of a descending 15° one-way platform
        /// </summary>
        OWThroughRS31,
        /// <summary>
        /// The second tile of a descending 15° one-way platform
        /// </summary>
        OWThroughRS32,
        /// <summary>
        /// The last tile of a descending 15° one-way platform
        /// </summary>
        OWThroughRS33
    }

    /// <summary>
    /// Defines how the game will split the level data for the front and back lanes.
    /// </summary>
    public enum ScreenSplitKind : int
    {
        /// <summary>
        /// No lanes.
        /// </summary>
        Normal = 0,
        /// <summary>
        /// Horizontal split, lanes are stacked vertically.
        /// </summary>
        Horizontal,
        /// <summary>
        /// Vertical split, lanes are stacked horizontally.
        /// </summary>
        Vertical
    }

    /// <summary>
    /// Defines how far apart the front and back lanes are.
    /// </summary>
    public enum LanePairKind : int
    {
        NormalFarLv1 = 0,
        NormalFarLv2,
        NormalFarLv3,
        /// <summary>
        /// Only exists in Triple Deluxe, but is unused.
        /// </summary>
        NearLv1Normal
    }
}
