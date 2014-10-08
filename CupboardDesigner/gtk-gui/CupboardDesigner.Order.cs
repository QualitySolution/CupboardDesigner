
// This file has been generated by the GUI designer. Do not modify.
namespace CupboardDesigner
{
	public partial class Order
	{
		private global::Gtk.UIManager UIManager;
		
		private global::Gtk.Action revertToSavedAction;
		
		private global::Gtk.Action goBackAction;
		
		private global::Gtk.Action goForwardAction;
		
		private global::Gtk.Action saveAction;
		
		private global::Gtk.VBox vbox2;
		
		private global::Gtk.Toolbar toolbar1;
		
		private global::Gtk.Notebook notebook1;
		
		private global::Gtk.HBox hbox1;
		
		private global::Gtk.Table table3;
		
		private global::Gtk.CheckButton checkEstimation;
		
		private global::Gtk.ComboBox comboExhibition;
		
		private global::QSWidgetLib.DatePicker dateArrval;
		
		private global::Gtk.Entry entryContract;
		
		private global::Gtk.Entry entryCustomer;
		
		private global::Gtk.Entry entryPhone1;
		
		private global::Gtk.Entry entryPhone2;
		
		private global::Gtk.ScrolledWindow GtkScrolledWindow;
		
		private global::Gtk.TextView textviewComments;
		
		private global::Gtk.ScrolledWindow GtkScrolledWindow2;
		
		private global::Gtk.TextView textAddress;
		
		private global::Gtk.HBox hbox4;
		
		private global::QSWidgetLib.DatePicker dateDeadlineS;
		
		private global::Gtk.Label label17;
		
		private global::QSWidgetLib.DatePicker dateDeadlineE;
		
		private global::Gtk.Label label1;
		
		private global::Gtk.Label label10;
		
		private global::Gtk.Label label11;
		
		private global::Gtk.Label label12;
		
		private global::Gtk.Label label13;
		
		private global::Gtk.Label label14;
		
		private global::Gtk.Label label4;
		
		private global::Gtk.Label label6;
		
		private global::Gtk.Label label9;
		
		private global::Gtk.Label label7;
		
		private global::Gtk.VBox vbox7;
		
		private global::Gtk.Table table1;
		
		private global::Gtk.CheckButton checkCuttingBase;
		
		private global::Gtk.ComboBox comboCubeH;
		
		private global::Gtk.ComboBox comboCubeV;
		
		private global::Gtk.Label label2;
		
		private global::Gtk.Label label3;
		
		private global::Gtk.ScrolledWindow scrolledTypesH;
		
		private global::Gtk.Label label5;
		
		private global::Gtk.VBox vbox3;
		
		private global::Gtk.Table tableConstructor;
		
		private global::Gtk.Button buttonCubeListOrientation;
		
		private global::Gtk.DrawingArea drawCupboard;
		
		private global::Gtk.ScrolledWindow scrolledCubeListH;
		
		private global::Gtk.ScrolledWindow scrolledCubeListV;
		
		private global::Gtk.Label label8;
		
		private global::Gtk.VBox vbox5;
		
		private global::Gtk.ScrolledWindow GtkScrolledWindow1;
		
		private global::Gtk.TreeView treeviewComponents;
		
		private global::Gtk.HBox hbox6;
		
		private global::Gtk.CheckButton checkbuttonShowPrice;
		
		private global::Gtk.CheckButton checkbuttonDiscount;
		
		private global::Gtk.SpinButton spinbutton1;
		
		private global::Gtk.Label label18;
		
		private global::Gtk.Label labelTotalCount;
		
		private global::Gtk.Label label15;
		
		private global::fyiReporting.RdlGtkViewer.ReportViewer reportviewer1;
		
		private global::Gtk.Label label16;
		
		private global::fyiReporting.RdlGtkViewer.ReportViewer reportviewer2;
		
		private global::Gtk.Label label19;

		protected virtual void Build ()
		{
			global::Stetic.Gui.Initialize (this);
			// Widget CupboardDesigner.Order
			this.UIManager = new global::Gtk.UIManager ();
			global::Gtk.ActionGroup w1 = new global::Gtk.ActionGroup ("Default");
			this.revertToSavedAction = new global::Gtk.Action ("revertToSavedAction", global::Mono.Unix.Catalog.GetString ("Отменить"), global::Mono.Unix.Catalog.GetString ("Отменить изменения"), "gtk-revert-to-saved");
			this.revertToSavedAction.ShortLabel = global::Mono.Unix.Catalog.GetString ("Отменить");
			w1.Add (this.revertToSavedAction, null);
			this.goBackAction = new global::Gtk.Action ("goBackAction", global::Mono.Unix.Catalog.GetString ("Назад"), global::Mono.Unix.Catalog.GetString ("К предыдущему шагу."), "gtk-go-back");
			this.goBackAction.Sensitive = false;
			this.goBackAction.ShortLabel = global::Mono.Unix.Catalog.GetString ("Назад");
			w1.Add (this.goBackAction, null);
			this.goForwardAction = new global::Gtk.Action ("goForwardAction", global::Mono.Unix.Catalog.GetString ("Далее"), global::Mono.Unix.Catalog.GetString ("Следующий шаг"), "gtk-go-forward");
			this.goForwardAction.ShortLabel = global::Mono.Unix.Catalog.GetString ("Далее");
			w1.Add (this.goForwardAction, null);
			this.saveAction = new global::Gtk.Action ("saveAction", global::Mono.Unix.Catalog.GetString ("Сохранить"), global::Mono.Unix.Catalog.GetString ("Сохранить изменения"), "gtk-save");
			this.saveAction.ShortLabel = global::Mono.Unix.Catalog.GetString ("Сохранить");
			w1.Add (this.saveAction, null);
			this.UIManager.InsertActionGroup (w1, 0);
			this.AddAccelGroup (this.UIManager.AccelGroup);
			this.Name = "CupboardDesigner.Order";
			this.Title = global::Mono.Unix.Catalog.GetString ("Новый заказ");
			this.WindowPosition = ((global::Gtk.WindowPosition)(4));
			// Container child CupboardDesigner.Order.Gtk.Container+ContainerChild
			this.vbox2 = new global::Gtk.VBox ();
			this.vbox2.Name = "vbox2";
			this.vbox2.Spacing = 6;
			// Container child vbox2.Gtk.Box+BoxChild
			this.UIManager.AddUiFromString ("<ui><toolbar name='toolbar1'><toolitem name='goBackAction' action='goBackAction'/><toolitem name='goForwardAction' action='goForwardAction'/><toolitem name='revertToSavedAction' action='revertToSavedAction'/><toolitem name='saveAction' action='saveAction'/></toolbar></ui>");
			this.toolbar1 = ((global::Gtk.Toolbar)(this.UIManager.GetWidget ("/toolbar1")));
			this.toolbar1.Name = "toolbar1";
			this.toolbar1.ShowArrow = false;
			this.toolbar1.ToolbarStyle = ((global::Gtk.ToolbarStyle)(2));
			this.toolbar1.IconSize = ((global::Gtk.IconSize)(3));
			this.vbox2.Add (this.toolbar1);
			global::Gtk.Box.BoxChild w2 = ((global::Gtk.Box.BoxChild)(this.vbox2 [this.toolbar1]));
			w2.Position = 0;
			w2.Expand = false;
			w2.Fill = false;
			// Container child vbox2.Gtk.Box+BoxChild
			this.notebook1 = new global::Gtk.Notebook ();
			this.notebook1.CanFocus = true;
			this.notebook1.Name = "notebook1";
			this.notebook1.CurrentPage = 0;
			// Container child notebook1.Gtk.Notebook+NotebookChild
			this.hbox1 = new global::Gtk.HBox ();
			this.hbox1.Name = "hbox1";
			this.hbox1.Spacing = 6;
			// Container child hbox1.Gtk.Box+BoxChild
			this.table3 = new global::Gtk.Table (((uint)(10)), ((uint)(2)), false);
			this.table3.Name = "table3";
			this.table3.RowSpacing = ((uint)(6));
			this.table3.ColumnSpacing = ((uint)(6));
			this.table3.BorderWidth = ((uint)(36));
			// Container child table3.Gtk.Table+TableChild
			this.checkEstimation = new global::Gtk.CheckButton ();
			this.checkEstimation.CanFocus = true;
			this.checkEstimation.Name = "checkEstimation";
			this.checkEstimation.Label = global::Mono.Unix.Catalog.GetString ("Предварительный расчет");
			this.checkEstimation.Active = true;
			this.checkEstimation.DrawIndicator = true;
			this.checkEstimation.UseUnderline = true;
			this.table3.Add (this.checkEstimation);
			global::Gtk.Table.TableChild w3 = ((global::Gtk.Table.TableChild)(this.table3 [this.checkEstimation]));
			w3.LeftAttach = ((uint)(1));
			w3.RightAttach = ((uint)(2));
			w3.XOptions = ((global::Gtk.AttachOptions)(4));
			w3.YOptions = ((global::Gtk.AttachOptions)(4));
			// Container child table3.Gtk.Table+TableChild
			this.comboExhibition = new global::Gtk.ComboBox ();
			this.comboExhibition.Name = "comboExhibition";
			this.table3.Add (this.comboExhibition);
			global::Gtk.Table.TableChild w4 = ((global::Gtk.Table.TableChild)(this.table3 [this.comboExhibition]));
			w4.TopAttach = ((uint)(9));
			w4.BottomAttach = ((uint)(10));
			w4.LeftAttach = ((uint)(1));
			w4.RightAttach = ((uint)(2));
			w4.XOptions = ((global::Gtk.AttachOptions)(4));
			w4.YOptions = ((global::Gtk.AttachOptions)(4));
			// Container child table3.Gtk.Table+TableChild
			this.dateArrval = new global::QSWidgetLib.DatePicker ();
			this.dateArrval.Events = ((global::Gdk.EventMask)(256));
			this.dateArrval.Name = "dateArrval";
			this.dateArrval.Date = new global::System.DateTime (0);
			this.dateArrval.IsEditable = true;
			this.dateArrval.AutoSeparation = true;
			this.table3.Add (this.dateArrval);
			global::Gtk.Table.TableChild w5 = ((global::Gtk.Table.TableChild)(this.table3 [this.dateArrval]));
			w5.TopAttach = ((uint)(6));
			w5.BottomAttach = ((uint)(7));
			w5.LeftAttach = ((uint)(1));
			w5.RightAttach = ((uint)(2));
			w5.XOptions = ((global::Gtk.AttachOptions)(4));
			w5.YOptions = ((global::Gtk.AttachOptions)(4));
			// Container child table3.Gtk.Table+TableChild
			this.entryContract = new global::Gtk.Entry ();
			this.entryContract.Sensitive = false;
			this.entryContract.CanFocus = true;
			this.entryContract.Name = "entryContract";
			this.entryContract.IsEditable = true;
			this.entryContract.InvisibleChar = '●';
			this.table3.Add (this.entryContract);
			global::Gtk.Table.TableChild w6 = ((global::Gtk.Table.TableChild)(this.table3 [this.entryContract]));
			w6.TopAttach = ((uint)(1));
			w6.BottomAttach = ((uint)(2));
			w6.LeftAttach = ((uint)(1));
			w6.RightAttach = ((uint)(2));
			w6.XOptions = ((global::Gtk.AttachOptions)(4));
			w6.YOptions = ((global::Gtk.AttachOptions)(4));
			// Container child table3.Gtk.Table+TableChild
			this.entryCustomer = new global::Gtk.Entry ();
			this.entryCustomer.CanFocus = true;
			this.entryCustomer.Name = "entryCustomer";
			this.entryCustomer.IsEditable = true;
			this.entryCustomer.InvisibleChar = '●';
			this.table3.Add (this.entryCustomer);
			global::Gtk.Table.TableChild w7 = ((global::Gtk.Table.TableChild)(this.table3 [this.entryCustomer]));
			w7.TopAttach = ((uint)(2));
			w7.BottomAttach = ((uint)(3));
			w7.LeftAttach = ((uint)(1));
			w7.RightAttach = ((uint)(2));
			w7.XOptions = ((global::Gtk.AttachOptions)(4));
			w7.YOptions = ((global::Gtk.AttachOptions)(4));
			// Container child table3.Gtk.Table+TableChild
			this.entryPhone1 = new global::Gtk.Entry ();
			this.entryPhone1.CanFocus = true;
			this.entryPhone1.Name = "entryPhone1";
			this.entryPhone1.IsEditable = true;
			this.entryPhone1.InvisibleChar = '●';
			this.table3.Add (this.entryPhone1);
			global::Gtk.Table.TableChild w8 = ((global::Gtk.Table.TableChild)(this.table3 [this.entryPhone1]));
			w8.TopAttach = ((uint)(4));
			w8.BottomAttach = ((uint)(5));
			w8.LeftAttach = ((uint)(1));
			w8.RightAttach = ((uint)(2));
			w8.XOptions = ((global::Gtk.AttachOptions)(4));
			w8.YOptions = ((global::Gtk.AttachOptions)(4));
			// Container child table3.Gtk.Table+TableChild
			this.entryPhone2 = new global::Gtk.Entry ();
			this.entryPhone2.CanFocus = true;
			this.entryPhone2.Name = "entryPhone2";
			this.entryPhone2.IsEditable = true;
			this.entryPhone2.InvisibleChar = '●';
			this.table3.Add (this.entryPhone2);
			global::Gtk.Table.TableChild w9 = ((global::Gtk.Table.TableChild)(this.table3 [this.entryPhone2]));
			w9.TopAttach = ((uint)(5));
			w9.BottomAttach = ((uint)(6));
			w9.LeftAttach = ((uint)(1));
			w9.RightAttach = ((uint)(2));
			w9.XOptions = ((global::Gtk.AttachOptions)(4));
			w9.YOptions = ((global::Gtk.AttachOptions)(4));
			// Container child table3.Gtk.Table+TableChild
			this.GtkScrolledWindow = new global::Gtk.ScrolledWindow ();
			this.GtkScrolledWindow.Name = "GtkScrolledWindow";
			this.GtkScrolledWindow.ShadowType = ((global::Gtk.ShadowType)(1));
			// Container child GtkScrolledWindow.Gtk.Container+ContainerChild
			this.textviewComments = new global::Gtk.TextView ();
			this.textviewComments.CanFocus = true;
			this.textviewComments.Name = "textviewComments";
			this.GtkScrolledWindow.Add (this.textviewComments);
			this.table3.Add (this.GtkScrolledWindow);
			global::Gtk.Table.TableChild w11 = ((global::Gtk.Table.TableChild)(this.table3 [this.GtkScrolledWindow]));
			w11.TopAttach = ((uint)(8));
			w11.BottomAttach = ((uint)(9));
			w11.LeftAttach = ((uint)(1));
			w11.RightAttach = ((uint)(2));
			w11.YOptions = ((global::Gtk.AttachOptions)(4));
			// Container child table3.Gtk.Table+TableChild
			this.GtkScrolledWindow2 = new global::Gtk.ScrolledWindow ();
			this.GtkScrolledWindow2.Name = "GtkScrolledWindow2";
			this.GtkScrolledWindow2.ShadowType = ((global::Gtk.ShadowType)(1));
			// Container child GtkScrolledWindow2.Gtk.Container+ContainerChild
			this.textAddress = new global::Gtk.TextView ();
			this.textAddress.CanFocus = true;
			this.textAddress.Name = "textAddress";
			this.GtkScrolledWindow2.Add (this.textAddress);
			this.table3.Add (this.GtkScrolledWindow2);
			global::Gtk.Table.TableChild w13 = ((global::Gtk.Table.TableChild)(this.table3 [this.GtkScrolledWindow2]));
			w13.TopAttach = ((uint)(3));
			w13.BottomAttach = ((uint)(4));
			w13.LeftAttach = ((uint)(1));
			w13.RightAttach = ((uint)(2));
			w13.YOptions = ((global::Gtk.AttachOptions)(4));
			// Container child table3.Gtk.Table+TableChild
			this.hbox4 = new global::Gtk.HBox ();
			this.hbox4.Name = "hbox4";
			this.hbox4.Spacing = 6;
			// Container child hbox4.Gtk.Box+BoxChild
			this.dateDeadlineS = new global::QSWidgetLib.DatePicker ();
			this.dateDeadlineS.Events = ((global::Gdk.EventMask)(256));
			this.dateDeadlineS.Name = "dateDeadlineS";
			this.dateDeadlineS.Date = new global::System.DateTime (0);
			this.dateDeadlineS.IsEditable = true;
			this.dateDeadlineS.AutoSeparation = true;
			this.hbox4.Add (this.dateDeadlineS);
			global::Gtk.Box.BoxChild w14 = ((global::Gtk.Box.BoxChild)(this.hbox4 [this.dateDeadlineS]));
			w14.Position = 0;
			// Container child hbox4.Gtk.Box+BoxChild
			this.label17 = new global::Gtk.Label ();
			this.label17.Name = "label17";
			this.label17.LabelProp = global::Mono.Unix.Catalog.GetString ("-");
			this.hbox4.Add (this.label17);
			global::Gtk.Box.BoxChild w15 = ((global::Gtk.Box.BoxChild)(this.hbox4 [this.label17]));
			w15.Position = 1;
			w15.Expand = false;
			w15.Fill = false;
			// Container child hbox4.Gtk.Box+BoxChild
			this.dateDeadlineE = new global::QSWidgetLib.DatePicker ();
			this.dateDeadlineE.Events = ((global::Gdk.EventMask)(256));
			this.dateDeadlineE.Name = "dateDeadlineE";
			this.dateDeadlineE.Date = new global::System.DateTime (0);
			this.dateDeadlineE.IsEditable = true;
			this.dateDeadlineE.AutoSeparation = true;
			this.hbox4.Add (this.dateDeadlineE);
			global::Gtk.Box.BoxChild w16 = ((global::Gtk.Box.BoxChild)(this.hbox4 [this.dateDeadlineE]));
			w16.Position = 2;
			this.table3.Add (this.hbox4);
			global::Gtk.Table.TableChild w17 = ((global::Gtk.Table.TableChild)(this.table3 [this.hbox4]));
			w17.TopAttach = ((uint)(7));
			w17.BottomAttach = ((uint)(8));
			w17.LeftAttach = ((uint)(1));
			w17.RightAttach = ((uint)(2));
			w17.XOptions = ((global::Gtk.AttachOptions)(4));
			w17.YOptions = ((global::Gtk.AttachOptions)(4));
			// Container child table3.Gtk.Table+TableChild
			this.label1 = new global::Gtk.Label ();
			this.label1.Name = "label1";
			this.label1.Xalign = 1F;
			this.label1.LabelProp = global::Mono.Unix.Catalog.GetString ("Договор:");
			this.table3.Add (this.label1);
			global::Gtk.Table.TableChild w18 = ((global::Gtk.Table.TableChild)(this.table3 [this.label1]));
			w18.TopAttach = ((uint)(1));
			w18.BottomAttach = ((uint)(2));
			w18.XOptions = ((global::Gtk.AttachOptions)(4));
			w18.YOptions = ((global::Gtk.AttachOptions)(4));
			// Container child table3.Gtk.Table+TableChild
			this.label10 = new global::Gtk.Label ();
			this.label10.Name = "label10";
			this.label10.Xalign = 1F;
			this.label10.LabelProp = global::Mono.Unix.Catalog.GetString ("Ф.И.О. Заказчика:");
			this.table3.Add (this.label10);
			global::Gtk.Table.TableChild w19 = ((global::Gtk.Table.TableChild)(this.table3 [this.label10]));
			w19.TopAttach = ((uint)(2));
			w19.BottomAttach = ((uint)(3));
			w19.XOptions = ((global::Gtk.AttachOptions)(4));
			w19.YOptions = ((global::Gtk.AttachOptions)(4));
			// Container child table3.Gtk.Table+TableChild
			this.label11 = new global::Gtk.Label ();
			this.label11.Name = "label11";
			this.label11.Xalign = 1F;
			this.label11.LabelProp = global::Mono.Unix.Catalog.GetString ("Дата прихода Заказа:");
			this.table3.Add (this.label11);
			global::Gtk.Table.TableChild w20 = ((global::Gtk.Table.TableChild)(this.table3 [this.label11]));
			w20.TopAttach = ((uint)(6));
			w20.BottomAttach = ((uint)(7));
			w20.XOptions = ((global::Gtk.AttachOptions)(4));
			w20.YOptions = ((global::Gtk.AttachOptions)(4));
			// Container child table3.Gtk.Table+TableChild
			this.label12 = new global::Gtk.Label ();
			this.label12.Name = "label12";
			this.label12.Xalign = 1F;
			this.label12.LabelProp = global::Mono.Unix.Catalog.GetString ("Дата сдачи Заказа:");
			this.table3.Add (this.label12);
			global::Gtk.Table.TableChild w21 = ((global::Gtk.Table.TableChild)(this.table3 [this.label12]));
			w21.TopAttach = ((uint)(7));
			w21.BottomAttach = ((uint)(8));
			w21.XOptions = ((global::Gtk.AttachOptions)(4));
			w21.YOptions = ((global::Gtk.AttachOptions)(4));
			// Container child table3.Gtk.Table+TableChild
			this.label13 = new global::Gtk.Label ();
			this.label13.Name = "label13";
			this.label13.Xalign = 1F;
			this.label13.Yalign = 0F;
			this.label13.LabelProp = global::Mono.Unix.Catalog.GetString ("Примечания:");
			this.table3.Add (this.label13);
			global::Gtk.Table.TableChild w22 = ((global::Gtk.Table.TableChild)(this.table3 [this.label13]));
			w22.TopAttach = ((uint)(8));
			w22.BottomAttach = ((uint)(9));
			w22.XOptions = ((global::Gtk.AttachOptions)(4));
			w22.YOptions = ((global::Gtk.AttachOptions)(4));
			// Container child table3.Gtk.Table+TableChild
			this.label14 = new global::Gtk.Label ();
			this.label14.Name = "label14";
			this.label14.Xalign = 1F;
			this.label14.LabelProp = global::Mono.Unix.Catalog.GetString ("Дополнительный телефон:");
			this.table3.Add (this.label14);
			global::Gtk.Table.TableChild w23 = ((global::Gtk.Table.TableChild)(this.table3 [this.label14]));
			w23.TopAttach = ((uint)(5));
			w23.BottomAttach = ((uint)(6));
			w23.XOptions = ((global::Gtk.AttachOptions)(4));
			w23.YOptions = ((global::Gtk.AttachOptions)(4));
			// Container child table3.Gtk.Table+TableChild
			this.label4 = new global::Gtk.Label ();
			this.label4.Name = "label4";
			this.label4.Xalign = 1F;
			this.label4.LabelProp = global::Mono.Unix.Catalog.GetString ("Выставка:");
			this.table3.Add (this.label4);
			global::Gtk.Table.TableChild w24 = ((global::Gtk.Table.TableChild)(this.table3 [this.label4]));
			w24.TopAttach = ((uint)(9));
			w24.BottomAttach = ((uint)(10));
			w24.XOptions = ((global::Gtk.AttachOptions)(4));
			w24.YOptions = ((global::Gtk.AttachOptions)(4));
			// Container child table3.Gtk.Table+TableChild
			this.label6 = new global::Gtk.Label ();
			this.label6.Name = "label6";
			this.label6.Xalign = 1F;
			this.label6.Yalign = 0F;
			this.label6.LabelProp = global::Mono.Unix.Catalog.GetString ("Адрес доставки:");
			this.table3.Add (this.label6);
			global::Gtk.Table.TableChild w25 = ((global::Gtk.Table.TableChild)(this.table3 [this.label6]));
			w25.TopAttach = ((uint)(3));
			w25.BottomAttach = ((uint)(4));
			w25.XOptions = ((global::Gtk.AttachOptions)(4));
			w25.YOptions = ((global::Gtk.AttachOptions)(4));
			// Container child table3.Gtk.Table+TableChild
			this.label9 = new global::Gtk.Label ();
			this.label9.Name = "label9";
			this.label9.Xalign = 1F;
			this.label9.LabelProp = global::Mono.Unix.Catalog.GetString ("Основной телефон:");
			this.table3.Add (this.label9);
			global::Gtk.Table.TableChild w26 = ((global::Gtk.Table.TableChild)(this.table3 [this.label9]));
			w26.TopAttach = ((uint)(4));
			w26.BottomAttach = ((uint)(5));
			w26.XOptions = ((global::Gtk.AttachOptions)(4));
			w26.YOptions = ((global::Gtk.AttachOptions)(4));
			this.hbox1.Add (this.table3);
			global::Gtk.Box.BoxChild w27 = ((global::Gtk.Box.BoxChild)(this.hbox1 [this.table3]));
			w27.Position = 0;
			this.notebook1.Add (this.hbox1);
			// Notebook tab
			this.label7 = new global::Gtk.Label ();
			this.label7.Name = "label7";
			this.label7.LabelProp = global::Mono.Unix.Catalog.GetString ("Информация");
			this.notebook1.SetTabLabel (this.hbox1, this.label7);
			this.label7.ShowAll ();
			// Container child notebook1.Gtk.Notebook+NotebookChild
			this.vbox7 = new global::Gtk.VBox ();
			this.vbox7.Name = "vbox7";
			this.vbox7.Spacing = 6;
			// Container child vbox7.Gtk.Box+BoxChild
			this.table1 = new global::Gtk.Table (((uint)(2)), ((uint)(3)), false);
			this.table1.Name = "table1";
			this.table1.RowSpacing = ((uint)(6));
			this.table1.ColumnSpacing = ((uint)(6));
			// Container child table1.Gtk.Table+TableChild
			this.checkCuttingBase = new global::Gtk.CheckButton ();
			this.checkCuttingBase.CanFocus = true;
			this.checkCuttingBase.Name = "checkCuttingBase";
			this.checkCuttingBase.Label = global::Mono.Unix.Catalog.GetString ("Усечённое основание");
			this.checkCuttingBase.DrawIndicator = true;
			this.checkCuttingBase.UseUnderline = true;
			this.table1.Add (this.checkCuttingBase);
			global::Gtk.Table.TableChild w29 = ((global::Gtk.Table.TableChild)(this.table1 [this.checkCuttingBase]));
			w29.TopAttach = ((uint)(1));
			w29.BottomAttach = ((uint)(2));
			w29.LeftAttach = ((uint)(2));
			w29.RightAttach = ((uint)(3));
			w29.YOptions = ((global::Gtk.AttachOptions)(4));
			// Container child table1.Gtk.Table+TableChild
			this.comboCubeH = global::Gtk.ComboBox.NewText ();
			this.comboCubeH.AppendText (global::Mono.Unix.Catalog.GetString ("1"));
			this.comboCubeH.AppendText (global::Mono.Unix.Catalog.GetString ("2"));
			this.comboCubeH.AppendText (global::Mono.Unix.Catalog.GetString ("3"));
			this.comboCubeH.AppendText (global::Mono.Unix.Catalog.GetString ("4"));
			this.comboCubeH.AppendText (global::Mono.Unix.Catalog.GetString ("5"));
			this.comboCubeH.AppendText (global::Mono.Unix.Catalog.GetString ("6"));
			this.comboCubeH.AppendText (global::Mono.Unix.Catalog.GetString ("7"));
			this.comboCubeH.AppendText (global::Mono.Unix.Catalog.GetString ("8"));
			this.comboCubeH.AppendText (global::Mono.Unix.Catalog.GetString ("9"));
			this.comboCubeH.AppendText (global::Mono.Unix.Catalog.GetString ("10"));
			this.comboCubeH.AppendText (global::Mono.Unix.Catalog.GetString ("11"));
			this.comboCubeH.AppendText (global::Mono.Unix.Catalog.GetString ("12"));
			this.comboCubeH.AppendText (global::Mono.Unix.Catalog.GetString ("13"));
			this.comboCubeH.AppendText (global::Mono.Unix.Catalog.GetString ("14"));
			this.comboCubeH.AppendText (global::Mono.Unix.Catalog.GetString ("15"));
			this.comboCubeH.Name = "comboCubeH";
			this.comboCubeH.Active = 0;
			this.table1.Add (this.comboCubeH);
			global::Gtk.Table.TableChild w30 = ((global::Gtk.Table.TableChild)(this.table1 [this.comboCubeH]));
			w30.LeftAttach = ((uint)(1));
			w30.RightAttach = ((uint)(2));
			w30.XOptions = ((global::Gtk.AttachOptions)(4));
			w30.YOptions = ((global::Gtk.AttachOptions)(4));
			// Container child table1.Gtk.Table+TableChild
			this.comboCubeV = global::Gtk.ComboBox.NewText ();
			this.comboCubeV.AppendText (global::Mono.Unix.Catalog.GetString ("1"));
			this.comboCubeV.AppendText (global::Mono.Unix.Catalog.GetString ("2"));
			this.comboCubeV.AppendText (global::Mono.Unix.Catalog.GetString ("3"));
			this.comboCubeV.AppendText (global::Mono.Unix.Catalog.GetString ("4"));
			this.comboCubeV.AppendText (global::Mono.Unix.Catalog.GetString ("5"));
			this.comboCubeV.AppendText (global::Mono.Unix.Catalog.GetString ("6"));
			this.comboCubeV.AppendText (global::Mono.Unix.Catalog.GetString ("7"));
			this.comboCubeV.AppendText (global::Mono.Unix.Catalog.GetString ("8"));
			this.comboCubeV.AppendText (global::Mono.Unix.Catalog.GetString ("9"));
			this.comboCubeV.AppendText (global::Mono.Unix.Catalog.GetString ("10"));
			this.comboCubeV.Name = "comboCubeV";
			this.comboCubeV.Active = 0;
			this.table1.Add (this.comboCubeV);
			global::Gtk.Table.TableChild w31 = ((global::Gtk.Table.TableChild)(this.table1 [this.comboCubeV]));
			w31.TopAttach = ((uint)(1));
			w31.BottomAttach = ((uint)(2));
			w31.LeftAttach = ((uint)(1));
			w31.RightAttach = ((uint)(2));
			w31.XOptions = ((global::Gtk.AttachOptions)(4));
			w31.YOptions = ((global::Gtk.AttachOptions)(4));
			// Container child table1.Gtk.Table+TableChild
			this.label2 = new global::Gtk.Label ();
			this.label2.Name = "label2";
			this.label2.Xalign = 1F;
			this.label2.LabelProp = global::Mono.Unix.Catalog.GetString ("Кубов по вертикали:");
			this.label2.UseMarkup = true;
			this.table1.Add (this.label2);
			global::Gtk.Table.TableChild w32 = ((global::Gtk.Table.TableChild)(this.table1 [this.label2]));
			w32.TopAttach = ((uint)(1));
			w32.BottomAttach = ((uint)(2));
			w32.XOptions = ((global::Gtk.AttachOptions)(4));
			w32.YOptions = ((global::Gtk.AttachOptions)(4));
			// Container child table1.Gtk.Table+TableChild
			this.label3 = new global::Gtk.Label ();
			this.label3.Name = "label3";
			this.label3.Xalign = 1F;
			this.label3.LabelProp = global::Mono.Unix.Catalog.GetString ("Кубов по горизонтали:");
			this.label3.UseMarkup = true;
			this.table1.Add (this.label3);
			global::Gtk.Table.TableChild w33 = ((global::Gtk.Table.TableChild)(this.table1 [this.label3]));
			w33.XOptions = ((global::Gtk.AttachOptions)(4));
			w33.YOptions = ((global::Gtk.AttachOptions)(4));
			this.vbox7.Add (this.table1);
			global::Gtk.Box.BoxChild w34 = ((global::Gtk.Box.BoxChild)(this.vbox7 [this.table1]));
			w34.Position = 0;
			w34.Expand = false;
			w34.Fill = false;
			// Container child vbox7.Gtk.Box+BoxChild
			this.scrolledTypesH = new global::Gtk.ScrolledWindow ();
			this.scrolledTypesH.CanFocus = true;
			this.scrolledTypesH.Name = "scrolledTypesH";
			this.scrolledTypesH.VscrollbarPolicy = ((global::Gtk.PolicyType)(2));
			this.scrolledTypesH.ShadowType = ((global::Gtk.ShadowType)(1));
			this.vbox7.Add (this.scrolledTypesH);
			global::Gtk.Box.BoxChild w35 = ((global::Gtk.Box.BoxChild)(this.vbox7 [this.scrolledTypesH]));
			w35.Position = 1;
			this.notebook1.Add (this.vbox7);
			global::Gtk.Notebook.NotebookChild w36 = ((global::Gtk.Notebook.NotebookChild)(this.notebook1 [this.vbox7]));
			w36.Position = 1;
			// Notebook tab
			this.label5 = new global::Gtk.Label ();
			this.label5.Name = "label5";
			this.label5.LabelProp = global::Mono.Unix.Catalog.GetString ("ШАГ-1 (выбор каркаса)");
			this.notebook1.SetTabLabel (this.vbox7, this.label5);
			this.label5.ShowAll ();
			// Container child notebook1.Gtk.Notebook+NotebookChild
			this.vbox3 = new global::Gtk.VBox ();
			this.vbox3.Name = "vbox3";
			this.vbox3.Spacing = 6;
			// Container child vbox3.Gtk.Box+BoxChild
			this.tableConstructor = new global::Gtk.Table (((uint)(2)), ((uint)(2)), false);
			this.tableConstructor.Name = "tableConstructor";
			this.tableConstructor.RowSpacing = ((uint)(6));
			this.tableConstructor.ColumnSpacing = ((uint)(6));
			// Container child tableConstructor.Gtk.Table+TableChild
			this.buttonCubeListOrientation = new global::Gtk.Button ();
			this.buttonCubeListOrientation.TooltipMarkup = "Изменить положение листа";
			this.buttonCubeListOrientation.CanFocus = true;
			this.buttonCubeListOrientation.Name = "buttonCubeListOrientation";
			this.buttonCubeListOrientation.UseUnderline = true;
			this.buttonCubeListOrientation.Relief = ((global::Gtk.ReliefStyle)(2));
			global::Gtk.Image w37 = new global::Gtk.Image ();
			w37.Pixbuf = global::Gdk.Pixbuf.LoadFromResource ("CupboardDesigner.icons.emblem-synchronizing.png");
			this.buttonCubeListOrientation.Image = w37;
			this.tableConstructor.Add (this.buttonCubeListOrientation);
			global::Gtk.Table.TableChild w38 = ((global::Gtk.Table.TableChild)(this.tableConstructor [this.buttonCubeListOrientation]));
			w38.TopAttach = ((uint)(1));
			w38.BottomAttach = ((uint)(2));
			w38.LeftAttach = ((uint)(1));
			w38.RightAttach = ((uint)(2));
			w38.XOptions = ((global::Gtk.AttachOptions)(4));
			w38.YOptions = ((global::Gtk.AttachOptions)(4));
			// Container child tableConstructor.Gtk.Table+TableChild
			this.drawCupboard = new global::Gtk.DrawingArea ();
			this.drawCupboard.Name = "drawCupboard";
			this.tableConstructor.Add (this.drawCupboard);
			// Container child tableConstructor.Gtk.Table+TableChild
			this.scrolledCubeListH = new global::Gtk.ScrolledWindow ();
			this.scrolledCubeListH.CanFocus = true;
			this.scrolledCubeListH.Name = "scrolledCubeListH";
			this.scrolledCubeListH.VscrollbarPolicy = ((global::Gtk.PolicyType)(2));
			this.scrolledCubeListH.ShadowType = ((global::Gtk.ShadowType)(1));
			this.tableConstructor.Add (this.scrolledCubeListH);
			global::Gtk.Table.TableChild w40 = ((global::Gtk.Table.TableChild)(this.tableConstructor [this.scrolledCubeListH]));
			w40.TopAttach = ((uint)(1));
			w40.BottomAttach = ((uint)(2));
			w40.YOptions = ((global::Gtk.AttachOptions)(6));
			// Container child tableConstructor.Gtk.Table+TableChild
			this.scrolledCubeListV = new global::Gtk.ScrolledWindow ();
			this.scrolledCubeListV.CanFocus = true;
			this.scrolledCubeListV.Name = "scrolledCubeListV";
			this.scrolledCubeListV.VscrollbarPolicy = ((global::Gtk.PolicyType)(0));
			this.scrolledCubeListV.HscrollbarPolicy = ((global::Gtk.PolicyType)(2));
			this.scrolledCubeListV.ShadowType = ((global::Gtk.ShadowType)(1));
			this.tableConstructor.Add (this.scrolledCubeListV);
			global::Gtk.Table.TableChild w41 = ((global::Gtk.Table.TableChild)(this.tableConstructor [this.scrolledCubeListV]));
			w41.LeftAttach = ((uint)(1));
			w41.RightAttach = ((uint)(2));
			w41.XOptions = ((global::Gtk.AttachOptions)(4));
			this.vbox3.Add (this.tableConstructor);
			global::Gtk.Box.BoxChild w42 = ((global::Gtk.Box.BoxChild)(this.vbox3 [this.tableConstructor]));
			w42.PackType = ((global::Gtk.PackType)(1));
			w42.Position = 0;
			this.notebook1.Add (this.vbox3);
			global::Gtk.Notebook.NotebookChild w43 = ((global::Gtk.Notebook.NotebookChild)(this.notebook1 [this.vbox3]));
			w43.Position = 2;
			// Notebook tab
			this.label8 = new global::Gtk.Label ();
			this.label8.Name = "label8";
			this.label8.LabelProp = global::Mono.Unix.Catalog.GetString ("ШАГ-2 (выбор заполнения)");
			this.notebook1.SetTabLabel (this.vbox3, this.label8);
			this.label8.ShowAll ();
			// Container child notebook1.Gtk.Notebook+NotebookChild
			this.vbox5 = new global::Gtk.VBox ();
			this.vbox5.Name = "vbox5";
			this.vbox5.Spacing = 6;
			// Container child vbox5.Gtk.Box+BoxChild
			this.GtkScrolledWindow1 = new global::Gtk.ScrolledWindow ();
			this.GtkScrolledWindow1.Name = "GtkScrolledWindow1";
			this.GtkScrolledWindow1.ShadowType = ((global::Gtk.ShadowType)(1));
			// Container child GtkScrolledWindow1.Gtk.Container+ContainerChild
			this.treeviewComponents = new global::Gtk.TreeView ();
			this.treeviewComponents.CanFocus = true;
			this.treeviewComponents.Name = "treeviewComponents";
			this.GtkScrolledWindow1.Add (this.treeviewComponents);
			this.vbox5.Add (this.GtkScrolledWindow1);
			global::Gtk.Box.BoxChild w45 = ((global::Gtk.Box.BoxChild)(this.vbox5 [this.GtkScrolledWindow1]));
			w45.Position = 0;
			// Container child vbox5.Gtk.Box+BoxChild
			this.hbox6 = new global::Gtk.HBox ();
			this.hbox6.Name = "hbox6";
			this.hbox6.Spacing = 6;
			// Container child hbox6.Gtk.Box+BoxChild
			this.checkbuttonShowPrice = new global::Gtk.CheckButton ();
			this.checkbuttonShowPrice.CanFocus = true;
			this.checkbuttonShowPrice.Name = "checkbuttonShowPrice";
			this.checkbuttonShowPrice.Label = global::Mono.Unix.Catalog.GetString ("Показать расчет стоимости");
			this.checkbuttonShowPrice.Active = true;
			this.checkbuttonShowPrice.DrawIndicator = true;
			this.checkbuttonShowPrice.UseUnderline = true;
			this.hbox6.Add (this.checkbuttonShowPrice);
			global::Gtk.Box.BoxChild w46 = ((global::Gtk.Box.BoxChild)(this.hbox6 [this.checkbuttonShowPrice]));
			w46.Position = 0;
			// Container child hbox6.Gtk.Box+BoxChild
			this.checkbuttonDiscount = new global::Gtk.CheckButton ();
			this.checkbuttonDiscount.CanFocus = true;
			this.checkbuttonDiscount.Name = "checkbuttonDiscount";
			this.checkbuttonDiscount.Label = global::Mono.Unix.Catalog.GetString ("Скидка:");
			this.checkbuttonDiscount.DrawIndicator = true;
			this.checkbuttonDiscount.UseUnderline = true;
			this.hbox6.Add (this.checkbuttonDiscount);
			global::Gtk.Box.BoxChild w47 = ((global::Gtk.Box.BoxChild)(this.hbox6 [this.checkbuttonDiscount]));
			w47.Position = 1;
			w47.Expand = false;
			// Container child hbox6.Gtk.Box+BoxChild
			this.spinbutton1 = new global::Gtk.SpinButton (-100, 100, 1);
			this.spinbutton1.CanDefault = true;
			this.spinbutton1.CanFocus = true;
			this.spinbutton1.Name = "spinbutton1";
			this.spinbutton1.Adjustment.PageIncrement = 10;
			this.spinbutton1.ClimbRate = 1;
			this.spinbutton1.Numeric = true;
			this.spinbutton1.UpdatePolicy = ((global::Gtk.SpinButtonUpdatePolicy)(1));
			this.hbox6.Add (this.spinbutton1);
			global::Gtk.Box.BoxChild w48 = ((global::Gtk.Box.BoxChild)(this.hbox6 [this.spinbutton1]));
			w48.Position = 2;
			w48.Expand = false;
			w48.Fill = false;
			// Container child hbox6.Gtk.Box+BoxChild
			this.label18 = new global::Gtk.Label ();
			this.label18.Name = "label18";
			this.label18.LabelProp = global::Mono.Unix.Catalog.GetString ("%        ");
			this.hbox6.Add (this.label18);
			global::Gtk.Box.BoxChild w49 = ((global::Gtk.Box.BoxChild)(this.hbox6 [this.label18]));
			w49.Position = 3;
			w49.Expand = false;
			w49.Fill = false;
			// Container child hbox6.Gtk.Box+BoxChild
			this.labelTotalCount = new global::Gtk.Label ();
			this.labelTotalCount.Name = "labelTotalCount";
			this.labelTotalCount.Xalign = 1F;
			this.labelTotalCount.LabelProp = global::Mono.Unix.Catalog.GetString ("label2");
			this.hbox6.Add (this.labelTotalCount);
			global::Gtk.Box.BoxChild w50 = ((global::Gtk.Box.BoxChild)(this.hbox6 [this.labelTotalCount]));
			w50.Position = 4;
			w50.Expand = false;
			w50.Fill = false;
			this.vbox5.Add (this.hbox6);
			global::Gtk.Box.BoxChild w51 = ((global::Gtk.Box.BoxChild)(this.vbox5 [this.hbox6]));
			w51.Position = 1;
			w51.Expand = false;
			w51.Fill = false;
			this.notebook1.Add (this.vbox5);
			global::Gtk.Notebook.NotebookChild w52 = ((global::Gtk.Notebook.NotebookChild)(this.notebook1 [this.vbox5]));
			w52.Position = 3;
			// Notebook tab
			this.label15 = new global::Gtk.Label ();
			this.label15.Name = "label15";
			this.label15.LabelProp = global::Mono.Unix.Catalog.GetString ("ШАГ-3 (выбор отделки)");
			this.notebook1.SetTabLabel (this.vbox5, this.label15);
			this.label15.ShowAll ();
			// Container child notebook1.Gtk.Notebook+NotebookChild
			this.reportviewer1 = new global::fyiReporting.RdlGtkViewer.ReportViewer ();
			this.reportviewer1.WidthRequest = 0;
			this.reportviewer1.HeightRequest = 0;
			this.reportviewer1.Events = ((global::Gdk.EventMask)(256));
			this.reportviewer1.Name = "reportviewer1";
			this.reportviewer1.ShowErrors = false;
			this.reportviewer1.ShowParameters = false;
			this.notebook1.Add (this.reportviewer1);
			global::Gtk.Notebook.NotebookChild w53 = ((global::Gtk.Notebook.NotebookChild)(this.notebook1 [this.reportviewer1]));
			w53.Position = 4;
			// Notebook tab
			this.label16 = new global::Gtk.Label ();
			this.label16.Name = "label16";
			this.label16.LabelProp = global::Mono.Unix.Catalog.GetString ("ШАГ-4 (печать заказа)");
			this.notebook1.SetTabLabel (this.reportviewer1, this.label16);
			this.label16.ShowAll ();
			// Container child notebook1.Gtk.Notebook+NotebookChild
			this.reportviewer2 = new global::fyiReporting.RdlGtkViewer.ReportViewer ();
			this.reportviewer2.WidthRequest = 0;
			this.reportviewer2.HeightRequest = 0;
			this.reportviewer2.Events = ((global::Gdk.EventMask)(256));
			this.reportviewer2.Name = "reportviewer2";
			this.reportviewer2.ShowErrors = false;
			this.reportviewer2.ShowParameters = false;
			this.notebook1.Add (this.reportviewer2);
			global::Gtk.Notebook.NotebookChild w54 = ((global::Gtk.Notebook.NotebookChild)(this.notebook1 [this.reportviewer2]));
			w54.Position = 5;
			// Notebook tab
			this.label19 = new global::Gtk.Label ();
			this.label19.Name = "label19";
			this.label19.LabelProp = global::Mono.Unix.Catalog.GetString ("ШАГ-5 (печать для производства)");
			this.notebook1.SetTabLabel (this.reportviewer2, this.label19);
			this.label19.ShowAll ();
			this.vbox2.Add (this.notebook1);
			global::Gtk.Box.BoxChild w55 = ((global::Gtk.Box.BoxChild)(this.vbox2 [this.notebook1]));
			w55.Position = 1;
			this.Add (this.vbox2);
			if ((this.Child != null)) {
				this.Child.ShowAll ();
			}
			this.DefaultWidth = 1045;
			this.DefaultHeight = 602;
			this.Show ();
			this.revertToSavedAction.Activated += new global::System.EventHandler (this.OnRevertToSavedActionActivated);
			this.goBackAction.Activated += new global::System.EventHandler (this.OnGoBackActionActivated);
			this.goForwardAction.Activated += new global::System.EventHandler (this.OnGoForwardActionActivated);
			this.saveAction.Activated += new global::System.EventHandler (this.OnSaveActionActivated);
			this.notebook1.SwitchPage += new global::Gtk.SwitchPageHandler (this.OnNotebook1SwitchPage);
			this.dateDeadlineS.DateChanged += new global::System.EventHandler (this.OnDateDeadlineSDateChanged);
			this.dateDeadlineE.DateChanged += new global::System.EventHandler (this.OnDateDeadlineEDateChanged);
			this.entryContract.Changed += new global::System.EventHandler (this.OnEntryContractChanged);
			this.checkEstimation.Clicked += new global::System.EventHandler (this.OnCheckEstimationClicked);
			this.comboCubeV.Changed += new global::System.EventHandler (this.OnComboCubeVChanged);
			this.comboCubeH.Changed += new global::System.EventHandler (this.OnComboCubeHChanged);
			this.drawCupboard.ExposeEvent += new global::Gtk.ExposeEventHandler (this.OnDrawCupboardExposeEvent);
			this.drawCupboard.SizeAllocated += new global::Gtk.SizeAllocatedHandler (this.OnDrawCupboardSizeAllocated);
			this.drawCupboard.DragMotion += new global::Gtk.DragMotionHandler (this.OnDrawCupboardDragMotion);
			this.drawCupboard.DragDrop += new global::Gtk.DragDropHandler (this.OnDrawCupboardDragDrop);
			this.drawCupboard.DragBegin += new global::Gtk.DragBeginHandler (this.OnDrawCupboardDragBegin);
			this.drawCupboard.DragDataDelete += new global::Gtk.DragDataDeleteHandler (this.OnDrawCupboardDragDataDelete);
			this.buttonCubeListOrientation.Clicked += new global::System.EventHandler (this.OnButtonCubeListOrientationClicked);
			this.treeviewComponents.ButtonReleaseEvent += new global::Gtk.ButtonReleaseEventHandler (this.OnTreeviewComponentsButtonReleaseEvent);
			this.checkbuttonShowPrice.Toggled += new global::System.EventHandler (this.OnCheckbutton2Toggled);
			this.checkbuttonDiscount.Toggled += new global::System.EventHandler (this.OnCheckbutton1Toggled);
			this.spinbutton1.ValueChanged += new global::System.EventHandler (this.OnSpinbutton1ValueChanged);
		}
	}
}
