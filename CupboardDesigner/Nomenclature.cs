using System;
using System.IO;
using QSProjectsLib;
using Mono.Data.Sqlite;
using NLog;
using Gtk;
using Cairo;

namespace CupboardDesigner
{
	public partial class Nomenclature : Gtk.Dialog
	{
		private static Logger logger = LogManager.GetCurrentClassLogger();
		public bool NewItem;
		private int ItemId;
		private byte[] ImageFile;
		private bool ImageChanged = false;

		internal enum NomType {cube, construct};

		public Nomenclature()
		{
			this.Build();

			OnComboTypeChanged(comboType, EventArgs.Empty);
			drawCube.SetSizeRequest(250, 250);
		}

		protected void OnComboTypeChanged(object sender, EventArgs e)
		{
			buttonLoadImage.Sensitive = comboType.Active == 0;
			int SizeStep = (comboType.Active == 0) ? 400 : 1;
			spinH.Adjustment.StepIncrement = spinL.Adjustment.StepIncrement = spinW.Adjustment.StepIncrement = SizeStep;
			checkPlusH.Visible = checkPlusL.Visible = comboType.Active == 1;
		}

		public void Fill(int id)
		{
			ItemId = id;
			NewItem = false;

			MainClass.StatusMessage(String.Format ("Запрос номеклатуры №{0}...", id));
			string sql = "SELECT nomenclature.* FROM nomenclature WHERE nomenclature.id = @id";
			try
			{
				SqliteCommand cmd = new SqliteCommand(sql, (SqliteConnection) QSMain.ConnectionDB);

				cmd.Parameters.AddWithValue("@id", id);

				using(SqliteDataReader rdr = cmd.ExecuteReader())
				{
					rdr.Read();

					labelId.Text = rdr["id"].ToString();
					entryName.Text = rdr["name"].ToString();
					comboType.Active = (int) Enum.Parse(typeof(NomType), rdr["type"].ToString());
					entryArticle.Text = DBWorks.GetString(rdr, "article", "");
					entryDescription.Text = DBWorks.GetString(rdr, "description", "");
					spinH.Value = DBWorks.GetInt(rdr, "height", 0);
					spinL.Value = DBWorks.GetInt(rdr, "lenght", 0);
					spinW.Value = DBWorks.GetInt(rdr, "widht", 0);
					if(comboType.Active == (int) NomType.construct)
					{
						checkPlusH.Active = DBWorks.GetBoolean(rdr, "plush", false);
						checkPlusL.Active = DBWorks.GetBoolean(rdr, "plusl", false);
					}

					if(rdr["image"] != DBNull.Value)
					{
						int size = DBWorks.GetInt(rdr, "image_size", 0);
						ImageFile = new byte[size];
						rdr.GetBytes(rdr.GetOrdinal("image"), 0, ImageFile, 0, size);
						drawCube.QueueDraw();
					}
				}

				MainClass.StatusMessage("Ok");
				this.Title = entryName.Text;
			}
			catch (Exception ex)
			{
				MainClass.StatusMessage("Ошибка получения информации о номенклатуре!");
				logger.Error(ex.ToString());
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
			if(NewItem)
			{
				sql = "INSERT INTO nomenclature (type, article, name, description, widht, lenght, height, plusl, plush) " +
					"VALUES (@type, @article, @name, @description, @widht, @lenght, @height, @plusl, @plush)";
			}
			else
			{
				sql = "UPDATE nomenclature SET type = @type, article = @article, name = @name, description = @description, " +
					"widht = @widht, lenght = @lenght, height = @height, plusl = @plusl, plush = @plush WHERE id = @id";
			}
			MainClass.StatusMessage("Запись номенклатуры...");
			SqliteTransaction trans = (SqliteTransaction)QSMain.ConnectionDB.BeginTransaction();
			try 
			{
				SqliteCommand cmd = new SqliteCommand(sql, (SqliteConnection)QSMain.ConnectionDB, trans);

				cmd.Parameters.AddWithValue("@id", ItemId);
				cmd.Parameters.AddWithValue("@name", entryName.Text);
				cmd.Parameters.AddWithValue("@type", ((NomType) comboType.Active).ToString() );
				cmd.Parameters.AddWithValue("@article", DBWorks.ValueOrNull(entryArticle.Text != "", entryArticle.Text));
				cmd.Parameters.AddWithValue("@description", DBWorks.ValueOrNull(entryDescription.Text != "", entryDescription.Text));
				cmd.Parameters.AddWithValue("@widht", DBWorks.ValueOrNull(spinW.ValueAsInt > 0, spinW.ValueAsInt));
				cmd.Parameters.AddWithValue("@lenght", DBWorks.ValueOrNull(spinL.ValueAsInt > 0, spinL.ValueAsInt));
				cmd.Parameters.AddWithValue("@height", DBWorks.ValueOrNull(spinH.ValueAsInt > 0, spinH.ValueAsInt));
				cmd.Parameters.AddWithValue("@plush", DBWorks.ValueOrNull(comboType.Active == (int)NomType.construct , checkPlusH.Active));
				cmd.Parameters.AddWithValue("@plusl", DBWorks.ValueOrNull(comboType.Active == (int)NomType.construct, checkPlusL.Active));

				cmd.ExecuteNonQuery();

				if(NewItem)
				{
					sql = @"select last_insert_rowid()";
					cmd = new SqliteCommand(sql, (SqliteConnection)QSMain.ConnectionDB, trans);
					ItemId = Convert.ToInt32(cmd.ExecuteScalar());

					sql = "UPDATE nomenclature SET ordinal = @id WHERE id = @id";
					cmd = new SqliteCommand(sql, (SqliteConnection)QSMain.ConnectionDB, trans);
					cmd.Parameters.AddWithValue("@id", ItemId);
					cmd.ExecuteNonQuery();
				}

				if(ImageChanged)
				{
					sql = "UPDATE nomenclature SET image_size = @image_size, image = @image WHERE id = @id";
					cmd = new SqliteCommand(sql, (SqliteConnection)QSMain.ConnectionDB, trans);
					cmd.Parameters.AddWithValue("@id", ItemId);
					cmd.Parameters.AddWithValue("@image_size", ImageFile.Length);
					cmd.Parameters.AddWithValue("@image", ImageFile);
					cmd.ExecuteNonQuery();
				}

				trans.Commit();
				MainClass.StatusMessage("Ok");
				Respond (Gtk.ResponseType.Ok);
			} 
			catch (Exception ex) 
			{
				trans.Rollback();
				MainClass.StatusMessage("Ошибка записи номенклатуры!");
				logger.Error(ex.ToString());
				QSMain.ErrorMessage(this,ex);
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

