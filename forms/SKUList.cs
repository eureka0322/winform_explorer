﻿using mjc_dev.common.components;
using mjc_dev.common;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using mjc_dev.model;
using mjc_dev.forms.modals;

namespace mjc_dev.forms
{
    public partial class SKUList : GlobalLayout
    {
        private HotkeyButton hkAdds = new HotkeyButton("Ins", "Adds", Keys.Insert);
        private HotkeyButton hkDeletes = new HotkeyButton("Del", "Deletes", Keys.Delete);
        private HotkeyButton hkSelects = new HotkeyButton("Enter", "Selects", Keys.Enter);
        private HotkeyButton hkCrossRefLookup = new HotkeyButton("F2", "Cross Ref Lookup", Keys.F2);
        private HotkeyButton hkView = new HotkeyButton("F3", "View", Keys.F3);
        private HotkeyButton hkAdjustQty = new HotkeyButton("F4", "AdjustQty", Keys.F4);
        private HotkeyButton hkSKUHistory = new HotkeyButton("F5", "SKU History", Keys.F5);
        private HotkeyButton hkProfileHistory = new HotkeyButton("F6", "SKU History", Keys.F6);
        private HotkeyButton hkArchivedSKUs = new HotkeyButton("F8", "Archived SKUs", Keys.F8);

        private GridViewOrigin SKUListGrid = new GridViewOrigin();
        private DataGridView SKUGridRefer;
        private DashboardModel model = new DashboardModel();

        public SKUList() : base("SKU List", "Select a held order to open")
        {
            InitializeComponent();
            _initBasicSize();

            HotkeyButton[] hkButtons = new HotkeyButton[9] { hkAdds, hkDeletes, hkSelects, hkCrossRefLookup, hkView, hkAdjustQty, hkSKUHistory, hkProfileHistory, hkArchivedSKUs };
            _initializeHKButtons(hkButtons);
            AddHotKeyEvents();

            InitPriceTierGrid();
        }

        private void AddHotKeyEvents()
        {
            hkAdds.GetButton().Click += (sender, e) =>
            {
                SkuDetail detailModal = new SkuDetail();
                if (detailModal.ShowDialog() == DialogResult.OK)
                {
                    LoadSKUList();
                }
            };
            hkDeletes.GetButton().Click += (sender, e) =>
            {
                int selectedSKUId = 0;
                if (SKUGridRefer.SelectedRows.Count > 0)
                {
                    foreach (DataGridViewRow row in SKUGridRefer.SelectedRows)
                    {
                        selectedSKUId = (int)row.Cells[0].Value;
                    }
                }
                bool refreshData = model.DeleteSKU(selectedSKUId);
                if (refreshData)
                {
                    LoadSKUList();
                }
            };
            hkSelects.GetButton().Click += (sender, e) =>
            {
                updateSKU();
            };
        }

        private void InitPriceTierGrid()
        {
            SKUGridRefer = SKUListGrid.GetGrid();
            SKUGridRefer.Location = new Point(0, 95);
            SKUGridRefer.Width = this.Width;
            SKUGridRefer.Height = this.Height - 295;
            this.Controls.Add(SKUGridRefer);
            this.SKUGridRefer.CellDoubleClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.SKUGridView_CellDoubleClick);

            LoadSKUList();
        }


        private void LoadSKUList()
        {
            string filter = "";
            var refreshData = model.LoadSKUData(filter);
            if (refreshData)
            {
                SKUGridRefer.DataSource = model.SKUDataList;
                SKUGridRefer.Columns[0].Visible = false;
                SKUGridRefer.Columns[1].HeaderText = "SKU#";
                SKUGridRefer.Columns[1].Width = 300;
                SKUGridRefer.Columns[2].HeaderText = "Category";
                SKUGridRefer.Columns[2].Width = 300;
                SKUGridRefer.Columns[3].HeaderText = "Description";
                SKUGridRefer.Columns[3].Width = 500;
                SKUGridRefer.Columns[4].HeaderText = "Qty Avail";
                SKUGridRefer.Columns[4].Width = 300;
                SKUGridRefer.Columns[5].HeaderText = "Qty Tracking";
                SKUGridRefer.Columns[5].Width = 300;
            }
        }

        private void updateSKU()
        {
            SkuDetail detailModal = new SkuDetail(); ;

            int rowIndex = SKUGridRefer.CurrentCell.RowIndex;

            DataGridViewRow row = SKUGridRefer.Rows[rowIndex];
            int skuId = (int)row.Cells[0].Value;

            List<dynamic> skuData = new List<dynamic>();
            skuData = model.GetSKUData(skuId);
            detailModal.setDetails(skuData, skuData[0].id);

            if (detailModal.ShowDialog() == DialogResult.OK)
            {
                LoadSKUList();
            }
        }

        private void SKUGridView_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            updateSKU();
        }
    }
}
