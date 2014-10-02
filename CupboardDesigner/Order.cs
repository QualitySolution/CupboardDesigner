using System;
using System.IO;
using System.Globalization;
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
		private bool FillInProgress = false;
		private TreeStore ComponentsStore;
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
		private TreeIter BasisIter;
		private Decimal TotalPrice;
		private int PriceCorrection = 0;
		private Gtk.TreeViewColumn ColumnPrice;
		private Gtk.TreeViewColumn ColumnCount;
		private Gtk.TreeViewColumn ColumnMaterial;
		private Gtk.TreeViewColumn ColumnFacing;
		private Gtk.TreeViewColumn ColumnPriceTotal;
		private Gtk.TreeViewColumn ColumnComment;
		Gtk.TargetEntry[] TargetTable = new Gtk.TargetEntry[] {
			new Gtk.TargetEntry ("application/cube", Gtk.TargetFlags.App, 0)
		};

		private enum ComponentCol{
			row_id,
			nomenclature_type,
			nomenclature_id,
			nomenclature,
			nomenclature_title,
			nomenclature_description,
			count,
			material_id,
			material,
			facing_id,
			facing,
			comment,
			price,
			price_total,
			editable_count,
			editable_price,
			editable_material,
			editable_facing,
			editable_comment
		}

		public Order() : base(Gtk.WindowType.Toplevel)
		{
			this.Build();
			notebook1.CurrentPage = 0;
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

			ComponentsStore = new TreeStore(typeof(long), typeof(Nomenclature.NomType), typeof(int), typeof(string), typeof(string), typeof(string), typeof(int), typeof(int), typeof(string), typeof(int), typeof(string), typeof(string), typeof(string), typeof(string), typeof(bool), typeof(bool), typeof(bool), typeof(bool), typeof(bool));
			BasisIter = ComponentsStore.AppendValues ((long)-1, Enum.Parse(typeof(Nomenclature.NomType), "construct"), 1, null, "Каркас", null, 1, -1, "", -1, "", "", "", "", false, false, false, false, false);

			ColumnCount = new Gtk.TreeViewColumn ();
			ColumnCount.Title = "Кол-во";
			Gtk.CellRendererText CellCount = new CellRendererText ();
			CellCount.Editable = true;
			CellCount.Edited += OnCountEdited;
			ColumnCount.PackStart (CellCount, true);
			ColumnCount.AddAttribute(CellCount, "text", (int)ComponentCol.count);
			ColumnCount.AddAttribute(CellCount, "editable", (int)ComponentCol.editable_count);

			ColumnMaterial = new Gtk.TreeViewColumn ();
			ColumnMaterial.Title = "Отделка кубов";
			ColumnMaterial.MinWidth = 180;
			Gtk.CellRendererCombo CellMaterial = new CellRendererCombo();
			CellMaterial.TextColumn = 0;
			CellMaterial.Editable = true;
			CellMaterial.Model = MaterialNameList;
			CellMaterial.HasEntry = false;
			CellMaterial.Edited += OnMaterialComboEdited;
			ColumnMaterial.PackStart (CellMaterial, true);
			ColumnMaterial.AddAttribute(CellMaterial, "text", (int)ComponentCol.material);
			ColumnMaterial.AddAttribute(CellMaterial, "editable", (int)ComponentCol.editable_material);

			ColumnFacing = new Gtk.TreeViewColumn ();
			ColumnFacing.Title = "Отделка фасада";
			ColumnFacing.MinWidth = 180;
			Gtk.CellRendererCombo CellFacing = new CellRendererCombo();
			CellFacing.TextColumn = 0;
			CellFacing.Editable = true;
			CellFacing.Model = FacingNameList;
			CellFacing.HasEntry = false;
			CellFacing.Edited += OnFacingComboEdited;
			ColumnFacing.PackStart (CellFacing, true);
			ColumnFacing.AddAttribute(CellFacing, "text", (int)ComponentCol.facing);
			ColumnFacing.AddAttribute(CellFacing, "editable", (int)ComponentCol.editable_facing);


			ColumnPrice = new Gtk.TreeViewColumn ();
			ColumnPrice.Title = "Цена";
			ColumnPrice.Visible = false;
			Gtk.CellRendererText CellPrice = new CellRendererText ();
			CellPrice.Editable = true;
			CellPrice.Edited += OnPriceEdited;
			ColumnPrice.PackStart (CellPrice, true);
			ColumnPrice.AddAttribute(CellPrice, "text", (int)ComponentCol.price);
			ColumnPrice.AddAttribute(CellPrice, "editable", (int)ComponentCol.editable_price);

			ColumnPriceTotal = new Gtk.TreeViewColumn ();
			ColumnPriceTotal.Title = "Сумма";
			ColumnPriceTotal.Visible = false;
			Gtk.CellRendererText CellPriceTotal = new CellRendererText ();
			CellPriceTotal.Editable = false;
			ColumnPriceTotal.PackStart (CellPriceTotal, true);
			ColumnPriceTotal.AddAttribute(CellPriceTotal, "text", (int)ComponentCol.price_total);


			ColumnComment = new Gtk.TreeViewColumn ();
			ColumnComment.Title = "Комментарий";
			Gtk.CellRendererText CellComment = new Gtk.CellRendererText ();
			CellComment.WrapMode = Pango.WrapMode.WordChar;
			CellComment.WrapWidth = 500;
			CellComment.Editable = true;
			CellComment.Edited += OnCommentTextEdited;
			ColumnComment.MaxWidth = 500;
			ColumnComment.PackStart (CellComment, true);
			ColumnComment.AddAttribute(CellComment, "text", (int)ComponentCol.comment);
			ColumnComment.AddAttribute(CellComment, "editable", (int)ComponentCol.editable_comment);

			treeviewComponents.AppendColumn("Название", new Gtk.CellRendererText (), "text", (int)ComponentCol.nomenclature_title);
			treeviewComponents.AppendColumn(ColumnCount);
			treeviewComponents.AppendColumn (ColumnPrice);
			treeviewComponents.AppendColumn (ColumnPriceTotal);
			treeviewComponents.AppendColumn(ColumnMaterial);
			treeviewComponents.AppendColumn(ColumnFacing);
			treeviewComponents.AppendColumn(ColumnComment);
			treeviewComponents.Model = ComponentsStore;
			treeviewComponents.TooltipColumn = (int)ComponentCol.nomenclature_description;
			treeviewComponents.ShowAll();

			spinbutton1.Sensitive = false;
			spinbutton1.Value = PriceCorrection;
			checkbuttonShowPrice.Active = false;

			CurrentDrag = new DragInformation();
			//Загрузка списка кубов
			CubeList = new List<Cube>();
			CubeWidgetList = new List<CubeListItem>();
			vboxCubeList = new VBox(false, 6);
			hboxCubeList = new HBox(false, 20);
			string sql = "SELECT * FROM cubes ORDER BY ordinal";

			SqliteCommand cmd = new SqliteCommand(sql, (SqliteConnection)QSMain.ConnectionDB);
			using (SqliteDataReader rdr = cmd.ExecuteReader())
			{
				while(rdr.Read())
				{
					if (rdr["image"] == DBNull.Value)
						continue;
					Cube TempCube = new Cube();
					TempCube.NomenclatureId = rdr.GetInt32(rdr.GetOrdinal("id"));
					TempCube.Name = DBWorks.GetString(rdr, "name", "");
					TempCube.Description = DBWorks.GetString(rdr, "description", "");
					TempCube.Height = DBWorks.GetInt(rdr, "height", 0) * 400;
					TempCube.Widht = DBWorks.GetInt(rdr, "width", 0) * 400;
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
			OnBasisChanged(null, EventArgs.Empty);

			//Настраиваем DND
			Gtk.Drag.DestSet(drawCupboard, DestDefaults.Motion, TargetTable, Gdk.DragAction.Move);
			Gtk.Drag.SourceSet(drawCupboard, ModifierType.Button1Mask, TargetTable, Gdk.DragAction.Move);
			Gtk.Drag.DestSet(vboxCubeList, DestDefaults.Motion, TargetTable, Gdk.DragAction.Move);
			Gtk.Drag.DestSet(hboxCubeList, DestDefaults.Motion, TargetTable, Gdk.DragAction.Move);
			vboxCubeList.DragDrop += OnCubeListDragDrop;
			hboxCubeList.DragDrop += OnCubeListDragDrop;
		}

		/// <summary>
		/// Saving order information to DB.
		/// </summary>
		private bool Save()
		{
			string sql;
			if (NewItem) {
				sql = "INSERT INTO orders (customer, estimation, contract, address, phone1, phone2, exhibition_id, basis_id, " +
					"arrval, deadline_s, deadline_e, comment, cupboard, total_price, price_correction, cutting_base) " +
					"VALUES (@customer, @estimation, @contract, @address, @phone1, @phone2, @exhibition_id, @basis_id, @arrval, " +
					"@deadline_s, @deadline_e, @comment, @cupboard, @total_price, @price_correction, @cutting_base)";
			}
			else {
				sql = "UPDATE orders SET customer = @customer, estimation = @estimation, contract = @contract, address = @address, " +
					"phone1 = @phone1, phone2 = @phone2, exhibition_id = @exhibition_id, basis_id = @basis_id, arrval = @arrval, " +
					"deadline_s = @deadline_s, deadline_e = @deadline_e, comment = @comment, cupboard = @cupboard, " +
					"total_price = @total_price, price_correction = @price_correction, cutting_base = @cutting_base WHERE id = @id";
			}
			SqliteTransaction trans = ((SqliteConnection)QSMain.ConnectionDB).BeginTransaction();
			MainClass.StatusMessage("Запись заказа...");
			try {
				int contract;

				SqliteCommand cmd = new SqliteCommand(sql, (SqliteConnection)QSMain.ConnectionDB, trans);
				cmd.Parameters.AddWithValue("@id", ItemId);
				cmd.Parameters.AddWithValue("@customer", DBWorks.ValueOrNull(entryCustomer.Text != "", entryCustomer.Text));
				cmd.Parameters.AddWithValue("@estimation", checkEstimation.Active);
				cmd.Parameters.AddWithValue("@contract", DBWorks.ValueOrNull(int.TryParse(entryContract.Text, out contract), contract));
				cmd.Parameters.AddWithValue("@address", DBWorks.ValueOrNull(textAddress.Buffer.Text != "", textAddress.Buffer.Text));
				cmd.Parameters.AddWithValue("@phone1", DBWorks.ValueOrNull(entryPhone1.Text != "", entryPhone1.Text));
				cmd.Parameters.AddWithValue("@phone2", DBWorks.ValueOrNull(entryPhone2.Text != "", entryPhone2.Text));
				cmd.Parameters.AddWithValue("@arrval", DBWorks.ValueOrNull(!dateArrval.IsEmpty, dateArrval.Date));
				cmd.Parameters.AddWithValue("@deadline_s", DBWorks.ValueOrNull(!dateDeadlineS.IsEmpty, dateDeadlineS.Date));
				cmd.Parameters.AddWithValue("@deadline_e", DBWorks.ValueOrNull(!dateDeadlineE.IsEmpty, dateDeadlineE.Date));
				CupboardListItem basis = TypeWidgetList.Find(w => w.Button.Active);
				cmd.Parameters.AddWithValue("basis_id", DBWorks.ValueOrNull(basis != null, basis.id));
				cmd.Parameters.AddWithValue("exhibition_id", ComboWorks.GetActiveIdOrNull(comboExhibition));
				cmd.Parameters.AddWithValue("@comment", DBWorks.ValueOrNull(textviewComments.Buffer.Text != "", textviewComments.Buffer.Text));
				cmd.Parameters.AddWithValue("@cupboard", OrderCupboard.SaveToString());
				cmd.Parameters.AddWithValue("@total_price", TotalPrice.ToString());
				cmd.Parameters.AddWithValue("@price_correction", PriceCorrection);
				cmd.Parameters.AddWithValue("@cutting_base", checkCuttingBase.Active);

				cmd.ExecuteNonQuery();

				if(NewItem) {
					sql = @"select last_insert_rowid()";
					cmd = new SqliteCommand(sql, (SqliteConnection)QSMain.ConnectionDB, trans);
					ItemId = Convert.ToInt32(cmd.ExecuteScalar());
					NewItem = false;
				}
				// Запись компонент
				TreeIter iter, childIter;
				if(ComponentsStore.GetIterFirst(out iter)) {
					do {	//If item is cube
						if ((Nomenclature.NomType)ComponentsStore.GetValue(iter, (int)ComponentCol.nomenclature_type) == Nomenclature.NomType.cube) {
							//Writing common cube info to order_details
							bool HasValue = (int) ComponentsStore.GetValue(iter, (int)ComponentCol.count) > 0;
							bool InDB = (long)ComponentsStore.GetValue(iter, (int)ComponentCol.row_id) > 0;
							if(HasValue) {
								if(!InDB)
									sql = "INSERT INTO order_details (order_id, cube_id, count, facing_id, facing, material_id, material, comment) " +
										"VALUES (@order_id, @nomenclature_id, @count, @facing_id, @facing, @material_id, @material, @comment)";
								else
									sql = "UPDATE order_details SET count = @count, facing_id = @facing_id, facing = @facing, " +
										"material_id = @material_id, material = @material, comment = @comment WHERE id = @id";
								cmd = new SqliteCommand(sql, (SqliteConnection)QSMain.ConnectionDB, trans);
								cmd.Parameters.AddWithValue("@id", (long)ComponentsStore.GetValue(iter, (int)ComponentCol.row_id));
								cmd.Parameters.AddWithValue("@order_id", ItemId);
								cmd.Parameters.AddWithValue("@nomenclature_id", ComponentsStore.GetValue(iter,(int)ComponentCol.nomenclature_id));
								cmd.Parameters.AddWithValue("@count", ComponentsStore.GetValue(iter,(int)ComponentCol.count));
								cmd.Parameters.AddWithValue("@material_id", DBWorks.ValueOrNull((int)ComponentsStore.GetValue(iter,(int)ComponentCol.material_id) > 0, ComponentsStore.GetValue(iter, (int)ComponentCol.material_id)));
								cmd.Parameters.AddWithValue("@facing_id", DBWorks.ValueOrNull((int)ComponentsStore.GetValue(iter, (int)ComponentCol.facing_id) > 0, ComponentsStore.GetValue(iter, (int)ComponentCol.facing_id)));
								cmd.Parameters.AddWithValue("@material", DBWorks.ValueOrNull((string)ComponentsStore.GetValue(iter,(int)ComponentCol.material) != "", ComponentsStore.GetValue(iter, (int)ComponentCol.material)));
								cmd.Parameters.AddWithValue("@facing", DBWorks.ValueOrNull((string)ComponentsStore.GetValue(iter, (int)ComponentCol.facing) != "", ComponentsStore.GetValue(iter, (int)ComponentCol.facing)));
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
								sql = "DELETE FROM order_details WHERE id = @id";
								cmd = new SqliteCommand(sql, (SqliteConnection)QSMain.ConnectionDB, trans);
								cmd.Parameters.AddWithValue("@id", ComponentsStore.GetValue(iter, (int)ComponentCol.row_id));
								cmd.ExecuteNonQuery();
							}
							if(ComponentsStore.IterHasChild(iter)) {
								ComponentsStore.IterChildren(out childIter, iter);
								do { //Adding every nomenclature for cube
									HasValue = (int) ComponentsStore.GetValue(childIter, (int)ComponentCol.count) > 0;
									InDB = (long)ComponentsStore.GetValue(childIter, (int)ComponentCol.row_id) > 0;
									if(HasValue) {
										if(!InDB)
											sql = "INSERT INTO order_cubes_details (order_id, cube_id, nomenclature_id, count, price, " +
												"comment) VALUES (@order_id, @cube_id, @nomenclature_id, @count, @price, @comment)";
										else
											sql = "UPDATE order_cubes_details SET count = @count, price = @price, comment = @comment WHERE id = @id";
										cmd = new SqliteCommand(sql, (SqliteConnection)QSMain.ConnectionDB, trans);
										cmd.Parameters.AddWithValue("@id", (long)ComponentsStore.GetValue(childIter, (int)ComponentCol.row_id));
										cmd.Parameters.AddWithValue("@order_id", ItemId);
										cmd.Parameters.AddWithValue("@cube_id", ComponentsStore.GetValue(iter, (int)ComponentCol.nomenclature_id));
										cmd.Parameters.AddWithValue("@nomenclature_id", ComponentsStore.GetValue(childIter,(int)ComponentCol.nomenclature_id));
										cmd.Parameters.AddWithValue("@count", ComponentsStore.GetValue(childIter,(int)ComponentCol.count));
										cmd.Parameters.AddWithValue("@price", ComponentsStore.GetValue(childIter, (int)ComponentCol.price));
										cmd.Parameters.AddWithValue("@comment", DBWorks.ValueOrNull((string)ComponentsStore.GetValue(childIter, (int)ComponentCol.comment) != "", ComponentsStore.GetValue(childIter, (int)ComponentCol.comment)));
										cmd.ExecuteNonQuery();
										if(!InDB)
										{
											sql = @"select last_insert_rowid()";
											cmd = new SqliteCommand(sql, (SqliteConnection)QSMain.ConnectionDB, trans);
											long RowId = (long) cmd.ExecuteScalar();
											ComponentsStore.SetValue(childIter, (int)ComponentCol.row_id, (object)RowId);
										}
									}
									else if(InDB)
									{
										sql = "DELETE FROM order_cubes_details WHERE id = @id";
										cmd = new SqliteCommand(sql, (SqliteConnection)QSMain.ConnectionDB, trans);
										cmd.Parameters.AddWithValue("@id", ComponentsStore.GetValue(childIter, (int)ComponentCol.row_id));
										cmd.ExecuteNonQuery();
									}
								} while (ComponentsStore.IterNext(ref childIter));
							}
						}
						else //Item is basis
						{
							if(ComponentsStore.IterHasChild(iter)) {
								ComponentsStore.IterChildren(out childIter, iter);
								do { //Adding every nomenclature for basis

									bool HasValue = (int) ComponentsStore.GetValue(childIter, (int)ComponentCol.count) > 0;
									bool InDB = (long)ComponentsStore.GetValue(childIter, (int)ComponentCol.row_id) > 0;
									if(HasValue) {
										if(!InDB)
											sql = "INSERT INTO order_basis_details (order_id, basis_id, nomenclature_id, count, price, comment) " +
												"VALUES (@order_id, @basis_id, @nomenclature_id, @count, @price, @comment)";
										else
											sql = "UPDATE order_basis_details SET count = @count, price = @price, comment = @comment WHERE id = @id";
										cmd = new SqliteCommand(sql, (SqliteConnection)QSMain.ConnectionDB, trans);
										cmd.Parameters.AddWithValue("@id", (long)ComponentsStore.GetValue(childIter, (int)ComponentCol.row_id));
										cmd.Parameters.AddWithValue("@order_id", ItemId);
										cmd.Parameters.AddWithValue("@basis_id", DBWorks.ValueOrNull(basis != null, basis.id));
										cmd.Parameters.AddWithValue("@nomenclature_id", ComponentsStore.GetValue(childIter,(int)ComponentCol.nomenclature_id));
										cmd.Parameters.AddWithValue("@count", ComponentsStore.GetValue(childIter,(int)ComponentCol.count));
										cmd.Parameters.AddWithValue("@price", ComponentsStore.GetValue(childIter, (int)ComponentCol.price));
										cmd.Parameters.AddWithValue("@comment", DBWorks.ValueOrNull((string)ComponentsStore.GetValue(childIter, (int)ComponentCol.comment) != "", ComponentsStore.GetValue(childIter, (int)ComponentCol.comment)));
										cmd.ExecuteNonQuery();
										if(!InDB)
										{
											sql = @"select last_insert_rowid()";
											cmd = new SqliteCommand(sql, (SqliteConnection)QSMain.ConnectionDB, trans);
											long RowId = (long) cmd.ExecuteScalar();
											ComponentsStore.SetValue(childIter, (int)ComponentCol.row_id, (object)RowId);
										}
									}
									else if(InDB)
									{
										sql = "DELETE FROM order_basis_details WHERE id = @id";
										cmd = new SqliteCommand(sql, (SqliteConnection)QSMain.ConnectionDB, trans);
										cmd.Parameters.AddWithValue("@id", ComponentsStore.GetValue(childIter, (int)ComponentCol.row_id));
										cmd.ExecuteNonQuery();
									}
								} while (ComponentsStore.IterNext(ref childIter));
							}
						}
					} while (ComponentsStore.IterNext(ref iter));
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
			
		/// <summary>
		/// Method for loading existing orders.
		/// </summary>
		/// <param name="id">Identifier - order id in database</param>
		/// <param name="copy">If set to <c>true</c> copy.</param>
		public void Fill(int id, bool copy)
		{
			FillInProgress = true;
			NewItem = copy;
			if(!copy)
				ItemId = id;

			MainClass.StatusMessage(String.Format ("Запрос заказа №{0}...", id));
			string sql = "SELECT orders.* FROM orders WHERE orders.id = @id";
			try
			{
				SqliteCommand cmd = new SqliteCommand(sql, (SqliteConnection) QSMain.ConnectionDB);
				cmd.Parameters.AddWithValue("@id", id);

				CupboardListItem basis;

				using(SqliteDataReader rdr = cmd.ExecuteReader()) {
					rdr.Read();
					if(!copy) {
						this.Title = String.Format("Заказ №{0}", rdr["id"].ToString());
						dateArrval.Date = DBWorks.GetDateTime(rdr, "arrval", new DateTime());
					}
					checkEstimation.Active = DBWorks.GetBoolean(rdr, "estimation", true);
					entryContract.Text = rdr["contract"].ToString();
					entryCustomer.Text = rdr["customer"].ToString();
					entryPhone1.Text = rdr["phone1"].ToString();
					entryPhone2.Text = rdr["phone2"].ToString();
					textAddress.Buffer.Text = rdr["address"].ToString();
					dateDeadlineS.Date = DBWorks.GetDateTime(rdr, "deadline_s", new DateTime());
					dateDeadlineE.Date = DBWorks.GetDateTime(rdr, "deadline_e", new DateTime());
					int basis_id = rdr.GetInt32(rdr.GetOrdinal("basis_id"));
					basis = TypeWidgetList.Find(w => w.id == basis_id);
					ComboWorks.SetActiveItem(comboExhibition, DBWorks.GetInt(rdr, "exhibition_id", -1));
					textviewComments.Buffer.Text = rdr["comment"].ToString();
					OrderCupboard = Cupboard.Load(rdr["cupboard"].ToString(), CubeList);
					comboCubeH.Active = OrderCupboard.CubesH - 1;
					comboCubeV.Active = OrderCupboard.CubesV - 1;
					checkCuttingBase.Active = DBWorks.GetBoolean(rdr, "cutting_base", false);
					CalculateCubePxSize(drawCupboard.Allocation);
					PriceCorrection = DBWorks.GetInt(rdr, "price_correction", 0);
					TotalPrice = DBWorks.GetDecimal(rdr, "total_price", 0);
					spinbutton1.Value = PriceCorrection;
					if (PriceCorrection != 0)
						checkbuttonDiscount.Active = true;
					labelTotalCount.LabelProp = String.Format("Итого {0} руб.", TotalPrice);
					ComponentsStore.Remove(ref BasisIter);
					BasisIter = ComponentsStore.AppendValues (
						(long)-1, 
						Enum.Parse(typeof(Nomenclature.NomType), "construct"), 
						basis.id, 
						null, 
						"Каркас", 
						null, 
						1, 
						-1,
						"",
						-1,
						"",
						"",
						"", 
						"",
						false,
						false,
						false,
						false,
						false
					);
				}
				//Loading basis and it's contents.

				sql = "SELECT order_basis_details.*, nomenclature.type, nomenclature.name, nomenclature.description" +
					" FROM order_basis_details " +
					"LEFT JOIN nomenclature ON order_basis_details.nomenclature_id = nomenclature.id " +
					"WHERE order_basis_details.order_id = @order_id and order_basis_details.basis_id = @basis_id";
				cmd = new SqliteCommand(sql, (SqliteConnection)QSMain.ConnectionDB);
				cmd.Parameters.AddWithValue("@order_id", id);
				cmd.Parameters.AddWithValue("@basis_id", basis.id);
				using (SqliteDataReader rdr = cmd.ExecuteReader()) {
					while(rdr.Read()) {
						int count = DBWorks.GetInt(rdr, "count", -1);
						decimal price = DBWorks.GetDecimal(rdr, "price", -1);
						ComponentsStore.AppendValues(
							BasisIter,
							DBWorks.GetLong(rdr, "id", -1),
							Enum.Parse(typeof(Nomenclature.NomType), rdr["type"].ToString()),
							DBWorks.GetInt(rdr, "nomenclature_id", -1),
							DBWorks.GetString(rdr, "name", ""), 
							ReplaceArticle(DBWorks.GetString(rdr, "name", "")),
							DBWorks.GetString(rdr, "description", ""),
							count,
							-1,
							"",
							-1,
							"",
							DBWorks.GetString(rdr, "comment", ""),
							price.ToString(),
							(price * count).ToString(),
							true,
							true,
							false,
							false,
							true
						);
					}
				}
				//Loading cubes.
				sql = "SELECT order_details.*, cubes.name FROM order_details " +
					"LEFT JOIN cubes ON order_details.cube_id = cubes.id WHERE order_id = @order_id";
				cmd = new SqliteCommand(sql, (SqliteConnection)QSMain.ConnectionDB);
				cmd.Parameters.AddWithValue("@order_id", id);
				using (SqliteDataReader rdr = cmd.ExecuteReader()) {
					while (rdr.Read()) {
						TreeIter CubeIter = ComponentsStore.AppendValues (
							DBWorks.GetLong(rdr, "id", -1), 
							Enum.Parse (typeof(Nomenclature.NomType), "cube"), 
							DBWorks.GetInt(rdr, "cube_id", -1),
							null, 
							DBWorks.GetString(rdr, "name", ""), 
							null, 
							DBWorks.GetInt(rdr, "count", 1),
							DBWorks.GetInt(rdr, "material_id", -1),
							DBWorks.GetString(rdr, "material", ""),
							DBWorks.GetInt(rdr, "facing_id", -1),
							DBWorks.GetString(rdr, "facing", ""),
							DBWorks.GetString(rdr, "comment", ""),
							"",
							"", 
							false,
							false,
							true,
							true,
							true
						);
						string contents_sql = "SELECT order_cubes_details.*, nomenclature.type AS type, nomenclature.name AS name, " +
							"nomenclature.description AS description FROM order_cubes_details LEFT JOIN nomenclature ON " +
							"order_cubes_details.nomenclature_id = nomenclature.id WHERE order_cubes_details.order_id = @order_id " +
							"AND order_cubes_details.cube_id = @cube_id";
						SqliteCommand contents_cmd = new SqliteCommand (contents_sql, (SqliteConnection)QSMain.ConnectionDB);
						contents_cmd.Parameters.AddWithValue ("@cube_id", DBWorks.GetInt(rdr, "cube_id", -1));
						contents_cmd.Parameters.AddWithValue ("@order_id", id);
						decimal Price = 0;
						using (SqliteDataReader contents_rdr = contents_cmd.ExecuteReader ()) {
							while (contents_rdr.Read ()) {
								Decimal NomenclaturePrice = DBWorks.GetDecimal (contents_rdr, "price", 0) * DBWorks.GetDecimal (contents_rdr, "count", 1);
								Price += NomenclaturePrice;
								ComponentsStore.AppendValues (
									CubeIter,
									DBWorks.GetLong(contents_rdr, "id", -1),
									Enum.Parse (typeof(Nomenclature.NomType), contents_rdr ["type"].ToString ()),
									DBWorks.GetInt (contents_rdr, "nomenclature_id", -1),
									DBWorks.GetString (contents_rdr, "name", "нет"),
									ReplaceArticle (DBWorks.GetString (contents_rdr, "name", "нет")),
									DBWorks.GetString (contents_rdr, "description", ""),
									DBWorks.GetInt (contents_rdr, "count", 1),
									-1,
									"",
									-1,
									"",
									DBWorks.GetString(contents_rdr, "comment", ""),
									(DBWorks.GetDecimal(contents_rdr, "price", 0)).ToString(),
									NomenclaturePrice.ToString(),
									true,
									true,
									false,
									false,
									true
								);
							}
							ComponentsStore.SetValue (CubeIter, (int)ComponentCol.price_total, Price.ToString ());
						}
					}
				}
				CalculateTotalCount();
				basis.Button.Click();
				FillInProgress = false;
				MainClass.StatusMessage("Ok");
			}
			catch (Exception ex)
			{
				logger.ErrorException("Ошибка получения информации о заказе!", ex);
				QSMain.ErrorMessage(this, ex);
			}
			TestCanSave();
		}

		void OnCountEdited(object o, EditedArgs args)
		{
			TreeIter iter;
			int NewValue;
			Decimal Price;

			if (!ComponentsStore.GetIterFromString (out iter, args.Path))
				return;
			try { 
				if (args.NewText == null)
					NewValue = 1;
				else
					NewValue = int.Parse (args.NewText); 
				Price = Decimal.Parse((String)ComponentsStore.GetValue(iter, (int)ComponentCol.price));
				ComponentsStore.SetValue(iter, (int)ComponentCol.count, NewValue);
				ComponentsStore.SetValue(iter, (int)ComponentCol.price_total, (Price * NewValue).ToString());
			} catch(Exception e) { logger.WarnException ("Error occured in OnCountEdited.", e);}
		}

		void OnPriceEdited(object o, EditedArgs args) 
		{
			TreeIter iter;
			Decimal NewValue;
			if (!ComponentsStore.GetIterFromString (out iter, args.Path))
				return;
			try {
				if (args.NewText == null)
					NewValue = 0;
				else
					NewValue = Decimal.Parse (args.NewText);
				int count = (int)ComponentsStore.GetValue (iter, (int)ComponentCol.count);
				ComponentsStore.SetValue(iter, (int)ComponentCol.price_total, (count * NewValue).ToString());
				ComponentsStore.SetValue(iter, (int)ComponentCol.price, (NewValue).ToString());
				CalculateTotalCount ();
				return;
			} catch(Exception e) { logger.WarnException ("Error occured in OnPriceEdited", e);}
		}

		void OnMaterialComboEdited (object o, EditedArgs args)
		{
			TreeIter iter, RefIter;
			if (!ComponentsStore.GetIterFromString (out iter, args.Path))
				return;
			if(args.NewText == null) {
				logger.Warn("newtext is empty");
				return;
			}
			if(ListStoreWorks.SearchListStore((ListStore)MaterialNameList, args.NewText, out RefIter)) {
				ComponentsStore.SetValue(iter, (int)ComponentCol.material, args.NewText);
				ComponentsStore.SetValue(iter, (int)ComponentCol.material_id, MaterialNameList.GetValue(RefIter, 1));
			}
		}

		void OnFacingComboEdited (object o, EditedArgs args)
		{
			TreeIter iter, RefIter;
			if (!ComponentsStore.GetIterFromString (out iter, args.Path))
				return;
			if(args.NewText == null) {
				logger.Warn("newtext is empty");
				return;
			}
			if(ListStoreWorks.SearchListStore((ListStore)FacingNameList, args.NewText, out RefIter)) {
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
			bool Dateok = dateArrval.IsEmpty || dateDeadlineS.IsEmpty || dateArrval.Date <= dateDeadlineS.Date;

			saveAction.Sensitive = Dateok;
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
			UpdateArticles();
		}

		protected void OnComboCubeVChanged(object sender, EventArgs e)
		{
			OrderCupboard.CubesV = int.Parse(comboCubeV.ActiveText);
			if(OrderCupboard.BorderImage != null)
				OrderCupboard.BorderImage.ModifyDrawingImage();
			CalculateCubePxSize(drawCupboard.Allocation);
			UpdateArticles();
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
			int ListPixelAddV = !VerticalCubeList ? scrolledCubeListH.HScrollbar.Allocation.Height + 64 : 0;
			int ListPixelAddH = VerticalCubeList ? scrolledCubeListV.VScrollbar.Allocation.Width + 16 : 0;
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
			if (notebook1.CurrentPage == 2)
			{
				if (OrderCupboard.Clean())
					UpdateCubeComponents();
			}
			if (notebook1.CurrentPage == 4)
			{
				PrerareReport();
			}
		
			goBackAction.Sensitive = notebook1.CurrentPage != 0;
			goForwardAction.Sensitive = notebook1.CurrentPage != 4;
		}

		/// <summary>
		/// Updates the basis components.
		/// </summary>
		/// <param name="id">Identifier of basis.</param>
		private void UpdateBasisComponents(int id)
		{
			if (FillInProgress)
				return;
			Dictionary<int, TreeIter> pairs= new Dictionary<int, TreeIter> ();
			TreeIter iter;
			//Making all components inside basis NULL.
			if (ComponentsStore.IterHasChild (BasisIter)) {
				ComponentsStore.IterChildren (out iter, BasisIter);
				do {
					ComponentsStore.SetValue (iter, (int)ComponentCol.count, 0);
					ComponentsStore.SetValue (iter, (int)ComponentCol.price_total, "0");
					pairs.Add((int)ComponentsStore.GetValue(iter, (int)ComponentCol.nomenclature_id), iter);
				} while (ComponentsStore.IterNext (ref iter));
			}

			string sql = "SELECT nomenclature.name as nomenclature, nomenclature.type, nomenclature.description, nomenclature.price, basis_items.* " +
				"FROM basis_items LEFT JOIN nomenclature ON nomenclature.id = basis_items.item_id WHERE basis_id = @basis_id";
			SqliteCommand cmd = new SqliteCommand(sql, (SqliteConnection)QSMain.ConnectionDB);
			cmd.Parameters.AddWithValue("@basis_id", id);
			using (SqliteDataReader rdr = cmd.ExecuteReader())
			{
				while (rdr.Read())
				{
					if (pairs.TryGetValue (DBWorks.GetInt (rdr, "item_id", -1), out iter)) {
						int count = DBWorks.GetInt (rdr, "count", 1);
						Decimal price = count * Decimal.Parse(ComponentsStore.GetValue (iter, (int)ComponentCol.price).ToString());
						ComponentsStore.SetValue (iter, (int)ComponentCol.count, count);
						ComponentsStore.SetValue (iter, (int)ComponentCol.price_total, price.ToString());
					} else {
						ComponentsStore.AppendValues (
							BasisIter,
							(long)-1,
							Enum.Parse (typeof(Nomenclature.NomType), rdr ["type"].ToString ()),
							DBWorks.GetInt (rdr, "item_id", -1),
							DBWorks.GetString (rdr, "nomenclature", "нет"),
							ReplaceArticle (DBWorks.GetString (rdr, "nomenclature", "нет")),
							DBWorks.GetString (rdr, "description", ""),
							DBWorks.GetInt (rdr, "count", 1),
							-1,
							"",
							-1,
							"",
							"",
							DBWorks.GetDecimal (rdr, "price", 0).ToString (),
							(DBWorks.GetDecimal (rdr, "price", 0) * DBWorks.GetInt (rdr, "count", 0)).ToString (),
							true,
							true,
							false,
							false,
							true
						);
					}
				}
			}
			CalculateTotalCount();
		}
			
		/// <summary>
		/// Updates the cube components. Adding one or removing if needed.
		/// </summary>
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
							UpdateTable(iter);
						}
						else if((long)ComponentsStore.GetValue(iter, (int)ComponentCol.row_id) >= 0)
							ComponentsStore.SetValue(iter, (int)ComponentCol.count, 0);
						else
							ComponentsStore.Remove(ref iter);
					}
				}
				while(ComponentsStore.IterNext(ref iter));
			}

			foreach (KeyValuePair<int, int> pair in Counts) {
				Cube cube = OrderCupboard.Cubes.Find (c => c.NomenclatureId == pair.Key);
				TreeIter CubeIter = ComponentsStore.AppendValues ((long)-1, Enum.Parse (typeof(Nomenclature.NomType), "cube"), pair.Key, null, cube.Name, null, pair.Value, -1, "", -1, "", "", "", "",false, false, true, true, true);
				string sql = "SELECT nomenclature.name as nomenclature, nomenclature.type, nomenclature.description, nomenclature.price, cubes_items.* FROM cubes_items " +
					"LEFT JOIN nomenclature ON nomenclature.id = cubes_items.item_id " +
					"WHERE cubes_id = @cubes_id";
				SqliteCommand cmd = new SqliteCommand (sql, (SqliteConnection)QSMain.ConnectionDB);
				cmd.Parameters.AddWithValue ("@cubes_id", pair.Key);
				Decimal Price = 0;
				using (SqliteDataReader rdr = cmd.ExecuteReader ()) {
					while (rdr.Read ()) {
						Price += (DBWorks.GetDecimal (rdr, "price", 0) * pair.Value);
						ComponentsStore.AppendValues (
							CubeIter,
							(long)-1,
							Enum.Parse (typeof(Nomenclature.NomType), rdr ["type"].ToString ()),
							DBWorks.GetInt (rdr, "item_id", -1),
							DBWorks.GetString (rdr, "nomenclature", "нет"),
							ReplaceArticle (DBWorks.GetString (rdr, "nomenclature", "нет")),
							DBWorks.GetString (rdr, "description", ""),
							DBWorks.GetInt (rdr, "count", 1) * pair.Value,
							-1,
							"",
							-1,
							"",
							"",
							(DBWorks.GetDecimal(rdr, "price", 0)).ToString(),
							(DBWorks.GetDecimal(rdr, "price", 0) * DBWorks.GetInt (rdr, "count", 1) * pair.Value).ToString(),
							true,
							true,
							false,
							false,
							true
						);
					}
					ComponentsStore.SetValue (CubeIter, (int)ComponentCol.price_total, Price.ToString ());
				}
			}
			CalculateTotalCount();
		}

//TODO
		private void UpdateArticles()
		{
			/*TreeIter iter;
			if (ComponentsStore.GetIterFirst(out iter))
			{
				do
				{
					ComponentsStore.SetValue(iter, (int)ComponentCol.nomenclature_title, 
						ReplaceArticle((string)ComponentsStore.GetValue(iter, (int)ComponentCol.nomenclature)));
				}
				while(ComponentsStore.IterNext(ref iter));
			}*/
		}

		private string ReplaceArticle(string text)
		{
			string half = text.Replace("{L}", String.Format("{0}", OrderCupboard.CubesH * 40));
			return half.Replace("{H}", String.Format("{0}", OrderCupboard.CubesV * 40));
		}

		/// <summary>
		/// Updates the count and total price of cube component.
		/// </summary>
		/// <param name="iter">Iterator for parent, containing cube components.</param>
		void UpdateTable(TreeIter iter) {
			TreeIter childIter;
			int parentCount, childNomenclatureId, parentId;
			string SQL;

			if (ComponentsStore.IterHasChild (iter)) {
				parentId = (int)ComponentsStore.GetValue(iter, (int)ComponentCol.nomenclature_id);
				parentCount = (int)ComponentsStore.GetValue(iter, (int)ComponentCol.count);
				ComponentsStore.IterChildren (out childIter, iter);
				do {
					childNomenclatureId = (int)ComponentsStore.GetValue(childIter, (int)ComponentCol.nomenclature_id);
					SQL = "SELECT count FROM cubes_items WHERE item_id = @item_id and cubes_id = @cubes_id";
					SqliteCommand cmd = new SqliteCommand (SQL, (SqliteConnection)QSMain.ConnectionDB);
					cmd.Parameters.AddWithValue ("@cubes_id", parentId.ToString());
					cmd.Parameters.AddWithValue ("@item_id", childNomenclatureId.ToString());
					using (SqliteDataReader rdr = cmd.ExecuteReader()) {
						rdr.Read();
						int newCount = DBWorks.GetInt(rdr, "count", 1) * parentCount;
						ComponentsStore.SetValue(childIter, (int)ComponentCol.count, newCount);
						ComponentsStore.SetValue(childIter, (int)ComponentCol.price_total, (newCount * Decimal.Parse((String)ComponentsStore.GetValue(childIter, (int)ComponentCol.price))).ToString());
					}
				} while(ComponentsStore.IterNext(ref childIter));
			}
		}

		/// <summary>
		/// Calculates/corrects the total price for items on order change and for whole order too.
		/// </summary>
		void CalculateTotalCount()
		{
			Decimal TempTotal;
			TreeIter iter, childIter;

			if (ComponentsStore.GetIterFirst(out iter))
			{
				TotalPrice = 0;
				do {
					if (ComponentsStore.IterHasChild(iter))
					{
						TempTotal = 0;
						ComponentsStore.IterChildren(out childIter, iter);
						do {
							TempTotal += Decimal.Parse((String)ComponentsStore.GetValue(childIter, (int)ComponentCol.price_total));
						} while (ComponentsStore.IterNext(ref childIter));
						TotalPrice += TempTotal;
						ComponentsStore.SetValue(iter, (int)ComponentCol.price_total, TempTotal.ToString());
					}
				} while(ComponentsStore.IterNext (ref iter));
			}
			labelTotalCount.LabelProp = String.Format("Итого: {0:C} ", Decimal.Round(TotalPrice + (TotalPrice / 100 * PriceCorrection), 0));
		}

		private void PrerareReport()
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

			string ReportPath = System.IO.Path.Combine( Directory.GetCurrentDirectory(), "Reports", "order" + ".rdl");
			reportviewer1.LoadReport(new Uri(ReportPath), param, QSMain.ConnectionString);
		}

		protected void OnOrderDatesChanged(object sender, EventArgs e)
		{
			TestCanSave();
			bool Dateok = dateArrval.IsEmpty || dateDeadlineS.IsEmpty || dateArrval.Date <= dateDeadlineS.Date;
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

		protected void OnCheckEstimationClicked(object sender, EventArgs e)
		{
			entryContract.Sensitive = !checkEstimation.Active;
		}

		protected void OnEntryContractChanged(object sender, EventArgs e)
		{
			int number;
			if(int.TryParse(entryContract.Text, out number))
				entryContract.ModifyText(StateType.Normal);
			else
				entryContract.ModifyText(StateType.Normal, new Gdk.Color(255,0,0)); 
		}

		protected void OnDateDeadlineSDateChanged(object sender, EventArgs e)
		{
			CultureInfo currentCulture = CultureInfo.CurrentCulture;
			int WeekS = currentCulture.Calendar.GetWeekOfYear(dateDeadlineS.Date, currentCulture.DateTimeFormat.CalendarWeekRule,
				           currentCulture.DateTimeFormat.FirstDayOfWeek);
			int WeekE = currentCulture.Calendar.GetWeekOfYear(dateDeadlineE.Date, currentCulture.DateTimeFormat.CalendarWeekRule,
				currentCulture.DateTimeFormat.FirstDayOfWeek);
			if (WeekE != WeekS)
				dateDeadlineE.Date = dateDeadlineS.Date.AddDays((double)(5 - currentCulture.Calendar.GetDayOfWeek(dateDeadlineS.Date)));
			OnOrderDatesChanged(sender, e);
		}

		protected void OnZoomInActionActivated (object sender, EventArgs e)
		{
			throw new NotImplementedException ();
		}

		protected void OnZoomOutActionActivated (object sender, EventArgs e)
		{
			throw new NotImplementedException ();
		}

		protected void OnPdfActionActivated (object sender, EventArgs e)
		{
			throw new NotImplementedException ();
		}

		protected void OnRefreshActionActivated (object sender, EventArgs e)
		{
			throw new NotImplementedException ();
		}

		protected void OnCheckbutton2Toggled (object sender, EventArgs e)
		{
			ColumnPrice.Visible = ColumnPriceTotal.Visible = checkbuttonShowPrice.Active;
		}

		protected void OnCheckbutton1Toggled (object sender, EventArgs e)
		{
			spinbutton1.Sensitive = checkbuttonDiscount.Active;
			if (spinbutton1.Sensitive == false)
				PriceCorrection = 0;
			else 
				PriceCorrection = spinbutton1.ValueAsInt;
			CalculateTotalCount ();
		}

		protected void OnSpinbutton1ValueChanged (object sender, EventArgs e) 
		{
			PriceCorrection = spinbutton1.ValueAsInt;
			CalculateTotalCount ();
			if (spinbutton1.ValueAsInt > 0)
				checkbuttonDiscount.Label = "Наценка:";
			else
				checkbuttonDiscount.Label = "Скидка:";
		}
	}
}

