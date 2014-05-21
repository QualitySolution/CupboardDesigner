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
	public partial class Basis : Gtk.Dialog
	{
		private static Logger logger = LogManager.GetCurrentClassLogger();
		public bool NewItem;
		private int ItemId;
		private Dictionary<string, bool> NomenclatureInDB;
		private SVGHelper ImageHelper;
		private bool ImageChanged = false;

		public Basis()
		{
			this.Build();
			drawBasis.SetSizeRequest(250, 250);
			//Загрузка списка номенклатур
			string sql = "SELECT id, name FROM nomenclature WHERE type = 'construct'";
			SqliteCommand cmd = new SqliteCommand(sql, (SqliteConnection)QSMain.ConnectionDB);
			NomenclatureInDB = new Dictionary<string, bool>();
			using (SqliteDataReader rdr = cmd.ExecuteReader())
			{
				while(rdr.Read())
				{
					checksNom.AddCheckButton(rdr["id"].ToString(), rdr["name"].ToString());
					NomenclatureInDB.Add(rdr["id"].ToString(), false);
				}
			}

		}

		public void Fill(int id)
		{
			ItemId = id;
			NewItem = false;

			MainClass.StatusMessage(String.Format ("Запрос основы №{0}...", id));
			string sql = "SELECT basis.* FROM basis WHERE basis.id = @id";
			try
			{
				SqliteCommand cmd = new SqliteCommand(sql, (SqliteConnection) QSMain.ConnectionDB);

				cmd.Parameters.AddWithValue("@id", id);

				using(SqliteDataReader rdr = cmd.ExecuteReader())
				{
					rdr.Read();

					labelId.Text = rdr["id"].ToString();
					entryName.Text = rdr["name"].ToString();
					spinH.Value = rdr.GetDouble(rdr.GetOrdinal("delta_h"));
					spinL.Value = rdr.GetDouble(rdr.GetOrdinal("delta_l"));

					if(rdr["image"] != DBNull.Value)
					{
						int size = DBWorks.GetInt(rdr, "image_size", 0);
						byte[] ImageFile = new byte[size];
						rdr.GetBytes(rdr.GetOrdinal("image"), 0, ImageFile, 0, size);
						ImageHelper = new SVGHelper();
						ImageHelper.LoadImage(ImageFile);
						drawBasis.QueueDraw();
					}
				}

				sql = "SELECT * FROM basis_items WHERE basis_id = @id";
				cmd = new SqliteCommand(sql, (SqliteConnection)QSMain.ConnectionDB);
				cmd.Parameters.AddWithValue("@id", id);
				using(SqliteDataReader rdr = cmd.ExecuteReader())
				{
					while(rdr.Read())
					{
						checksNom.CheckButtons[rdr["item_id"].ToString()].Active = true;
						NomenclatureInDB[rdr["item_id"].ToString()] = true;
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
			buttonOk.Sensitive = Nameok;
		}

		protected void OnButtonOkClicked(object sender, EventArgs e)
		{
			string sql;
			if (NewItem)
			{
				sql = "INSERT INTO basis (name, delta_l, delta_h) " +
					"VALUES (@name, @delta_l, @delta_h)";
			}
			else
			{
				sql = "UPDATE basis SET name = @name, delta_l = @delta_l, delta_h = @delta_h WHERE id = @id";
			}
			SqliteTransaction trans = ((SqliteConnection)QSMain.ConnectionDB).BeginTransaction();
			MainClass.StatusMessage("Запись основы...");
			try
			{
				SqliteCommand cmd = new SqliteCommand(sql, (SqliteConnection)QSMain.ConnectionDB, trans);

				cmd.Parameters.AddWithValue("@id", ItemId);
				cmd.Parameters.AddWithValue("@name", entryName.Text);
				cmd.Parameters.AddWithValue("@delta_l", spinL.ValueAsInt);
				cmd.Parameters.AddWithValue("@delta_h", spinH.ValueAsInt);

				cmd.ExecuteNonQuery();

				if(NewItem)
				{
					sql = @"select last_insert_rowid()";
					cmd = new SqliteCommand(sql, (SqliteConnection)QSMain.ConnectionDB, trans);
					ItemId = Convert.ToInt32(cmd.ExecuteScalar());
				}

				// Запись Номенклатур
				foreach(KeyValuePair<string, CheckButton> pair in checksNom.CheckButtons)
				{
					if(pair.Value.Active && !NomenclatureInDB[pair.Key])
					{
						sql = "INSERT INTO basis_items (basis_id, item_id) VALUES (@basis_id, @item_id)";

						cmd = new SqliteCommand(sql, (SqliteConnection)QSMain.ConnectionDB, trans);
						cmd.Parameters.AddWithValue("@basis_id", ItemId);
						cmd.Parameters.AddWithValue("@item_id", pair.Key);
						cmd.ExecuteNonQuery();
					}
					if(!pair.Value.Active && NomenclatureInDB[pair.Key])
					{
						sql = "DELETE FROM basis_items WHERE basis_id = @basis_id AND item_id = @item_id";

						cmd = new SqliteCommand(sql, (SqliteConnection)QSMain.ConnectionDB, trans);
						cmd.Parameters.AddWithValue("@basis_id", ItemId);
						cmd.Parameters.AddWithValue("@item_id", pair.Key);
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

					sql = "UPDATE basis SET image_size = @image_size, image = @image WHERE id = @id";
					cmd = new SqliteCommand(sql, (SqliteConnection)QSMain.ConnectionDB, trans);
					cmd.Parameters.AddWithValue("@id", ItemId);
					cmd.Parameters.AddWithValue("@image_size", ImageHelper.OriginalFile.Length);
					cmd.Parameters.AddWithValue("@image", ImageHelper.OriginalFile);
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
			Filter.AddMimeType("image/svg+xml");
			Filter.Name = "SVG изображение";
			Chooser.AddFilter(Filter);

			if((ResponseType) Chooser.Run () == ResponseType.Accept)
			{
				Chooser.Hide();
				MainClass.StatusMessage("Загрузка изображения основы...");
				if(entryName.Text == "")
				{
					entryName.Text = System.IO.Path.GetFileNameWithoutExtension(Chooser.Filename);
				}
				using (FileStream fs = new FileStream(Chooser.Filename, FileMode.Open, FileAccess.Read))
				{
					using (MemoryStream ms = new MemoryStream())
					{
						fs.CopyTo(ms);
						SVGHelper FrameTest = new SVGHelper();
						byte[] NewFile = ms.ToArray();
						if(FrameTest.LoadImage(NewFile))
						{
							ImageHelper = FrameTest;
							ImageHelper.PrepairForDBSave();
							ImageChanged = true;
						}
						else
						{
							MessageDialog md = new MessageDialog ( this, DialogFlags.DestroyWithParent,
								MessageType.Warning, 
								ButtonsType.Ok, 
								"Не удалось загрузить изображение основы. Для успешной загрузки формат файла должен быть svg. " +
								"В файле изображения должен быть прямоугольник(rect) с id=framework указывающий положение рамки в которую вставлюятся кубы. " +
								"Размерность исходного изображения должна быть 1 куб.");
							md.Run ();
							md.Destroy();
						}
					}
				}
				drawBasis.QueueDraw();
				MainClass.StatusMessage("Ok");
			}
			Chooser.Destroy ();
		}

		protected void OnDrawBasisExposeEvent(object o, ExposeEventArgs args)
		{
			if (ImageHelper == null)
				return;
			logger.Debug("Render Cairo");
			using (Context cr = Gdk.CairoHelper.Create (args.Event.Window)) 
			{
				int MaxWidth, MaxHeight;
				args.Event.Window.GetSize(out MaxWidth, out MaxHeight);
				logger.Debug("Image widget size W: {0} H: {1}", MaxWidth, MaxHeight);

				int CubeSize = (int)(args.Event.Area.Height / 2.2);

				cr.Translate((MaxWidth - CubeSize) / 2, (MaxHeight - CubeSize) / 2 );
				ImageHelper.DrawBasis(cr, CubeSize);
			}
		}
	}
}

