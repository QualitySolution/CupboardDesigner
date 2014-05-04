using System;
using NLog;
using Mono.Data.Sqlite;
using QSProjectsLib;
using System.Collections.Generic;
using Gtk;

namespace CupboardDesigner
{
	public partial class Basis : Gtk.Dialog
	{
		private static Logger logger = LogManager.GetCurrentClassLogger();
		public bool NewItem;
		private int ItemId;
		private Dictionary<string, bool> NomenclatureInDB;

		public Basis()
		{
			this.Build();

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
				MainClass.StatusMessage("Ошибка получения информации о типе шкафа!");
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
			if (NewItem)
			{
				sql = "INSERT INTO basis (name) " +
				"VALUES (@name)";
			}
			else
			{
				sql = "UPDATE basis SET name = @name WHERE id = @id";
			}
			SqliteTransaction trans = ((SqliteConnection)QSMain.ConnectionDB).BeginTransaction();
			MainClass.StatusMessage("Запись основы...");
			try
			{
				SqliteCommand cmd = new SqliteCommand(sql, (SqliteConnection)QSMain.ConnectionDB, trans);

				cmd.Parameters.AddWithValue("@id", ItemId);
				cmd.Parameters.AddWithValue("@name", entryName.Text);

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

				trans.Commit();
				MainClass.StatusMessage("Ok");
				Respond(Gtk.ResponseType.Ok);
			}
			catch (Exception ex)
			{
				trans.Rollback();
				MainClass.StatusMessage("Ошибка записи основы!");
				logger.Error(ex.ToString());
				QSMain.ErrorMessage(this, ex);
			}
		}
	}
}

