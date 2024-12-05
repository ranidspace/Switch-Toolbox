using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Collada141;
using System.Runtime.InteropServices;
using System.Diagnostics;
using System.Globalization;
using System.Xml;
using OpenTK;
using Toolbox.Library.Rendering;
using Toolbox.Library.Khronos;
using Toolbox.Library.IO;

namespace Toolbox.Library
{
    public class GLTF
    {
        public class ExportSettings
        {
            public bool UseTextureChannelComponents = true;
            public bool UseVertexColors = true;
            public bool FlipTexCoordsVertical = true;
            public Version FileVersion = new Version();
            public bool ExportTextures = true;
            public string ImageExtension = ".png";
            public string ImageFolder = "";
        }
        public class Version
        {
            public int Major = 2;
            public int Minor = 0;
        }
        public static void Export(string FileName, ExportSettings settings, STGenericObject mesh)
        {
            Export(FileName, settings, new List<STGenericObject>() { mesh },
                new List<STGenericMaterial>(), new List<STGenericTexture>());
        }

        public static void Export(string FileName, ExportSettings settings, STGenericModel model, List<STGenericTexture> Textures, STSkeleton skeleton = null, List<int> NodeArray = null)
        {
            Export(FileName, settings, model.Objects.ToList(), model.Materials.ToList(), Textures, skeleton, NodeArray);
        }
        public static void Export(string FileName, ExportSettings settings,
            List<STGenericObject> Meshes, List<STGenericMaterial> Materials,
            List<STGenericTexture> Textures, STSkeleton skeleton = null, List<int> NodeArray = null)
        {
            List<string> failedTextureExport = new List<string>();

            STProgressBar progressBar = new STProgressBar();
            progressBar.Task = "Exporting Model...";
            progressBar.Value = 0;
            progressBar.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            progressBar.Show();
            progressBar.Refresh();

            string TexturePath = System.IO.Path.GetDirectoryName(FileName);
            Dictionary<string, STGenericMaterial> MaterialRemapper = new Dictionary<string, STGenericMaterial>();

            using (GLTFWriter writer = new GLTFWriter(FileName, settings))
            {
                writer.WriteAsset();
            }
        }
    }
}