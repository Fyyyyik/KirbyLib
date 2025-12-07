using KirbyLib.Crypto;
using KirbyLib.IO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KirbyLib
{
    /// <summary>
    /// File format for scene preload information.
    /// </summary>
    public class FDG
    {
        /// <summary>
        /// Information about a single scene that contains information on which assets to load.
        /// </summary>
        public class Scene
        {
            /// <summary>
            /// The name of the scene.
            /// </summary>
            public string Name = "";
            /// <summary>
            /// This scene can be accessed directly by the FDG Manager if set.
            /// </summary>
            public bool Public = false;
            /// <summary>
            /// A list of scenes that this scene will also load.
            /// </summary>
            public List<Scene> Dependencies = new List<Scene>();
            /// <summary>
            /// The list of assets that this scene will load.
            /// </summary>
            public List<string> Assets = new List<string>();

            public bool HasDependency(string name)
            {
                return Dependencies.Any(x => x.Name == name);
            }
        }

        public const uint FDG_MAGIC = 0x46444748; //"FDGH" in hex; unlike other formats, this can't be a char array

        public XData XData { get; private set; } = new XData();

        public List<Scene> Scenes = new List<Scene>();

        /// <summary>
        /// FDG Version.<br/>
        /// <list type="bullet">
        ///     <item><b>2</b>: 32-bit string offsets.<br/>Used in Kirby Fighters 2 and earlier.</item>
        ///     <item><b>3</b>: 64-bit string offsets. Strings are also hashed with FNV-1a.<br/>Used in Kirby and the Forgotten Land and later.</item>
        /// </list>
        /// </summary>
        public int Version = 2;

        public FDG() { }

        public FDG(EndianBinaryReader reader)
        {
            Read(reader);
        }

        public void Read(EndianBinaryReader reader)
        {
            XData.Read(reader);

            uint magic = reader.ReadUInt32();
            if (magic != FDG_MAGIC)
                throw new InvalidDataException("FDG magic \"FDGH\" not found!");

            Version = reader.ReadInt32();

            uint publicScnSection = reader.ReadUInt32();
            uint sceneSection = reader.ReadUInt32();
            uint assetSection = reader.ReadUInt32();

            reader.BaseStream.Position = assetSection;
            List<string> assetList = new List<string>();
            if (Version > 2)
            {
                uint count = reader.ReadUInt32();
                for (uint i = 0; i < count; i++)
                {
                    //The first entry is the string's FNV-1a hash, we don't need it when reading because we can just calculate it later
                    reader.BaseStream.Position = assetSection + 8 + (i * 0x10) + 8;
                    assetList.Add(reader.ReadStringOffset());
                }
            }
            else
            {
                int count = reader.ReadInt32();
                for (int i = 0; i < count; i++)
                {
                    reader.BaseStream.Position = assetSection + 4 + (i * 4);
                    assetList.Add(reader.ReadStringOffset());
                }
            }

            reader.BaseStream.Position = sceneSection;
            Scenes = new List<Scene>();
            uint sceneCount = reader.ReadUInt32();
            for (uint i = 0; i < sceneCount; i++)
            {
                Scene scene = new Scene();

                reader.BaseStream.Position = sceneSection + 4 + (i * 0xC);

                scene.Name = reader.ReadStringOffset();
                uint deps = reader.ReadUInt32();
                uint assets = reader.ReadUInt32();

                // Dependencies are handled second

                reader.BaseStream.Position = assets;
                uint assetCount = reader.ReadUInt32();
                for (uint a = 0; a < assetCount; a++)
                {
                    int assetIdx = reader.ReadInt32();
                    if (assetIdx < 0 || assetIdx >= assetList.Count)
                        throw new IndexOutOfRangeException($"Asset {i} in scene \"{scene.Name}\" indexes out of asset array bounds. (Offset 0x{reader.BaseStream.Position - 4:X8})");

                    // Asset references are stored as indexes into the asset section
                    scene.Assets.Add(assetList[assetIdx]);
                }

                Scenes.Add(scene);
            }

            // Get dependencies
            for (int i = 0; i < sceneCount; i++)
            {
                Scene scene = Scenes[i];

                reader.BaseStream.Position = sceneSection + 4 + (i * 0xC) + 4; // Skip name offset

                uint deps = reader.ReadUInt32();

                reader.BaseStream.Position = deps;
                uint depCount = reader.ReadUInt32();
                for (uint d = 0; d < depCount; d++)
                {
                    reader.BaseStream.Position = deps + 4 + (d * 4);

                    int depIdx = reader.ReadInt32();
                    if (depIdx < 0 || depIdx >= sceneCount)
                        throw new IndexOutOfRangeException($"Dependency {i} in scene \"{scene.Name}\" indexes out of scene array bounds. (Offset 0x{reader.BaseStream.Position - 4:X8})");

                    // Dependency references are stored as indexes into the scenes section
                    scene.Dependencies.Add(GetScene(depIdx));
                }
            }

            reader.BaseStream.Position = publicScnSection;
            uint publicScnCount = reader.ReadUInt32();
            for (uint i = 0; i < publicScnCount; i++)
            {
                int scnIdx = reader.ReadInt32();
                if (scnIdx < 0 || scnIdx >= Scenes.Count)
                    throw new IndexOutOfRangeException($"Public Scene entry {i} indexes out of scene array bounds. (Offset 0x{reader.BaseStream.Position - 4:X8})");

                Scenes[scnIdx].Public = true;
            }
        }

        public void Write(EndianBinaryWriter writer)
        {
            XData.WriteHeader(writer);

            StringHelperContainer strings = new StringHelperContainer();

            long headerStart = writer.BaseStream.Position;

            writer.Write(FDG_MAGIC);
            writer.Write(Version);
            writer.Write(-1);
            writer.Write(-1);
            writer.Write(-1);

            List<string> assetList = new List<string>();
            for (int i = 0; i < Scenes.Count; i++)
            {
                foreach (string str in Scenes[i].Assets)
                {
                    if (!assetList.Contains(str))
                        assetList.Add(str);
                }
            }
            assetList = assetList.Order(StringComparer.Ordinal).ToList();

            writer.WritePositionAt(headerStart + 0x8);
            writer.Write(Scenes.Count(x => x.Public));
            for (int i = 0; i < Scenes.Count; i++)
            {
                if (Scenes[i].Public)
                    writer.Write(i);
            }

            long sceneSection = writer.BaseStream.Position;
            writer.WritePositionAt(headerStart + 0xC);
            writer.Write(Scenes.Count);
            for (int i = 0; i < Scenes.Count; i++)
            {
                writer.Write(-1);
                writer.Write(-1);
                writer.Write(-1);
            }

            for (int i = 0; i < Scenes.Count; i++)
            {
                var scene = Scenes[i];

                long scenePos = sceneSection + 4 + (i * 0xC);
                writer.WritePositionAt(scenePos);
                writer.WriteStringHAL(scene.Name);

                writer.WritePositionAt(scenePos + 0x4);
                writer.Write(scene.Dependencies.Count);
                for (int d = 0; d < scene.Dependencies.Count; d++)
                {
                    int idx = Scenes.FindIndex(x => x.Name == scene.Dependencies[d].Name);
                    if (idx < 0)
                        throw new KeyNotFoundException($"Scene {scene.Name} depends on scene {scene.Dependencies[d].Name}, but it is not present in the scene list!");
                    writer.Write(idx);
                }

                writer.WritePositionAt(scenePos + 0x8);
                writer.Write(scene.Assets.Count);
                for (int a = 0; a < scene.Assets.Count; a++)
                    writer.Write(assetList.IndexOf(scene.Assets[a]));
            }

            long stringSection = writer.BaseStream.Position;
            writer.WritePositionAt(headerStart + 0x10);
            writer.Write(assetList.Count);
            if (Version > 2)
            {
                for (int i = 0; i < assetList.Count; i++)
                {
                    writer.Write(0);
                    writer.Write(FNV1a.Calculate(Encoding.UTF8.GetBytes(assetList[i])));
                    strings.Add(writer.BaseStream.Position, assetList[i]);
                    writer.Write(-1);
                }
            }
            else
            {
                for (int i = 0; i < assetList.Count; i++)
                {
                    strings.Add(writer.BaseStream.Position, assetList[i]);
                    writer.Write(-1);
                }
            }

            strings.WriteAll(writer);

            XData.WriteFilesize(writer);
            XData.WriteFooter(writer);
        }

        /// <summary>
        /// Gets a scene by name.
        /// </summary>
        /// <param name="name">The name of the scene to get.</param>
        /// <returns>The scene by the given name.</returns>
        /// <exception cref="KeyNotFoundException"></exception>
        public Scene GetScene(string name)
        {
            for (int i = 0; i < Scenes.Count; i++)
            {
                if (Scenes[i].Name == name)
                    return Scenes[i];
            }

            throw new KeyNotFoundException($"Scene {name} does not exist in FDG.");
        }

        /// <summary>
        /// Gets a scene by index in the list.
        /// </summary>
        /// <param name="index">The index to get.</param>
        /// <returns>The scene at the given index.</returns>
        public Scene GetScene(int index)
        {
            return Scenes[index];
        }

        /// <summary>
        /// Checks if a scene exists.
        /// </summary>
        /// <param name="name">The name of the scene to find.</param>
        /// <returns>True if the scene exists.</returns>
        public bool SceneExists(string name)
        {
            for (int i = 0; i < Scenes.Count; i++)
            {
                if (Scenes[i].Name == name)
                    return true;
            }

            return false;
        }

        /// <summary>
        /// Adds a dependency to a scene by name. If it does not exist, a new one is created and appended to the scene list.
        /// </summary>
        /// <param name="scene">The scene to add a dependency to.</param>
        /// <param name="name">The name of the scene to add as a dependency.</param>
        /// <returns>The scene added as a dependency.</returns>
        public Scene AddDependency(Scene scene, string name)
        {
            if (!scene.HasDependency(name))
            {
                Scene scn;
                if (!SceneExists(name))
                {
                    scn = new Scene() { Name = name };
                    Scenes.Add(scn);
                }
                else
                    scn = GetScene(name);

                scene.Dependencies.Add(scn);
                return scn;
            }

            return scene.Dependencies.First(x => x.Name == name);
        }

        private string[] RecurseAssets(Scene scene)
        {
            List<string> assets = new List<string>();
            assets.AddRange(scene.Assets);
            for (int i = 0; i < scene.Dependencies.Count; i++)
            {
                foreach (string a in RecurseAssets(scene.Dependencies[i]))
                {
                    if (!assets.Contains(a))
                        assets.Add(a);
                }
            }

            return assets.ToArray();
        }

        /// <summary>
        /// Gets the full asset list of a given scene, including all assets of its dependencies.
        /// </summary>
        /// <param name="scene">The scene to get the assets of.</param>
        /// <returns>An array of all assets loaded by this scene.</returns>
        public string[] GetFullAssetList(Scene scene)
        {
            return RecurseAssets(scene);
        }

        public Scene this[string key] => GetScene(key);
        public Scene this[int index] => GetScene(index);
    }
}
