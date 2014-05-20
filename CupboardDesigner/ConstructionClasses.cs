using System;
using System.Collections.Generic;
using Cairo;
using NLog;
using System.Xml.Serialization;
using System.IO;

namespace CupboardDesigner
{
	public class Cube
	{
		[XmlIgnore]
		private static Logger logger = LogManager.GetCurrentClassLogger();
		[XmlIgnore]
		public string Name;
		[XmlIgnore]
		public int Widht;
		[XmlIgnore]
		public int Height;
		[XmlIgnore]
		public Rsvg.Handle SvgImage;
		[XmlIgnore]
		private byte[] ImageFile;

		public int NomenclatureId;
		[XmlIgnore]
		public Gtk.Widget Widget;

		public int BoardPositionX;
		public int BoardPositionY;

		public int CubesH
		{
			get{return Widht / 400;}
		}

		public int CubesV
		{
			get{return Height / 400;}
		}

		public Cube()
		{
		}

		public Cube Clone()
		{
			Cube NewCube = new Cube();
			NewCube.Name = Name;
			NewCube.Widht = Widht;
			NewCube.Height = Height;
			NewCube.NomenclatureId = NomenclatureId;
			NewCube.LoadSvg(ImageFile);
			return NewCube;
		}

		public void LoadSvg(byte[] file)
		{
			ImageFile = file;
			SvgImage = new Rsvg.Handle(ImageFile);
		}

		public void DrawCube(Context cr, int CubePxSize, bool Coloring)
		{
			logger.Debug("Начали рисовать куб {0}", Name);
			int PxWidth = CubesH * CubePxSize;
			int PxHeight = CubesV * CubePxSize;

			//Фон
			if(Coloring)
			{
				cr.Rectangle(0, 0, PxWidth, PxHeight);
				//cr.SetSourceRGB(1, 0.9, 0.6);
				cr.SetSourceRGB(0.77254902, 0.631372549, 0.435294118);
				cr.Fill();
			}

			//Куб
			double vratio = (double) PxHeight / SvgImage.Dimensions.Height;
			double hratio = (double) PxWidth / SvgImage.Dimensions.Width;
			double ratio = Math.Min(vratio, hratio);
			cr.Scale(ratio, ratio);
			SvgImage.RenderCairo(cr);
			logger.Debug("Закончили рисовать куб.");
		}
	}

	public class Cupboard
	{
		[XmlIgnore]
		private static Logger logger = LogManager.GetCurrentClassLogger();
		[XmlIgnore]
		private int cubesV = 1;
		[XmlIgnore]
		private int cubesH = 1;
		[XmlIgnore]
		public SVGHelper BorderImage;

		[XmlIgnore]
		public int CupboardZeroX { get; private set;}
		[XmlIgnore]
		public int CupboardZeroY { get; private set;}

		public List<Cube> Cubes;

		public Cupboard()
		{
			Cubes = new List<Cube>();
			BorderImage = new SVGHelper();
		}

		public int CubesV
		{
			get
			{
				return cubesV;
			}
			set
			{
				cubesV = value;
				if (BorderImage != null)
					BorderImage.CubesV = value;
			}
		}

		public int CubesH
		{
			get
			{
				return cubesH;
			}
			set
			{
				cubesH = value;
				if (BorderImage != null)
					BorderImage.CubesH = value;
			}
		}

		public bool TestPutCube(Cube cube, int x, int y)
		{
			if (cube == null)
				return false;
			//Проверяем соответствие границам.
			if (x < 0 || y < 0)
				return false;
			if (x + cube.CubesH > this.CubesH || y + cube.CubesV > this.CubesV)
				return false;
			logger.Debug("Testcubes");
			//Проверяем на свободно ли место
			foreach(Cube item in Cubes)
			{
				if ((x <= item.BoardPositionX + item.CubesH - 1 && item.BoardPositionX <= x + cube.CubesH - 1) &&
					(y <= item.BoardPositionY + item.CubesV - 1 && item.BoardPositionY <= y + cube.CubesV - 1))
					return false;
			}
			return true;
		}

		public Cube GetCube(int x, int y)
		{
			foreach(Cube item in Cubes)
			{
				if ((x <= item.BoardPositionX + item.CubesH - 1 && item.BoardPositionX <= x ) &&
					(y <= item.BoardPositionY + item.CubesV - 1 && item.BoardPositionY <= y ))
					return item;
			}
			return null;
		}

		public Dictionary<int, int> GetAmounts()
		{
			Dictionary<int, int> Counts = new Dictionary<int, int>();

			foreach(Cube item in Cubes)
			{
				if (Counts.ContainsKey(item.NomenclatureId))
					Counts[item.NomenclatureId]++;
				else
					Counts.Add(item.NomenclatureId, 1);
			}
			return Counts;
		}

		public string SaveToString()
		{
			XmlSerializer serializer = new XmlSerializer(typeof(Cupboard));
			TextWriter Writer = new StringWriter();
			serializer.Serialize(Writer, this);
			return Writer.ToString();
		}

		public static Cupboard Load(string xml, List<Cube> cubesinfo)
		{
			XmlSerializer serializer = new XmlSerializer(typeof(Cupboard));
			TextReader Reader = new StringReader(xml);
			Cupboard board = (Cupboard)serializer.Deserialize(Reader);
			foreach(Cube cube in board.Cubes)
			{
				Cube found = cubesinfo.Find(delegate(Cube obj) {
					return obj.NomenclatureId == cube.NomenclatureId;
				});
				if(found != null)
				{
					cube.Height = found.Height;
					cube.SvgImage = found.SvgImage;
					cube.Name = found.Name;
					cube.Widht = found.Widht;
				}
				else
				{
					board.Cubes.Remove(cube);
					logger.Warn("Куб с id={0} удален из конфигурации шкафа так как, не найден в справочнике кубов");
				}
			}
			return board;
		}

		public bool Clean()
		{
			bool Catch = false;
			foreach(Cube item in Cubes.FindAll(c => c.BoardPositionX + c.CubesH > this.CubesH 
				|| c.BoardPositionY + c.CubesV > this.CubesV))
			{
				this.Cubes.Remove(item);
				Catch = true;
			}
			return Catch;
		}

		public void Draw (Context cr, int width, int height, int CubePxSize, bool ForPrint)
		{
			int CupboardPxSizeH = (int)(CubePxSize * (CubesH + 1.2)) ;
			int CupboardPxSizeV = (int)(CubePxSize * (CubesV + 1.2));

			int ShiftX = (width - CupboardPxSizeH) / 2;
			int ShiftY = (height - CupboardPxSizeV) / 2;

			CupboardZeroX = ShiftX + (int)(CubePxSize * 0.6);
			CupboardZeroY = ShiftY + (int)(CubePxSize * 0.6);

			cr.Translate(CupboardZeroX, CupboardZeroY);
			if(!ForPrint)
				DrawGrid(cr, CubePxSize);
			cr.Save();
			if (BorderImage != null)
				BorderImage.DrawBasis(cr, CubePxSize);
			cr.Restore();

			foreach(Cube cube in Cubes)
			{
				cr.Save();
				cr.Translate(cube.BoardPositionX * CubePxSize, cube.BoardPositionY * CubePxSize);
				cube.DrawCube(cr, CubePxSize, !ForPrint);
				cr.Restore();
			}
		}

		void DrawGrid(Context cr, int CubePxSize)
		{
			cr.SetSourceRGB(1, 1, 1);
			cr.SetDash(new double[]{2.0, 3.0}, 0.0);
			for (int x = 0; x <= CubesH; x++)
			{
				cr.MoveTo(x * CubePxSize, 0);
				cr.LineTo(x * CubePxSize, CubePxSize * CubesV);
			}
			for (int y = 0; y <= CubesV; y++)
			{
				cr.MoveTo(0, y * CubePxSize);
				cr.LineTo(CubesH * CubePxSize, CubePxSize * y);
			}
			cr.Stroke();
		}

	}

	public class DragInformation
	{
		public Cube cube;
		public bool FromList;
		public int IconPosX, IconPosY;
	}
}

