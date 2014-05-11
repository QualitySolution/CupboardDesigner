using System;
using NLog;
using Gtk;
using Mono.Data.Sqlite;
using QSProjectsLib;
using System.Collections.Generic;
using Cairo;
using Gdk;

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
		private int CupboardZeroX, CupboardZeroY;
		private VBox vboxCubeList, vboxTypeList;
		private HBox hboxCubeList, hboxTypeList;
		private List<CupboardListItem> TypeWidgetList;
		private Cupboard OrderCupboard;
		private DragInformation CurrentDrag;

		private enum ComponentCol{
			row_id,
			component_id,
			component,
			material_id,
			material,
			facing_id,
			facing
		}

		Gtk.TargetEntry[] TargetTable = new Gtk.TargetEntry[] {
			new Gtk.TargetEntry ("application/cube", Gtk.TargetFlags.App, 0)
		};

		public Order()
		{
			this.Build();

			ComboWorks.ComboFillReference(comboExhibition, "exhibition", ComboWorks.ListMode.WithNo);

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

			CurrentDrag = new DragInformation();
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
					TempWidget.DragInfo = CurrentDrag;
					vboxCubeList.PackEnd(TempWidget);
				}
				vboxCubeList.ShowAll();
				scrolledCubeList.AddWithViewport(vboxCubeList);
			}

			//Загрузка Списка типов шкафов
			TypeWidgetList = new List<CupboardListItem>();
			hboxTypeList = new HBox(false, 6);
			sql = "SELECT * FROM basis";
			cmd = new SqliteCommand(sql, (SqliteConnection)QSMain.ConnectionDB);
			using (SqliteDataReader rdr = cmd.ExecuteReader())
			{
				GLib.SList Group = null;
				while(rdr.Read())
				{
					if (rdr["image"] == DBNull.Value)
						continue;

					//Добавляем виджеты в лист
					CupboardListItem TempWidget = new CupboardListItem();
					TempWidget.id = rdr.GetInt32(rdr.GetOrdinal("id"));
					TempWidget.ItemName = DBWorks.GetString(rdr, "name", "");
					TempWidget.CubePxSize = CubePxSize;
					if (Group == null)
						Group = TempWidget.Button.Group;
					else
						TempWidget.Button.Group = Group;
					int size = DBWorks.GetInt(rdr, "image_size", 0);
					byte[] ImageFile = new byte[size];
					rdr.GetBytes(rdr.GetOrdinal("image"), 0, ImageFile, 0, size);
					TempWidget.Image = new SVGHelper();
					if (!TempWidget.Image.LoadImage(ImageFile))
						continue;
					TempWidget.Button.Clicked += OnBasisChanged;
					TypeWidgetList.Add(TempWidget);
					hboxTypeList.PackEnd(TempWidget);
				}
				hboxTypeList.ShowAll();
				scrolledTypes.AddWithViewport(hboxTypeList);
			}

			OrderCupboard = new Cupboard();
			OrderCupboard.BorderImage = new SVGHelper();

			//Настраиваем DND
			Gtk.Drag.DestSet(drawCupboard, DestDefaults.Motion, TargetTable, Gdk.DragAction.Move);
			Gtk.Drag.SourceSet(drawCupboard, ModifierType.Button1Mask, TargetTable, Gdk.DragAction.Move);
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
					int basis_id = rdr.GetInt32(rdr.GetOrdinal("basis_id"));
					CupboardListItem basis = TypeWidgetList.Find(w => w.id == basis_id);
					basis.Button.Active = true;
					ComboWorks.SetActiveItem(comboExhibition, DBWorks.GetInt(rdr, "exhibition_id", -1));
					textviewComments.Buffer.Text = rdr["comment"].ToString();
					OrderCupboard = Cupboard.Load(rdr["cupboard"].ToString(), CubeList);
					comboCubeH.Active = OrderCupboard.CubesH - 1;
					comboCubeV.Active = OrderCupboard.CubesV - 1;
					SetInfo();
					CalculateCubePxSize(drawCupboard.Allocation);
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
				sql = "INSERT INTO orders (customer, exhibition_id, basis_id, arrval, delivery, comment, cupboard) " +
					"VALUES (@customer, @exhibition_id, @basis_id, @arrval, @delivery, @comment, @cupboard)";
			}
			else
			{
				sql = "UPDATE orders SET customer = @customer, exhibition_id = @exhibition_id, basis_id = @basis_id, arrval = @arrval, " +
					"delivery = @delivery, comment = @comment, cupboard = @cupboard WHERE id = @id";
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
				CupboardListItem basis = TypeWidgetList.Find(w => w.Button.Active);
				cmd.Parameters.AddWithValue("basis_id", DBWorks.ValueOrNull(basis != null, basis.id));
				cmd.Parameters.AddWithValue("exhibition_id", ComboWorks.GetActiveIdOrNull(comboExhibition));
				cmd.Parameters.AddWithValue("@comment", DBWorks.ValueOrNull(textviewComments.Buffer.Text != "", textviewComments.Buffer.Text));
				cmd.Parameters.AddWithValue("@cupboard", OrderCupboard.SaveToString());

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
			CupboardListItem basis = TypeWidgetList.Find(w => w.Button.Active);
			string name = basis != null ? basis.ItemName : "Тип не выбран";

			labelInfo.LabelProp = String.Format("{0} Ширина:{1}cм Высота:{2}cм", name, 
				int.Parse(comboCubeH.ActiveText) * 40, int.Parse(comboCubeV.ActiveText) * 40);
		}

		protected void OnBasisChanged(object sender, EventArgs e)
		{
			CupboardListItem basis = TypeWidgetList.Find(w => w.Button.Active);
			OrderCupboard.BorderImage.LoadImage(basis.Image.OriginalFile);
			SetInfo();
		}

		protected void OnComboCubeHChanged(object sender, EventArgs e)
		{
			SetInfo();
			OrderCupboard.CubesH = int.Parse(comboCubeH.ActiveText);
			if(OrderCupboard.BorderImage != null)
				OrderCupboard.BorderImage.ModifyDrawingImage();
			CalculateCubePxSize(drawCupboard.Allocation);
		}

		protected void OnComboCubeVChanged(object sender, EventArgs e)
		{
			SetInfo();
			OrderCupboard.CubesV = int.Parse(comboCubeV.ActiveText);
			if(OrderCupboard.BorderImage != null)
				OrderCupboard.BorderImage.ModifyDrawingImage();
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

			CupboardZeroX = ShiftX + BorderPxSize;
			CupboardZeroY = ShiftY + BorderPxSize;

			cr.Translate(CupboardZeroX, CupboardZeroY);
			DrawGrid(cr);
			cr.Save();
			if (OrderCupboard.BorderImage != null)
				OrderCupboard.BorderImage.DrawBasis(cr, CubePxSize);
			cr.Restore();

			foreach(Cube cube in OrderCupboard.Cubes)
			{
				cr.Save();
				cr.Translate(cube.BoardPositionX * CubePxSize, cube.BoardPositionY * CubePxSize);
				cube.DrawCube(cr, CubePxSize);
				cr.Restore();
			}
		}

		void DrawGrid(Context cr)
		{
			int CubesH = int.Parse(comboCubeH.ActiveText);
			int CubesV = int.Parse(comboCubeV.ActiveText);
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

		protected void OnDrawCupboardDragMotion(object o, DragMotionArgs args)
		{
			logger.Debug ("Drag motion x={0} y={1}", args.X, args.Y);
			int CubePosX = (args.X + (int)(CubePxSize * 1.5) - CurrentDrag.IconPosX - CupboardZeroX) / CubePxSize;
			int CubePosY = (args.Y + (int)(CubePxSize * 1.5) - CurrentDrag.IconPosY - CupboardZeroY) / CubePxSize;
			//Для того что бы корректно посчитать 0 ячейку добавли 1, сейчас онимаем.
			CubePosX--; CubePosY--;
			logger.Debug ("CupBoard pos x={0} y={1}", CubePosX, CubePosY);
			bool CanDrag = OrderCupboard.TestPutCube(CurrentDrag.cube, CubePosX, CubePosY);
			if (CanDrag)
				Gdk.Drag.Status(args.Context, args.Context.SuggestedAction,	args.Time);
			else
				Gdk.Drag.Status(args.Context, Gdk.DragAction.Private, args.Time);
			args.RetVal = true;
		}

		protected void OnDrawCupboardDragDrop(object o, DragDropArgs args)
		{
			logger.Debug ("Drop");
			int CubePosX = (args.X + (int)(CubePxSize * 1.5) - CurrentDrag.IconPosX - CupboardZeroX) / CubePxSize;
			int CubePosY = (args.Y + (int)(CubePxSize * 1.5) - CurrentDrag.IconPosY - CupboardZeroY) / CubePxSize;
			//Для того что бы корректно посчитать 0 ячейку добавли 1, сейчас онимаем.
			CubePosX--; CubePosY--;
			logger.Debug ("CupBoard pos x={0} y={1}", CubePosX, CubePosY);
			bool CanDrag = OrderCupboard.TestPutCube(CurrentDrag.cube, CubePosX, CubePosY);
			if (CanDrag)
			{
				if(CurrentDrag.FromList)
				{
					Cube NewCube = CurrentDrag.cube.Clone();
					NewCube.BoardPositionX = CubePosX;
					NewCube.BoardPositionY = CubePosY;
					OrderCupboard.Cubes.Add(NewCube);
				} 
				else
				{
					CurrentDrag.cube.BoardPositionX = CubePosX;
					CurrentDrag.cube.BoardPositionY = CubePosY;
				}
				Gtk.Drag.Finish(args.Context, true, false, args.Time);
			}
			else
				Gtk.Drag.Finish(args.Context, false, true, args.Time);
			drawCupboard.QueueDraw();
		}

		protected void OnDrawCupboardDragBegin(object o, DragBeginArgs args)
		{
			int MousePosX, MousePosY;
			drawCupboard.GetPointer(out MousePosX, out MousePosY);

			int CubePosX = (MousePosX - CupboardZeroX) / CubePxSize;
			int CubePosY = (MousePosY - CupboardZeroY) / CubePxSize;
			Cube cube = OrderCupboard.GetCube(CubePosX, CubePosY);

			if(cube == null)
			{
				args.RetVal = false;
				Gdk.Drag.Abort(args.Context, args.Context.StartTime);
				return;
			}

			Pixmap pix = new Pixmap(drawCupboard.GdkWindow, cube.CubesH * CubePxSize, cube.CubesV * CubePxSize);

			using (Context cr = Gdk.CairoHelper.Create(pix))
			{
				cube.DrawCube(cr, CubePxSize);
			}
			Gdk.Pixbuf pixbuf = Gdk.Pixbuf.FromDrawable(pix, Gdk.Colormap.System, 0, 0, 0, 0, cube.CubesH * CubePxSize, cube.CubesV * CubePxSize);
			CurrentDrag.IconPosX = MousePosX - CupboardZeroX - (cube.BoardPositionX * CubePxSize);
			CurrentDrag.IconPosY = MousePosY - CupboardZeroY - (cube.BoardPositionY * CubePxSize);

			Gtk.Drag.SetIconPixbuf(args.Context, pixbuf, CurrentDrag.IconPosX, CurrentDrag.IconPosY);
			CurrentDrag.FromList = false;
			CurrentDrag.cube = cube;
		}

		protected void OnDrawCupboardDragDataDelete(object o, DragDataDeleteArgs args)
		{
			OrderCupboard.Cubes.Remove(CurrentDrag.cube);
			args.RetVal = true;
		}

		protected void OnDrawCupboardMotionNotifyEvent(object o, MotionNotifyEventArgs args)
		{
			/*int CubePosX = ((int)args.Event.X - CupboardZeroX) / CubePxSize;
			int CubePosY = ((int)args.Event.Y - CupboardZeroY) / CubePxSize;
			Cube cube = OrderCupboard.GetCube(CubePosX, CubePosY);
			logger.Debug("Moition x={0} y={1}", CubePosX, CubePosY);

			if(cube == null)
				Gtk.Drag.SourceUnset(drawCupboard);
			else
			{
				Gtk.Drag.SourceSet(drawCupboard, ModifierType.Button1Mask, TargetTable, Gdk.DragAction.Move);
			} */
		}

		protected void OnDrawCupboardButtonPressEvent(object o, ButtonPressEventArgs args)
		{
			/*logger.Debug("Button");
			if(args.Event.Button == 1)
			{
				int CubePosX = ((int)args.Event.X - CupboardZeroX) / CubePxSize;
				int CubePosY = ((int)args.Event.Y - CupboardZeroY) / CubePxSize;
				Cube cube = OrderCupboard.GetCube(CubePosX, CubePosY);
				if(cube != null)
				{
					Gtk.Drag.SourceSet(drawCupboard, ModifierType.Button1Mask, TargetTable, Gdk.DragAction.Move);
					//Drag.Begin(drawCupboard, TargetTable, DragAction.Move, 1, )
				}
				else
					Gtk.Drag.SourceUnset(drawCupboard);
			} */
		}

	}
}

