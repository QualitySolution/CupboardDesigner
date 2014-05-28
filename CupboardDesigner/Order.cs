﻿using System;
using System.IO;
using NLog;
using Gtk;
using Mono.Data.Sqlite;
using QSProjectsLib;
using System.Collections.Generic;
using Cairo;
using Gdk;

namespace CupboardDesigner
{
	public partial class Order : Gtk.Window
	{
		private static Logger logger = LogManager.GetCurrentClassLogger();
		private bool NewItem = true;
		private int ItemId;
		private ListStore ComponentsStore;
		private TreeModel MaterialNameList, FacingNameList;
		private List<Cube> CubeList;
		private int CubePxSize = 100;
		private VBox vboxCubeList;
		private HBox hboxCubeList, hboxTypeList;
		private int MaxCubeVSize, MaxCubeHSize;
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

		public Order() : 
			base(Gtk.WindowType.Toplevel)
		{
			this.Build();
			ComboWorks.ComboFillReference(comboExhibition, "exhibition", ComboWorks.ListMode.WithNo, true, "ordinal");
			dateArrval.Date = DateTime.Today;

			//Создаем таблицу номенклатуры
			ComboBox TempCombo = new ComboBox();
			ComboWorks.ComboFillReference(TempCombo, "materials", ComboWorks.ListMode.WithNo, true, "ordinal");
			MaterialNameList = TempCombo.Model;
			TempCombo.Destroy ();

			TempCombo = new ComboBox();
			ComboWorks.ComboFillReference(TempCombo, "facing", ComboWorks.ListMode.WithNo, true, "ordinal");
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

			treeviewComponents.AppendColumn("Название", new Gtk.CellRendererText (), "text", (int)ComponentCol.nomenclature);
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
			vboxCubeList = new VBox(false, 6);
			hboxCubeList = new HBox(false, 20);
			string sql = "SELECT * FROM nomenclature WHERE type = @type ORDER BY ordinal";
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
					byte[] ImageFile = new byte[size];
					rdr.GetBytes(rdr.GetOrdinal("image"), 0, ImageFile, 0, size);
					TempCube.LoadSvg(ImageFile);
					CubeList.Add(TempCube);
					MaxCubeVSize = Math.Max(MaxCubeVSize, TempCube.CubesV);
					MaxCubeHSize = Math.Max(MaxCubeHSize, TempCube.CubesH);

					//Добавляем виджеты в лист
					CubeListItem TempWidget = new CubeListItem();
					TempCube.Widget = TempWidget;
					TempWidget.CubeItem = TempCube;
					TempWidget.CubePxSize = CubePxSize;
					TempWidget.DragInfo = CurrentDrag;
					CubeWidgetList.Add(TempWidget);
				}
				UpdateCubeList();
				scrolledCubeListV.AddWithViewport(vboxCubeList);
				scrolledCubeListH.AddWithViewport(hboxCubeList);
			}

			//Загрузка Списка типов шкафов
			TypeWidgetList = new List<CupboardListItem>();
			hboxTypeList = new HBox(false, 2);
			Stream stream = System.Reflection.Assembly.GetExecutingAssembly().GetManifestResourceStream( "CupboardDesigner.icons.Yes_check.svg" );
			byte[] temparray;
			using(MemoryStream mstream = new MemoryStream())
			{
				stream.CopyTo(mstream);
				temparray = mstream.ToArray();
			}
			Rsvg.Handle CheckImage = new Rsvg.Handle(temparray);
			sql = "SELECT * FROM basis ORDER BY ordinal ";
			cmd = new SqliteCommand(sql, (SqliteConnection)QSMain.ConnectionDB);
			using (SqliteDataReader rdr = cmd.ExecuteReader())
			{
				Gtk.RadioButton FirstButton = null;
				while(rdr.Read())
				{
					if (rdr["image"] == DBNull.Value)
						continue;

					//Добавляем виджеты в лист
					CupboardListItem TempWidget = new CupboardListItem(CheckImage);
					TempWidget.id = rdr.GetInt32(rdr.GetOrdinal("id"));
					TempWidget.DeltaH = rdr.GetInt32(rdr.GetOrdinal("delta_h"));
					TempWidget.DeltaL = rdr.GetInt32(rdr.GetOrdinal("delta_l"));
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
					hboxTypeList.Add(TempWidget);
				}
				scrolledTypesH.AddWithViewport(hboxTypeList);
				hboxTypeList.ShowAll();
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
					entryPhone1.Text = rdr["phone1"].ToString();
					entryPhone2.Text = rdr["phone2"].ToString();
					textAddress.Buffer.Text = rdr["address"].ToString();
					dateArrval.Date = DBWorks.GetDateTime(rdr, "arrval", new DateTime());
					dateDelivery.Date = DBWorks.GetDateTime(rdr, "delivery", new DateTime());
					int basis_id = rdr.GetInt32(rdr.GetOrdinal("basis_id"));
					basis = TypeWidgetList.Find(w => w.id == basis_id);
					ComboWorks.SetActiveItem(comboExhibition, DBWorks.GetInt(rdr, "exhibition_id", -1));
					textviewComments.Buffer.Text = rdr["comment"].ToString();
					OrderCupboard = Cupboard.Load(rdr["cupboard"].ToString(), CubeList);
					comboCubeH.Active = OrderCupboard.CubesH - 1;
					comboCubeV.Active = OrderCupboard.CubesV - 1;
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
			bool Dateok = dateArrval.IsEmpty || dateDelivery.IsEmpty || dateArrval.Date <= dateDelivery.Date;

			saveAction.Sensitive = printAction.Sensitive = Dateok;
		}

		private bool Save()
		{
			string sql;
			if (NewItem)
			{
				sql = "INSERT INTO orders (customer, address, phone1, phone2, exhibition_id, basis_id, arrval, delivery, comment, cupboard) " +
					"VALUES (@customer, @address, @phone1, @phone2, @exhibition_id, @basis_id, @arrval, @delivery, @comment, @cupboard)";
			}
			else
			{
				sql = "UPDATE orders SET customer = @customer, address = @address, phone1 = @phone1, phone2 = @phone2, " +
					"exhibition_id = @exhibition_id, basis_id = @basis_id, arrval = @arrval, " +
					"delivery = @delivery, comment = @comment, cupboard = @cupboard WHERE id = @id";
			}
			SqliteTransaction trans = ((SqliteConnection)QSMain.ConnectionDB).BeginTransaction();
			MainClass.StatusMessage("Запись заказа...");
			try
			{
				SqliteCommand cmd = new SqliteCommand(sql, (SqliteConnection)QSMain.ConnectionDB, trans);

				cmd.Parameters.AddWithValue("@id", ItemId);
				cmd.Parameters.AddWithValue("@customer", DBWorks.ValueOrNull(entryCustomer.Text != "", entryCustomer.Text));
				cmd.Parameters.AddWithValue("@address", DBWorks.ValueOrNull(textAddress.Buffer.Text != "", textAddress.Buffer.Text));
				cmd.Parameters.AddWithValue("@phone1", DBWorks.ValueOrNull(entryPhone1.Text != "", entryPhone1.Text));
				cmd.Parameters.AddWithValue("@phone2", DBWorks.ValueOrNull(entryPhone2.Text != "", entryPhone2.Text));
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
					NewItem = false;
				}

				// Запись компонент
				TreeIter iter;
				if(ComponentsStore.GetIterFirst(out iter))

				{
					do
					{
						bool HasValue = (int) ComponentsStore.GetValue(iter, (int)ComponentCol.count) > 0;
						bool InDB = (long)ComponentsStore.GetValue(iter, (int)ComponentCol.row_id) > 0;
						if(HasValue)
						{
							if(!InDB)
								sql = "INSERT INTO order_components (order_id, nomenclature_id, count, material_id, facing_id, comment) " +
									"VALUES (@order_id, @nomenclature_id, @count, @material_id, @facing_id, @comment)";
							else
								sql = "UPDATE order_components SET material_id = @material_id, facing_id = @facing_id, count = @count, " +
									"comment = @comment " +
									"WHERE id = @id";

							cmd = new SqliteCommand(sql, (SqliteConnection)QSMain.ConnectionDB, trans);
							cmd.Parameters.AddWithValue("@id", (long)ComponentsStore.GetValue(iter, (int)ComponentCol.row_id));
							cmd.Parameters.AddWithValue("@order_id", ItemId);
							cmd.Parameters.AddWithValue("@nomenclature_id", ComponentsStore.GetValue(iter,(int)ComponentCol.nomenclature_id));
							cmd.Parameters.AddWithValue("@count", ComponentsStore.GetValue(iter,(int)ComponentCol.count));
							cmd.Parameters.AddWithValue("@material_id", DBWorks.ValueOrNull((int) ComponentsStore.GetValue(iter,(int)ComponentCol.material_id) > 0, ComponentsStore.GetValue(iter, (int)ComponentCol.material_id)));
							cmd.Parameters.AddWithValue("@facing_id", DBWorks.ValueOrNull((int)ComponentsStore.GetValue(iter, (int)ComponentCol.facing_id) > 0, ComponentsStore.GetValue(iter, (int)ComponentCol.facing_id)));
							cmd.Parameters.AddWithValue("@comment", DBWorks.ValueOrNull((string)ComponentsStore.GetValue(iter, (int)ComponentCol.comment) != "", ComponentsStore.GetValue(iter, (int)ComponentCol.comment)));
							cmd.ExecuteNonQuery();
							if(!InDB)
							{
								sql = @"select last_insert_rowid()";
								cmd = new SqliteCommand(sql, (SqliteConnection)QSMain.ConnectionDB, trans);
								long RowId = (long) cmd.ExecuteScalar();
								ComponentsStore.SetValue(iter, (int)ComponentCol.row_id, (object)RowId);
							}
						}
						else if(InDB)
						{
							sql = "DELETE FROM order_components WHERE id = @id";

							cmd = new SqliteCommand(sql, (SqliteConnection)QSMain.ConnectionDB, trans);
							cmd.Parameters.AddWithValue("@id", ComponentsStore.GetValue(iter, (int)ComponentCol.row_id));
							cmd.ExecuteNonQuery();
						}
					}
					while(ComponentsStore.IterNext(ref iter));
				}

				trans.Commit();
				MainClass.StatusMessage("Ok");
				MainClass.MainWin.UpdateOrders();
				return true;
			}
			catch (Exception ex)
			{
				trans.Rollback();
				logger.ErrorException("Ошибка записи заказа!", ex);
				QSMain.ErrorMessage(this, ex);
				return false;
			}
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
			UpdateBasisComponents(basis.id);
		}

		protected void OnComboCubeHChanged(object sender, EventArgs e)
		{
			OrderCupboard.CubesH = int.Parse(comboCubeH.ActiveText);
			if(OrderCupboard.BorderImage != null)
				OrderCupboard.BorderImage.ModifyDrawingImage();
			CalculateCubePxSize(drawCupboard.Allocation);
		}

		protected void OnComboCubeVChanged(object sender, EventArgs e)
		{
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
				OrderCupboard.Draw(cr, w, h, CubePxSize, false);
			}
		}

		protected void OnDrawCupboardSizeAllocated(object o, SizeAllocatedArgs args)
		{
			CalculateCubePxSize(args.Allocation);
		}

		private void CalculateCubePxSize(Gdk.Rectangle CupboardPlace)
		{

			int WidthWithoutGrid = CupboardPlace.Width ;
			int HeightWithoutGrid = CupboardPlace.Height;
			int HeightTable = tableConstructor.Allocation.Height;
			int WidthTable = tableConstructor.Allocation.Width;

			//Добавляем высоту листа кубов с низу, что бы ресайз не зацикливался.
			int ListCybesV = !VerticalCubeList ? MaxCubeVSize : 0;
			int ListCybesH = VerticalCubeList ? MaxCubeHSize : 0;
			int ListPixelAddV = !VerticalCubeList ? scrolledCubeListH.HScrollbar.HeightRequest + 64 : 0;
			int ListPixelAddH = VerticalCubeList ? scrolledCubeListV.VScrollbar.WidthRequest + 16 : 0;
			if(VerticalCubeList)
			{
				WidthWithoutGrid = WidthTable - ListPixelAddH;
			}
			else
			{
				HeightWithoutGrid = HeightTable - ListPixelAddV;
			}

			// 1.2 это 2 бортика по караям которые равны 60% от куба
			int MinCubeSizeForH = Convert.ToInt32(WidthWithoutGrid / (double.Parse(comboCubeH.ActiveText) + 1.2 + ListCybesH));
			int MinCubeSizeForV = Convert.ToInt32(HeightWithoutGrid / (double.Parse(comboCubeV.ActiveText) + 1.2 + ListCybesV));

			int NeedCubePxSize = Math.Min(MinCubeSizeForH, MinCubeSizeForV);

			if (NeedCubePxSize > 100)
				NeedCubePxSize = 100;

			if(CubePxSize != NeedCubePxSize)
			{
				CubePxSize = NeedCubePxSize;

				int MaxHeight = 0, MaxWidth = 0;
				foreach(Cube cube in CubeList)
				{
					((CubeListItem)cube.Widget).CubePxSize = CubePxSize;
					Requisition req = ((CubeListItem)cube.Widget).SizeRequest();
					MaxHeight = Math.Max(MaxHeight, req.Height);
					MaxWidth = Math.Max(MaxWidth, req.Width);
				}
				hboxCubeList.HeightRequest = MaxHeight;
				vboxCubeList.WidthRequest = MaxWidth;
			}
		}

		protected void OnDrawCupboardDragMotion(object o, DragMotionArgs args)
		{
			logger.Debug ("Drag motion x={0} y={1}", args.X, args.Y);
			int CubePosX = (args.X + (int)(CubePxSize * 1.5) - CurrentDrag.IconPosX - OrderCupboard.CupboardZeroX) / CubePxSize;
			int CubePosY = (args.Y + (int)(CubePxSize * 1.5) - CurrentDrag.IconPosY - OrderCupboard.CupboardZeroY) / CubePxSize;
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
			int CubePosX = (args.X + (int)(CubePxSize * 1.5) - CurrentDrag.IconPosX - OrderCupboard.CupboardZeroX) / CubePxSize;
			int CubePosY = (args.Y + (int)(CubePxSize * 1.5) - CurrentDrag.IconPosY - OrderCupboard.CupboardZeroY) / CubePxSize;
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

			int CubePosX = (MousePosX - OrderCupboard.CupboardZeroX) / CubePxSize;
			int CubePosY = (MousePosY - OrderCupboard.CupboardZeroY) / CubePxSize;
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
				cube.DrawCube(cr, CubePxSize, true);
			}
			Gdk.Pixbuf pixbuf = Gdk.Pixbuf.FromDrawable(pix, Gdk.Colormap.System, 0, 0, 0, 0, cube.CubesH * CubePxSize, cube.CubesV * CubePxSize);
			CurrentDrag.IconPosX = MousePosX - OrderCupboard.CupboardZeroX - (cube.BoardPositionX * CubePxSize);
			CurrentDrag.IconPosY = MousePosY - OrderCupboard.CupboardZeroY - (cube.BoardPositionY * CubePxSize);

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
			goBackAction.Sensitive = notebook1.CurrentPage != 0;
			goForwardAction.Sensitive = notebook1.CurrentPage != 2;
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

		protected void OnPrintActionActivated(object sender, EventArgs e)
		{
			if (!Save())
				return;
			string TempImagePath = System.IO.Path.Combine (System.IO.Path.GetTempPath (), String.Format("Cupboard{0}.png", ItemId));
			int widght = 2244;
			int height = 1181;
			ImageSurface surf = new ImageSurface(Format.ARGB32, widght, height);
			Cairo.Context cr = new Context(surf);

			int MinCubeSizeForH = Convert.ToInt32(widght / (OrderCupboard.CubesH + 1.2));
			int MinCubeSizeForV = Convert.ToInt32(height / (OrderCupboard.CubesV + 1.2));
			int NeedCubePxSize = Math.Min(MinCubeSizeForH, MinCubeSizeForV);

			OrderCupboard.Draw(cr, widght, height, NeedCubePxSize, true);
			surf.Flush();
			surf.WriteToPng(TempImagePath);
			logger.Debug("Writed {0}", TempImagePath);
			string param = "id=" + ItemId.ToString() +
				"&image=" + TempImagePath +
				"&basel=" + OrderCupboard.CubesH.ToString() + 
				"&baseh=" + OrderCupboard.CubesV.ToString();
			ViewReportExt.Run ("order", param);
		}

		protected void OnOrderDatesChanged(object sender, EventArgs e)
		{
			TestCanSave();
			bool Dateok = dateArrval.IsEmpty || dateDelivery.IsEmpty || dateArrval.Date <= dateDelivery.Date;
			if(!Dateok)
			{
				MessageDialog md = new MessageDialog ( this, DialogFlags.DestroyWithParent,
					MessageType.Warning, 
					ButtonsType.Ok, 
					"Дата сдачи должна быть позже даты прихода.");
				md.Run ();
				md.Destroy();
			}
		}

		protected void OnSaveActionActivated(object sender, EventArgs e)
		{
			if (Save())
				this.Destroy();
		}

		protected void OnGoBackActionActivated(object sender, EventArgs e)
		{
			notebook1.PrevPage();
		}

		protected void OnGoForwardActionActivated(object sender, EventArgs e)
		{
			notebook1.NextPage();
		}

		protected void OnRevertToSavedActionActivated(object sender, EventArgs e)
		{
			this.Destroy();
		}
	}
}

