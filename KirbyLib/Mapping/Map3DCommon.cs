using KirbyLib.IO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace KirbyLib.Mapping
{
    public class Map3DCommon
    {
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
        /// A Warp Star that appears after all the wave of enemies have been cleared. Unused in Blowout Blast.
        /// </summary>
        public struct WarpStar
        {
            public uint Unknown0x0;
            public Vector3 Position;
        }

        #endregion

        public const uint HEADER_END = 0x12345678;

        public List<Gimmick> Gimmicks { get; set; } = new List<Gimmick>();

        public List<StartPortal> StartPortals { get; set; } = new List<StartPortal>();

        public List<WarpStar> WarpStars { get; set; } = new List<WarpStar>();

        public void ReadGimmicks(EndianBinaryReader reader, uint count)
        {
            for (int i = 0; i < count; i++)
            {
                Gimmick gimmick = new Gimmick();
                gimmick.Kind = reader.ReadUInt32();
                gimmick.Unknown0x4 = reader.ReadUInt32();
                gimmick.Unknown0x8 = reader.ReadUInt32();
                gimmick.Angle = reader.ReadSingle();
                Vector3 pos = new Vector3();
                pos.X = reader.ReadSingle();
                pos.Y = reader.ReadSingle();
                pos.Z = reader.ReadSingle();
                gimmick.Position = pos;
                Gimmicks.Add(gimmick);
            }
        }

        public void ReadStartPortals(EndianBinaryReader reader, uint count)
        {
            for (int i = 0; i < count; i++)
            {
                StartPortal startPortal = new StartPortal();
                startPortal.Unknown0x0 = reader.ReadUInt32();
                startPortal.Unknown0x4 = reader.ReadUInt32();
                startPortal.Unknown0x8 = reader.ReadUInt32();
                startPortal.Angle = reader.ReadSingle();
                Vector3 pos = new Vector3();
                pos.X = reader.ReadSingle();
                pos.Y = reader.ReadSingle();
                pos.Z = reader.ReadSingle();
                startPortal.Position = pos;
                StartPortals.Add(startPortal);
            }
        }

        public void ReadWarpStars(EndianBinaryReader reader, uint count)
        {
            for (int i = 0; i < count; i++)
            {
                WarpStar warpStar = new WarpStar();
                warpStar.Unknown0x0 = reader.ReadUInt32();
                Vector3 pos = new Vector3();
                pos.X = reader.ReadSingle();
                pos.Y = reader.ReadSingle();
                pos.Z = reader.ReadSingle();
                warpStar.Position = pos;
                WarpStars.Add(warpStar);
            }
        }

        public void WriteGimmicks(EndianBinaryWriter writer)
        {
            writer.WritePositionAt(0x2C);
            foreach (Gimmick gimmick in Gimmicks)
            {
                writer.Write(gimmick.Kind);
                writer.Write(gimmick.Unknown0x4);
                writer.Write(gimmick.Unknown0x8);
                writer.Write(gimmick.Angle);
                writer.Write(gimmick.Position.X);
                writer.Write(gimmick.Position.Y);
                writer.Write(gimmick.Position.Z);
            }
        }

        public void WriteStartPortals(EndianBinaryWriter writer)
        {
            writer.WritePositionAt(0x34);
            foreach (StartPortal portal in StartPortals)
            {
                writer.Write(portal.Unknown0x0);
                writer.Write(portal.Unknown0x4);
                writer.Write(portal.Unknown0x8);
                writer.Write(portal.Angle);
                writer.Write(portal.Position.X);
                writer.Write(portal.Position.Y);
                writer.Write(portal.Position.Z);
            }
        }

        public void WriteWarpStars(EndianBinaryWriter writer, bool isKBB)
        {
            if (isKBB)
                writer.WritePositionAt(0x4C);
            else
                writer.WritePositionAt(0x44);

            foreach (WarpStar warpStar in WarpStars)
            {
                writer.Write(warpStar.Unknown0x0);
                writer.Write(warpStar.Position.X);
                writer.Write(warpStar.Position.Y);
                writer.Write(warpStar.Position.Z);
            }
        }
    }
}
