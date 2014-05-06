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
		public string Name;
		[XmlIgnore]
		public int Widht;
		[XmlIgnore]
		public int Height;
		[XmlIgnore]
		public byte[] ImageFile;
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
			NewCube.ImageFile = (byte[])ImageFile.Clone();
			return NewCube;
		}

		public void DrawCube(Context cr, int CubePxSize)
		{
			int PxWidth = CubesH * CubePxSize;
			int PxHeight = CubesV * CubePxSize;

			//Фон
			cr.Rectangle(0, 0, PxWidth, PxHeight);
			cr.SetSourceRGB(1, 0.9, 0.6);
			cr.Fill();

			//Куб
			Rsvg.Handle svg = new Rsvg.Handle(ImageFile);
			double vratio = (double) PxHeight / svg.Dimensions.Height;
			double hratio = (double) PxWidth / svg.Dimensions.Width;
			double ratio = Math.Min(vratio, hratio);
			cr.Scale(ratio, ratio);
			svg.RenderCairo(cr);
		}
	}

	public class Cupboard
	{
		[XmlIgnore]
		private static Logger logger = LogManager.GetCurrentClassLogger();
		public int CubesV = 1;
		public int CubesH = 1;

		public List<Cube> Cubes;

		public Cupboard()
		{
			Cubes = new List<Cube>();
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
					cube.ImageFile = found.ImageFile;
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
	}

	public class DragInformation
	{
		public Cube cube;
		public bool FromList;
		public int IconPosX, IconPosY;
	}
}

