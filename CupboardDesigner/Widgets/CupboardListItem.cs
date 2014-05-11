﻿using System;
using NLog;
using Cairo;
using Gtk;

namespace CupboardDesigner
{
	[System.ComponentModel.ToolboxItem(true)]
	public partial class CupboardListItem : Gtk.Bin
	{
		private static Logger logger = LogManager.GetCurrentClassLogger();
		public int id;
		public string _Name;
		public SVGHelper Image;
		public Gtk.RadioButton Button;

		public CupboardListItem()
		{
			this.Build();
			Button = radioType;
			drawImage.AddEvents((int)Gdk.EventMask.ButtonPressMask);
		}

		int _CubePxSize;
		public int CubePxSize
		{
			get{return _CubePxSize;}
			set{
				_CubePxSize = value;
				UpdateItem();
			}
		}

		public string ItemName
		{
			get
			{
				return _Name;
			}
			set
			{
				_Name = value;
				radioType.Label = _Name;
			}
		}

		public void UpdateItem()
		{
			// 2.2 = 1 + бортики 0.6*2
			drawImage.SetSizeRequest(Convert.ToInt32(CubePxSize * 2.2) , Convert.ToInt32(CubePxSize * 2.2));
		}

		protected void OnDrawImageExposeEvent(object o, Gtk.ExposeEventArgs args)
		{
			if(Image == null)
				return;
			using (Context cr = Gdk.CairoHelper.Create (args.Event.Window)) 
			{
				cr.Translate(CubePxSize * 0.6, CubePxSize * 0.6);
				Image.DrawBasis(cr, CubePxSize);
			}
		}

		protected void OnDrawImageButtonPressEvent(object o, Gtk.ButtonPressEventArgs args)
		{
			if(args.Event.Button == 1)
			{
				radioType.Active = true;
			}
		}
	}
}

