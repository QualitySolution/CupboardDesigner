
// This file has been generated by the GUI designer. Do not modify.

public partial class MainWindow
{
	private global::Gtk.UIManager UIManager;
	
	private global::Gtk.Action Action;
	
	private global::Gtk.Action quitAction;
	
	private global::Gtk.Action Action14;
	
	private global::Gtk.Action Action16;
	
	private global::Gtk.Action aboutAction;
	
	private global::Gtk.Action Action9;
	
	private global::Gtk.Action Action11;
	
	private global::Gtk.Action Action10;
	
	private global::Gtk.Action Action12;
	
	private global::Gtk.Action Action13;
	
	private global::Gtk.ToggleAction dialogAuthenticationAction;
	
	private global::Gtk.Action ActionPassword;
	
	private global::Gtk.Action Action15;
	
	private global::Gtk.Action ActionSite;
	
	private global::Gtk.Action ActionHistory;
	
	private global::Gtk.Action goUpAction;
	
	private global::Gtk.VBox vbox1;
	
	private global::Gtk.MenuBar menubar1;
	
	private global::Gtk.HBox hbox5;
	
	private global::Gtk.Label label6;
	
	private global::Gtk.Entry entrySearch;
	
	private global::Gtk.Button buttonClearSearch;
	
	private global::Gtk.ScrolledWindow GtkScrolledWindow;
	
	private global::Gtk.TreeView treeviewOrders;
	
	private global::Gtk.HBox hbox4;
	
	private global::Gtk.Button buttonAdd;
	
	private global::Gtk.Button buttonEdit;
	
	private global::Gtk.Button buttonCopy;
	
	private global::Gtk.Button buttonDel;
	
	private global::Gtk.Statusbar statusbar1;
	
	private global::Gtk.Label labelStatus;

	protected virtual void Build ()
	{
		global::Stetic.Gui.Initialize (this);
		// Widget MainWindow
		this.UIManager = new global::Gtk.UIManager ();
		global::Gtk.ActionGroup w1 = new global::Gtk.ActionGroup ("Default");
		this.Action = new global::Gtk.Action ("Action", global::Mono.Unix.Catalog.GetString ("Файл"), null, null);
		this.Action.ShortLabel = global::Mono.Unix.Catalog.GetString ("Файл");
		w1.Add (this.Action, null);
		this.quitAction = new global::Gtk.Action ("quitAction", global::Mono.Unix.Catalog.GetString ("В_ыход"), null, "gtk-quit");
		this.quitAction.ShortLabel = global::Mono.Unix.Catalog.GetString ("В_ыход");
		w1.Add (this.quitAction, null);
		this.Action14 = new global::Gtk.Action ("Action14", global::Mono.Unix.Catalog.GetString ("Справочники"), null, null);
		this.Action14.ShortLabel = global::Mono.Unix.Catalog.GetString ("Справочники");
		w1.Add (this.Action14, null);
		this.Action16 = new global::Gtk.Action ("Action16", global::Mono.Unix.Catalog.GetString ("Справка"), null, null);
		this.Action16.ShortLabel = global::Mono.Unix.Catalog.GetString ("Справка");
		w1.Add (this.Action16, null);
		this.aboutAction = new global::Gtk.Action ("aboutAction", global::Mono.Unix.Catalog.GetString ("_О программе"), null, "gtk-about");
		this.aboutAction.ShortLabel = global::Mono.Unix.Catalog.GetString ("_О программе");
		w1.Add (this.aboutAction, null);
		this.Action9 = new global::Gtk.Action ("Action9", global::Mono.Unix.Catalog.GetString ("Номенклатура"), null, null);
		this.Action9.ShortLabel = global::Mono.Unix.Catalog.GetString ("Номенклатура");
		w1.Add (this.Action9, null);
		this.Action11 = new global::Gtk.Action ("Action11", global::Mono.Unix.Catalog.GetString ("Каркасы"), null, null);
		this.Action11.ShortLabel = global::Mono.Unix.Catalog.GetString ("Виды оснований");
		w1.Add (this.Action11, null);
		this.Action10 = new global::Gtk.Action ("Action10", global::Mono.Unix.Catalog.GetString ("Выставки"), null, null);
		this.Action10.ShortLabel = global::Mono.Unix.Catalog.GetString ("Типы компонентов");
		w1.Add (this.Action10, null);
		this.Action12 = new global::Gtk.Action ("Action12", global::Mono.Unix.Catalog.GetString ("Отделка кубов"), null, null);
		this.Action12.ShortLabel = global::Mono.Unix.Catalog.GetString ("Отделка кубов");
		w1.Add (this.Action12, null);
		this.Action13 = new global::Gtk.Action ("Action13", global::Mono.Unix.Catalog.GetString ("Отделка фасадов"), null, null);
		this.Action13.ShortLabel = global::Mono.Unix.Catalog.GetString ("Отделка фасадов");
		w1.Add (this.Action13, null);
		this.dialogAuthenticationAction = new global::Gtk.ToggleAction ("dialogAuthenticationAction", global::Mono.Unix.Catalog.GetString ("Административный режим"), null, "gtk-dialog-authentication");
		this.dialogAuthenticationAction.ShortLabel = global::Mono.Unix.Catalog.GetString ("Административный режим");
		w1.Add (this.dialogAuthenticationAction, null);
		this.ActionPassword = new global::Gtk.Action ("ActionPassword", global::Mono.Unix.Catalog.GetString ("Изменить пароль"), null, "gtk-dialog-authentication");
		this.ActionPassword.ShortLabel = global::Mono.Unix.Catalog.GetString ("Изменить пароль");
		w1.Add (this.ActionPassword, null);
		this.Action15 = new global::Gtk.Action ("Action15", global::Mono.Unix.Catalog.GetString ("Кубы"), null, null);
		this.Action15.ShortLabel = global::Mono.Unix.Catalog.GetString ("Кубы");
		w1.Add (this.Action15, null);
		this.ActionSite = new global::Gtk.Action ("ActionSite", global::Mono.Unix.Catalog.GetString ("Посетить сайт"), null, null);
		this.ActionSite.ShortLabel = global::Mono.Unix.Catalog.GetString ("Посетить сайт");
		w1.Add (this.ActionSite, null);
		this.ActionHistory = new global::Gtk.Action ("ActionHistory", global::Mono.Unix.Catalog.GetString ("История версий"), null, null);
		this.ActionHistory.ShortLabel = global::Mono.Unix.Catalog.GetString ("История версий");
		w1.Add (this.ActionHistory, null);
		this.goUpAction = new global::Gtk.Action ("goUpAction", global::Mono.Unix.Catalog.GetString ("Проверить обновление..."), null, "gtk-go-up");
		this.goUpAction.ShortLabel = global::Mono.Unix.Catalog.GetString ("Проверить обновление...");
		w1.Add (this.goUpAction, null);
		this.UIManager.InsertActionGroup (w1, 0);
		this.AddAccelGroup (this.UIManager.AccelGroup);
		this.Name = "MainWindow";
		this.Title = global::Mono.Unix.Catalog.GetString ("QS: Конфигуратор шкафа");
		this.Icon = global::Gdk.Pixbuf.LoadFromResource ("CupboardDesigner.icons.logo.ico");
		this.WindowPosition = ((global::Gtk.WindowPosition)(4));
		// Container child MainWindow.Gtk.Container+ContainerChild
		this.vbox1 = new global::Gtk.VBox ();
		this.vbox1.Name = "vbox1";
		this.vbox1.Spacing = 6;
		// Container child vbox1.Gtk.Box+BoxChild
		this.UIManager.AddUiFromString ("<ui><menubar name='menubar1'><menu name='Action' action='Action'><menuitem name='dialogAuthenticationAction' action='dialogAuthenticationAction'/><menuitem name='ActionPassword' action='ActionPassword'/><separator/><menuitem name='quitAction' action='quitAction'/></menu><menu name='Action14' action='Action14'><menuitem name='Action9' action='Action9'/><menuitem name='Action15' action='Action15'/><menuitem name='Action11' action='Action11'/><separator/><menuitem name='Action10' action='Action10'/><separator/><menuitem name='Action12' action='Action12'/><menuitem name='Action13' action='Action13'/></menu><menu name='Action16' action='Action16'><menuitem name='ActionSite' action='ActionSite'/><menuitem name='ActionHistory' action='ActionHistory'/><menuitem name='goUpAction' action='goUpAction'/><separator/><menuitem name='aboutAction' action='aboutAction'/></menu></menubar></ui>");
		this.menubar1 = ((global::Gtk.MenuBar)(this.UIManager.GetWidget ("/menubar1")));
		this.menubar1.Name = "menubar1";
		this.vbox1.Add (this.menubar1);
		global::Gtk.Box.BoxChild w2 = ((global::Gtk.Box.BoxChild)(this.vbox1 [this.menubar1]));
		w2.Position = 0;
		w2.Expand = false;
		w2.Fill = false;
		// Container child vbox1.Gtk.Box+BoxChild
		this.hbox5 = new global::Gtk.HBox ();
		this.hbox5.Name = "hbox5";
		this.hbox5.Spacing = 6;
		// Container child hbox5.Gtk.Box+BoxChild
		this.label6 = new global::Gtk.Label ();
		this.label6.Name = "label6";
		this.label6.LabelProp = global::Mono.Unix.Catalog.GetString ("Поиск:");
		this.hbox5.Add (this.label6);
		global::Gtk.Box.BoxChild w3 = ((global::Gtk.Box.BoxChild)(this.hbox5 [this.label6]));
		w3.Position = 0;
		w3.Expand = false;
		w3.Fill = false;
		// Container child hbox5.Gtk.Box+BoxChild
		this.entrySearch = new global::Gtk.Entry ();
		this.entrySearch.CanFocus = true;
		this.entrySearch.Name = "entrySearch";
		this.entrySearch.IsEditable = true;
		this.entrySearch.InvisibleChar = '●';
		this.hbox5.Add (this.entrySearch);
		global::Gtk.Box.BoxChild w4 = ((global::Gtk.Box.BoxChild)(this.hbox5 [this.entrySearch]));
		w4.Position = 1;
		// Container child hbox5.Gtk.Box+BoxChild
		this.buttonClearSearch = new global::Gtk.Button ();
		this.buttonClearSearch.CanFocus = true;
		this.buttonClearSearch.Name = "buttonClearSearch";
		this.buttonClearSearch.UseUnderline = true;
		global::Gtk.Image w5 = new global::Gtk.Image ();
		w5.Pixbuf = global::Stetic.IconLoader.LoadIcon (this, "gtk-clear", global::Gtk.IconSize.Menu);
		this.buttonClearSearch.Image = w5;
		this.hbox5.Add (this.buttonClearSearch);
		global::Gtk.Box.BoxChild w6 = ((global::Gtk.Box.BoxChild)(this.hbox5 [this.buttonClearSearch]));
		w6.Position = 2;
		w6.Expand = false;
		w6.Fill = false;
		this.vbox1.Add (this.hbox5);
		global::Gtk.Box.BoxChild w7 = ((global::Gtk.Box.BoxChild)(this.vbox1 [this.hbox5]));
		w7.Position = 1;
		w7.Expand = false;
		w7.Fill = false;
		// Container child vbox1.Gtk.Box+BoxChild
		this.GtkScrolledWindow = new global::Gtk.ScrolledWindow ();
		this.GtkScrolledWindow.Name = "GtkScrolledWindow";
		this.GtkScrolledWindow.ShadowType = ((global::Gtk.ShadowType)(1));
		// Container child GtkScrolledWindow.Gtk.Container+ContainerChild
		this.treeviewOrders = new global::Gtk.TreeView ();
		this.treeviewOrders.CanFocus = true;
		this.treeviewOrders.Name = "treeviewOrders";
		this.GtkScrolledWindow.Add (this.treeviewOrders);
		this.vbox1.Add (this.GtkScrolledWindow);
		global::Gtk.Box.BoxChild w9 = ((global::Gtk.Box.BoxChild)(this.vbox1 [this.GtkScrolledWindow]));
		w9.Position = 2;
		// Container child vbox1.Gtk.Box+BoxChild
		this.hbox4 = new global::Gtk.HBox ();
		this.hbox4.Name = "hbox4";
		this.hbox4.Spacing = 6;
		// Container child hbox4.Gtk.Box+BoxChild
		this.buttonAdd = new global::Gtk.Button ();
		this.buttonAdd.CanFocus = true;
		this.buttonAdd.Name = "buttonAdd";
		this.buttonAdd.UseUnderline = true;
		this.buttonAdd.Label = global::Mono.Unix.Catalog.GetString ("Новый заказ");
		global::Gtk.Image w10 = new global::Gtk.Image ();
		w10.Pixbuf = global::Stetic.IconLoader.LoadIcon (this, "gtk-add", global::Gtk.IconSize.Menu);
		this.buttonAdd.Image = w10;
		this.hbox4.Add (this.buttonAdd);
		global::Gtk.Box.BoxChild w11 = ((global::Gtk.Box.BoxChild)(this.hbox4 [this.buttonAdd]));
		w11.Position = 0;
		w11.Expand = false;
		w11.Fill = false;
		// Container child hbox4.Gtk.Box+BoxChild
		this.buttonEdit = new global::Gtk.Button ();
		this.buttonEdit.Sensitive = false;
		this.buttonEdit.CanFocus = true;
		this.buttonEdit.Name = "buttonEdit";
		this.buttonEdit.UseUnderline = true;
		this.buttonEdit.Label = global::Mono.Unix.Catalog.GetString ("Изменить");
		global::Gtk.Image w12 = new global::Gtk.Image ();
		w12.Pixbuf = global::Stetic.IconLoader.LoadIcon (this, "gtk-edit", global::Gtk.IconSize.Menu);
		this.buttonEdit.Image = w12;
		this.hbox4.Add (this.buttonEdit);
		global::Gtk.Box.BoxChild w13 = ((global::Gtk.Box.BoxChild)(this.hbox4 [this.buttonEdit]));
		w13.Position = 1;
		w13.Expand = false;
		w13.Fill = false;
		// Container child hbox4.Gtk.Box+BoxChild
		this.buttonCopy = new global::Gtk.Button ();
		this.buttonCopy.Sensitive = false;
		this.buttonCopy.CanFocus = true;
		this.buttonCopy.Name = "buttonCopy";
		this.buttonCopy.UseUnderline = true;
		this.buttonCopy.Label = global::Mono.Unix.Catalog.GetString ("Копировать");
		global::Gtk.Image w14 = new global::Gtk.Image ();
		w14.Pixbuf = global::Stetic.IconLoader.LoadIcon (this, "gtk-copy", global::Gtk.IconSize.Menu);
		this.buttonCopy.Image = w14;
		this.hbox4.Add (this.buttonCopy);
		global::Gtk.Box.BoxChild w15 = ((global::Gtk.Box.BoxChild)(this.hbox4 [this.buttonCopy]));
		w15.Position = 2;
		w15.Expand = false;
		w15.Fill = false;
		// Container child hbox4.Gtk.Box+BoxChild
		this.buttonDel = new global::Gtk.Button ();
		this.buttonDel.Sensitive = false;
		this.buttonDel.CanFocus = true;
		this.buttonDel.Name = "buttonDel";
		this.buttonDel.UseUnderline = true;
		this.buttonDel.Label = global::Mono.Unix.Catalog.GetString ("Удалить");
		global::Gtk.Image w16 = new global::Gtk.Image ();
		w16.Pixbuf = global::Stetic.IconLoader.LoadIcon (this, "gtk-delete", global::Gtk.IconSize.Menu);
		this.buttonDel.Image = w16;
		this.hbox4.Add (this.buttonDel);
		global::Gtk.Box.BoxChild w17 = ((global::Gtk.Box.BoxChild)(this.hbox4 [this.buttonDel]));
		w17.Position = 3;
		w17.Expand = false;
		w17.Fill = false;
		this.vbox1.Add (this.hbox4);
		global::Gtk.Box.BoxChild w18 = ((global::Gtk.Box.BoxChild)(this.vbox1 [this.hbox4]));
		w18.Position = 3;
		w18.Expand = false;
		w18.Fill = false;
		// Container child vbox1.Gtk.Box+BoxChild
		this.statusbar1 = new global::Gtk.Statusbar ();
		this.statusbar1.Name = "statusbar1";
		this.statusbar1.Spacing = 6;
		// Container child statusbar1.Gtk.Box+BoxChild
		this.labelStatus = new global::Gtk.Label ();
		this.labelStatus.Name = "labelStatus";
		this.labelStatus.LabelProp = global::Mono.Unix.Catalog.GetString ("label4");
		this.statusbar1.Add (this.labelStatus);
		global::Gtk.Box.BoxChild w19 = ((global::Gtk.Box.BoxChild)(this.statusbar1 [this.labelStatus]));
		w19.Position = 2;
		w19.Expand = false;
		w19.Fill = false;
		this.vbox1.Add (this.statusbar1);
		global::Gtk.Box.BoxChild w20 = ((global::Gtk.Box.BoxChild)(this.vbox1 [this.statusbar1]));
		w20.Position = 4;
		w20.Expand = false;
		w20.Fill = false;
		this.Add (this.vbox1);
		if ((this.Child != null)) {
			this.Child.ShowAll ();
		}
		this.DefaultWidth = 725;
		this.DefaultHeight = 465;
		this.Show ();
		this.DeleteEvent += new global::Gtk.DeleteEventHandler (this.OnDeleteEvent);
		this.quitAction.Activated += new global::System.EventHandler (this.OnQuitActionActivated);
		this.aboutAction.Activated += new global::System.EventHandler (this.OnAboutActionActivated);
		this.Action9.Activated += new global::System.EventHandler (this.OnAction9Activated);
		this.Action11.Activated += new global::System.EventHandler (this.OnAction4Activated);
		this.Action10.Activated += new global::System.EventHandler (this.OnAction5Activated);
		this.Action12.Activated += new global::System.EventHandler (this.OnAction8Activated);
		this.Action13.Activated += new global::System.EventHandler (this.OnAction7Activated);
		this.dialogAuthenticationAction.Toggled += new global::System.EventHandler (this.OnDialogAuthenticationActionToggled);
		this.ActionPassword.Activated += new global::System.EventHandler (this.OnActionPasswordActivated);
		this.Action15.Activated += new global::System.EventHandler (this.OnAction15Activated);
		this.ActionSite.Activated += new global::System.EventHandler (this.OnActionSiteActivated);
		this.ActionHistory.Activated += new global::System.EventHandler (this.OnActionHistoryActivated);
		this.goUpAction.Activated += new global::System.EventHandler (this.OnActionUpdateActivated);
		this.entrySearch.Changed += new global::System.EventHandler (this.OnEntrySearchChanged);
		this.buttonClearSearch.Clicked += new global::System.EventHandler (this.OnButtonClearSearchClicked);
		this.treeviewOrders.RowActivated += new global::Gtk.RowActivatedHandler (this.OnTreeviewOrdersRowActivated);
		this.buttonAdd.Clicked += new global::System.EventHandler (this.OnButtonAddClicked);
		this.buttonEdit.Clicked += new global::System.EventHandler (this.OnButtonEditClicked);
		this.buttonCopy.Clicked += new global::System.EventHandler (this.OnButtonCopyClicked);
		this.buttonDel.Clicked += new global::System.EventHandler (this.OnButtonDelClicked);
	}
}
