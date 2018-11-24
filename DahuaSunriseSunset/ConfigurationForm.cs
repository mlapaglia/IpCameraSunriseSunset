﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SunriseSunset
{
	public partial class ConfigurationForm : Form
	{
		public ConfigurationForm()
		{
			InitializeComponent();
		}

		private void btnAdd_Click(object sender, EventArgs e)
		{
			AddCameraForm f = new AddCameraForm();
			f.FormClosed += F_FormClosed;
			f.ShowDialog(this);
		}

		private void F_FormClosed(object sender, FormClosedEventArgs e)
		{
			AddCameraForm f = (AddCameraForm)sender;
			if (f.newCamera == null)
				return;
			lbCameras.Items.Add(f.newCamera);
		}

		private void btnRemove_Click(object sender, EventArgs e)
		{
			List<int> indices = new List<int>();
			foreach (int index in lbCameras.SelectedIndices)
				indices.Add(index);
			indices.Reverse();
			foreach (int i in indices)
				lbCameras.Items.RemoveAt(i);
		}

		private void ConfigurationForm_FormClosing(object sender, FormClosingEventArgs e)
		{
			SunriseSunsetConfig cfg = new SunriseSunsetConfig();
			cfg.Load();

			try
			{
				cfg.latitude = double.Parse(txtLat.Text);
				cfg.longitude = double.Parse(txtLon.Text);
				cfg.sunriseOffsetHours = double.Parse(txtRiseOffset.Text);
				cfg.sunsetOffsetHours = double.Parse(txtSetOffset.Text);
				cfg.Cameras = BuildCameraList();

				cfg.Save();
			}
			catch (Exception ex)
			{
				e.Cancel = true;
				MessageBox.Show(ex.Message);
			}
		}

		private List<CameraDefinition> BuildCameraList()
		{
			List<CameraDefinition> cams = new List<CameraDefinition>();
			foreach (CameraDefinition cam in lbCameras.Items)
				cams.Add(cam);
			return cams;
		}

		private void ConfigurationForm_Load(object sender, EventArgs e)
		{
			SunriseSunsetConfig cfg = new SunriseSunsetConfig();
			cfg.Load();
			cfg.SaveIfNoExist();

			txtLat.Text = cfg.latitude.ToString();
			txtLon.Text = cfg.longitude.ToString();
			txtRiseOffset.Text = cfg.sunriseOffsetHours.ToString();
			txtSetOffset.Text = cfg.sunsetOffsetHours.ToString();

			foreach (CameraDefinition cam in cfg.Cameras)
				lbCameras.Items.Add(cam);
		}

		private int editingIndex = -1;
		private void btnEditSelected_Click(object sender, EventArgs e)
		{
			if (lbCameras.SelectedIndices.Count != 1)
				return;
			editingIndex = lbCameras.SelectedIndex;
			CameraDefinition selectedCamera = (CameraDefinition)lbCameras.SelectedItem;
			if (selectedCamera != null)
			{
				AddCameraForm f = new AddCameraForm();
				f.ConvertIntoEditForm(selectedCamera);
				f.FormClosed += F_FormClosed_Edit;
				f.ShowDialog(this);
			}
		}

		private void F_FormClosed_Edit(object sender, FormClosedEventArgs e)
		{
			AddCameraForm f = (AddCameraForm)sender;
			if (f.newCamera == null)
				return;
			lbCameras.Items[editingIndex] = f.newCamera;
		}
	}
}
