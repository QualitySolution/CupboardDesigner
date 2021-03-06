﻿using System;
using System.IO;
using NLog;
using Gtk;
using Mono.Data.Sqlite;
using QSProjectsLib;
using System.Collections.Generic;
using Cairo;
using Gdk;

namespace CupboardDesigner {
	public partial class Order : Gtk.Window {
		private static Logger logger = LogManager.GetCurrentClassLogger();
		private int ItemId, PriceCorrection = 0, CubePxSize = 100, MaxCubeVSize, MaxCubeHSize;
		private bool FillInProgress = false, VerticalCubeList = true, NewItem = true;
		private VBox vboxCubeList;
		private HBox hboxCubeList, hboxTypeList;
		private List<Cube> CubeList;
		private List<CupboardListItem> TypeWidgetList;
		private List<CubeListItem> CubeWidgetList;
		private Cupboard OrderCupboard;
		private DragInformation CurrentDrag;
		private Decimal TotalPrice;
		private TreeStore ComponentsStore;
		private TreeModel MaterialNameList, FacingNameList;
		private TreeIter BasisIter, ServiceIter;
		private Gtk.TreeViewColumn ColumnPrice, ColumnCount, ColumnMaterial, ColumnFacing, ColumnPriceTotal, ColumnComment, ColumnDiscount, ColumnName;

		Gtk.TargetEntry[] TargetTable = new Gtk.TargetEntry[] {
			new Gtk.TargetEntry ("application/cube", Gtk.TargetFlags.App, 0)
		};

		private enum ComponentCol {
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
			editable_comment,
			editable_discount,
			discount,
			editable_name
		}

		public Order() : base(Gtk.WindowType.Toplevel) {
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

			ComponentsStore = new TreeStore(
				typeof(long), //row_id
				typeof(Nomenclature.NomType), //nomenclature_type
				typeof(int), //nomenclature_id
				typeof(string), //nomenclature
				typeof(string), //nomenclature_title
				typeof(string), //nomenclature_description
				typeof(int), //count
				typeof(int), //material_id
				typeof(string), //material
				typeof(int), //facing_id
				typeof(string), //facing
				typeof(string), //comment
				typeof(string), //price
				typeof(string), //price_total
				typeof(bool), //editable_count
				typeof(bool), //editable_price
				typeof(bool), //editable_material
				typeof(bool), //editable_facing
				typeof(bool), //editable_comment
				typeof(bool), //editable_discount
				typeof(int), //discount
				typeof(bool)); //editable_name

			BasisIter = ComponentsStore.AppendValues (
				(long)-1, 
				Enum.Parse(typeof(Nomenclature.NomType), "construct"), 
				1, 
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
				false, 
				false, 
				null, 
				false);

			ServiceIter = ComponentsStore.InsertNodeAfter (BasisIter);

			ComponentsStore.SetValues (
				ServiceIter, 
				(long)-1, 
				Enum.Parse (typeof(Nomenclature.NomType), "other"), 
				1, 
				null, 
				"Услуги",
				"Кликните правой кнопкой мышы для добавления услуги", 
				0, 
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
				false, 
				false, 
				null, 
				false);

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

			ColumnDiscount = new Gtk.TreeViewColumn ();
			ColumnDiscount.Title = "Наценка";
			Gtk.CellRendererSpin CellDiscount = new Gtk.CellRendererSpin ();
			CellDiscount.Visible = false;
			CellDiscount.Edited += OnDiscountEdited;
			CellDiscount.Adjustment = new Adjustment (0, -100, 100, 1, 10, 0);
			ColumnDiscount.PackStart (CellDiscount, true);
			ColumnDiscount.AddAttribute (CellDiscount, "text", (int)ComponentCol.discount);
			ColumnDiscount.AddAttribute (CellDiscount, "visible", (int)ComponentCol.editable_discount);
			ColumnDiscount.AddAttribute (CellDiscount, "editable", (int)ComponentCol.editable_discount);

			ColumnName = new Gtk.TreeViewColumn ();
			ColumnName.Title = "Название";
			Gtk.CellRendererText CellName = new CellRendererText ();
			CellName.Edited += OnCellNameEdited;
			ColumnName.PackStart (CellName, true);
			ColumnName.AddAttribute (CellName, "editable", (int)ComponentCol.editable_name);
			ColumnName.AddAttribute (CellName, "text", (int)ComponentCol.nomenclature_title);

			treeviewComponents.AppendColumn(ColumnName);
			treeviewComponents.AppendColumn(ColumnCount);
			treeviewComponents.AppendColumn(ColumnPrice);
			treeviewComponents.AppendColumn(ColumnDiscount); 
			treeviewComponents.AppendColumn(ColumnPriceTotal);
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
			using (SqliteDataReader rdr = cmd.ExecuteReader()) {
				while(rdr.Read()) {
					if (rdr["image"] == DBNull.Value)
						continue;
					Cube TempCube = new Cube();
					TempCube.NomenclatureId = rdr.GetInt32(rdr.GetOrdinal("id"));
					TempCube.Name = DBWorks.GetString(rdr, "name", "");
					TempCube.Description = DBWorks.GetString(rdr, "description", "");
					TempCube.Height = DBWorks.GetInt(rdr, "height", 0) * 400;
					TempCube.Widht = DBWorks.GetInt(rdr, "width", 0) * 400;
					byte[] ImageFile = (byte[])rdr[rdr.GetOrdinal("image")];
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
			using(MemoryStream mstream = new MemoryStream()) {
				stream.CopyTo(mstream);
				temparray = mstream.ToArray();
			}
			Rsvg.Handle CheckImage = new Rsvg.Handle(temparray);
			sql = "SELECT * FROM basis ORDER BY ordinal ";
			cmd = new SqliteCommand(sql, (SqliteConnection)QSMain.ConnectionDB);
			using (SqliteDataReader rdr = cmd.ExecuteReader()) {
				Gtk.RadioButton FirstButton = null;
				while(rdr.Read()) {
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
		private bool Save() {
			string sql;
			if (NewItem) {
				sql = "INSERT INTO orders (customer, estimation, contract, address, phone1, phone2, exhibition_id, basis_id, " +
					"arrval, deadline_s, deadline_e, comment, cupboard, total_price, price_correction, cutting_base, basis_price) " +
					"VALUES (@customer, @estimation, @contract, @address, @phone1, @phone2, @exhibition_id, @basis_id, @arrval, " +
					"@deadline_s, @deadline_e, @comment, @cupboard, @total_price, @price_correction, @cutting_base, @basis_price)";
			}
			else {
				sql = "UPDATE orders SET customer = @customer, estimation = @estimation, contract = @contract, address = @address, " +
					"phone1 = @phone1, phone2 = @phone2, exhibition_id = @exhibition_id, basis_id = @basis_id, arrval = @arrval, " +
					"deadline_s = @deadline_s, deadline_e = @deadline_e, comment = @comment, cupboard = @cupboard, " +
					"total_price = @total_price, price_correction = @price_correction, cutting_base = @cutting_base, basis_price = @basis_price WHERE id = @id";
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
				cmd.Parameters.AddWithValue("basis_price", ComponentsStore.GetValue(BasisIter, (int)ComponentCol.price_total));
				cmd.Parameters.AddWithValue("exhibition_id", ComboWorks.GetActiveIdOrNull(comboExhibition));
				cmd.Parameters.AddWithValue("@comment", DBWorks.ValueOrNull(textviewComments.Buffer.Text != "", textviewComments.Buffer.Text));
				cmd.Parameters.AddWithValue("@cupboard", OrderCupboard.SaveToString());
				cmd.Parameters.AddWithValue("@total_price", TotalPrice);
				cmd.Parameters.AddWithValue("@price_correction", PriceCorrection);
				cmd.Parameters.AddWithValue("@cutting_base", checkCuttingBase.Active);

				cmd.ExecuteNonQuery();

				if(NewItem) {
					sql = @"select last_insert_rowid()";
					cmd = new SqliteCommand(sql, (SqliteConnection)QSMain.ConnectionDB, trans);
					ItemId = Convert.ToInt32(cmd.ExecuteScalar());
					NewItem = false;
				}
				// Saving components
				// Saving services
				TreeIter iter, childIter;
				if(ComponentsStore.IterHasChild(ServiceIter)) {
					ComponentsStore.IterChildren(out childIter, ServiceIter);
					do {
						bool InDB = (long)ComponentsStore.GetValue(childIter, (int)ComponentCol.row_id) > 0;
						if(!InDB)
							sql = "INSERT INTO order_services (order_id, name, price, discount, comment) " +
								"VALUES (@order_id, @name, @price, @discount, @comment)";
						else
							sql = "UPDATE order_services " +
								"SET name = @name, price = @price, discount = @discount, comment = @comment " +
								"WHERE id = @id";
						cmd = new SqliteCommand(sql, (SqliteConnection)QSMain.ConnectionDB, trans);
						cmd.Parameters.AddWithValue("@id", (long)ComponentsStore.GetValue(childIter, (int)ComponentCol.row_id));
						cmd.Parameters.AddWithValue("@order_id", ItemId);
						cmd.Parameters.AddWithValue("@name", (string)ComponentsStore.GetValue(childIter,(int)ComponentCol.nomenclature_title));
						cmd.Parameters.AddWithValue("@price", ComponentsStore.GetValue(childIter,(int)ComponentCol.price));
						cmd.Parameters.AddWithValue("@discount", (int)ComponentsStore.GetValue(childIter, (int)ComponentCol.discount));
						cmd.Parameters.AddWithValue("@comment", (string)ComponentsStore.GetValue(childIter, (int)ComponentCol.comment));
						cmd.ExecuteNonQuery();
						if(!InDB) {
							sql = @"select last_insert_rowid()";
							cmd = new SqliteCommand(sql, (SqliteConnection)QSMain.ConnectionDB, trans);
							long RowId = (long) cmd.ExecuteScalar();
							ComponentsStore.SetValue(childIter, (int)ComponentCol.row_id, (object)RowId);
						}
					} while (ComponentsStore.IterNext(ref childIter));
					childIter = new TreeIter();
				}
				if(ComponentsStore.GetIterFirst(out iter)) {
					do {	//If item is cube
						if ((Nomenclature.NomType)ComponentsStore.GetValue(iter, (int)ComponentCol.nomenclature_type) == Nomenclature.NomType.other)
							continue;
						if ((Nomenclature.NomType)ComponentsStore.GetValue(iter, (int)ComponentCol.nomenclature_type) == Nomenclature.NomType.cube) {
							//Writing common cube info to order_details
							bool HasValue = (int) ComponentsStore.GetValue(iter, (int)ComponentCol.count) > 0;
							bool InDB = (long)ComponentsStore.GetValue(iter, (int)ComponentCol.row_id) > 0;
							if(HasValue) {
								if(!InDB)
									sql = "INSERT INTO order_details (order_id, cube_id, count, facing_id, material_id, comment, price) " +
										"VALUES (@order_id, @nomenclature_id, @count, @facing_id, @material_id, @comment, @price)";
								else
									sql = "UPDATE order_details SET count = @count, facing_id = @facing_id, " +
										"material_id = @material_id, comment = @comment, price = @price WHERE id = @id";
								cmd = new SqliteCommand(sql, (SqliteConnection)QSMain.ConnectionDB, trans);
								cmd.Parameters.AddWithValue("@id", (long)ComponentsStore.GetValue(iter, (int)ComponentCol.row_id));
								cmd.Parameters.AddWithValue("@order_id", ItemId);
								cmd.Parameters.AddWithValue("@nomenclature_id", ComponentsStore.GetValue(iter,(int)ComponentCol.nomenclature_id));
								cmd.Parameters.AddWithValue("@count", ComponentsStore.GetValue(iter,(int)ComponentCol.count));
								cmd.Parameters.AddWithValue("@material_id", DBWorks.ValueOrNull((int)ComponentsStore.GetValue(iter,(int)ComponentCol.material_id) > 0, ComponentsStore.GetValue(iter, (int)ComponentCol.material_id)));
								cmd.Parameters.AddWithValue("@facing_id", DBWorks.ValueOrNull((int)ComponentsStore.GetValue(iter, (int)ComponentCol.facing_id) > 0, ComponentsStore.GetValue(iter, (int)ComponentCol.facing_id)));
								cmd.Parameters.AddWithValue("@comment", DBWorks.ValueOrNull((string)ComponentsStore.GetValue(iter, (int)ComponentCol.comment) != "", ComponentsStore.GetValue(iter, (int)ComponentCol.comment)));
								cmd.Parameters.AddWithValue("@price", ComponentsStore.GetValue(iter, (int)ComponentCol.price_total));
								cmd.ExecuteNonQuery();
								if(!InDB) {
									sql = @"select last_insert_rowid()";
									cmd = new SqliteCommand(sql, (SqliteConnection)QSMain.ConnectionDB, trans);
									long RowId = (long) cmd.ExecuteScalar();
									ComponentsStore.SetValue(iter, (int)ComponentCol.row_id, (object)RowId);
								}
							}
							else if(InDB) {
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
											sql = "INSERT INTO order_cubes_details (order_id, cube_id, nomenclature_id, count, price, comment, " +
												"discount) VALUES (@order_id, @cube_id, @nomenclature_id, @count, @price, @comment, @discount)";
										else
											sql = "UPDATE order_cubes_details " +
												"SET count = @count, price = @price, comment = @comment, discount = @discount WHERE id = @id";
										cmd = new SqliteCommand(sql, (SqliteConnection)QSMain.ConnectionDB, trans);
										cmd.Parameters.AddWithValue("@id", (long)ComponentsStore.GetValue(childIter, (int)ComponentCol.row_id));
										cmd.Parameters.AddWithValue("@order_id", ItemId);
										cmd.Parameters.AddWithValue("@cube_id", ComponentsStore.GetValue(iter, (int)ComponentCol.nomenclature_id));
										cmd.Parameters.AddWithValue("@nomenclature_id", ComponentsStore.GetValue(childIter,(int)ComponentCol.nomenclature_id));
										cmd.Parameters.AddWithValue("@count", ComponentsStore.GetValue(childIter,(int)ComponentCol.count));
										cmd.Parameters.AddWithValue("@price", ComponentsStore.GetValue(childIter, (int)ComponentCol.price));
										cmd.Parameters.AddWithValue("@comment", DBWorks.ValueOrNull((string)ComponentsStore.GetValue(childIter, (int)ComponentCol.comment) != "", ComponentsStore.GetValue(childIter, (int)ComponentCol.comment)));
										cmd.Parameters.AddWithValue("@discount", ComponentsStore.GetValue(childIter, (int)ComponentCol.discount));
										cmd.ExecuteNonQuery();
										if(!InDB) {
											sql = @"select last_insert_rowid()";
											cmd = new SqliteCommand(sql, (SqliteConnection)QSMain.ConnectionDB, trans);
											long RowId = (long) cmd.ExecuteScalar();
											ComponentsStore.SetValue(childIter, (int)ComponentCol.row_id, (object)RowId);
										}
									}
									else if(InDB) {
										sql = "DELETE FROM order_cubes_details WHERE id = @id";
										cmd = new SqliteCommand(sql, (SqliteConnection)QSMain.ConnectionDB, trans);
										cmd.Parameters.AddWithValue("@id", ComponentsStore.GetValue(childIter, (int)ComponentCol.row_id));
										cmd.ExecuteNonQuery();
									}
								} while (ComponentsStore.IterNext(ref childIter));
							}
						}
						else {	//Item is basis 
							if(ComponentsStore.IterHasChild(iter)) {
								ComponentsStore.IterChildren(out childIter, iter);
								do { //Adding every nomenclature for basis
									bool HasValue = (int) ComponentsStore.GetValue(childIter, (int)ComponentCol.count) > 0;
									bool InDB = (long)ComponentsStore.GetValue(childIter, (int)ComponentCol.row_id) > 0;
									if(HasValue) {
										if(!InDB)
											sql = "INSERT INTO order_basis_details " +
												"(order_id, nomenclature_id, count, price, comment, discount, facing_id, material_id) " +
												"VALUES (@order_id, @nomenclature_id, @count, @price, @comment, @discount, @facing_id, @material_id)";
										else
											sql = "UPDATE order_basis_details SET count = @count, price = @price, comment = @comment, " +
												"discount = @discount, facing_id = @facing_id, material_id = @material_id WHERE id = @id";
										cmd = new SqliteCommand(sql, (SqliteConnection)QSMain.ConnectionDB, trans);
										cmd.Parameters.AddWithValue("@id", (long)ComponentsStore.GetValue(childIter, (int)ComponentCol.row_id));
										cmd.Parameters.AddWithValue("@order_id", ItemId);
										cmd.Parameters.AddWithValue("@nomenclature_id", ComponentsStore.GetValue(childIter,(int)ComponentCol.nomenclature_id));
										cmd.Parameters.AddWithValue("@count", ComponentsStore.GetValue(childIter,(int)ComponentCol.count));
										cmd.Parameters.AddWithValue("@price", ComponentsStore.GetValue(childIter, (int)ComponentCol.price));
										cmd.Parameters.AddWithValue("@comment", DBWorks.ValueOrNull((string)ComponentsStore.GetValue(childIter, (int)ComponentCol.comment) != "", ComponentsStore.GetValue(childIter, (int)ComponentCol.comment)));
										cmd.Parameters.AddWithValue("@discount", ComponentsStore.GetValue(childIter, (int)ComponentCol.discount));
										cmd.Parameters.AddWithValue("@facing_id", ComponentsStore.GetValue(childIter, (int)ComponentCol.facing_id));
										cmd.Parameters.AddWithValue("@material_id", ComponentsStore.GetValue(childIter, (int)ComponentCol.material_id));

										cmd.ExecuteNonQuery();
										if(!InDB) {
											sql = @"select last_insert_rowid()";
											cmd = new SqliteCommand(sql, (SqliteConnection)QSMain.ConnectionDB, trans);
											long RowId = (long) cmd.ExecuteScalar();
											ComponentsStore.SetValue(childIter, (int)ComponentCol.row_id, (object)RowId);
										}
									}
									else if(InDB) {
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
			catch (Exception ex) {
				trans.Rollback();
				QSMain.ErrorMessageWithLog(this, "Ошибка записи заказа!", logger, ex);
				return false;
			}
		}
			
		/// <summary>
		/// Method for loading existing orders.
		/// </summary>
		/// <param name="id">Identifier - order id in database</param>
		/// <param name="copy">If set to <c>true</c> copy.</param>
		public void Fill(int id, bool copy) {
			FillInProgress = true;
			NewItem = copy;
			if(!copy)
				ItemId = id;
			MainClass.StatusMessage(String.Format ("Запрос заказа №{0}...", id));
			string sql = "SELECT orders.* FROM orders WHERE orders.id = @id";
			try {
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
					BasisIter = ComponentsStore.InsertNodeBefore(ServiceIter);
					ComponentsStore.SetValues (
						BasisIter,
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
						false,
						false,
						0,
						false
					);
				}
				//Loading services.
				sql = "select * from order_services where order_id = @order_id";
				cmd = new SqliteCommand(sql, (SqliteConnection)QSMain.ConnectionDB);
				cmd.Parameters.AddWithValue("@order_id", id);
				using (SqliteDataReader rdr = cmd.ExecuteReader()) {
					while(rdr.Read()) {
						Decimal price = DBWorks.GetDecimal(rdr, "price", 0);
						ComponentsStore.AppendValues (
							ServiceIter,
							copy ? -1 : DBWorks.GetLong(rdr, "id", -1),
							Enum.Parse (typeof(Nomenclature.NomType), "other"),
							-1,
							"",
							DBWorks.GetString(rdr, "name", ""), //Service name
							"",
							1,
							-1,
							"",
							-1,
							"",
							DBWorks.GetString(rdr, "comment", ""),
							price.ToString(),
							(Math.Round(price + price / 100 * DBWorks.GetInt(rdr, "discount", 0), 0)).ToString(),
							false,
							true,
							false,
							false,
							true,
							true, 
							DBWorks.GetInt(rdr, "discount", 0),
							true
						);
					}
				}
				CalculateServiceCount();
				//Loading basis and it's contents.

				sql = "SELECT * FROM (" +
					"SELECT order_basis_details.id as id, order_basis_details.nomenclature_id as nomenclature_id, order_basis_details.count as count, " +
					"order_basis_details.price as price, order_basis_details.comment as comment, order_basis_details.discount as discount, " +
					"order_basis_details.facing_id as facing_id, facing.name as facing, order_basis_details.material_id as material_id, " +
					"materials.name as material, nomenclature.type, nomenclature.name, nomenclature.description, nomenclature.price_type " +
					"FROM order_basis_details " +
					"LEFT JOIN nomenclature ON order_basis_details.nomenclature_id = nomenclature.id " +
					"LEFT JOIN facing ON order_basis_details.facing_id = facing.id " +
					"LEFT JOIN materials ON order_basis_details.material_id = materials.id " +
					"WHERE order_basis_details.order_id = @order_id " +
					"UNION " +
					"SELECT NULL as id, nomenclature.id AS nomenclature_id, 0 AS count, nomenclature.price AS price, " +
					"NULL AS comment, 0 AS discount, -1 AS facing_id, NULL AS facing, -1 as material_id, NULL AS material, " +
					"nomenclature.type AS type, nomenclature.name AS name, nomenclature.description AS description, nomenclature.price_type " +
					"FROM nomenclature " +
					"LEFT JOIN basis_items ON nomenclature.id = basis_items.item_id " +
					"WHERE basis_items.basis_id = @basis_id) group by name";
				cmd = new SqliteCommand(sql, (SqliteConnection)QSMain.ConnectionDB);
				cmd.Parameters.AddWithValue("@order_id", id);
				cmd.Parameters.AddWithValue("@basis_id", basis.id);
				using (SqliteDataReader rdr = cmd.ExecuteReader()) {
					while(rdr.Read()) {
						int count = DBWorks.GetInt(rdr, "count", 0);
						decimal price = DBWorks.GetDecimal(rdr, "price", 0);
						if (count < 1)
							if (rdr["price_type"].ToString() == "width")
								price *= OrderCupboard.CubesH;
							else if (rdr["price_type"].ToString() == "height")
								price *= OrderCupboard.CubesV;
						ComponentsStore.AppendValues(
							BasisIter,
							copy ? -1 : DBWorks.GetLong(rdr, "id", -1),
							Enum.Parse(typeof(Nomenclature.NomType), rdr["type"].ToString()),
							DBWorks.GetInt(rdr, "nomenclature_id", -1),
							DBWorks.GetString(rdr, "name", ""), 
							ReplaceArticle(DBWorks.GetString(rdr, "name", "")),
							DBWorks.GetString(rdr, "description", ""),
							count,
							DBWorks.GetInt(rdr, "material_id", -1),
							DBWorks.GetString(rdr, "material", ""),
							DBWorks.GetInt(rdr, "facing_id", -1),
							DBWorks.GetString(rdr, "facing", ""),
							DBWorks.GetString(rdr, "comment", ""),
							price.ToString(),
							"",
							true,
							true,
							true,
							true,
							true,
							true,
							DBWorks.GetInt(rdr, "discount", 0),
							false
						);
					}
				}
				//Loading cubes.
				sql = "SELECT order_details.id as id, order_details.cube_id as cube_id, order_details.count as count, order_details.facing_id as facing_id, " +
					"facing.name as facing, order_details.material_id as material_id, materials.name as material, order_details.comment as comment, " +
					"order_details.price as price, cubes.name, cubes.width as width, cubes.height as height " +
					"FROM order_details " +
					"LEFT JOIN cubes ON order_details.cube_id = cubes.id " +
					"LEFT JOIN facing ON facing.id = order_details.facing_id " +
					"LEFT JOIN materials ON materials.id = order_details.material_id " +
					"WHERE order_id = @order_id;";
				cmd = new SqliteCommand(sql, (SqliteConnection)QSMain.ConnectionDB);
				cmd.Parameters.AddWithValue("@order_id", id);
				using (SqliteDataReader rdr = cmd.ExecuteReader()) {
					while (rdr.Read()) {
						TreeIter CubeIter = ComponentsStore.InsertNodeBefore(ServiceIter);
							ComponentsStore.SetValues (
							CubeIter,
							copy ? -1 : DBWorks.GetLong(rdr, "id", -1), 
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
							true,
							false,
							0,
							false
						);
						string contents_sql = "SELECT * FROM (" +
							"SELECT order_cubes_details.id, order_cubes_details.nomenclature_id, order_cubes_details.count as count, " +
							"order_cubes_details.price as price, order_cubes_details.comment as comment, order_cubes_details.discount as discount, " +
							"nomenclature.type AS type, nomenclature.name AS name, nomenclature.description AS description, nomenclature.price_type as price_type " +
							"FROM order_cubes_details " +
							"LEFT JOIN nomenclature ON order_cubes_details.nomenclature_id = nomenclature.id " +
							"WHERE order_cubes_details.order_id = @order_id AND order_cubes_details.cube_id = @cube_id " +
							"UNION " +
							"SELECT NULL AS id, nomenclature.id AS nomenclature_id, 0 AS count, nomenclature.price AS price, NULL AS comment, 0 AS discount, " +
							"nomenclature.type AS type, nomenclature.name AS name, nomenclature.description AS description, nomenclature.price_type as price_type " +
							"FROM nomenclature " +
							"LEFT JOIN cubes_items ON nomenclature.id = cubes_items.item_id " +
							"WHERE cubes_items.cubes_id = @cube_id" +
							") group by name;";
						SqliteCommand contents_cmd = new SqliteCommand (contents_sql, (SqliteConnection)QSMain.ConnectionDB);
						contents_cmd.Parameters.AddWithValue ("@cube_id", DBWorks.GetInt(rdr, "cube_id", -1));
						contents_cmd.Parameters.AddWithValue ("@order_id", id);
						int width = DBWorks.GetInt(rdr, "width", 1);
						int height = DBWorks.GetInt(rdr, "height", 1);
						using (SqliteDataReader contents_rdr = contents_cmd.ExecuteReader ()) {
							while (contents_rdr.Read ()) {
								Decimal NomenclaturePrice = DBWorks.GetDecimal (contents_rdr, "price", 0);
								if (DBWorks.GetDecimal (contents_rdr, "count", 1) < 1)
									if (contents_rdr["price_type"].ToString() == "width")
										NomenclaturePrice *= width;
									else if (contents_rdr["price_type"].ToString() == "height")
										NomenclaturePrice *= height;
								ComponentsStore.AppendValues (
									CubeIter,
									copy ? -1 : DBWorks.GetLong(contents_rdr, "id", -1),
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
									NomenclaturePrice.ToString(),
									"",
									true,
									true,
									false,
									false,
									true,
									true,
									DBWorks.GetInt(contents_rdr, "discount", 0),
									false
								);
							}
						}
					}
				}
				CalculateTotalPrice();
				basis.Button.Click();
				FillInProgress = false;
				MainClass.StatusMessage("Ok");
			}
			catch (Exception ex) {
				QSMain.ErrorMessageWithLog(this, "Ошибка получения информации о заказе!", logger, ex);
			}
			TestCanSave();
		}

		void OnCountEdited(object o, EditedArgs args) {
			TreeIter iter;
			int NewValue;

			if (!ComponentsStore.GetIterFromString (out iter, args.Path))
				return;
			try { 
				if (args.NewText == null)
					NewValue = 1;
				else
					NewValue = int.Parse (args.NewText); 
				ComponentsStore.SetValue(iter, (int)ComponentCol.count, NewValue);
				CalculateTotalPrice ();
			} catch(Exception e) { logger.Warn (e, "Error occured in OnCountEdited.");}
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
				ComponentsStore.SetValue(iter, (int)ComponentCol.price, (NewValue).ToString());
				CalculateTotalPrice ();
			} catch(Exception e) { logger.Warn (e, "Error occured in OnPriceEdited");}
		}

		void OnMaterialComboEdited (object o, EditedArgs args) {
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

		void OnDiscountEdited(object o, EditedArgs args) {
			TreeIter iter;
			int discount;

			if (!ComponentsStore.GetIterFromString (out iter, args.Path))
				return;
			if (args.NewText == null){
				logger.Warn ("text is empty");
				return;
			}
			if (int.TryParse (args.NewText, out discount)) {
				ComponentsStore.SetValue (iter, (int)ComponentCol.discount, discount);
				CalculateTotalPrice ();
			}
		}

		void OnFacingComboEdited (object o, EditedArgs args) {
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

		void OnCommentTextEdited (object o, EditedArgs args) {
			TreeIter iter;
			if (!ComponentsStore.GetIterFromString (out iter, args.Path))
				return;
			if(args.NewText == null) {
				logger.Warn("newtext is empty");
				return;
			}
			ComponentsStore.SetValue(iter, (int)ComponentCol.comment, args.NewText);
		}

		void OnCellNameEdited (object o, EditedArgs args) {
			TreeIter iter;
			if (!ComponentsStore.GetIterFromString (out iter, args.Path))
				return;
			if(args.NewText == null) {
				logger.Warn("newtext is empty");
				return;
			}
			ComponentsStore.SetValue(iter, (int)ComponentCol.nomenclature_title, args.NewText);
		}

		protected void TestCanSave () {
			bool Dateok = dateArrval.IsEmpty || dateDeadlineS.IsEmpty || dateArrval.Date <= dateDeadlineS.Date;
			saveAction.Sensitive = Dateok;
		}


		protected void OnBasisChanged(object sender, EventArgs e) {
			CupboardListItem basis = TypeWidgetList.Find(w => w.Button.Active);
			if(basis == null) {
				logger.Warn("Не найдена активная основа");
				return;
			}
			if (sender != null && basis.Button != sender)
				return;
			OrderCupboard.BorderImage.LoadImage(basis.Image.OriginalFile);
			UpdateBasisComponents(basis.id);
		}

		protected void OnComboCubeHChanged(object sender, EventArgs e) {
			CupboardListItem basis = TypeWidgetList.Find (w => w.Button.Active);
			if(basis == null) {
				logger.Warn("Не найдена активная основа");
				return;
			}
			OrderCupboard.CubesH = int.Parse(comboCubeH.ActiveText);
			if(OrderCupboard.BorderImage != null)
				OrderCupboard.BorderImage.ModifyDrawingImage();
			CalculateCubePxSize(drawCupboard.Allocation);
			UpdateBasisComponents (basis.id);
		}

		protected void OnComboCubeVChanged(object sender, EventArgs e) {
			CupboardListItem basis = TypeWidgetList.Find (w => w.Button.Active);
			if(basis == null) {
				logger.Warn("Не найдена активная основа");
				return;
			}
			OrderCupboard.CubesV = int.Parse(comboCubeV.ActiveText);
			if(OrderCupboard.BorderImage != null)
				OrderCupboard.BorderImage.ModifyDrawingImage();
			CalculateCubePxSize(drawCupboard.Allocation);
			UpdateBasisComponents (basis.id);
		}

		protected void OnDrawCupboardExposeEvent(object o, ExposeEventArgs args) {
			using (Context cr = Gdk.CairoHelper.Create (args.Event.Window)) {
				int w, h;
				args.Event.Window.GetSize (out w, out h);
				OrderCupboard.Draw(cr, w, h, CubePxSize, false);
			}
		}

		protected void OnDrawCupboardSizeAllocated(object o, SizeAllocatedArgs args) {
			CalculateCubePxSize(args.Allocation);
		}

		private void CalculateCubePxSize(Gdk.Rectangle CupboardPlace) {

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
				WidthWithoutGrid = WidthTable - ListPixelAddH;
			else
				HeightWithoutGrid = HeightTable - ListPixelAddV;

			// 1.2 это 2 бортика по караям которые равны 60% от куба
			int MinCubeSizeForH = Convert.ToInt32(WidthWithoutGrid / (double.Parse(comboCubeH.ActiveText) + 1.2 + ListCybesH));
			int MinCubeSizeForV = Convert.ToInt32(HeightWithoutGrid / (double.Parse(comboCubeV.ActiveText) + 1.2 + ListCybesV));

			int NeedCubePxSize = Math.Min(MinCubeSizeForH, MinCubeSizeForV);

			if (NeedCubePxSize > 100)
				NeedCubePxSize = 100;

			if(CubePxSize != NeedCubePxSize) {
				CubePxSize = NeedCubePxSize;

				int MaxHeight = 0, MaxWidth = 0;
				foreach(Cube cube in CubeList) {
					((CubeListItem)cube.Widget).CubePxSize = CubePxSize;
					Requisition req = ((CubeListItem)cube.Widget).SizeRequest();
					MaxHeight = Math.Max(MaxHeight, req.Height);
					MaxWidth = Math.Max(MaxWidth, req.Width);
				}
				hboxCubeList.HeightRequest = MaxHeight;
				vboxCubeList.WidthRequest = MaxWidth;
			}
		}

		protected void OnDrawCupboardDragMotion(object o, DragMotionArgs args) {
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

		protected void OnDrawCupboardDragDrop(object o, DragDropArgs args) {
			logger.Debug ("Drop");
			int CubePosX = (args.X + (int)(CubePxSize * 1.5) - CurrentDrag.IconPosX - OrderCupboard.CupboardZeroX) / CubePxSize;
			int CubePosY = (args.Y + (int)(CubePxSize * 1.5) - CurrentDrag.IconPosY - OrderCupboard.CupboardZeroY) / CubePxSize;
			//Для того что бы корректно посчитать 0 ячейку добавли 1, сейчас онимаем.
			CubePosX--; CubePosY--;
			logger.Debug ("CupBoard pos x={0} y={1}", CubePosX, CubePosY);
			bool CanDrag = OrderCupboard.TestPutCube(CurrentDrag.cube, CubePosX, CubePosY);
			if (CanDrag) {
				if(CurrentDrag.FromList) {
					Cube NewCube = CurrentDrag.cube.Clone();
					NewCube.BoardPositionX = CubePosX;
					NewCube.BoardPositionY = CubePosY;
					OrderCupboard.Cubes.Add(NewCube);
					UpdateCubeComponents();
				} 
				else {
					CurrentDrag.cube.BoardPositionX = CubePosX;
					CurrentDrag.cube.BoardPositionY = CubePosY;
				}
				Gtk.Drag.Finish(args.Context, true, false, args.Time);
			}
			else
				Gtk.Drag.Finish(args.Context, false, true, args.Time);
			drawCupboard.QueueDraw();
		}

		protected void OnDrawCupboardDragBegin(object o, DragBeginArgs args) {
			int MousePosX, MousePosY;
			drawCupboard.GetPointer(out MousePosX, out MousePosY);

			int CubePosX = (MousePosX - OrderCupboard.CupboardZeroX) / CubePxSize;
			int CubePosY = (MousePosY - OrderCupboard.CupboardZeroY) / CubePxSize;
			Cube cube = OrderCupboard.GetCube(CubePosX, CubePosY);

			if(cube == null) {
				args.RetVal = false;
				Gdk.Drag.Abort(args.Context, args.Context.StartTime);
				return;
			}

			Pixmap pix = new Pixmap(drawCupboard.GdkWindow, cube.CubesH * CubePxSize, cube.CubesV * CubePxSize);

			using (Context cr = Gdk.CairoHelper.Create(pix)) {
				cube.DrawCube(cr, CubePxSize, true);
			}
			Gdk.Pixbuf pixbuf = Gdk.Pixbuf.FromDrawable(pix, Gdk.Colormap.System, 0, 0, 0, 0, cube.CubesH * CubePxSize, cube.CubesV * CubePxSize);
			CurrentDrag.IconPosX = MousePosX - OrderCupboard.CupboardZeroX - (cube.BoardPositionX * CubePxSize);
			CurrentDrag.IconPosY = MousePosY - OrderCupboard.CupboardZeroY - (cube.BoardPositionY * CubePxSize);

			Gtk.Drag.SetIconPixbuf(args.Context, pixbuf, CurrentDrag.IconPosX, CurrentDrag.IconPosY);
			CurrentDrag.FromList = false;	
			CurrentDrag.cube = cube;
		}

		protected void OnDrawCupboardDragDataDelete(object o, DragDataDeleteArgs args) {
			OrderCupboard.Cubes.Remove(CurrentDrag.cube);
			UpdateCubeComponents();
			args.RetVal = true;
		}

		protected void OnCubeListDragDrop(object o, DragDropArgs args) {
			logger.Debug ("Drop to CubeList");
			if (CurrentDrag.FromList)
				Gtk.Drag.Finish(args.Context, false, false, args.Time);
			else {
				Gtk.Drag.Finish(args.Context, true, true, args.Time);
				drawCupboard.QueueDraw();
			}
			args.RetVal = true;
		}

		void UpdateCubeList() {
			scrolledCubeListV.Visible = VerticalCubeList;
			scrolledCubeListH.Visible = !VerticalCubeList;
			Box boxCubeList = VerticalCubeList ? (Box)vboxCubeList : (Box) hboxCubeList;
			Box ForRemove = !VerticalCubeList ? (Box)vboxCubeList : (Box) hboxCubeList;

			foreach(CubeListItem item in CubeWidgetList) {
				if(item.Parent != null)
					ForRemove.Remove(item);
				boxCubeList.Add(item);
			}
			boxCubeList.ShowAll();
		}

		protected void OnButtonCubeListOrientationClicked(object sender, EventArgs e) {
			VerticalCubeList = !VerticalCubeList;
			UpdateCubeList();
		}

		protected void OnNotebook1SwitchPage(object o, SwitchPageArgs args)	{
			if (notebook1.CurrentPage == 2)
				if (OrderCupboard.Clean())
					UpdateCubeComponents();
			if (notebook1.CurrentPage == 4)
				PrerareReport(true);
			if (notebook1.CurrentPage == 5)
				PrerareReport (false);
		
			goBackAction.Sensitive = notebook1.CurrentPage != 0;
			goForwardAction.Sensitive = notebook1.CurrentPage != 4;
		}

		/// <summary>
		/// Updates the basis components.
		/// </summary>
		/// <param name="id">Identifier of basis.</param>
		private void UpdateBasisComponents(int id) {
			if (FillInProgress)
				return;
			Dictionary<int, TreeIter> pairs = new Dictionary<int, TreeIter> ();
			TreeIter iter;
			//Making all components inside basis NULL.
			if (ComponentsStore.IterHasChild (BasisIter)) {
				ComponentsStore.IterChildren (out iter, BasisIter);
				bool removed;
				do {
					removed = false;
					if ((long)ComponentsStore.GetValue(iter, (int)ComponentCol.row_id) == (long)-1) {
						if (!ComponentsStore.Remove(ref iter))
							break;
						else {
							removed = true;
							continue;
						}
					}
					ComponentsStore.SetValue (iter, (int)ComponentCol.count, 0);
					pairs.Add((int)ComponentsStore.GetValue(iter, (int)ComponentCol.nomenclature_id), iter);
				} while (removed || ComponentsStore.IterNext (ref iter));
			}

			string sql = "SELECT nomenclature.name as nomenclature, nomenclature.type, nomenclature.description, nomenclature.price, nomenclature.price_type, " +
				"basis_items.* FROM basis_items LEFT JOIN nomenclature ON nomenclature.id = basis_items.item_id WHERE basis_id = @basis_id";
			SqliteCommand cmd = new SqliteCommand(sql, (SqliteConnection)QSMain.ConnectionDB);
			cmd.Parameters.AddWithValue("@basis_id", id);
			using (SqliteDataReader rdr = cmd.ExecuteReader()) {
				while (rdr.Read()) {
					int count = DBWorks.GetInt (rdr, "count", 1);
					Decimal price = DBWorks.GetDecimal (rdr, "price", 0);
					if (rdr["price_type"].ToString() == "width")
						price *= OrderCupboard.CubesH;
					else if (rdr["price_type"].ToString() == "height")
						price *= OrderCupboard.CubesV;
					if (pairs.TryGetValue (DBWorks.GetInt (rdr, "item_id", -1), out iter)) {
						ComponentsStore.SetValue (iter, (int)ComponentCol.price, price.ToString());
						ComponentsStore.SetValue (iter, (int)ComponentCol.count, count);
					} 
					else {
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
							price.ToString (),
							"",
							true,
							true,
							true,
							true,
							true,
							true, 
							0,
							false
						);
					}
				}
			}
			CalculateTotalPrice();
		}
			
		/// <summary>
		/// Updates the cube components. Adding one or removing if needed.
		/// </summary>
		private void UpdateCubeComponents() {
			TreeIter iter;
			Dictionary<int, int> Counts = OrderCupboard.GetAmounts();

			if (ComponentsStore.GetIterFirst(out iter)) {
				do {
					if ((Nomenclature.NomType)ComponentsStore.GetValue(iter, (int)ComponentCol.nomenclature_type) == Nomenclature.NomType.cube) {
						int NomId = (int)ComponentsStore.GetValue(iter, (int)ComponentCol.nomenclature_id);
						if(Counts.ContainsKey(NomId)) {
							ComponentsStore.SetValue(iter, (int)ComponentCol.count, Counts[NomId]);
							Counts.Remove(NomId);
							UpdateTable(iter);
						}
						else if((long)ComponentsStore.GetValue(iter, (int)ComponentCol.row_id) >= 0) {
							ComponentsStore.SetValue(iter, (int)ComponentCol.count, 0);
							TreeIter child;
							if (ComponentsStore.IterHasChild(iter)) {
								ComponentsStore.IterChildren(out child, iter);
								do {
									ComponentsStore.SetValue(child, (int)ComponentCol.count, 0);
								} while (ComponentsStore.IterNext(ref child));
							}
						}
						else
							ComponentsStore.Remove(ref iter);
					}
				}
				while(ComponentsStore.IterNext(ref iter));
			}

			foreach (KeyValuePair<int, int> pair in Counts) {
				Cube cube = OrderCupboard.Cubes.Find (c => c.NomenclatureId == pair.Key);
				TreeIter CubeIter = ComponentsStore.InsertNodeBefore (ServiceIter);
				ComponentsStore.SetValues (
					CubeIter, 
					(long)-1, 
					Enum.Parse (typeof(Nomenclature.NomType), "cube"), 
					pair.Key, 
					null, 
					cube.Name, 
					null, 
					pair.Value, 
					-1, 
					"", 
					-1, 
					"", 
					"", 
					"", 
					"",
					false, 
					false, 
					true, 
					true, 
					true, 
					false, 
					0, 
					false
				);
				string sql = "SELECT nomenclature.name as nomenclature, nomenclature.type, nomenclature.description, nomenclature.price, nomenclature.price_type, cubes_items.* FROM cubes_items " +
					"LEFT JOIN nomenclature ON nomenclature.id = cubes_items.item_id " +
					"WHERE cubes_id = @cubes_id";
				SqliteCommand cmd = new SqliteCommand (sql, (SqliteConnection)QSMain.ConnectionDB);
				cmd.Parameters.AddWithValue ("@cubes_id", pair.Key);
				using (SqliteDataReader rdr = cmd.ExecuteReader ()) {
					while (rdr.Read ()) {
						Decimal price = DBWorks.GetDecimal (rdr, "price", 0);
						if (rdr["price_type"].ToString() == "width")
							price *= cube.CubesH;
						else if (rdr["price_type"].ToString() == "height")
							price *= cube.CubesV;
						int count = DBWorks.GetInt(rdr, "count", 1) * pair.Value;
						ComponentsStore.AppendValues (
							CubeIter,
							(long)-1,
							Enum.Parse (typeof(Nomenclature.NomType), rdr ["type"].ToString ()),
							DBWorks.GetInt (rdr, "item_id", -1),
							DBWorks.GetString (rdr, "nomenclature", "нет"),
							ReplaceArticle (DBWorks.GetString (rdr, "nomenclature", "нет")),
							DBWorks.GetString (rdr, "description", ""),
							count,
							-1,
							"",
							-1,
							"",
							"",
							price.ToString(),
							"",
							true,
							true,
							false,
							false,
							true,
							true, 
							0,
							false
						);
					}
				}
			}
			CalculateTotalPrice();
		}

		private string ReplaceArticle(string text) {
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
					}
				} while(ComponentsStore.IterNext(ref childIter));
				CalculateTotalPrice ();
			}
		}

		/// <summary>
		/// Calculates/corrects the total price for items on order change and for whole order too.
		/// </summary>
		void CalculateTotalPrice()
		{
			Decimal TempTotal;
			TreeIter iter, childIter;

			if (ComponentsStore.GetIterFirst(out iter)) {
				TotalPrice = 0;
				do {
					if (ComponentsStore.IterHasChild(iter)) {
						TempTotal = 0;
						ComponentsStore.IterChildren(out childIter, iter);
						do {
							Decimal ComponentPrice = Decimal.Parse((String)ComponentsStore.GetValue(childIter, (int)ComponentCol.price));
							int ComponentCount = (int)ComponentsStore.GetValue(childIter, (int)ComponentCol.count);
							int Discount = (int)ComponentsStore.GetValue(childIter, (int)ComponentCol.discount);
							Decimal ComponentTotal = ComponentPrice * ComponentCount;
							ComponentTotal = Math.Round(ComponentTotal + ComponentTotal * Discount / 100, 0);
							ComponentsStore.SetValue(childIter, (int)ComponentCol.price_total, ComponentTotal.ToString());
							TempTotal += ComponentTotal;
						} while (ComponentsStore.IterNext(ref childIter));
						TotalPrice += TempTotal;
						ComponentsStore.SetValue(iter, (int)ComponentCol.price_total, TempTotal.ToString());
					}
				} while(ComponentsStore.IterNext (ref iter));
			}
			labelTotalCount.LabelProp = String.Format("Итого: {0:C} ", Decimal.Round(TotalPrice + (TotalPrice / 100 * PriceCorrection), 0));
		}

		private void PrerareReport(bool Client) {
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
			string ReportPath;
			if (Client) {
				ReportPath = System.IO.Path.Combine (Directory.GetCurrentDirectory (), "Reports", "order" + ".rdl");
				reportviewer1.LoadReport (new Uri (ReportPath), param, QSMain.ConnectionString);
			} 
			else {
				ReportPath = System.IO.Path.Combine (Directory.GetCurrentDirectory (), "Reports", "order_factory" + ".rdl");
				reportviewer2.LoadReport(new Uri(ReportPath), param, QSMain.ConnectionString);
			}

		}

		protected void OnSaveActionActivated(object sender, EventArgs e) {
			if (Save())
				this.Destroy();
		}

		protected void OnGoBackActionActivated(object sender, EventArgs e) {
			notebook1.PrevPage();
		}

		protected void OnGoForwardActionActivated(object sender, EventArgs e) {
			notebook1.NextPage();
		}

		protected void OnRevertToSavedActionActivated(object sender, EventArgs e) {
			this.Destroy();
		}

		protected void OnCheckEstimationClicked(object sender, EventArgs e) {
			entryContract.Sensitive = !checkEstimation.Active;
		}

		protected void OnEntryContractChanged(object sender, EventArgs e) {
			int number;
			if(int.TryParse(entryContract.Text, out number))
				entryContract.ModifyText(StateType.Normal);
			else
				entryContract.ModifyText(StateType.Normal, new Gdk.Color(255,0,0)); 
		}

		protected void OnDateDeadlineSDateChanged(object sender, EventArgs e) {
			dateDeadlineE.Date = dateDeadlineS.Date.AddDays ((double)7);
			TestCanSave ();
			if (!(dateArrval.IsEmpty || dateDeadlineS.IsEmpty || dateArrval.Date <= dateDeadlineS.Date)) {
				MessageDialog md = new MessageDialog (this, DialogFlags.DestroyWithParent,
					                   MessageType.Warning, 
					                   ButtonsType.Ok, 
					                   "Дата сдачи заказа должна быть позже даты прихода.");
				md.Run ();
				md.Destroy ();
				dateDeadlineS.Date = dateArrval.Date;
			}
			if (!(dateDeadlineS.Date <= dateDeadlineE.Date)) {
				MessageDialog md = new MessageDialog (this, DialogFlags.DestroyWithParent,
					                   MessageType.Warning, 
					                   ButtonsType.Ok, 
					                   "Крайний срок периода сдачи заказа не может быть раньше даты начала.");
				md.Run ();
				md.Destroy ();
				dateDeadlineE.Date = dateDeadlineS.Date.AddDays ((double)7);
			}
		}

		protected void OnDateDeadlineEDateChanged (object sender, EventArgs e) {
			if (dateDeadlineS.IsEmpty && !dateDeadlineE.IsEmpty) {
				MessageDialog md = new MessageDialog (this, DialogFlags.DestroyWithParent,
					                   MessageType.Warning, 
					                   ButtonsType.Ok, 
					                   "Пожалуйста, введите сначала начальную дату периода сдачи заказа.");
				md.Run ();
				md.Destroy ();
				saveAction.Sensitive = false;
			}
		}

		protected void OnCheckbutton2Toggled (object sender, EventArgs e) {
			ColumnPrice.Visible = ColumnPriceTotal.Visible = ColumnDiscount.Visible = checkbuttonShowPrice.Active;
		}

		protected void OnCheckbutton1Toggled (object sender, EventArgs e) {
			spinbutton1.Sensitive = checkbuttonDiscount.Active;
			if (spinbutton1.Sensitive == false)
				PriceCorrection = 0;
			else 
				PriceCorrection = spinbutton1.ValueAsInt;
			CalculateTotalPrice ();
		}

		protected void OnSpinbutton1ValueChanged (object sender, EventArgs e)  {
			PriceCorrection = spinbutton1.ValueAsInt;
			CalculateTotalPrice ();
			if (spinbutton1.ValueAsInt > 0)
				checkbuttonDiscount.Label = "Наценка:";
			else
				checkbuttonDiscount.Label = "Скидка:";
		}

		protected void OnTreeviewComponentsButtonReleaseEvent (object o, ButtonReleaseEventArgs args) {
			TreeIter iter, tempIter;
			if (args.Event.Button == 3) {
				if (treeviewComponents.Selection.CountSelectedRows() == 1 && treeviewComponents.Selection.GetSelected(out iter)) {
					if (ComponentsStore.IterParent (out tempIter, iter) || iter.Equals(ServiceIter)) {
						if (tempIter.Equals(ServiceIter) || iter.Equals(ServiceIter)) {
							Gtk.Menu menu = new Gtk.Menu ();
							Gtk.MenuItem add = new Gtk.MenuItem ("Добавить новую услугу");
							add.ButtonReleaseEvent += PopupMenuAddEvent;
							Gtk.MenuItem delete = new Gtk.MenuItem ("Удалить услугу");
							delete.ButtonReleaseEvent += PopupMenuDeleteEvent;
							if (iter.Equals(ServiceIter))
								delete.Sensitive = false;
							menu.Append (add);
							menu.Append (delete);
							menu.ShowAll ();
							menu.Popup (); 
						}
					}
				}
			}
		}

		private void PopupMenuAddEvent(object sender, EventArgs args) {
			ComponentsStore.AppendValues (
				ServiceIter,
				(long)-1,
				Enum.Parse (typeof(Nomenclature.NomType), "other"),
				-1,
				"",
				"Название услуги", //Service name
				"",
				1,
				-1,
				"",
				-1,
				"",
				"",
				"0",
				"0",
				false,
				true,
				false,
				false,
				true,
				true, 
				0,
				true
			);
			CalculateServiceCount();
			CalculateTotalPrice ();
		}

		private void CalculateServiceCount()
		{
			ComponentsStore.SetValue(ServiceIter, (int)ComponentCol.count, 
				(int)ComponentsStore.IterNChildren(ServiceIter));
		}

		private void PopupMenuDeleteEvent(object sender, EventArgs args) {
			TreeIter iter;
			long row_id;
			if (treeviewComponents.Selection.GetSelected (out iter)) {
				if ((row_id = (long)ComponentsStore.GetValue (iter, (int)ComponentCol.row_id)) > 0) {
					string sql = "delete from order_services where id = @id";
					SqliteCommand cmd = new SqliteCommand(sql, (SqliteConnection)QSMain.ConnectionDB);
					cmd.Parameters.AddWithValue("@id", row_id);
					cmd.ExecuteNonQuery();
				}
				ComponentsStore.Remove (ref iter);
			}
			CalculateServiceCount();
			CalculateTotalPrice ();
		}
	}
}