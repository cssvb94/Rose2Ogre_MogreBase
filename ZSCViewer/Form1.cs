using Revise.STB;
using Revise.ZON;
using Revise.ZSC;
using System;
using System.Data;
using System.Linq;
using System.Windows.Forms;

namespace ZSCViewer
{
    public partial class Form1 : Form
    {
        private ModelListFile zsc;
        private DataFile stb;
        private ZoneFile zon;

        public Form1()
        {
            InitializeComponent();
        }

        private void open_zsc_Click(object sender, EventArgs e)
        {

        }

        private void objects_view_RowEnter(object sender, DataGridViewCellEventArgs e)
        {
            var row_idx = e.RowIndex;
            parts_view.DataSource = zsc.Objects[row_idx].Parts
                .Where(p => p.Model >= 0 && p.Model < zsc.ModelFiles.Count && p.Texture >= 0 && p.Texture < zsc.TextureFiles.Count)
                .Select(part_idx => new
                {
                    Model = zsc.ModelFiles[part_idx.Model],
                    Texture = zsc.TextureFiles[part_idx.Texture].FilePath
                }).ToList();
            if (zsc.EffectFiles.Any())
            {
                effects_view.DataSource = zsc.Objects[row_idx].Effects
                    .Where(eff => eff.Effect >= 0 && eff.Effect < zsc.EffectFiles.Count)
                    .Select(eff_idx => new
                    {
                        Effect = zsc.EffectFiles[eff_idx.Effect]
                    }).ToList();
            }
            else
            {
                effects_view.DataSource = zsc.Objects[row_idx].Effects
                    .Select(eff => new
                    {
                        Effect = eff.Effect
                    }).ToList();
            }

        }

        private void open_stb_Click(object sender, EventArgs e)
        {

        }

        private void openZSCToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (open_zsc_dialog.ShowDialog() == DialogResult.OK)
            {
                zsc = new ModelListFile();
                try
                {
                    parts_view.DataSource = null;
                    effects_view.DataSource = null;

                    zsc.Load(open_zsc_dialog.FileName);
                }
                catch
                {
                    throw;
                }
                tabControl1.SelectedTab = zsc_page;
                var data = zsc.Objects.Select((o, index) =>
                new
                {
                    Index = index,
                    Description = $"Parts: {o.Parts.Count} Effects: {o.Effects.Count}"
                }).ToList();

                objects_view.DataSource = data;
                Text = $"ZSC Viewer: \"{open_zsc_dialog.FileName}\"";
            }
        }

        private void openSTBToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (open_stb_dialog.ShowDialog() == DialogResult.OK)
            {
                stb = new DataFile();
                try
                {
                    stb.Load(open_stb_dialog.FileName);
                }
                catch (Exception)
                {

                    throw;
                }
                stb_view.DataSource = null;
                stb_view.Rows.Clear();
                stb_view.Columns.Clear();

                Text = $"STB Viewer: \"{open_stb_dialog.FileName}\"";
                tabControl1.SelectedTab = stb_page;

                stb_view.Columns.Add("0", "0");
                for (int column_idx = 0; column_idx < stb.ColumnCount; column_idx++)
                {
                    stb_view.Columns.Add($"{column_idx + 1}", $"[{column_idx + 1}] {stb.GetColumnName(column_idx)}");
                }

                for (int row_idx = 0; row_idx < stb.RowCount; row_idx++)
                {
                    stb_view.Rows.Add(stb.RowsData(row_idx).ToArray());
                }
            }
        }

        private void openZONToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if(open_zon_dialog.ShowDialog() == DialogResult.OK)
            {
                zon = new ZoneFile();
                try
                {
                    zon.Load(open_zon_dialog.FileName);
                }
                catch (Exception)
                {
                    throw;
                }
                tabControl1.SelectedTab = zon_page;
                zon_view.DataSource = zon;
            }
        }
    }
}
