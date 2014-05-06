using System;
using System.Data.Common;
using Gtk;
using NLog;
using QSProjectsLib;
using System.Collections.Generic;
using Nini.Config;

namespace CupboardDesigner
{
	class MainClass
	{
		public static Label StatusBarLabel;
		public static MainWindow MainWin;
		public static QSSupportLib.BaseParam Parameters;
		private static Logger logger = LogManager.GetCurrentClassLogger();

		public static void Main(string[] args)
		{
			Application.Init();
			AppDomain.CurrentDomain.UnhandledException += delegate(object sender, UnhandledExceptionEventArgs e) 
			{
				QSMain.ErrorMessage(MainWin, (Exception) e.ExceptionObject);
			};
			CreateProjectParam();
			//Настраиваем общую билиотеку
			QSMain.NewStatusText += delegate(object sender, QSProjectsLib.QSMain.NewStatusTextEventArgs e) 
			{
				StatusMessage (e.NewText);
			};
			CreateConnection();

			MainWin = new MainWindow();
			MainWin.Show();
			Application.Run();
		}

		static void CreateProjectParam()
		{
			QSMain.AdminFieldName = "admin";
			QSMain.ProjectPermission = new Dictionary<string, UserPermission>();
			//QSMain.ProjectPermission.Add("edit_slips", new UserPermission("edit_slips", "Изменение кассы задним числом",
			//"Пользователь может изменять или добавлять кассовые документы задним числом."));

			QSMain.User = new UserInfo();

			//Параметры удаления
			Dictionary<string, TableInfo> Tables = new Dictionary<string, TableInfo>();
			QSMain.ProjectTables = Tables;
			TableInfo PrepareTable;

			PrepareTable = new TableInfo();
			PrepareTable.ObjectsName = "Пользователи";
			PrepareTable.ObjectName = "пользователя"; 
			PrepareTable.SqlSelect = "SELECT name, id FROM users ";
			PrepareTable.DisplayString = "{0}";
			PrepareTable.PrimaryKey = new TableInfo.PrimaryKeys("id");
			Tables.Add("users", PrepareTable);

			PrepareTable = new TableInfo();
			PrepareTable.ObjectsName = "Услуги";
			PrepareTable.ObjectName = "услуга";
			PrepareTable.SqlSelect = "SELECT name , id FROM services ";
			PrepareTable.DisplayString = "Услуга {0}";
			PrepareTable.PrimaryKey = new  TableInfo.PrimaryKeys("id");
			PrepareTable.DeleteItems.Add("order_pays", 
				new TableInfo.DeleteDependenceItem("WHERE service_id = @id ", "", "@id"));
			Tables.Add("services", PrepareTable);
		}

		private static void CreateConnection()
		{
			string ConfigFileName = System.Reflection.Assembly.GetExecutingAssembly().GetName().Name.ToString() + ".ini";
			string DataBase = "Cupboard.db3";

			string configfile = System.IO.Path.Combine (Environment.GetFolderPath (Environment.SpecialFolder.ApplicationData), ConfigFileName);
			IniConfigSource Configsource;
			try
			{
				Configsource = new IniConfigSource(configfile);
				Configsource.Reload();
				DataBase = Configsource.Configs["Login"].Get("DataBase", DataBase);
			} 
			catch (Exception ex)
			{
				logger.Warn(ex.Message);
				logger.Warn("Конфигурационный фаил не найден. Создаем новый.");
				Configsource = new IniConfigSource();

				IConfig config = Configsource.AddConfig("Login");
				config.Set("DataBase", DataBase);
				Configsource.Save(configfile);
			}

			//Создаем соедиение
			try
			{
				QSMain.DBMS = QSMain.DataProviders.Factory;
				QSMain.ProviderDB = DbProviderFactories.GetFactory("Mono.Data.Sqlite");
				QSMain.ConnectionDB = QSMain.ProviderDB.CreateConnection();
				QSMain.ConnectionDB.ConnectionString = String.Format("Data Source={0};Version=3;", DataBase);
				QSMain.ConnectionDB.Open();
				logger.Info("Открытие базы данных прошло успешно.");
				logger.Info("Читаем параметры");
				Parameters = new QSSupportLib.BaseParam(QSMain.ConnectionDB);
			}
			catch (Exception ex)
			{
				string Error = String.Format("Не получилось открыть базу данных {0}", DataBase);
				logger.FatalException(Error, ex);
				QSMain.ErrorMessage(null, ex);
				Environment.Exit(1);
			}

		}

		public static void StatusMessage(string message)
		{
			StatusBarLabel.Text = message;
			logger.Info(message);
			while (GLib.MainContext.Pending())
			{
				Gtk.Main.Iteration();
			}
		}

	}
}
