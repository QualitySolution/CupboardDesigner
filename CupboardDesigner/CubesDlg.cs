using System;
using System.IO;
using System.Collections.Generic;
using NLog;
using Mono.Data.Sqlite;
using QSProjectsLib;
using Gtk;
using Cairo;

namespace CupboardDesigner
{
	public partial class CubesDlg : Gtk.Dialog
	{
		private static Logger logger = LogManager.GetCurrentClassLogger();
		public bool NewItem;
		private int ItemId;
		private byte[] ImageFile;
		private bool ImageChanged = false;
		private ListStore NomenclatureStore;

		enum NomenclatureCol {
			id,
			selected,
			nomenclature_id,
			nomenclature,
			count
		}

		public CubesDlg()
		{
			this.Build();
			drawCube.SetSizeRequest(250, 250);
			//Создаем таблицу номенклатур
			NomenclatureStore = new ListStore(typeof(long), typeof(bool), typeof(int), typeof(string), typeof(int));

			CellRendererToggle CellSelected = new CellRendererToggle();
			CellSelected.Activatable = true;
			CellSelected.Toggled += onCellSelectToggled;

			Gtk.CellRendererSpin CellCount = new CellRendererSpin();
			CellCount.Editable = true;

			Adjustment adjCost = new Adjustment(0,0,100,1,5,0);
			CellCount.Adjustment = adjCost;
			CellCount.Edited += OnCountSpinEdited;

			treeviewNomenclature.AppendColumn("", CellSelected, "active", (int)NomenclatureCol.selected);
			treeviewNomenclature.AppendColumn("Название", new CellRendererText(), "text", (int)NomenclatureCol.nomenclature);
			treeviewNomenclature.AppendColumn("Количество", CellCount, "text", (int)NomenclatureCol.count);

			treeviewNomenclature.Model = NomenclatureStore;
			treeviewNomenclature.ShowAll();

			//Загрузка списка номенклатур
			string sql = "SELECT id, name FROM nomenclature WHERE type = 'cube'";
			SqliteCommand cmd = new SqliteCommand(sql, (SqliteConnection)QSMain.ConnectionDB);
			using (SqliteDataReader rdr = cmd.ExecuteReader())
			{
				while(rdr.Read())
				{
					NomenclatureStore.AppendValues((long) -1,
						false,
						DBWorks.GetInt(rdr, "id", -1),
						DBWorks.GetString(rdr, "name", ""),
						1
					);
				}
			}

		}

		public void Fill(int id)
		{
			ItemId = id;
			NewItem = false;

			MainClass.StatusMessage(String.Format ("Запрос основы №{0}...", id));
			string sql = "SELECT cubes.* FROM cubes WHERE cubes.id = @id";
			try
			{
				SqliteCommand cmd = new SqliteCommand(sql, (SqliteConnection) QSMain.ConnectionDB);

				cmd.Parameters.AddWithValue("@id", id);

				using(SqliteDataReader rdr = cmd.ExecuteReader())
				{
					rdr.Read();

					labelId.Text = rdr["id"].ToString();
					entryName.Text = rdr["name"].ToString();
					spinH.Value = rdr.GetDouble(rdr.GetOrdinal("height"));
					spinL.Value = rdr.GetDouble(rdr.GetOrdinal("width"));

					if(rdr["image"] != DBNull.Value)
					{
						int size = DBWorks.GetInt(rdr, "image_size", 0);
						ImageFile = new byte[size];
						rdr.GetBytes(rdr.GetOrdinal("image"), 0, ImageFile, 0, size);
						drawCube.QueueDraw();
					}
				}

				sql = "SELECT * FROM cubes_items WHERE cubes_id = @id";
				cmd = new SqliteCommand(sql, (SqliteConnection)QSMain.ConnectionDB);
				cmd.Parameters.AddWithValue("@id", id);
				using(SqliteDataReader rdr = cmd.ExecuteReader())
				{
					while(rdr.Read())
					{
						TreeIter iter;
						int nomenclatureId = rdr.GetInt32(rdr.GetOrdinal("item_id"));
						if(NomenclatureStore.GetIterFirst(out iter))
						{
							do
							{
								if((int) NomenclatureStore.GetValue(iter, (int)NomenclatureCol.nomenclature_id) == nomenclatureId)
								{
									NomenclatureStore.SetValue(iter, (int)NomenclatureCol.id, (object) rdr.GetInt64(rdr.GetOrdinal("id")));
									NomenclatureStore.SetValue(iter, (int)NomenclatureCol.count, rdr.GetInt32(rdr.GetOrdinal("count")));
									NomenclatureStore.SetValue(iter, (int)NomenclatureCol.selected, true);
								}
							}
							while(NomenclatureStore.IterNext(ref iter));
						}
					}
				}

				MainClass.StatusMessage("Ok");
				this.Title = entryName.Text;
			}
			catch (Exception ex)
			{
				logger.ErrorException("Ошибка получения информации о типе шкафа!", ex);
				QSMain.ErrorMessage(this,ex);
			}
			TestCanSave();
		}

		protected	void TestCanSave ()
		{
			bool Nameok = entryName.Text != "";
			bool ImageOk = ImageFile != null;
			buttonOk.Sensitive = Nameok && ImageOk;
		}

		void OnCountSpinEdited (object o, EditedArgs args)
		{
			TreeIter iter;
			if (!NomenclatureStore.GetIterFromString (out iter, args.Path))
				return;
			int count;
			if (int.TryParse (args.NewText, out count)) 
			{
				NomenclatureStore.SetValue (iter, (int)NomenclatureCol.count, count);
			}
		}

		void onCellSelectToggled(object o, ToggledArgs args) 
		{
			TreeIter iter;

			if (NomenclatureStore.GetIter (out iter, new TreePath(args.Path))) 
			{
				bool old = (bool) NomenclatureStore.GetValue(iter, (int)NomenclatureCol.selected);
				NomenclatureStore.SetValue(iter, (int)NomenclatureCol.selected, !old);
			}
		}

		protected void OnButtonOkClicked(object sender, EventArgs e)
		{
			string sql;
			if (NewItem)
			{
				sql = "INSERT INTO cubes (name, width, height) " +
					"VALUES (@name, @width, @height)";
			}
			else
			{
				sql = "UPDATE cubes SET name = @name, width = @width, height = @height WHERE id = @id";
			}
			SqliteTransaction trans = ((SqliteConnection)QSMain.ConnectionDB).BeginTransaction();
			MainClass.StatusMessage("Запись основы...");
			try
			{
				SqliteCommand cmd = new SqliteCommand(sql, (SqliteConnection)QSMain.ConnectionDB, trans);

				cmd.Parameters.AddWithValue("@id", ItemId);
				cmd.Parameters.AddWithValue("@name", entryName.Text);
				cmd.Parameters.AddWithValue("@width", spinL.ValueAsInt);
				cmd.Parameters.AddWithValue("@height", spinH.ValueAsInt);

				cmd.ExecuteNonQuery();

				if(NewItem)
				{
					sql = @"select last_insert_rowid()";
					cmd = new SqliteCommand(sql, (SqliteConnection)QSMain.ConnectionDB, trans);
					ItemId = Convert.ToInt32(cmd.ExecuteScalar());

					sql = "UPDATE cubes SET ordinal = @id WHERE id = @id";
					cmd = new SqliteCommand(sql, (SqliteConnection)QSMain.ConnectionDB, trans);
					cmd.Parameters.AddWithValue("@id", ItemId);
					cmd.ExecuteNonQuery();
				}

				// Запись Номенклатур
				foreach(object[] nomenclature in NomenclatureStore)
				{
					if((bool) nomenclature[(int)NomenclatureCol.selected])
					{
						if((long) nomenclature[(int)NomenclatureCol.id] > 0)
							sql = "UPDATE cubes_items SET count = @count WHERE id = @id";
						else
							sql = "INSERT INTO cubes_items (cubes_id, item_id, count) VALUES (@cubes_id, @item_id, @count)";

						cmd = new SqliteCommand(sql, (SqliteConnection)QSMain.ConnectionDB, trans);
						cmd.Parameters.AddWithValue("@cubes_id", ItemId);
						cmd.Parameters.AddWithValue("@id", nomenclature[(int)NomenclatureCol.id]);
						cmd.Parameters.AddWithValue("@item_id", nomenclature[(int)NomenclatureCol.nomenclature_id]);
						cmd.Parameters.AddWithValue("@count", nomenclature[(int)NomenclatureCol.count]);
						cmd.ExecuteNonQuery();
					}
					else if((long) nomenclature[(int)NomenclatureCol.id] > 0)
					{
						sql = "DELETE FROM cubes_items WHERE cubes_id = @cubes_id AND item_id = @item_id";

						cmd = new SqliteCommand(sql, (SqliteConnection)QSMain.ConnectionDB, trans);
						cmd.Parameters.AddWithValue("@cubes_id", ItemId);
						cmd.Parameters.AddWithValue("@item_id", nomenclature[(int)NomenclatureCol.nomenclature_id]);
						cmd.ExecuteNonQuery();
					}
				}

				if(ImageChanged)
				{
					if(NewItem)
					{
						sql = @"select last_insert_rowid()";
						cmd = new SqliteCommand(sql, (SqliteConnection)QSMain.ConnectionDB, trans);
						ItemId = Convert.ToInt32(cmd.ExecuteScalar());
					}

					sql = "UPDATE cubes SET image_size = @image_size, image = @image WHERE id = @id";
					cmd = new SqliteCommand(sql, (SqliteConnection)QSMain.ConnectionDB, trans);
					cmd.Parameters.AddWithValue("@id", ItemId);
					cmd.Parameters.AddWithValue("@image_size", ImageFile.Length);
					cmd.Parameters.AddWithValue("@image", ImageFile);
					cmd.ExecuteNonQuery();
				}

				trans.Commit();
				MainClass.StatusMessage("Ok");
				Respond(Gtk.ResponseType.Ok);
			}
			catch (Exception ex)
			{
				trans.Rollback();
				logger.ErrorException("Ошибка записи основы!", ex);
				QSMain.ErrorMessage(this, ex);
			}
		}

		protected void OnButtonLoadImageClicked(object sender, EventArgs e)
		{
			FileChooserDialog Chooser = new FileChooserDialog("Выберите svg для загрузки...",
				this,
				FileChooserAction.Open,
				"Отмена", ResponseType.Cancel,
				"Загрузить", ResponseType.Accept );

			FileFilter Filter = new FileFilter();
			Filter.Name = "SVG изображение";
			Filter.AddMimeType("image/svg+xml");
			Filter.AddPattern("*.svg");
			Chooser.AddFilter(Filter);

			Filter = new FileFilter();
			Filter.Name = "Все файлы";
			Filter.AddPattern("*.*");
			Chooser.AddFilter(Filter);

			if((ResponseType) Chooser.Run () == ResponseType.Accept)
			{
				Chooser.Hide();
				MainClass.StatusMessage("Загрузка изображения куба...");
				if(entryName.Text == "")
				{
					entryName.Text = System.IO.Path.GetFileNameWithoutExtension(Chooser.Filename);
				}
				using (FileStream fs = new FileStream(Chooser.Filename, FileMode.Open, FileAccess.Read))
				{
					using (MemoryStream ms = new MemoryStream())
					{
						fs.CopyTo(ms);
						ImageFile = ms.ToArray();
					}
				}
				ImageChanged = true;
				drawCube.QueueDraw();
				MainClass.StatusMessage("Ok");
			}
			Chooser.Destroy ();
			TestCanSave();
		}


		protected void OnDrawCubeExposeEvent(object o, ExposeEventArgs args)
		{
			if (ImageFile == null)
				return;
			logger.Debug("Render Cairo");
			using (Context cr = Gdk.CairoHelper.Create (args.Event.Window)) 
			{
				int MaxWidth = args.Event.Area.Width;
				int MaxHeight = args.Event.Area.Height;
				logger.Debug("Image widget size W: {0} H: {1}", MaxWidth, MaxHeight);

				Rsvg.Handle svg = new Rsvg.Handle(ImageFile);
				double vratio = (double) MaxHeight / svg.Dimensions.Height;
				double hratio = (double) MaxWidth / svg.Dimensions.Width;
				double ratio = Math.Min(vratio, hratio);
				cr.Scale(ratio, ratio);
				svg.RenderCairo(cr);
			}
		}
	}
}

