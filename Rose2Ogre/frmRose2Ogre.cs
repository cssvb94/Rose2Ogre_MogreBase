using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using System.Xml;
using OgreRose;
using RoseFormats;

namespace Rose2Ogre
{
    public partial class frmRose2Ogre : Form
    {
        private string ZMDFile;
        private List<string> ZMSFiles = new List<string>();
        private List<string> ZMOFiles = new List<string>();
        private readonly ZMD zmd = new ZMD();
        private readonly List<ZMS> zms = new List<ZMS>();
        private readonly List<ZMO> zmo = new List<ZMO>();

        private readonly string[] args;
        private List<string> ARGFiles = new List<string>();
        private bool bConvert;
       
        public frmRose2Ogre()
        {
            args = Environment.GetCommandLineArgs();

            InitializeComponent();

            btnConvert.Enabled = IsEnabledConvert();

            #region
            //model = COLLADA.Load("plane.dae");

            //// Iterate on libraries
            //foreach (var item in model.Items)
            //{
            //    var geometries = item as library_geometries;
            //    if (geometries == null)
            //        continue;

            //    // Iterate on geomerty in library_geometries 
            //    foreach (var geom in geometries.geometry)
            //    {
            //        var mesh = geom.Item as mesh;
            //        if (mesh == null)
            //            continue;

            //        // Dump source[] for geom
            //        foreach (var source in mesh.source)
            //        {
            //            var float_array = source.Item as float_array;
            //            if (float_array == null)
            //                continue;

            //            log.Trace("Geometry {0} source {1} : ", geom.id, source.id);
            //            foreach (var mesh_source_value in float_array.Values)
            //                log.Trace("source: {0} ", mesh_source_value);
            //        }

            //        // Dump Items[] for geom
            //        foreach (var meshItem in mesh.Items)
            //        {
            //            if (meshItem is vertices)
            //            {
            //                var vertices = meshItem as vertices;
            //                var inputs = vertices.input;
            //                foreach (var input in inputs)
            //                    log.Trace("Semantic {0} Source {1}", input.semantic, input.source);
            //            }
            //            else if (meshItem is triangles)
            //            {
            //                var triangles = meshItem as triangles;
            //                var inputs = triangles.input;
            //                foreach (var input in inputs)
            //                    log.Trace("Semantic {0} Source {1} Offset {2}", input.semantic, input.source, input.offset);
            //                log.Trace("Indices {0}", triangles.p);
            //            }
            //        }
            //    }
            //}
            #endregion
        }

        #region button events

        private void btnZMD_Click(object sender, EventArgs e)
        {
            dlgOpenFile.Filter = "ROSE Skeleton|*.ZMD";
            dlgOpenFile.Multiselect = false;

            if (dlgOpenFile.ShowDialog() == DialogResult.OK)
            {
                txtZMD.Text = dlgOpenFile.SafeFileName;
                ZMDFile = dlgOpenFile.FileName;
            }
        }

        private void btnZMS_Click(object sender, EventArgs e)
        {
            dlgOpenFile.Filter = "ROSE Mesh|*.ZMS";
            dlgOpenFile.Multiselect = true;
            if (dlgOpenFile.ShowDialog() == DialogResult.OK)
            {
                ZMSFiles.AddRange(dlgOpenFile.FileNames.ToList());
                lstZMS.Items.AddRange(dlgOpenFile.SafeFileNames);
            }
            btnConvert.Enabled = IsEnabledConvert();
        }

        private void btnZMO_Click(object sender, EventArgs e)
        {
            dlgOpenFile.Filter = "ROSE Animation|*.ZMO";
            dlgOpenFile.Multiselect = true;
            if (dlgOpenFile.ShowDialog() == DialogResult.OK)
            {
                ZMOFiles.AddRange(dlgOpenFile.FileNames.ToList());
                lstZMO.Items.AddRange(dlgOpenFile.SafeFileNames);
            }
            btnConvert.Enabled = IsEnabledConvert();
        }

        private void removeSelectedZMS_Click(object sender, EventArgs e)
        {
            if (lstZMS.SelectedIndex > -1)
            {
                ZMSFiles.RemoveAt(lstZMS.SelectedIndex);
                lstZMS.Items.RemoveAt(lstZMS.SelectedIndex);
            }
            btnConvert.Enabled = IsEnabledConvert();
        }

        private void removeAllZMS_Click(object sender, EventArgs e)
        {
            lstZMS.Items.Clear();
            ZMSFiles.Clear();
            btnConvert.Enabled = IsEnabledConvert();
        }

        private void removeSelectedZMO_Click(object sender, EventArgs e)
        {
            if (lstZMO.SelectedIndex > -1)
            {
                ZMOFiles.RemoveAt(lstZMO.SelectedIndex);
                lstZMO.Items.RemoveAt(lstZMO.SelectedIndex);
            }
            btnConvert.Enabled = IsEnabledConvert();
        }

        private void removeAllZMO_Click(object sender, EventArgs e)
        {
            lstZMO.Items.Clear();
            ZMOFiles.Clear();
            btnConvert.Enabled = IsEnabledConvert();
        }
        #endregion

        private bool IsEnabledConvert()
        {
            return lstZMS.Items.Count != 0 || txtZMD.Text.Trim().Length != 0;
        }

        private void Convert()
        {
            string skeletonfile = string.Empty;
            OgreSkeleton skeleton = null;

            zmd.Clear();

            if (!string.IsNullOrEmpty(ZMDFile))
            {
                zmd.Load(ZMDFile);
                skeleton = new OgreSkeleton(zmd);
                skeletonfile = Path.ChangeExtension(Path.GetFileName(ZMDFile), ".skeleton");
            }

            zms.Clear();
            for (int i = 0; i < ZMSFiles.Count; i++)
            {
                zms.Add(new ZMS(ZMSFiles[i]));
                OgreMesh mesh = new OgreMesh(zms[i]);

                if (!string.IsNullOrEmpty(ZMDFile) && zms[i].BonesCount > 0)
                {
                    XmlElement xmlmesh = mesh.XMLDoc.DocumentElement;
                    XmlNode xmlskeletonlink = mesh.XMLDoc.CreateNode(XmlNodeType.Element, "skeletonlink", null);
                    XmlAttribute xmlskeletonname = mesh.XMLDoc.CreateAttribute("name");
                    xmlskeletonname.Value = skeletonfile;
                    xmlskeletonlink.Attributes.Append(xmlskeletonname);
                    xmlmesh.AppendChild(xmlskeletonlink);
                }

                mesh.XMLDoc.Save(Path.ChangeExtension(ZMSFiles[i], ".mesh.xml"));
            }

            if (!string.IsNullOrEmpty(ZMDFile))
            {
                zmo.Clear();
                if ((zmd.Bone.Count > 0) && (ZMOFiles.Count > 0))
                {
                    for (int i = 0; i < ZMOFiles.Count; i++)
                    {
                        zmo.Add(new ZMO(ZMOFiles[i], zmd));
                        OgreAnimation ogreanim = new OgreAnimation(zmd, zmo[i], skeleton.XMLDoc, Path.GetFileNameWithoutExtension(ZMOFiles[i]));
                    }
                }
                skeleton.XMLDoc.Save(Path.ChangeExtension(ZMDFile, ".skeleton.xml"));
            }
        }

        private void btnConvert_Click(object sender, EventArgs e)
        {
            Convert();
            MessageBox.Show("The file(s) are converted to Ogre3D XML format.\nUse Ogre3D SDK 'OgreCommandLineTools\\OgreXmlConverter.exe' to convert it to binary.", "Done");
        }

        private void ProcessFileList(List<string> FileNameList)
        {
            // Get only files that exist
            var existing = from f in FileNameList where File.Exists(f) select f;
            // Separate by types
            var zmss = from f in existing where Path.GetExtension(f).IndexOf("ZMS", StringComparison.OrdinalIgnoreCase) >= 0 select f;
            var zmos = from f in existing where Path.GetExtension(f).IndexOf("ZMO", StringComparison.OrdinalIgnoreCase) >= 0 select f;
            var zmds = from f in existing where Path.GetExtension(f).IndexOf("ZMD", StringComparison.OrdinalIgnoreCase) >= 0 select f;
            
            // Fill in the listboxes and lists

            if (zmds.Any())
            {
                ZMDFile = zmds.ToList()[0];
                txtZMD.Text = Path.GetFileName(ZMDFile);
            }

            if (zmss.Any())
            {
                ZMSFiles.AddRange(zmss.ToList());
                ZMSFiles.Sort();
                ZMSFiles = ZMSFiles.Distinct().ToList();
                lstZMS.Items.Clear();
                foreach (string fzms in ZMSFiles)
                {
                    lstZMS.Items.Add(Path.GetFileName(fzms));
                }
            }


            if (zmos.Any())
            {
                ZMOFiles.AddRange(zmos.ToList());
                ZMOFiles.Sort();
                ZMOFiles = ZMOFiles.Distinct().ToList();
                lstZMO.Items.Clear();
                foreach (string fzmo in ZMOFiles)
                {
                    lstZMO.Items.Add(Path.GetFileName(fzmo));
                }
            }
        }

        private void frmRose2Ogre_Load(object sender, EventArgs e)
        {
            if (args.Length > 1)
            {
                if (!string.IsNullOrEmpty(args[1]))
                {
                    ARGFiles = args.ToList();
                    foreach (string arg in ARGFiles)
                    {
                        bConvert |= arg.Equals("/CONVERT", StringComparison.OrdinalIgnoreCase);
                    }

                    ProcessFileList(ARGFiles);

                    if (bConvert)
                    {
                        Convert();
                    }
                }
            } // count > 0
        } // Load

        private void frmRose2Ogre_Shown(object sender, EventArgs e)
        {
            if (bConvert)
                Close();
        }

        private void frmRose2Ogre_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
                e.Effect = DragDropEffects.Copy;
            else
                e.Effect = DragDropEffects.None;
        }

        private void frmRose2Ogre_DragDrop(object sender, DragEventArgs e)
        {
            try
            {
                Array a = (Array)e.Data.GetData(DataFormats.FileDrop);
                List<string> files = new List<string>();

                foreach (string fname in a)
                {
                    files.Add(fname);   
                }

                ProcessFileList(files);

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error drag'n'drop file(s)");
            }
        }

        private void txtZMD_TextChanged(object sender, EventArgs e)
        {
            btnConvert.Enabled = IsEnabledConvert();
        }
    } // class
}
