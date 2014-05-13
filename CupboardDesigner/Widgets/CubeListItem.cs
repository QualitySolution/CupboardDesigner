using System;
using NLog;
using Cairo;
using Gdk;
using Gtk;

namespace CupboardDesigner
{
	[System.ComponentModel.ToolboxItem(true)]
	public partial class CubeListItem : Gtk.Bin
	{
		private static Logger logger = LogManager.GetCurrentClassLogger();
		public Cube CubeItem;
		public DragInformation DragInfo;

		public CubeListItem()
		{
			this.Build();
			Gtk.TargetEntry[] source_table = new Gtk.TargetEntry[] {
				new Gtk.TargetEntry ("application/cube", Gtk.TargetFlags.App, 0)
			};
			Gtk.Drag.SourceSet(drawCube, Gdk.ModifierType.Button1Mask, source_table, Gdk.DragAction.Move);
		}

		int _CubePxSize;
		public int CubePxSize
		{
			get{return _CubePxSize;}
			set{
				_CubePxSize = value;
				UpdateCube();
			}
		}

		public void UpdateCube()
		{
			if (CubeItem == null)
				return;

			labelName.LabelProp = CubeItem.Name;
			drawCube.SetSizeRequest(CubeItem.CubesH * CubePxSize, CubeItem.CubesV * CubePxSize);
			logger.Debug("Update Size w={0} h={1}", CubeItem.CubesH * CubePxSize, CubeItem.CubesV * CubePxSize);
			this.CheckResize();
		}

		protected void OnDrawCubeExposeEvent(object o, Gtk.ExposeEventArgs args)
		{
			if (CubeItem == null)
				return;
			using (Context cr = Gdk.CairoHelper.Create (args.Event.Window)) 
			{
				CubeItem.DrawCube(cr, CubePxSize, true);
			}
		}

		protected void OnDrawCubeDragBegin(object o, Gtk.DragBeginArgs args)
		{
			Pixmap pix = new Pixmap(drawCube.GdkWindow, CubeItem.CubesH * CubePxSize, CubeItem.CubesV * CubePxSize);

			using (Context cr = Gdk.CairoHelper.Create(pix))
			{
				CubeItem.DrawCube(cr, CubePxSize, true);
			}
			Gdk.Pixbuf pixbuf = Gdk.Pixbuf.FromDrawable(pix, Gdk.Colormap.System, 0, 0, 0, 0, CubeItem.CubesH * CubePxSize, CubeItem.CubesV * CubePxSize);

			((Gtk.DrawingArea)o).GetPointer(out DragInfo.IconPosX, out DragInfo.IconPosY);
			Gtk.Drag.SetIconPixbuf(args.Context, pixbuf, DragInfo.IconPosX, DragInfo.IconPosY);
			DragInfo.FromList = true;
			DragInfo.cube = CubeItem;
		}

		protected override void OnSizeRequested(ref Requisition requisition)
		{
			base.OnSizeRequested(ref requisition);
			logger.Debug("Size requested w={0} h={1}", requisition.Width, requisition.Height);
		}
	}
}

