using System;
using NLog;
using Gtk;
using Mono.Data.Sqlite;
using QSProjectsLib;
using System.Collections.Generic;
using Cairo;

namespace CupboardDesigner
{
	public partial class Order : Gtk.Dialog
	{
		private static Logger logger = LogManager.GetCurrentClassLogger();
		private bool NewItem = true;
		private int ItemId;
		private ListStore ComponentsStore;
		private TreeModel MaterialNameList, FacingNameList;
		private List<Cube> CubeList;
		private int CubePxSize = 100;
		private int BorderPxSize = 30;
		private VBox vboxCubeList;
		private Cupboard OrderCupboard;

		private enum ComponentCol{
			row_id,
			component_id,
			component,
			material_id,
			material,
			facing_id,
			facing
		}

		public Order()
		{
			this.Build();

			ComboWorks.ComboFillReference(comboBasis, "basis", ComboWorks.ListMode.OnlyItems);

			ComboBox TempCombo = new ComboBox();
			ComboWorks.ComboFillReference(TempCombo, "materials", ComboWorks.ListMode.WithNo);
			MaterialNameList = TempCombo.Model;
			TempCombo.Destroy ();

			TempCombo = new ComboBox();
			ComboWorks.ComboFillReference(TempCombo, "facing", ComboWorks.ListMode.WithNo);
			FacingNameList = TempCombo.Model;
			TempCombo.Destroy ();

			ComponentsStore = new ListStore(typeof(long), typeof(int), typeof(string), typeof(int), typeof(string), typeof(int), typeof(string));

			Gtk.TreeViewColumn ColumnMaterial = new Gtk.TreeViewColumn ();
			ColumnMaterial.Title = "Материал";
			ColumnMaterial.MinWidth = 180;
			Gtk.CellRendererCombo CellMaterial = new CellRendererCombo();
			CellMaterial.TextColumn = 0;
			CellMaterial.Editable = true;
			CellMaterial.Model = MaterialNameList;
			CellMaterial.HasEntry = false;
			CellMaterial.Edited += OnMaterialComboEdited;
			ColumnMaterial.PackStart (CellMaterial, true);
			ColumnMaterial.AddAttribute(CellMaterial, "text", (int)ComponentCol.material);

			Gtk.TreeViewColumn ColumnFacing = new Gtk.TreeViewColumn ();
			ColumnFacing.Title = "Отделка";
			ColumnFacing.MinWidth = 180;
			Gtk.CellRendererCombo CellFacing = new CellRendererCombo();
			CellFacing.TextColumn = 0;
			CellFacing.Editable = true;
			CellFacing.Model = FacingNameList;
			CellFacing.HasEntry = false;
			CellFacing.Edited += OnFacingComboEdited;
			ColumnFacing.PackStart (CellFacing, true);
			ColumnFacing.AddAttribute(CellFacing, "text", (int)ComponentCol.facing);

			treeviewComponents.AppendColumn("Наименование", new Gtk.CellRendererText (), "text", (int)ComponentCol.component);
			treeviewComponents.AppendColumn(ColumnMaterial);
			treeviewComponents.AppendColumn(ColumnFacing);

			treeviewComponents.Model = ComponentsStore;
			treeviewComponents.ShowAll();

			//Загрузка списка компонентов
			string sql = "SELECT id, name FROM components";
			SqliteCommand cmd = new SqliteCommand(sql, (SqliteConnection)QSMain.ConnectionDB);
			using (SqliteDataReader rdr = cmd.ExecuteReader())
			{
				while(rdr.Read())
				{
					ComponentsStore.AppendValues((long) -1,
						rdr.GetInt32(rdr.GetOrdinal("id")),
						rdr["name"].ToString()
					);
				}
			}

			//Загрузка списка кубов
			CubeList = new List<Cube>();
			vboxCubeList = new VBox(false, 6);
			sql = "SELECT * FROM nomenclature WHERE type = @type";
			cmd = new SqliteCommand(sql, (SqliteConnection)QSMain.ConnectionDB);
			cmd.Parameters.AddWithValue("@type", Nomenclature.NomType.cube.ToString());
			using (SqliteDataReader rdr = cmd.ExecuteReader())
			{
				while(rdr.Read())
				{
					if (rdr["image"] == DBNull.Value)
						continue;
					Cube TempCube = new Cube();
					TempCube.NomenclatureId = rdr.GetInt32(rdr.GetOrdinal("id"));
					TempCube.Name = DBWorks.GetString(rdr, "name", "");
					TempCube.Height = DBWorks.GetInt(rdr, "height", 0);
					TempCube.Widht = DBWorks.GetInt(rdr, "lenght", 0);
					int size = DBWorks.GetInt(rdr, "image_size", 0);
					TempCube.ImageFile = new byte[size];
					rdr.GetBytes(rdr.GetOrdinal("image"), 0, TempCube.ImageFile, 0, size);
					CubeList.Add(TempCube);

					//Добавляем виджеты в лист
					CubeListItem TempWidget = new CubeListItem();
					TempCube.Widget = TempWidget;
					TempWidget.CubeItem = TempCube;
					TempWidget.CubePxSize = CubePxSize;
					vboxCubeList.PackEnd(TempWidget);
				}
				vboxCubeList.ShowAll();
				scrolledCubeList.AddWithViewport(vboxCubeList);
			}

			OrderCupboard = new Cupboard();
			Cube temp = CubeList[9].Clone();
			temp.BoardPositionX = 1;
			temp.BoardPositionY = 4;
			OrderCupboard.Cubes.Add(temp);
		}

		public void Fill(int id)
		{
			ItemId = id;
			NewItem = false;

			MainClass.StatusMessage(String.Format ("Запрос заказа №{0}...", id));
			string sql = "SELECT orders.* FROM orders WHERE orders.id = @id";
			try
			{
				SqliteCommand cmd = new SqliteCommand(sql, (SqliteConnection) QSMain.ConnectionDB);

				cmd.Parameters.AddWithValue("@id", id);

				using(SqliteDataReader rdr = cmd.ExecuteReader())
				{
					rdr.Read();

					this.Title = String.Format("Заказ №{0}", rdr["id"].ToString());
					entryCustomer.Text = rdr["customer"].ToString();
					dateArrval.Date = DBWorks.GetDateTime(rdr, "arrval", new DateTime());
					dateDelivery.Date = DBWorks.GetDateTime(rdr, "delivery", new DateTime());
					ComboWorks.SetActiveItem(comboBasis, rdr.GetInt32(rdr.GetOrdinal("basis_id")));
					textviewComments.Buffer.Text = rdr["comment"].ToString();
				}

				sql = "SELECT order_components.*, materials.name as material, facing.name as facing FROM order_components " +
					"LEFT JOIN materials ON materials.id = order_components.material_id " +
					"LEFT JOIN facing ON facing.id = order_components.facing_id " +
					"WHERE order_components.order_id = @id";
				cmd = new SqliteCommand(sql, (SqliteConnection)QSMain.ConnectionDB);
				cmd.Parameters.AddWithValue("@id", id);
				using(SqliteDataReader rdr = cmd.ExecuteReader())
				{
					TreeIter iter;
					while(rdr.Read())
					{
						if(!ComponentsStore.GetIterFirst(out iter))
							break;
						do
						{

							if((int)ComponentsStore.GetValue(iter, (int)ComponentCol.component_id) == rdr.GetInt32(rdr.GetOrdinal("component_id")))
							{
								ComponentsStore.SetValue(iter, (int)ComponentCol.row_id, (object)DBWorks.GetLong(rdr, "id", -1));
								ComponentsStore.SetValue(iter, (int)ComponentCol.material_id, DBWorks.GetInt(rdr, "material_id", -1));
								ComponentsStore.SetValue(iter, (int)ComponentCol.material, DBWorks.GetString(rdr, "material", "нет"));
								ComponentsStore.SetValue(iter, (int)ComponentCol.facing_id, DBWorks.GetInt(rdr, "facing_id", -1));
								ComponentsStore.SetValue(iter, (int)ComponentCol.facing, DBWorks.GetString(rdr, "facing", "нет"));
								break;
							}
						} while(ComponentsStore.IterNext(ref iter));
					}
				}

				MainClass.StatusMessage("Ok");
			}
			catch (Exception ex)
			{
				MainClass.StatusMessage("Ошибка получения информации о заказе!");
				logger.Error(ex.ToString());
				QSMain.ErrorMessage(this,ex);
			}
			TestCanSave();
		}

		void OnMaterialComboEdited (object o, EditedArgs args)
		{
			TreeIter iter, RefIter;
			if (!ComponentsStore.GetIterFromString (out iter, args.Path))
				return;
			if(args.NewText == null)
			{
				logger.Warn("newtext is empty");
				return;
			}

			if(ListStoreWorks.SearchListStore((ListStore)MaterialNameList, args.NewText, out RefIter))
			{
				ComponentsStore.SetValue(iter, (int)ComponentCol.material, args.NewText);
				ComponentsStore.SetValue(iter, (int)ComponentCol.material_id, MaterialNameList.GetValue(RefIter, 1));
			}
		}

		void OnFacingComboEdited (object o, EditedArgs args)
		{
			TreeIter iter, RefIter;
			if (!ComponentsStore.GetIterFromString (out iter, args.Path))
				return;
			if(args.NewText == null)
			{
				logger.Warn("newtext is empty");
				return;
			}

			if(ListStoreWorks.SearchListStore((ListStore)FacingNameList, args.NewText, out RefIter))
			{
				ComponentsStore.SetValue(iter, (int)ComponentCol.facing, args.NewText);
				ComponentsStore.SetValue(iter, (int)ComponentCol.facing_id, FacingNameList.GetValue(RefIter, 1));
			}
		}

		protected void TestCanSave ()
		{
		}

		protected void OnButtonOkClicked(object sender, EventArgs e)
		{
			string sql;
			if (NewItem)
			{
				sql = "INSERT INTO orders (customer, basis_id, arrval, delivery, comment) " +
					"VALUES (@customer, @basis_id, @arrval, @delivery, @comment)";
			}
			else
			{
				sql = "UPDATE orders SET customer = @customer, basis_id = @basis_id, arrval = @arrval, " +
					"delivery = @delivery, comment = @comment WHERE id = @id";
			}
			SqliteTransaction trans = ((SqliteConnection)QSMain.ConnectionDB).BeginTransaction();
			MainClass.StatusMessage("Запись заказа...");
			try
			{
				SqliteCommand cmd = new SqliteCommand(sql, (SqliteConnection)QSMain.ConnectionDB, trans);

				cmd.Parameters.AddWithValue("@id", ItemId);
				cmd.Parameters.AddWithValue("@customer", DBWorks.ValueOrNull(entryCustomer.Text != "", entryCustomer.Text));
				cmd.Parameters.AddWithValue("@arrval", DBWorks.ValueOrNull(!dateArrval.IsEmpty, dateArrval.Date));
				cmd.Parameters.AddWithValue("@delivery", DBWorks.ValueOrNull(!dateDelivery.IsEmpty, dateDelivery.Date));
				cmd.Parameters.AddWithValue("basis_id", ComboWorks.GetActiveIdOrNull(comboBasis));
				cmd.Parameters.AddWithValue("@comment", DBWorks.ValueOrNull(textviewComments.Buffer.Text != "", textviewComments.Buffer.Text));

				cmd.ExecuteNonQuery();

				if(NewItem)
				{
					sql = @"select last_insert_rowid()";
					cmd = new SqliteCommand(sql, (SqliteConnection)QSMain.ConnectionDB, trans);
					ItemId = Convert.ToInt32(cmd.ExecuteScalar());
				}

				// Запись компонент
				foreach(object[] row in ComponentsStore)
				{
					bool HasValue = (int) row[(int)ComponentCol.facing_id] > 0 || (int) row[(int)ComponentCol.material_id] > 0;
					if(HasValue)
					{
						if((long)row[(int)ComponentCol.row_id] < 0)
							sql = "INSERT INTO order_components (order_id, component_id, material_id, facing_id) " +
								"VALUES (@order_id, @component_id, @material_id, @facing_id)";
						else
							sql = "UPDATE order_components SET material_id = @material_id, facing_id = @facing_id " +
								"WHERE id = @id";

						cmd = new SqliteCommand(sql, (SqliteConnection)QSMain.ConnectionDB, trans);
						cmd.Parameters.AddWithValue("@order_id", ItemId);
						cmd.Parameters.AddWithValue("@component_id", row[(int)ComponentCol.component_id]);
						cmd.Parameters.AddWithValue("@material_id", DBWorks.ValueOrNull((int) row[(int)ComponentCol.material_id] > 0, row[(int)ComponentCol.material_id]));
						cmd.Parameters.AddWithValue("@facing_id", DBWorks.ValueOrNull((int)row[(int)ComponentCol.facing_id] > 0, row[(int)ComponentCol.facing_id]));
						cmd.ExecuteNonQuery();
					}
					else if((long)row[(int)ComponentCol.row_id] > 0)
					{
						sql = "DELETE FROM order_components WHERE id = @id";

						cmd = new SqliteCommand(sql, (SqliteConnection)QSMain.ConnectionDB, trans);
						cmd.Parameters.AddWithValue("@id", row[(int)ComponentCol.row_id]);
						cmd.ExecuteNonQuery();
					}
				}

				trans.Commit();
				MainClass.StatusMessage("Ok");
				Respond(Gtk.ResponseType.Ok);
			}
			catch (Exception ex)
			{
				trans.Rollback();
				MainClass.StatusMessage("Ошибка записи заказа!");
				logger.Error(ex.ToString());
				QSMain.ErrorMessage(this, ex);
			}
		}

		void SetInfo()
		{
			labelInfo.LabelProp = String.Format("{0} Ширина:{1}cм Высота:{2}cм", comboBasis.ActiveText, 
				int.Parse(comboCubeH.ActiveText) * 40, int.Parse(comboCubeV.ActiveText) * 40);
		}

		protected void OnComboBasisChanged(object sender, EventArgs e)
		{
			SetInfo();
		}

		protected void OnComboCubeHChanged(object sender, EventArgs e)
		{
			SetInfo();
			CalculateCubePxSize(drawCupboard.Allocation);
		}

		protected void OnComboCubeVChanged(object sender, EventArgs e)
		{
			SetInfo();
			CalculateCubePxSize(drawCupboard.Allocation);
		}

		protected void OnDrawCupboardExposeEvent(object o, ExposeEventArgs args)
		{
			using (Context cr = Gdk.CairoHelper.Create (args.Event.Window)) {
				int w, h;
				args.Event.Window.GetSize (out w, out h);
				Draw (cr, w, h);
			}
		}

		void Draw (Context cr, int width, int height)
		{
			int CupboardPxSizeH = BorderPxSize * 2 + CubePxSize * int.Parse(comboCubeH.ActiveText);
			int CupboardPxSizeV = BorderPxSize * 2 + CubePxSize * int.Parse(comboCubeV.ActiveText);

			int ShiftX = (width - CupboardPxSizeH) / 2;
			int ShiftY = (height - CupboardPxSizeV) / 2;

			cr.Translate(ShiftX + BorderPxSize, ShiftY + BorderPxSize);
			DrawGrid(cr);

			foreach(Cube cube in OrderCupboard.Cubes)
			{
				cr.Save();
				cr.Translate(cube.BoardPositionX * CubePxSize, cube.BoardPositionY * CubePxSize);
				DrawCube(cr, cube);
				cr.Restore();
			}
		}

		void DrawGrid(Context cr)
		{
			int CubesH = int.Parse(comboCubeH.ActiveText);
			int CubesV = int.Parse(comboCubeV.ActiveText);
			cr.SetSourceRGB(155, 157, 158);
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

		void DrawCube(Context cr, Cube cube)
		{
			int MaxWidth = cube.CubesH * CubePxSize;
			int MaxHeight = cube.CubesV * CubePxSize;

			Rsvg.Handle svg = new Rsvg.Handle(cube.ImageFile);
			double vratio = (double) MaxHeight / svg.Dimensions.Height;
			double hratio = (double) MaxWidth / svg.Dimensions.Width;
			double ratio = Math.Min(vratio, hratio);
			cr.Scale(ratio, ratio);
			svg.RenderCairo(cr);
		}

		protected void OnDrawCupboardSizeAllocated(object o, SizeAllocatedArgs args)
		{
			CalculateCubePxSize(args.Allocation);
		}

		private void CalculateCubePxSize(Gdk.Rectangle CupboardPlace)
		{
			int WidhtWithoutGrid = CupboardPlace.Width ;
			int HeightWithoutGrid = CupboardPlace.Height;

			// 1.2 это 2 бортика по караям которые равны 60% от куба
			int MinCubeSizeForH = Convert.ToInt32(WidhtWithoutGrid / (double.Parse(comboCubeH.ActiveText) + 1.2));
			int MinCubeSizeForV = Convert.ToInt32(HeightWithoutGrid / (double.Parse(comboCubeV.ActiveText) + 1.2));

			int NeedCubePxSize = Math.Min(MinCubeSizeForH, MinCubeSizeForV);

			if (NeedCubePxSize > 100)
				NeedCubePxSize = 100;

			if(CubePxSize != NeedCubePxSize)
			{
				CubePxSize = NeedCubePxSize;
				BorderPxSize = Convert.ToInt32(CubePxSize * 0.3);

				foreach(Cube cube in CubeList)
				{
					((CubeListItem)cube.Widget).CubePxSize = CubePxSize;
				}
			}
		}

	}
}

