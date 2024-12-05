using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.IO;

namespace Toolbox.Library.Khronos
{
    public class GLTFWriter : IDisposable
    {
        // Based on the existing DAE writer.
        // Both Collada and GLTF are made by Khronos Group and they're similar.
        private JsonWriter Writer;
        private GLTF.ExportSettings Settings;
        private GLTF.Version Version;

        public GLTFWriter(StreamWriter file, GLTF.ExportSettings settings)
        {
            Settings = settings;
            Version = settings.FileVersion;
            Writer = new JsonTextWriter(file)
            {
                Formatting = Formatting.Indented
                Indentation = 4,
            };
            System.Globalization.CultureInfo customCulture = (System.Globalization.CultureInfo)System.Threading.Thread.CurrentThread.CurrentCulture.Clone();
            customCulture.NumberFormat.NumberDecimalSeparator = ".";

            System.Threading.Thread.CurrentThread.CurrentCulture = customCulture;

            WriteGLTFHeader();
        }
        public void WriteGLTFHeader()
        {
            Writer.WriteStartObject();

            Writer.WritePropertyName("asset");
            Writer.WriteStartObject();
            {
                Writer.WritePropertyName("generator");
                Writer.WriteValue("Switch Toolbox glTF Output");

                Writer.WritePropertyName("generator");
                Writer.WriteValue($"{Version.Major}.{Version.Minor}");
            }
            Writer.WriteEndObject();
        }

        public void Dispose()
        {
            Writer?.WriteEndObject();
            Writer?.Close();
        }
    }
    public class TextureInfo
    {
        public uint Index;
        public uint TexCoord;
    }

    public class Material
    {
        public string Name { get; set; }
        public PbrMetallicRoughness PbrMetallicRoughness { get; set; }
        public TextureInfo NormalTexture { get; set; }
        public TextureInfo OcclusionTexture { get; set; }
        public TextureInfo EmissiveTexture { get; set; }
        public List<float> emissiveFactor = new List<float>();
        public string AlphaMode { get; set; }
        public float AlphaCutoff;
        public bool DoubleSided = false;


        public class PbrMetallicRoughness
        {
            public list<float> BaseColorFactor = new list<float>();
            public TextureInfo BaseColorTexture = new TextureInfo();
            public list<float> MetallicFactor = new list<float>();
            public list<float> RoughnessFactor = new list<float>();
            public TextureInfo MetallicRoughnessTexture = new TextureInfo();
        }
    }

    public class Mesh
    {
        public string Name { get; set; }
        public List<Primitive> Primitives { get; set; }

        public class Primitive
        {
            public object Attributes { get; set; }
            public uint Indices;
            public uint Material;
        }
    }

    public class JointNode

    public class Sampler
    {
        public string Name = { get; set; }
        public int MagFilter;
        public int minFilter;
        public SamplerWrapMode WrapS = SamplerWrapMode.REPEAT;
        public SamplerWrapMode WrapT = SamplerWrapMode.REPEAT;
    }

    public class Image
    {
        public string Name { get; set; }
        public string MimeType = "image/png";
        public string Uri { get; set; }
    }

    public class Skin
    {
        public string Name { get; set; }
        public List<uint> Joints = new List<uint>();
        public uint InverseBindMatrices;
    }

    public enum ComponentType
    {
        BYTE = 5120,
        UNSIGNED_BYTE = 5121,
        SHORT = 5122,
        UNSIGNED_SHORT = 5123,
        UNSIGNED_INT = 5125,
        FLOAT = 5126,
    }

    public enum BufferTarget
    {
        ARRAY_BUFFER = 34962,
        ELEMENT_ARRAY_BUFFER = 34963,
    }

    public enum PrimitiveMode
    {
        POINTS,
        LINES,
        LINE_LOOP,
        LINE_STRIP,
        TRIANGLES,
        TRIANGLE_STRIP,
        TRIANGLE_FAN,
    }

    public enum SamplerFilterMode
    {
        NEAREST = 9728,
        LINEAR = 9729,
        NEAREST_MIPMAP_NEAREST = 9984,
        LINEAR_MIPMAP_NEAREST = 9985,
        NEAREST_MIPMAP_LINEAR = 9986,
        LINEAR_MIPMAP_LINEAR = 9987,
    }

    public enum SamplerWrapMode
    {
        CLAMP_TO_EDGE = 33071,
        MIRRORED_REPEAT = 33648,
        REPEAT = 10497,
    }

}