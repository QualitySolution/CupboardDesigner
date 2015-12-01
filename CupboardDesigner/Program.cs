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

		[STAThread]
		public static void Main(string[] args)
		{
			Application.Init();
			AppDomain.CurrentDomain.UnhandledException += delegate(object sender, UnhandledExceptionEventArgs e) 
			{
				QSMain.ErrorMessage(MainWin, (Exception) e.ExceptionObject);
			};
			CreateProjectParam();

			CreateConnection();

			MainWin = new MainWindow();
			MainWin.Show();
			Application.Run();
		}

		static void CreateProjectParam()
		{
			QSMain.ProjectPermission = new Dictionary<string, UserPermission>();

			QSMain.User = new UserInfo();

			//Параметры удаления
			Dictionary<string, TableInfo> Tables = new Dictionary<string, TableInfo>();
			QSMain.ProjectTables = Tables;
			TableInfo PrepareTable;

			PrepareTable = new TableInfo();
			PrepareTable.ObjectsName = "Каркасы";
			PrepareTable.ObjectName = "каркас"; 
			PrepareTable.SqlSelect = "SELECT name, id FROM basis ";
			PrepareTable.DisplayString = "{0}";
			PrepareTable.PrimaryKey = new TableInfo.PrimaryKeys("id");
			PrepareTable.DeleteItems.Add("orders", 
				new TableInfo.DeleteDependenceItem("WHERE basis_id = @id ", "", "@id"));
			Tables.Add("basis", PrepareTable);

			PrepareTable = new TableInfo();
			PrepareTable.ObjectsName = "Кубы";
			PrepareTable.ObjectName = "куб"; 
			PrepareTable.SqlSelect = "SELECT name, id FROM cubes ";
			PrepareTable.DisplayString = "{0}";
			PrepareTable.PrimaryKey = new TableInfo.PrimaryKeys("id");
			PrepareTable.DeleteItems.Add("order_details", 
				new TableInfo.DeleteDependenceItem("WHERE cube_id = @id ", "", "@id"));
			Tables.Add("cubes", PrepareTable);

			PrepareTable = new TableInfo();
			PrepareTable.ObjectsName = "Кубы в заказах";
			PrepareTable.ObjectName = "куб в заказе";
			PrepareTable.SqlSelect = "SELECT order_details.order_id, order_details.count, cubes.name, order_details.id FROM order_details " +
				"LEFT JOIN cubes ON order_details.cube_id = cubes.id ";
			PrepareTable.DisplayString = "{1} куб(ов) \"{2}\" в заказе №{0}";
			PrepareTable.PrimaryKey = new  TableInfo.PrimaryKeys("id");
			Tables.Add("order_details", PrepareTable);

			PrepareTable = new TableInfo();
			PrepareTable.ObjectsName = "Компоненты каркаса";
			PrepareTable.ObjectName = "компонент каркаса";
			PrepareTable.SqlSelect = "SELECT nomenclature.name, basis.name, basis_items.id FROM basis_items " +
				"LEFT JOIN nomenclature ON nomenclature.id = basis_items.item_id " +
				"LEFT JOIN basis ON basis_items.basis_id = basis.id ";
			PrepareTable.DisplayString = "Компонент {0} в {1}";
			PrepareTable.PrimaryKey = new  TableInfo.PrimaryKeys("id");
			Tables.Add("basis_items", PrepareTable);

			PrepareTable = new TableInfo();
			PrepareTable.ObjectsName = "Номенклатура";
			PrepareTable.ObjectName = "номенклатуру";
			PrepareTable.SqlSelect = "SELECT nomenclature.name, id FROM nomenclature ";
			PrepareTable.DisplayString = "{0}";
			PrepareTable.PrimaryKey = new  TableInfo.PrimaryKeys("id");
			PrepareTable.DeleteItems.Add("basis_items", 
				new TableInfo.DeleteDependenceItem("WHERE item_id = @id ", "", "@id"));
			PrepareTable.DeleteItems.Add("cubes_items", 
				new TableInfo.DeleteDependenceItem("WHERE item_id = @id ", "", "@id"));
			PrepareTable.DeleteItems.Add("order_basis_details", 
				new TableInfo.DeleteDependenceItem("WHERE nomenclature_id = @id ", "", "@id"));
			PrepareTable.DeleteItems.Add("order_cubes_details", 
				new TableInfo.DeleteDependenceItem("WHERE nomenclature_id = @id ", "", "@id"));
			Tables.Add("nomenclature", PrepareTable);

			PrepareTable = new TableInfo();
			PrepareTable.ObjectsName = "Компоненты куба";
			PrepareTable.ObjectName = "Компонент куба";
			PrepareTable.SqlSelect = "SELECT nomenclature.name, cubes.name, cubes_items.id FROM cubes_items " +
				"LEFT JOIN nomenclature ON cubes_items.item_id = nomenclature.id " +
				"LEFT JOIN cubes ON cubes_items.cubes_id = cubes.id ";
			PrepareTable.DisplayString = "{0} в кубе {1}";
			PrepareTable.PrimaryKey = new  TableInfo.PrimaryKeys("id");
			Tables.Add("cubes_items", PrepareTable);

			PrepareTable = new TableInfo();
			PrepareTable.ObjectsName = "Компоненты куба из заказов";
			PrepareTable.ObjectName = "Компонент куба из заказа";
			PrepareTable.SqlSelect = "SELECT nomenclature.name, order_cubes_details.order_id, order_cubes_details.id as id FROM order_cubes_details " +
				"LEFT JOIN nomenclature ON order_cubes_details.nomenclature_id = nomenclature.id ";
			PrepareTable.DisplayString = "{0} в заказе №{1}";
			PrepareTable.PrimaryKey = new  TableInfo.PrimaryKeys("id");
			Tables.Add("order_cubes_details", PrepareTable);

			PrepareTable = new TableInfo();
			PrepareTable.ObjectsName = "Выставки";
			PrepareTable.ObjectName = "выставку";
			PrepareTable.SqlSelect = "SELECT name, id FROM exhibition ";
			PrepareTable.DisplayString = "{0}";
			PrepareTable.PrimaryKey = new  TableInfo.PrimaryKeys("id");
			PrepareTable.ClearItems.Add ("orders", 
				new TableInfo.ClearDependenceItem ("WHERE exhibition_id = @exhibition_id ", "", "@exhibition_id", "exhibition_id"));
			Tables.Add("exhibition", PrepareTable);

			PrepareTable = new TableInfo();
			PrepareTable.ObjectsName = "Материалы";
			PrepareTable.ObjectName = "материал";
			PrepareTable.SqlSelect = "SELECT name, id FROM materials ";
			PrepareTable.DisplayString = "{0}";
			PrepareTable.PrimaryKey = new  TableInfo.PrimaryKeys("id");
			PrepareTable.ClearItems.Add ("order_details", 
				new TableInfo.ClearDependenceItem ("WHERE material_id = @material_id ", "", "@material_id", "material_id"));
			PrepareTable.ClearItems.Add ("order_basis_details", 
				new TableInfo.ClearDependenceItem ("WHERE material_id = @material_id ", "", "@material_id", "material_id"));
			Tables.Add("materials", PrepareTable);

			PrepareTable = new TableInfo();
			PrepareTable.ObjectsName = "Виды облицовки";
			PrepareTable.ObjectName = "вид облицовки";
			PrepareTable.SqlSelect = "SELECT name, id FROM facing ";
			PrepareTable.DisplayString = "{0}";
			PrepareTable.PrimaryKey = new  TableInfo.PrimaryKeys("id");
			PrepareTable.ClearItems.Add ("order_details", 
				new TableInfo.ClearDependenceItem ("WHERE facing_id = @facing_id ", "", "@facing_id", "facing_id"));
			PrepareTable.ClearItems.Add ("order_basis_details", 
				new TableInfo.ClearDependenceItem ("WHERE facing_id = @facing_id ", "", "@facing_id", "facing_id"));
			Tables.Add("facing", PrepareTable);

			PrepareTable = new TableInfo();
			PrepareTable.ObjectsName = "Компоненты каркаса из заказов";
			PrepareTable.ObjectName = "Компонент каркаса из заказов";
			PrepareTable.SqlSelect = "SELECT nomenclature.name, order_basis_details.order_id, order_basis_details.id as id FROM order_basis_details " +
				"LEFT JOIN nomenclature ON order_basis_details.nomenclature_id = nomenclature.id ";
			PrepareTable.DisplayString = "{0} в заказе №{1}";
			PrepareTable.PrimaryKey = new  TableInfo.PrimaryKeys("id");
			Tables.Add("order_basis_details", PrepareTable);

			PrepareTable = new TableInfo();
			PrepareTable.ObjectsName = "Заказы";
			PrepareTable.ObjectName = "заказ";
			PrepareTable.SqlSelect = "SELECT id FROM orders ";
			PrepareTable.DisplayString = "Заказ №{0}";
			PrepareTable.PrimaryKey = new  TableInfo.PrimaryKeys("id");
			Tables.Add("orders", PrepareTable);
		}

		private static void CreateConnection()
		{
			string AppName = System.Reflection.Assembly.GetExecutingAssembly().GetName().Name.ToString();
			string ConfigFileName = AppName + ".ini";
			string AppFolder;
			if(Environment.OSVersion.Platform == PlatformID.Unix)
				AppFolder = System.IO.Path.Combine (Environment.GetFolderPath (Environment.SpecialFolder.ApplicationData), AppName);
			else
				AppFolder = System.IO.Path.Combine (Environment.GetFolderPath (Environment.SpecialFolder.CommonApplicationData), AppName);
			string DataBase = System.IO.Path.Combine (AppFolder, "Cupboard.db3");

			string configfile = System.IO.Path.Combine (AppFolder, ConfigFileName);
			IniConfigSource Configsource;
			try
			{
				Configsource = new IniConfigSource(configfile);
				Configsource.Reload();
				DataBase = Configsource.Configs["Login"].Get("DataBase", DataBase);
			} 
			catch (Exception ex)
			{
				logger.Warn(ex, "Конфигурационный фаил не найден. Создаем новый.");
				Configsource = new IniConfigSource();

				IConfig config = Configsource.AddConfig("Login");
				config.Set("DataBase", DataBase);
				if (!System.IO.Directory.Exists(AppFolder))
					System.IO.Directory.CreateDirectory(AppFolder);
				Configsource.Save(configfile);
			}

			//Создаем соедиение
			try
			{
				QSMain.DBMS = QSMain.DataProviders.Factory;
				string Providers = "";
				foreach(System.Data.DataRow row in DbProviderFactories.GetFactoryClasses().Rows)
				{
					Providers += String.Format("{0}#{1}#{2}#{3}", row[0], row[1], row[2], row[3]);
					Providers += Environment.NewLine;
				}
				logger.Debug("Exist DataProviders:\n{0}", Providers);
				QSMain.ProviderDB = DbProviderFactories.GetFactory("Mono.Data.Sqlite");
				QSMain.ConnectionDB = QSMain.ProviderDB.CreateConnection();
				logger.Debug("Открываем базу:{0}", DataBase);
				QSMain.ConnectionString = String.Format("Data Source={0};Version=3;", DataBase);
				QSMain.ConnectionDB.ConnectionString = QSMain.ConnectionString;
				QSMain.ConnectionDB.Open();
				logger.Info("Открытие базы данных прошло успешно.");
				logger.Info("Читаем параметры");
				Parameters = new QSSupportLib.BaseParam(QSMain.ConnectionDB);
			}
			catch (Exception ex)
			{
				string Error = String.Format("Не получилось открыть базу данных {0}", DataBase);
				QSMain.ErrorMessageWithLog(null, Error, logger, ex);
				Environment.Exit(1);
			}

		}

		public static void StatusMessage(string message)
		{
			StatusBarLabel.LabelProp = message;
			while (GLib.MainContext.Pending())
			{
				Gtk.Main.Iteration();
			}
		}

	}
}
