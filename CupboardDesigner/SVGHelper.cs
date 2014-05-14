using System;
using System.Xml;
using System.IO;
using Cairo;
using Rsvg;
using NLog;
using Svg;

namespace CupboardDesigner
{
	public class SVGHelper
	{
		private static Logger logger = LogManager.GetCurrentClassLogger();
		public byte[] OriginalFile;
		private byte[] DrawingFile;
		private XmlDocument XmlSvg;

		private double BaseX, BaseY, BaseHeight, BaseWidht;
		private double CentreX, CentreY;
		private float AddX, AddY;
		private double SvgHeight, SvgWidht;
		private double FrameScale;
		bool FrameSet = false;
		public int CubesH = 1, CubesV = 1;

		public SVGHelper()
		{
		}

		public SVGHelper(byte[] svg)
		{
			this.LoadImage(svg);
		}

		public bool LoadImage(byte[] svg)
		{
			logger.Debug("Загружаем svg в помошник.");
			try
			{
				XmlSvg = new XmlDocument();
				XmlSvg.LoadXml(System.Text.Encoding.UTF8.GetString(svg));

				foreach (XmlNode node in XmlSvg.GetElementsByTagName("svg"))
				{
					string units;
					SvgHeight = ParseSize(node.Attributes["height"].Value, out units);
					if(units == "mm")
						SvgHeight *= 100;
					SvgWidht = ParseSize(node.Attributes["width"].Value, out units);
					if(units == "mm")
						SvgWidht *= 100;
					break;
				}

				foreach (XmlNode node in XmlSvg.GetElementsByTagName("rect"))
				{
					if(node.Attributes["id"].Value == "framework")
					{
						BaseX = XmlConvert.ToDouble(node.Attributes["x"].Value);
						BaseY = XmlConvert.ToDouble(node.Attributes["y"].Value);
						BaseWidht = XmlConvert.ToDouble(node.Attributes["width"].Value);
						BaseHeight = XmlConvert.ToDouble(node.Attributes["height"].Value);
						CentreX = BaseX + BaseWidht / 2;
						CentreY = BaseY + BaseHeight / 2;
						FrameSet = true;
						break;
					}
				}
				if(FrameSet)
				{
					OriginalFile = svg;
					DrawingFile = null;
				}
			}
			catch (Exception ex)
			{
				logger.ErrorException("Ошибка в чтении svg!", ex);
				return false;
			}
			logger.Debug("Закончили загрузку.");
			return FrameSet;
		}

		public void PrepairForDBSave()
		{

			foreach (XmlNode node in XmlSvg.GetElementsByTagName("svg"))
			{
				XmlAttribute attr = (XmlAttribute)node.Attributes["stroke-width"];
				if(attr != null)
					attr.Value = XmlConvert.ToString(BaseWidht / 100);
				break;
			}

			using (MemoryStream stream = new MemoryStream())
			{
				using (TextWriter writer = new StreamWriter(stream, new System.Text.UTF8Encoding(false)))
				{
					XmlSvg.Save(writer);
				}
				OriginalFile = stream.ToArray();
			}
			//logger.Debug(System.Text.Encoding.Default.GetString(OriginalFile));
		}

		private double ParseSize(string value)
		{
			string empty;
			return ParseSize(value, out empty);
		}

		private double ParseSize(string value, out string unit)
		{
			string[] units = new string[] { "em", "ex", "px", "pt", "pc", "cm", "mm", "in" };
			unit = "";
			foreach(string str in units)
			{
				if(value.EndsWith(str))
				{
					unit = str;
					value = value.Replace(str, "");
					break;
				}
			}
			return XmlConvert.ToDouble(value);
		}

		public void ModifyDrawingImage()
		{
			if (OriginalFile == null)
				return;
			if(CubesV == 1 && CubesH == 1)
			{
				DrawingFile = OriginalFile;
				return;
			}
			logger.Debug("Начала изменение svg");
			MemoryStream stream = new MemoryStream(OriginalFile);
			SvgDocument imagefile = SvgDocument.Open<SvgDocument>(stream);
			SvgClipPath clip = imagefile.GetElementById<SvgClipPath>("presentation_clip_path");
			if(clip != null)
				clip.Parent.Children.Remove(clip);

			AddX = (float)(BaseWidht / 2) * (CubesH - 1);
			AddY = (float)(BaseHeight / 2) * (CubesV - 1);
			foreach(SvgLine line in imagefile.Children.FindSvgElementsOf<SvgLine>())
			{
				line.StartX = FixX(line.StartX);
				line.EndX = FixX(line.EndX);
				line.StartY = FixY(line.StartY);
				line.EndY = FixY(line.EndY);
			}

			stream = new MemoryStream();
			imagefile.Write(stream);
			DrawingFile = stream.ToArray();
			//logger.Debug(System.Text.Encoding.Default.GetString(DrawingFile));
			logger.Debug("Закончили изменение svg.");
		}
			
		private SvgUnit FixX(SvgUnit OldX)
		{
			float newvalue = (OldX.Value > CentreX) ? OldX.Value + AddX : OldX.Value - AddX;
			return new SvgUnit(newvalue);
		}

		private SvgUnit FixY(SvgUnit OldY)
		{
			float newvalue = (OldY.Value > CentreY) ? OldY.Value + AddY : OldY.Value - AddY;
			return new SvgUnit(newvalue);
		}

		public void DrawBasis(Context cr, int CubePxSize)
		{
			if (OriginalFile == null)
				return;
			if (DrawingFile == null)
				ModifyDrawingImage();

			Rsvg.Handle svg = new Rsvg.Handle(DrawingFile);

			FrameScale = svg.Dimensions.Width / SvgWidht;

			double ratio = CubePxSize / (BaseWidht * FrameScale);
			cr.Scale(ratio, ratio);
			cr.Translate(0.0 - ((BaseX - AddX) * FrameScale), 0.0 - ((BaseY - AddY) * FrameScale));
			svg.RenderCairo(cr);
		}
	}
}

