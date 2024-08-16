using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Forms;
using Toolbox.Library.Forms;
using Toolbox.Library.IO;
using static SFGraphics.GLObjects.GLObject;
using System.Reflection;

namespace Toolbox.Library
{
    public class ExplorerFile : TreeNodeCustom
    {
        private string filePath;

        public ExplorerFile(string path)
        {
            filePath = path;
            Text = Path.GetFileName(filePath);

            ImageKey = "fileBlank";
            SelectedImageKey = "fileBlank";

            if (!CheckSupport())
            {
                ForeColor = FormThemes.BaseTheme.DisabledItemColor;
            }
        }

        public override void OnClick(TreeView treeview)
        {
            ArchiveFilePanel editor = (ArchiveFilePanel)LibraryGUI.GetActiveContent(typeof(ArchiveFilePanel));
            if (editor == null)
            {
                editor = new ArchiveFilePanel();
                editor.Dock = DockStyle.Fill;
                LibraryGUI.LoadEditor(editor);
            }

            var info = new ArchiveFileInfo()
            {
                FileDataStream = new FileStream(filePath, FileMode.Open, FileAccess.Read),
                FileName = Text,
            };

            editor.LoadFile(info);
            editor.UpdateEditor();
        }

        private IFileFormat OpenFile()
        {
            return STFileLoader.OpenFileFormat(new FileStream(filePath, FileMode.Open, FileAccess.Read),
              filePath, true);
        }

        public override void OnDoubleMouseClick(TreeView treeview)
        {
            IFileFormat file = OpenFile();
            if (file == null) //Format not supported so return
                return;

            if (Utils.HasInterface(file.GetType(), typeof(IEditor<>)))
            {
                OpenControlDialog(file);
            }
            else if (file is IArchiveFile)
            {
                var FileRoot = new ArchiveRootNodeWrapper(file.FileName, (IArchiveFile)file);
                FileRoot.FillTreeNodes();

                if (file is TreeNode) //It can still be both, so add all it's nodes
                {
                    foreach (TreeNode n in ((TreeNode)file).Nodes)
                        FileRoot.Nodes.Add(n);
                }

                ReplaceNode(this.Parent, this, FileRoot);
            }
            else if (file is TreeNode) {
                ReplaceNode(this.Parent, this, (TreeNode)file);
            }
        }

        private void OpenControlDialog(IFileFormat fileFormat)
        {
            UserControl form = GetEditorControl(fileFormat);

            form.Text = (((IFileFormat)fileFormat).FileName);

            var parentForm = LibraryGUI.GetActiveForm();

            GenericEditorForm editorForm = new GenericEditorForm(true, form);
            editorForm.FormClosing += (sender, e) => FormClosing(sender, e, fileFormat);
            if (editorForm.ShowDialog() == DialogResult.OK)
            {
            }
        }
        private void FormClosing(object sender, EventArgs args, IFileFormat fileFormat)
        {
            if (((Form)sender).DialogResult != DialogResult.OK)
                return;
        }

        public UserControl GetEditorControl(IFileFormat fileFormat)
        {
            Type objectType = fileFormat.GetType();
            foreach (var inter in objectType.GetInterfaces())
            {
                if (inter.IsGenericType && inter.GetGenericTypeDefinition() == typeof(IEditor<>))
                {
                    System.Reflection.MethodInfo method = objectType.GetMethod("OpenForm");
                    System.Reflection.MethodInfo methodFill = fileFormat.GetType().GetMethod("FillEditor");

                    var control = (UserControl)method.Invoke(fileFormat, new object[0]);
                    methodFill.Invoke(fileFormat, new object[1] { control });
                    return control;
                }
            }
            return null;
        }

        private static void ReplaceNode(TreeNode parentNode, TreeNode replaceNode, TreeNode newNode)
        {
            if (newNode == null)
                return;
            TreeView treeview = replaceNode.TreeView;
            if (parentNode == null)
            {
                int index = replaceNode.Index;
                treeview.Nodes.RemoveAt(index);
                treeview.Nodes.Insert(index, newNode);
            }
            else
            {
                int index = parentNode.Nodes.IndexOf(replaceNode);
                parentNode.Nodes.RemoveAt(index);
                parentNode.Nodes.Insert(index, newNode);
            }

            newNode.ImageKey = replaceNode.ImageKey;
            newNode.SelectedImageKey = replaceNode.SelectedImageKey;
            replaceNode.Text = newNode.Text;
            treeview.SelectedNode = newNode;

            if (newNode is ISingleTextureIconLoader)
            {
                ObjectEditor editor = LibraryGUI.GetObjectEditor();
                if (editor != null) //The editor isn't always in object editor so check
                    editor.UpdateTextureIcon((ISingleTextureIconLoader)newNode);
            }
        }

        private bool CheckSupport()
        {
         /*   string ext = Utils.GetExtension(filePath);
            foreach (var format in FileManager.GetFileFormats())
            {
                for (int i = 0; i < format.Extension.Length; i++)
                    if (format.Extension[i].Contains(ext))
                        return true;
            }*/

            return false;

                /*    using (var fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read))
                 {
                     if (fileStream.Length < 10)
                         return false;

                       foreach (var format in FileManager.GetFileFormats())
                          {
                              format.FilePath = filePath;
                              format.FileName = Text;
                              if (format.Identify(fileStream))
                                  return true;
                          }
                      }*/


                return false;
        }
    }
}
