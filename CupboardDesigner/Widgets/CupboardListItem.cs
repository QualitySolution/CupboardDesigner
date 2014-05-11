using System;
using NLog;
using Cairo;

namespace CupboardDesigner
{
	[System.ComponentModel.ToolboxItem(true)]
	public partial class CupboardListItem : Gtk.Bin
	{
		private static Logger logger = LogManager.GetCurrentClassLogger();
		public int id;
		public string _Name;
		public SVGHelper Image;

		public CupboardListItem()
		{
			this.Build();
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
				labelName.LabelProp = _Name;
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
	}
}

