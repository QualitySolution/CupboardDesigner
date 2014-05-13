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
		private int MaxCubeVSize;
		private bool VerticalTypeList = false;
		private bool VerticalCubeList = true;
		private List<CupboardListItem> TypeWidgetList;
		private List<CubeListItem> CubeWidgetList;
		private Cupboard OrderCupboard;
		private DragInformation CurrentDrag;

		private enum ComponentCol{
			row_id,
			nomenclature_type,
			nomenclature_id,
			nomenclature,
			count,
			material_id,
			material,
			facing_id,
			facing,
			comment
		}

		Gtk.TargetEntry[] TargetTable = new Gtk.TargetEntry[] {
			new Gtk.TargetEntry ("application/cube", Gtk.TargetFlags.App, 0)
		};

		public Order()
		{
			this.Build();

			ComboWorks.ComboFillReference(comboExhibition, "exhibition", ComboWorks.ListMode.WithNo);

			//Создаем таблицу номенклатуры
			ComboBox TempCombo = new ComboBox();
			ComboWorks.ComboFillReference(TempCombo, "materials", ComboWorks.ListMode.WithNo);
			MaterialNameList = TempCombo.Model;
			TempCombo.Destroy ();

			TempCombo = new ComboBox();
			ComboWorks.ComboFillReference(TempCombo, "facing", ComboWorks.ListMode.WithNo);
			FacingNameList = TempCombo.Model;
			TempCombo.Destroy ();

			ComponentsStore = new ListStore(typeof(long), typeof(Nomenclature.NomType), typeof(int), typeof(string), typeof(int), typeof(int), typeof(string), typeof(int), typeof(string), typeof(string));

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

			Gtk.TreeViewColumn ColumnComment = new Gtk.TreeViewColumn ();
			ColumnComment.Title = "Комментарий";
			Gtk.CellRendererText CellComment = new Gtk.CellRendererText ();
			CellComment.WrapMode = Pango.WrapMode.WordChar;
			CellComment.WrapWidth = 500;
			CellComment.Editable = true;
			CellComment.Edited += OnCommentTextEdited;
			ColumnComment.MaxWidth = 500;
			ColumnComment.PackStart (CellComment, true);
			ColumnComment.AddAttribute(CellComment, "text", (int)ComponentCol.comment);

			treeviewComponents.AppendColumn("Наименование", new Gtk.CellRendererText (), "text", (int)ComponentCol.nomenclature);
			treeviewComponents.AppendColumn("Кол-во", new Gtk.CellRendererText (), "text", (int)ComponentCol.count);
			treeviewComponents.AppendColumn(ColumnMaterial);
			treeviewComponents.AppendColumn(ColumnFacing);
			treeviewComponents.AppendColumn(ColumnComment);

			treeviewComponents.Model = ComponentsStore;
			treeviewComponents.ShowAll();

			CurrentDrag = new DragInformation();
			//Загрузка списка кубов
			CubeList = new List<Cube>();
			CubeWidgetList = new List<CubeListItem>();
			vboxCubeList = new VBox(false, 5);
			hboxCubeList = new HBox(false, 5);
			string sql = "SELECT * FROM nomenclature WHERE type = @type";
			SqliteCommand cmd = new SqliteCommand(sql, (SqliteConnection)QSMain.ConnectionDB);
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
					MaxCubeVSize = Math.Max(MaxCubeVSize, TempCube.CubesV);

					//Добавляем виджеты в лист
					CubeListItem TempWidget = new CubeListItem();
					TempCube.Widget = TempWidget;
					TempWidget.CubeItem = TempCube;
					TempWidget.CubePxSize = CubePxSize;
					TempWidget.DragInfo = CurrentDrag;
					CubeWidgetList.Add(TempWidget);
				}
				//hboxCubeList.Add(new Label("Ntcn dscjns"));
				UpdateCubeList();
				scrolledCubeListV.AddWithViewport(vboxCubeList);
				scrolledCubeListH.AddWithViewport(hboxCubeList);
			}

			//Загрузка Списка типов шкафов
			TypeWidgetList = new List<CupboardListItem>();
			hboxTypeList = new HBox(false, 3);
			vboxTypeList = new VBox(false, 3);
			sql = "SELECT * FROM basis";
			cmd = new SqliteCommand(sql, (SqliteConnection)QSMain.ConnectionDB);
			using (SqliteDataReader rdr = cmd.ExecuteReader())
			{
				Gtk.RadioButton FirstButton = null;
				while(rdr.Read())
				{
					if (rdr["image"] == DBNull.Value)
						continue;

					//Добавляем виджеты в лист
					CupboardListItem TempWidget = new CupboardListItem();
					TempWidget.id = rdr.GetInt32(rdr.GetOrdinal("id"));
					TempWidget.ItemName = DBWorks.GetString(rdr, "name", "");
					TempWidget.CubePxSize = CubePxSize;
					if (FirstButton == null)
						FirstButton = TempWidget.Button;
					else
						TempWidget.Button.Group = FirstButton.Group;
					int size = DBWorks.GetInt(rdr, "image_size", 0);
					byte[] ImageFile = new byte[size];
					rdr.GetBytes(rdr.GetOrdinal("image"), 0, ImageFile, 0, size);
					TempWidget.Image = new SVGHelper();
					if (!TempWidget.Image.LoadImage(ImageFile))
						continue;
					TempWidget.Button.Clicked += OnBasisChanged;
					TypeWidgetList.Add(TempWidget);
				}
				UpdateTypeList();
				scrolledTypesV.AddWithViewport(vboxTypeList);
				scrolledTypesH.AddWithViewport(hboxTypeList);
			}

			OrderCupboard = new Cupboard();

			//Настраиваем DND
			Gtk.Drag.DestSet(drawCupboard, DestDefaults.Motion, TargetTable, Gdk.DragAction.Move);
			Gtk.Drag.SourceSet(drawCupboard, ModifierType.Button1Mask, TargetTable, Gdk.DragAction.Move);
			Gtk.Drag.DestSet(vboxCubeList, DestDefaults.Motion, TargetTable, Gdk.DragAction.Move);
			Gtk.Drag.DestSet(hboxCubeList, DestDefaults.Motion, TargetTable, Gdk.DragAction.Move);
			vboxCubeList.DragDrop += OnCubeListDragDrop;
			hboxCubeList.DragDrop += OnCubeListDragDrop;
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

				CupboardListItem basis;
				using(SqliteDataReader rdr = cmd.ExecuteReader())
				{
					rdr.Read();

					this.Title = String.Format("Заказ №{0}", rdr["id"].ToString());
					entryCustomer.Text = rdr["customer"].ToString();

					dateArrval.Date = DBWorks.GetDateTime(rdr, "arrval", new DateTime());
					dateDelivery.Date = DBWorks.GetDateTime(rdr, "delivery", new DateTime());
					int basis_id = rdr.GetInt32(rdr.GetOrdinal("basis_id"));
					basis = TypeWidgetList.Find(w => w.id == basis_id);
					ComboWorks.SetActiveItem(comboExhibition, DBWorks.GetInt(rdr, "exhibition_id", -1));
					textviewComments.Buffer.Text = rdr["comment"].ToString();
					OrderCupboard = Cupboard.Load(rdr["cupboard"].ToString(), CubeList);
					comboCubeH.Active = OrderCupboard.CubesH - 1;
					comboCubeV.Active = OrderCupboard.CubesV - 1;
					SetInfo();
					CalculateCubePxSize(drawCupboard.Allocation);
				}

				sql = "SELECT order_components.*, materials.name as material, facing.name as facing, " +
					"nomenclature.name as nomenclature, nomenclature.type " +
					"FROM order_components " +
					"LEFT JOIN nomenclature ON nomenclature.id = order_components.nomenclature_id " +
					"LEFT JOIN materials ON materials.id = order_components.material_id " +
					"LEFT JOIN facing ON facing.id = order_components.facing_id " +
					"WHERE order_components.order_id = @id";
				cmd = new SqliteCommand(sql, (SqliteConnection)QSMain.ConnectionDB);
				cmd.Parameters.AddWithValue("@id", id);
				using(SqliteDataReader rdr = cmd.ExecuteReader())
				{
					while(rdr.Read())
					{
						ComponentsStore.AppendValues(
							DBWorks.GetLong(rdr, "id", -1),
							Enum.Parse(typeof(Nomenclature.NomType), rdr["type"].ToString()),
							DBWorks.GetInt(rdr, "nomenclature_id", -1),
							DBWorks.GetString(rdr, "nomenclature", "нет"),
							DBWorks.GetInt(rdr, "count", 0),
							DBWorks.GetInt(rdr, "material_id", -1),
							DBWorks.GetString(rdr, "material", "нет"),
							DBWorks.GetInt(rdr, "facing_id", -1),
							DBWorks.GetString(rdr, "facing", "нет"),
							DBWorks.GetString(rdr, "comment", "")
						);
					}
				}
				basis.Button.Active = true;

				MainClass.StatusMessage("Ok");
			}
			catch (Exception ex)
			{
				logger.ErrorException("Ошибка получения информации о заказе!", ex);
				QSMain.ErrorMessage(this, ex);
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

		void OnCommentTextEdited (object o, EditedArgs args)
		{
			TreeIter iter;
			if (!ComponentsStore.GetIterFromString (out iter, args.Path))
				return;
			if(args.NewText == null)
			{
				logger.Warn("newtext is empty");
				return;
			}

			ComponentsStore.SetValue(iter, (int)ComponentCol.comment, args.NewText);
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
					bool HasValue = (int) row[(int)ComponentCol.count] > 0;
                    if(HasValue)
				    {
						if((long)row[(int)ComponentCol.row_id] < 0)
							sql = "INSERT INTO order_components (order_id, nomenclature_id, count, material_id, facing_id, comment) " +
								"VALUES (@order_id, @nomenclature_id, @count, @material_id, @facing_id, @comment)";
						else
							sql = "UPDATE order_components SET material_id = @material_id, facing_id = @facing_id, count = @count, " +
								"comment = @comment " +
								"WHERE id = @id";

						cmd = new SqliteCommand(sql, (SqliteConnection)QSMain.ConnectionDB, trans);
						cmd.Parameters.AddWithValue("@id", (long)row[(int)ComponentCol.row_id]);
						cmd.Parameters.AddWithValue("@order_id", ItemId);
						cmd.Parameters.AddWithValue("@nomenclature_id", row[(int)ComponentCol.nomenclature_id]);
						cmd.Parameters.AddWithValue("@count", row[(int)ComponentCol.count]);
						cmd.Parameters.AddWithValue("@material_id", DBWorks.ValueOrNull((int) row[(int)ComponentCol.material_id] > 0, row[(int)ComponentCol.material_id]));
						cmd.Parameters.AddWithValue("@facing_id", DBWorks.ValueOrNull((int)row[(int)ComponentCol.facing_id] > 0, row[(int)ComponentCol.facing_id]));
						cmd.Parameters.AddWithValue("@comment", DBWorks.ValueOrNull((string)row[(int)ComponentCol.comment] != "", row[(int)ComponentCol.comment]));
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
				logger.ErrorException("Ошибка записи заказа!", ex);
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
			if(basis == null)
			{
				logger.Warn("Не найдена активная основа");
				return;
			}
			OrderCupboard.BorderImage.LoadImage(basis.Image.OriginalFile);
			SetInfo();
			UpdateBasisComponents(basis.id);
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
			int HeightTable = tableConstructor.Allocation.Height;

			//Добавляем высоту листа кубов с низу, что бы ресайз не зацикливался.
			int ListCybesV = !VerticalCubeList ? MaxCubeVSize : 0;
			int ListPixelAdd = !VerticalCubeList ? scrolledCubeListH.HScrollbar.HeightRequest + 48 : 0;
			if(!VerticalCubeList)
			{
				HeightWithoutGrid = HeightTable - ListPixelAdd;
			}

			// 1.2 это 2 бортика по караям которые равны 60% от куба
			int MinCubeSizeForH = Convert.ToInt32(WidhtWithoutGrid / (double.Parse(comboCubeH.ActiveText) + 1.2));
			int MinCubeSizeForV = Convert.ToInt32(HeightWithoutGrid / (double.Parse(comboCubeV.ActiveText) + 1.2 + ListCybesV));

			int NeedCubePxSize = Math.Min(MinCubeSizeForH, MinCubeSizeForV);

			if (NeedCubePxSize > 100)
				NeedCubePxSize = 100;

			if(CubePxSize != NeedCubePxSize)
			{
				CubePxSize = NeedCubePxSize;
				BorderPxSize = Convert.ToInt32(CubePxSize * 0.3);

				int MaxHeight = 0;
				foreach(Cube cube in CubeList)
				{
					((CubeListItem)cube.Widget).CubePxSize = CubePxSize;
					Requisition req = ((CubeListItem)cube.Widget).SizeRequest();
					MaxHeight = Math.Max(MaxHeight, req.Height);
				}
				hboxCubeList.HeightRequest = MaxHeight;
				int h = hboxCubeList.HeightRequest;
				logger.Debug("h={0}", MaxHeight);
				((Viewport)scrolledCubeListH.Child).CheckResize();
				scrolledCubeListH.CheckResize();
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
					UpdateCubeComponents();
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
			UpdateCubeComponents();
			args.RetVal = true;
		}

		protected void OnCubeListDragDrop(object o, DragDropArgs args)
		{
			logger.Debug ("Drop to CubeList");
			if (CurrentDrag.FromList)
				Gtk.Drag.Finish(args.Context, false, false, args.Time);
			else
			{
				Gtk.Drag.Finish(args.Context, true, true, args.Time);
				drawCupboard.QueueDraw();
			}
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

		protected void OnButtonTypeListOrientClicked(object sender, EventArgs e)
		{
			VerticalTypeList = !VerticalTypeList;
			UpdateTypeList();
		}

		void UpdateTypeList()
		{
			scrolledTypesV.Visible = VerticalTypeList;
			scrolledTypesH.Visible = !VerticalTypeList;
			Box boxTypeList = VerticalTypeList ? (Box)vboxTypeList : (Box) hboxTypeList;
			Box ForRemove = !VerticalTypeList ? (Box)vboxTypeList : (Box) hboxTypeList;

			foreach( CupboardListItem item in TypeWidgetList)
			{
				if(item.Parent != null)
					ForRemove.Remove(item);
				boxTypeList.PackEnd(item);
			}
			boxTypeList.ShowAll();
		}

		void UpdateCubeList()
		{
			scrolledCubeListV.Visible = VerticalCubeList;
			scrolledCubeListH.Visible = !VerticalCubeList;
			Box boxCubeList = VerticalCubeList ? (Box)vboxCubeList : (Box) hboxCubeList;
			Box ForRemove = !VerticalCubeList ? (Box)vboxCubeList : (Box) hboxCubeList;

			foreach(CubeListItem item in CubeWidgetList)
			{
				if(item.Parent != null)
					ForRemove.Remove(item);
				boxCubeList.Add(item);
			}
			boxCubeList.ShowAll();
		}

		protected void OnButtonCubeListOrientationClicked(object sender, EventArgs e)
		{
			VerticalCubeList = !VerticalCubeList;
			UpdateCubeList();
		}

		protected void OnNotebook1SwitchPage(object o, SwitchPageArgs args)
		{
			if (notebook1.CurrentPage == 1)
			{
				if (OrderCupboard.Clean())
					UpdateCubeComponents();
			}
		}

		private void UpdateBasisComponents(int id)
		{
			TreeIter iter;
			if (ComponentsStore.GetIterFirst(out iter))
			{
				do
				{
					if ((Nomenclature.NomType)ComponentsStore.GetValue(iter, (int)ComponentCol.nomenclature_type) == Nomenclature.NomType.construct)
					{
						if((long)ComponentsStore.GetValue(iter, (int)ComponentCol.row_id) > 0)
							ComponentsStore.SetValue(iter, (int)ComponentCol.count, 0);
						else
							ComponentsStore.Remove(ref iter);
					}
				}
				while(ComponentsStore.IterNext(ref iter));
			}
				
			string sql = "SELECT nomenclature.name as nomenclature, nomenclature.type, basis_items.* FROM basis_items " +
				"LEFT JOIN nomenclature ON nomenclature.id = basis_items.item_id " +
				"WHERE basis_id = @basis_id";
			SqliteCommand cmd = new SqliteCommand(sql, (SqliteConnection)QSMain.ConnectionDB);
			cmd.Parameters.AddWithValue("@basis_id", id);
			using (SqliteDataReader rdr = cmd.ExecuteReader())
			{
				while (rdr.Read())
				{
					bool Found = false;
					if (ComponentsStore.GetIterFirst(out iter))
					{
						do
						{
							if ((int)ComponentsStore.GetValue(iter, (int)ComponentCol.nomenclature_id) == rdr.GetInt32(rdr.GetOrdinal("item_id")))
							{
								Found = true;
								ComponentsStore.SetValue(iter, (int)ComponentCol.count, 1);
								break;
							}
						}
						while(ComponentsStore.IterNext(ref iter));
					}
					if(!Found)
					{
						ComponentsStore.AppendValues(
							(long)-1,
							Enum.Parse(typeof(Nomenclature.NomType), rdr["type"].ToString()),
							DBWorks.GetInt(rdr, "item_id", -1),
							DBWorks.GetString(rdr, "nomenclature", "нет"),
							1,
							-1,
							"",
							-1,
							"",
							""
						);
					}
				}
			}
			CalculateTotalCount();
		}

		private void UpdateCubeComponents()
		{
			TreeIter iter;
			Dictionary<int, int> Counts = OrderCupboard.GetAmounts();
			if (ComponentsStore.GetIterFirst(out iter))
			{
				do
				{
					if ((Nomenclature.NomType)ComponentsStore.GetValue(iter, (int)ComponentCol.nomenclature_type) == Nomenclature.NomType.cube)
					{
						int NomId = (int)ComponentsStore.GetValue(iter, (int)ComponentCol.nomenclature_id);
						if(Counts.ContainsKey(NomId))
						{
							ComponentsStore.SetValue(iter, (int)ComponentCol.count, Counts[NomId]);
							Counts.Remove(NomId);
						}
						else if((long)ComponentsStore.GetValue(iter, (int)ComponentCol.row_id) > 0)
							ComponentsStore.SetValue(iter, (int)ComponentCol.count, 0);
						else
							ComponentsStore.Remove(ref iter);
					}
				}
				while(ComponentsStore.IterNext(ref iter));
			}

			foreach (KeyValuePair<int, int> pair in Counts)
			{
				Cube cube = OrderCupboard.Cubes.Find(c => c.NomenclatureId == pair.Key);
				ComponentsStore.AppendValues(
					(long)-1,
					Nomenclature.NomType.cube,
					pair.Key,
					cube.Name,
					1,
					-1,
					"",
					-1,
					"",
					""
				);
			}
			CalculateTotalCount();
		}

		void CalculateTotalCount()
		{
			int TotalCount = 0;
			foreach(object[] row in ComponentsStore)
			{
				TotalCount += (int)row[(int)ComponentCol.count];
			}

			labelTotalCount.LabelProp = String.Format("Итого {0} единиц", TotalCount);
		}
	}
}

