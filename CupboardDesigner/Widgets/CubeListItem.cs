using System;
using NLog;
using Cairo;

namespace CupboardDesigner
{
	[System.ComponentModel.ToolboxItem(true)]
	public partial class CubeListItem : Gtk.Bin
	{
		private static Logger logger = LogManager.GetCurrentClassLogger();
		public Cube CubeItem;

		public CubeListItem()
		{
			this.Build();
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
		}

		protected void OnDrawCubeExposeEvent(object o, Gtk.ExposeEventArgs args)
		{
			if (CubeItem == null)
				return;
			using (Context cr = Gdk.CairoHelper.Create (args.Event.Window)) 
			{
				int MaxWidth, MaxHeight;
				args.Event.Window.GetSize(out MaxWidth, out MaxHeight);
				logger.Debug("Image widget size W: {0} H: {1}", MaxWidth, MaxHeight);

				Rsvg.Handle svg = new Rsvg.Handle(CubeItem.ImageFile);
				double vratio = (double) MaxHeight / svg.Dimensions.Height;
				double hratio = (double) MaxWidth / svg.Dimensions.Width;
				double ratio = Math.Min(vratio, hratio);
				cr.Scale(ratio, ratio);
				svg.RenderCairo(cr);
			}
		}
	}
}

