using System;
using NLog;
using QSProjectsLib;
using Mono.Data.Sqlite;

namespace CupboardDesigner
{
	public partial class Exhibition : Gtk.Dialog
	{
		private static Logger logger = LogManager.GetCurrentClassLogger();
		private bool NewItem = true;
		private int ItemId;

		public Exhibition()
		{
			this.Build();
		}

		public void Fill(int id)
		{
			ItemId = id;
			NewItem = false;

			MainClass.StatusMessage(String.Format ("Запрос выставки №{0}...", id));
			string sql = "SELECT exhibition.* FROM exhibition WHERE exhibition.id = @id";
			try
			{
				SqliteCommand cmd = new SqliteCommand(sql, (SqliteConnection) QSMain.ConnectionDB);

				cmd.Parameters.AddWithValue("@id", id);

				using(SqliteDataReader rdr = cmd.ExecuteReader())
				{
					rdr.Read();

					labelID.Text = rdr["id"].ToString();
					entryName.Text = rdr["name"].ToString();
					entryPhone1.Text = DBWorks.GetString(rdr, "phone", "");
					entryPhone2.Text = DBWorks.GetString(rdr, "phone2", "");
					textAddress.Buffer.Text = DBWorks.GetString(rdr, "address", "");
				}

				MainClass.StatusMessage("Ok");
				this.Title = entryName.Text;
			}
			catch (Exception ex)
			{
				QSMain.ErrorMessageWithLog(this, "Ошибка получения информации о номенклатуре!", logger, ex);
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
				sql = "INSERT INTO exhibition (name, phone, phone2, address) " +
					"VALUES (@name, @phone, @phone2, @address)";
			}
			else
			{
				sql = "UPDATE exhibition SET name = @name, phone = @phone, phone = @phone, " +
					"address = @address WHERE id = @id";
			}
			MainClass.StatusMessage("Запись номенклатуры...");
			SqliteTransaction trans = (SqliteTransaction)QSMain.ConnectionDB.BeginTransaction();
			try 
			{
				SqliteCommand cmd = new SqliteCommand(sql, (SqliteConnection)QSMain.ConnectionDB, trans);

				cmd.Parameters.AddWithValue("@id", ItemId);
				cmd.Parameters.AddWithValue("@name", entryName.Text);
				cmd.Parameters.AddWithValue("@phone", DBWorks.ValueOrNull(entryPhone1.Text != "", entryPhone1.Text));
				cmd.Parameters.AddWithValue("@phone2", DBWorks.ValueOrNull(entryPhone2.Text != "", entryPhone2.Text));
				cmd.Parameters.AddWithValue("@address", DBWorks.ValueOrNull(textAddress.Buffer.Text != "", textAddress.Buffer.Text));
				cmd.ExecuteNonQuery();

				if(NewItem)
				{
					sql = @"select last_insert_rowid()";
					cmd = new SqliteCommand(sql, (SqliteConnection)QSMain.ConnectionDB, trans);
					ItemId = Convert.ToInt32(cmd.ExecuteScalar());

					sql = "UPDATE exhibition SET ordinal = @id WHERE id = @id";
					cmd = new SqliteCommand(sql, (SqliteConnection)QSMain.ConnectionDB, trans);
					cmd.Parameters.AddWithValue("@id", ItemId);
					cmd.ExecuteNonQuery();
				}

				trans.Commit();
				MainClass.StatusMessage("Ok");
				Respond (Gtk.ResponseType.Ok);
			} 
			catch (Exception ex) 
			{
				trans.Rollback();
				QSMain.ErrorMessageWithLog(this, "Ошибка записи выставки", logger, ex);
			}

		}
	}
}

