using System;
using System.Collections.Generic;

namespace CupboardDesigner
{
	public class Cube
	{
		public string Name;
		public int Widht;
		public int Height;
		public byte[] ImageFile;
		public int NomenclatureId;
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
	}

	public class Cupboard
	{
		public int CubesV;
		public int CubesH;

		public List<Cube> Cubes;

		public Cupboard()
		{
			Cubes = new List<Cube>();
		}
	}
}

