using System;
using Gtk;
using CupboardDesigner;
using QSProjectsLib;
using NLog;
using QSSupportLib;
using System.IO;
using Mono.Data.Sqlite;

public partial class MainWindow: Gtk.Window
{
	private static Logger logger = LogManager.GetCurrentClassLogger();

	public MainWindow() : base(Gtk.WindowType.Toplevel)
	{
		Build();

		System.Reflection.AssemblyProductAttribute Product = (System.Reflection.AssemblyProductAttribute)
			System.Reflection.Assembly.GetExecutingAssembly().GetCustomAttributes(typeof(System.Reflection.AssemblyProductAttribute), false)[0];
		this.Title = Product.Product;

		MainClass.StatusBarLabel = labelStatus;
		QSMain.MakeNewStatusTargetForNlog("StatusMessage", "CupboardDesigner.MainClass, CupboardDesigner");
		Reference.RunReferenceItemDlg += OnRunReferenceItemDialog;
		QSMain.ReferenceUpdated += OnReferenceUpdate;

		PrerareOrders();
		SetAdminMode(false); 
		MainSupport.LoadBaseParameters ();
		if (CheckBaseVersion.Check ()) { //Проверяем версию базы
			if (CheckBaseVersion.ResultFlags.HasFlag (CheckBaseResult.BaseVersionLess)) {
				logger.Info ("Используется база старой версии. Обновляем...");
				SqliteTransaction trans = ((SqliteConnection)QSMain.ConnectionDB).BeginTransaction ();
				try {
					string sql;
					using (Stream stream = System.Reflection.Assembly.GetExecutingAssembly ().GetManifestResourceStream ("CupboardDesigner.Scripts.UpdateSchema.sql")) {
						StreamReader reader = new StreamReader (stream);
						sql = reader.ReadToEnd ();
					}

					SqliteCommand cmd = new SqliteCommand (sql, (SqliteConnection)QSMain.ConnectionDB, trans);
					cmd.ExecuteNonQuery ();
					trans.Commit ();
					logger.Info ("Обновление базы ОК.");
				} catch (Exception e) {
					trans.Rollback ();
					QSMain.ErrorMessageWithLog (this, "Обновление базы закончилось с ошибкой!", logger, e);
				}
			}
		}
		else
			logger.Info ("Обновление базы не требуется.");
	}

	protected void OnReferenceUpdate(object sender, QSMain.ReferenceUpdatedEventArgs e)
	{
		/*	switch (e.ReferenceTable) {
		case "doc_types":
			ComboWorks.ComboFillReference (comboDocType, "doc_types", 0);
		break;
		} */
	}

	protected void OnRunReferenceItemDialog(object sender, Reference.RunReferenceItemDlgEventArgs e)
	{
		ResponseType Result;
		switch (e.TableName)
		{
			case "nomenclature":
				Nomenclature ItemNomen = new Nomenclature();
				if(e.NewItem)
					ItemNomen.NewItem = true;
				else 
					ItemNomen.Fill(e.ItemId);
				ItemNomen.Show();
				Result = (ResponseType)ItemNomen.Run();
				ItemNomen.Destroy();
				break;
			case "basis":
				Basis BasisEdit = new Basis();
				if(e.NewItem)
					BasisEdit.NewItem = true;
				else 
					BasisEdit.Fill(e.ItemId);
				BasisEdit.Show();
				Result = (ResponseType)BasisEdit.Run();
				BasisEdit.Destroy();
				break;
			case "cubes":
				CubesDlg CubesEdit = new CubesDlg();
				if(e.NewItem)
					CubesEdit.NewItem = true;
				else 
				CubesEdit.Fill(e.ItemId);
				CubesEdit.Show();
				Result = (ResponseType)CubesEdit.Run();
				CubesEdit.Destroy();
				break;
			case "exhibition":
				Exhibition ExhibitionEdit = new Exhibition();
				if(!e.NewItem)
					ExhibitionEdit.Fill(e.ItemId);
				ExhibitionEdit.Show();
				Result = (ResponseType)ExhibitionEdit.Run();
				ExhibitionEdit.Destroy();
				break;
			default:
				logger.Warn("Диалог для справочника {0} не найден.", e.TableName);
				Result = ResponseType.None;
				break;
		}
		e.Result = Result;
	}

	protected void SetAdminMode(bool admin)
	{
		ActionPassword.Sensitive = Action9.Sensitive = Action11.Sensitive = Action10.Sensitive 
			= Action12.Sensitive = Action13.Sensitive = Action15.Sensitive = admin;
	}

	protected void OnDeleteEvent(object sender, DeleteEventArgs a)
	{
		Application.Quit();
		a.RetVal = true;
	}

	protected void OnAction5Activated(object sender, EventArgs e)
	{
		Reference winref = new Reference();
		winref.SqlSelect = "SELECT id, name, phone FROM @tablename ORDER BY ordinal";
		winref.OrdinalField = "ordinal";
		winref.Columns.Add(new Reference.ColumnInfo("Телефон", "{2}"));
		winref.SetMode(false,false,true,true,true);
		winref.FillList("exhibition","выставка", "Выставки");
		winref.Show();
		winref.Run();
		winref.Destroy();
	}

	protected void OnAction8Activated(object sender, EventArgs e)
	{
		Reference winref = new Reference();
		winref.SqlSelect = "SELECT id, name FROM @tablename ORDER BY ordinal";
		winref.OrdinalField = "ordinal";
		winref.SetMode(true,false,true,true,true);
		winref.FillList("materials","отделка куба", "Отделка кубов");
		winref.Show();
		winref.Run();
		winref.Destroy();
	}

	protected void OnAction7Activated(object sender, EventArgs e)
	{
		Reference winref = new Reference();
		winref.SqlSelect = "SELECT id, name FROM @tablename ORDER BY ordinal";
		winref.OrdinalField = "ordinal";
		winref.SetMode(true,false,true,true,true);
		winref.FillList("facing","отделка фасада", "Отделка фасадов");
		winref.Show();
		winref.Run();
		winref.Destroy();
	}

	protected void OnAboutActionActivated(object sender, EventArgs e)
	{
		QSMain.RunAboutDialog();
	}

	protected void OnAction9Activated(object sender, EventArgs e)
	{
		Reference winref = new Reference();
		winref.SqlSelect = "SELECT id, name, price FROM @tablename ORDER BY ordinal";
		winref.OrdinalField = "ordinal";
		winref.Columns.Add(new Reference.ColumnInfo("Цена", "{2:C}", false));
		winref.SetMode(false,false,true,true,true);
		winref.FillList("nomenclature", "номенклатура", "Номенклатура");
		winref.Show();
		winref.Run();
		winref.Destroy();
	}

	protected void OnAction4Activated(object sender, EventArgs e)
	{
		Reference winref = new Reference();
		winref.SqlSelect = "SELECT id, name FROM @tablename ORDER BY ordinal";
		winref.OrdinalField = "ordinal";
		winref.SetMode(false,false,true,true,true);
		winref.FillList("basis", "каркас", "Виды каркасов");
		winref.Show();
		winref.Run();
		winref.Destroy();
	}
		
	protected void OnDialogAuthenticationActionToggled(object sender, EventArgs e)
	{
		if(dialogAuthenticationAction.Active && MainClass.Parameters.All["admin_password"] != "")
		{
			AdminModePassword WinPass = new AdminModePassword();
			WinPass.Show();
			WinPass.Run();
			if(MainClass.Parameters.All["admin_password"] != WinPass.Password)
			{
				dialogAuthenticationAction.Active = false;
			}
			WinPass.Destroy();
		}
		SetAdminMode(dialogAuthenticationAction.Active);
	}
		
	protected void OnQuitActionActivated(object sender, EventArgs e)
	{
		Application.Quit();
	}

	protected void OnActionPasswordActivated(object sender, EventArgs e)
	{
		ChangePassword WinPass = new ChangePassword();
		WinPass.WorkMode = ChangePassword.Mode.Manual;
		WinPass.CanSetEmptyPassword = true;
		WinPass.Show();
		if(WinPass.Run() == (int) Gtk.ResponseType.Ok)
		{
			MainClass.Parameters.UpdateParameter(QSMain.ConnectionDB, "admin_password", WinPass.NewPassword);
		}
		WinPass.Destroy();
	}
	protected void OnAction15Activated (object sender, EventArgs e)
	{
		Reference winref = new Reference();
		winref.SqlSelect = "SELECT id, name FROM @tablename ORDER BY ordinal";
		winref.OrdinalField = "ordinal";
		winref.SetMode(false,false,true,true,true);
		winref.FillList("cubes", "куб", "Виды кубов");
		winref.Show();
		winref.Run();
		winref.Destroy();
	}
}
