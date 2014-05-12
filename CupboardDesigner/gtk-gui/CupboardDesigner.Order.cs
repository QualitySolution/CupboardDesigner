
// This file has been generated by the GUI designer. Do not modify.
namespace CupboardDesigner
{
	public partial class Order
	{
		private global::Gtk.Notebook notebook1;
		private global::Gtk.HBox hbox1;
		private global::Gtk.VBox vbox4;
		private global::Gtk.Table table3;
		private global::Gtk.ComboBox comboCubeH;
		private global::Gtk.ComboBox comboCubeV;
		private global::Gtk.ComboBox comboExhibition;
		private global::QSWidgetLib.DatePicker dateArrval;
		private global::QSWidgetLib.DatePicker dateDelivery;
		private global::Gtk.Entry entryCustomer;
		private global::Gtk.ScrolledWindow GtkScrolledWindow;
		private global::Gtk.TextView textviewComments;
		private global::Gtk.Label label10;
		private global::Gtk.Label label11;
		private global::Gtk.Label label12;
		private global::Gtk.Label label13;
		private global::Gtk.Label label2;
		private global::Gtk.Label label3;
		private global::Gtk.Label label4;
		private global::Gtk.VBox vbox6;
		private global::Gtk.HBox hbox5;
		private global::Gtk.Button buttonTypeListOrient;
		private global::Gtk.ScrolledWindow scrolledTypesH;
		private global::Gtk.ScrolledWindow scrolledTypesV;
		private global::Gtk.Label label7;
		private global::Gtk.VBox vbox2;
		private global::Gtk.HBox hbox6;
		private global::Gtk.Label labelInfo;
		private global::Gtk.Button buttonCubeListOrientation;
		private global::Gtk.Table tableConstructor;
		private global::Gtk.DrawingArea drawCupboard;
		private global::Gtk.ScrolledWindow scrolledCubeListH;
		private global::Gtk.ScrolledWindow scrolledCubeListV;
		private global::Gtk.Label label8;
		private global::Gtk.VBox vbox3;
		private global::Gtk.Label label1;
		private global::Gtk.ScrolledWindow GtkScrolledWindow1;
		private global::Gtk.TreeView treeviewComponents;
		private global::Gtk.Label labelTotalCount;
		private global::Gtk.Label label5;
		private global::Gtk.Button buttonCancel;
		private global::Gtk.Button buttonOk;

		protected virtual void Build ()
		{
			global::Stetic.Gui.Initialize (this);
			// Widget CupboardDesigner.Order
			this.Name = "CupboardDesigner.Order";
			this.WindowPosition = ((global::Gtk.WindowPosition)(4));
			// Internal child CupboardDesigner.Order.VBox
			global::Gtk.VBox w1 = this.VBox;
			w1.Name = "dialog1_VBox";
			w1.BorderWidth = ((uint)(2));
			// Container child dialog1_VBox.Gtk.Box+BoxChild
			this.notebook1 = new global::Gtk.Notebook ();
			this.notebook1.CanFocus = true;
			this.notebook1.Name = "notebook1";
			this.notebook1.CurrentPage = 2;
			// Container child notebook1.Gtk.Notebook+NotebookChild
			this.hbox1 = new global::Gtk.HBox ();
			this.hbox1.Name = "hbox1";
			this.hbox1.Spacing = 6;
			// Container child hbox1.Gtk.Box+BoxChild
			this.vbox4 = new global::Gtk.VBox ();
			this.vbox4.Name = "vbox4";
			this.vbox4.Spacing = 6;
			// Container child vbox4.Gtk.Box+BoxChild
			this.table3 = new global::Gtk.Table (((uint)(4)), ((uint)(4)), false);
			this.table3.Name = "table3";
			this.table3.RowSpacing = ((uint)(6));
			this.table3.ColumnSpacing = ((uint)(6));
			// Container child table3.Gtk.Table+TableChild
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
			this.comboCubeH.Name = "comboCubeH";
			this.comboCubeH.Active = 0;
			this.table3.Add (this.comboCubeH);
			global::Gtk.Table.TableChild w2 = ((global::Gtk.Table.TableChild)(this.table3 [this.comboCubeH]));
			w2.TopAttach = ((uint)(2));
			w2.BottomAttach = ((uint)(3));
			w2.LeftAttach = ((uint)(3));
			w2.RightAttach = ((uint)(4));
			w2.XOptions = ((global::Gtk.AttachOptions)(4));
			w2.YOptions = ((global::Gtk.AttachOptions)(4));
			// Container child table3.Gtk.Table+TableChild
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
			this.table3.Add (this.comboCubeV);
			global::Gtk.Table.TableChild w3 = ((global::Gtk.Table.TableChild)(this.table3 [this.comboCubeV]));
			w3.TopAttach = ((uint)(1));
			w3.BottomAttach = ((uint)(2));
			w3.LeftAttach = ((uint)(3));
			w3.RightAttach = ((uint)(4));
			w3.XOptions = ((global::Gtk.AttachOptions)(4));
			w3.YOptions = ((global::Gtk.AttachOptions)(4));
			// Container child table3.Gtk.Table+TableChild
			this.comboExhibition = new global::Gtk.ComboBox ();
			this.comboExhibition.Name = "comboExhibition";
			this.table3.Add (this.comboExhibition);
			global::Gtk.Table.TableChild w4 = ((global::Gtk.Table.TableChild)(this.table3 [this.comboExhibition]));
			w4.LeftAttach = ((uint)(3));
			w4.RightAttach = ((uint)(4));
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
			w5.TopAttach = ((uint)(1));
			w5.BottomAttach = ((uint)(2));
			w5.LeftAttach = ((uint)(1));
			w5.RightAttach = ((uint)(2));
			w5.YOptions = ((global::Gtk.AttachOptions)(4));
			// Container child table3.Gtk.Table+TableChild
			this.dateDelivery = new global::QSWidgetLib.DatePicker ();
			this.dateDelivery.Events = ((global::Gdk.EventMask)(256));
			this.dateDelivery.Name = "dateDelivery";
			this.dateDelivery.Date = new global::System.DateTime (0);
			this.dateDelivery.IsEditable = true;
			this.dateDelivery.AutoSeparation = true;
			this.table3.Add (this.dateDelivery);
			global::Gtk.Table.TableChild w6 = ((global::Gtk.Table.TableChild)(this.table3 [this.dateDelivery]));
			w6.TopAttach = ((uint)(2));
			w6.BottomAttach = ((uint)(3));
			w6.LeftAttach = ((uint)(1));
			w6.RightAttach = ((uint)(2));
			w6.YOptions = ((global::Gtk.AttachOptions)(4));
			// Container child table3.Gtk.Table+TableChild
			this.entryCustomer = new global::Gtk.Entry ();
			this.entryCustomer.CanFocus = true;
			this.entryCustomer.Name = "entryCustomer";
			this.entryCustomer.IsEditable = true;
			this.entryCustomer.InvisibleChar = '●';
			this.table3.Add (this.entryCustomer);
			global::Gtk.Table.TableChild w7 = ((global::Gtk.Table.TableChild)(this.table3 [this.entryCustomer]));
			w7.LeftAttach = ((uint)(1));
			w7.RightAttach = ((uint)(2));
			w7.YOptions = ((global::Gtk.AttachOptions)(4));
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
			global::Gtk.Table.TableChild w9 = ((global::Gtk.Table.TableChild)(this.table3 [this.GtkScrolledWindow]));
			w9.TopAttach = ((uint)(3));
			w9.BottomAttach = ((uint)(4));
			w9.LeftAttach = ((uint)(1));
			w9.RightAttach = ((uint)(3));
			w9.YOptions = ((global::Gtk.AttachOptions)(4));
			// Container child table3.Gtk.Table+TableChild
			this.label10 = new global::Gtk.Label ();
			this.label10.Name = "label10";
			this.label10.Xalign = 1F;
			this.label10.LabelProp = global::Mono.Unix.Catalog.GetString ("Клиент:");
			this.table3.Add (this.label10);
			global::Gtk.Table.TableChild w10 = ((global::Gtk.Table.TableChild)(this.table3 [this.label10]));
			w10.XOptions = ((global::Gtk.AttachOptions)(4));
			w10.YOptions = ((global::Gtk.AttachOptions)(4));
			// Container child table3.Gtk.Table+TableChild
			this.label11 = new global::Gtk.Label ();
			this.label11.Name = "label11";
			this.label11.LabelProp = global::Mono.Unix.Catalog.GetString ("Дата прихода:");
			this.table3.Add (this.label11);
			global::Gtk.Table.TableChild w11 = ((global::Gtk.Table.TableChild)(this.table3 [this.label11]));
			w11.TopAttach = ((uint)(1));
			w11.BottomAttach = ((uint)(2));
			w11.XOptions = ((global::Gtk.AttachOptions)(4));
			w11.YOptions = ((global::Gtk.AttachOptions)(4));
			// Container child table3.Gtk.Table+TableChild
			this.label12 = new global::Gtk.Label ();
			this.label12.Name = "label12";
			this.label12.Xalign = 1F;
			this.label12.LabelProp = global::Mono.Unix.Catalog.GetString ("Дата сдачи:");
			this.table3.Add (this.label12);
			global::Gtk.Table.TableChild w12 = ((global::Gtk.Table.TableChild)(this.table3 [this.label12]));
			w12.TopAttach = ((uint)(2));
			w12.BottomAttach = ((uint)(3));
			w12.XOptions = ((global::Gtk.AttachOptions)(4));
			w12.YOptions = ((global::Gtk.AttachOptions)(4));
			// Container child table3.Gtk.Table+TableChild
			this.label13 = new global::Gtk.Label ();
			this.label13.Name = "label13";
			this.label13.Xalign = 1F;
			this.label13.Yalign = 0F;
			this.label13.LabelProp = global::Mono.Unix.Catalog.GetString ("Комментарии:");
			this.table3.Add (this.label13);
			global::Gtk.Table.TableChild w13 = ((global::Gtk.Table.TableChild)(this.table3 [this.label13]));
			w13.TopAttach = ((uint)(3));
			w13.BottomAttach = ((uint)(4));
			w13.XOptions = ((global::Gtk.AttachOptions)(4));
			w13.YOptions = ((global::Gtk.AttachOptions)(4));
			// Container child table3.Gtk.Table+TableChild
			this.label2 = new global::Gtk.Label ();
			this.label2.Name = "label2";
			this.label2.Xalign = 1F;
			this.label2.LabelProp = global::Mono.Unix.Catalog.GetString ("Кубов по вертикали<span foreground=\"red\">*</span>:");
			this.label2.UseMarkup = true;
			this.table3.Add (this.label2);
			global::Gtk.Table.TableChild w14 = ((global::Gtk.Table.TableChild)(this.table3 [this.label2]));
			w14.TopAttach = ((uint)(1));
			w14.BottomAttach = ((uint)(2));
			w14.LeftAttach = ((uint)(2));
			w14.RightAttach = ((uint)(3));
			w14.XOptions = ((global::Gtk.AttachOptions)(4));
			w14.YOptions = ((global::Gtk.AttachOptions)(4));
			// Container child table3.Gtk.Table+TableChild
			this.label3 = new global::Gtk.Label ();
			this.label3.Name = "label3";
			this.label3.Xalign = 1F;
			this.label3.LabelProp = global::Mono.Unix.Catalog.GetString ("Кубов по горизонтали<span foreground=\"red\">*</span>:");
			this.label3.UseMarkup = true;
			this.table3.Add (this.label3);
			global::Gtk.Table.TableChild w15 = ((global::Gtk.Table.TableChild)(this.table3 [this.label3]));
			w15.TopAttach = ((uint)(2));
			w15.BottomAttach = ((uint)(3));
			w15.LeftAttach = ((uint)(2));
			w15.RightAttach = ((uint)(3));
			w15.XOptions = ((global::Gtk.AttachOptions)(4));
			w15.YOptions = ((global::Gtk.AttachOptions)(4));
			// Container child table3.Gtk.Table+TableChild
			this.label4 = new global::Gtk.Label ();
			this.label4.Name = "label4";
			this.label4.Xalign = 1F;
			this.label4.LabelProp = global::Mono.Unix.Catalog.GetString ("Выставка:");
			this.table3.Add (this.label4);
			global::Gtk.Table.TableChild w16 = ((global::Gtk.Table.TableChild)(this.table3 [this.label4]));
			w16.LeftAttach = ((uint)(2));
			w16.RightAttach = ((uint)(3));
			w16.XOptions = ((global::Gtk.AttachOptions)(4));
			w16.YOptions = ((global::Gtk.AttachOptions)(4));
			// Container child table3.Gtk.Table+TableChild
			this.vbox6 = new global::Gtk.VBox ();
			this.vbox6.Name = "vbox6";
			this.vbox6.Spacing = 6;
			// Container child vbox6.Gtk.Box+BoxChild
			this.hbox5 = new global::Gtk.HBox ();
			this.hbox5.Name = "hbox5";
			this.hbox5.Spacing = 6;
			// Container child hbox5.Gtk.Box+BoxChild
			this.buttonTypeListOrient = new global::Gtk.Button ();
			this.buttonTypeListOrient.TooltipMarkup = "Разворот панели";
			this.buttonTypeListOrient.CanFocus = true;
			this.buttonTypeListOrient.Name = "buttonTypeListOrient";
			this.buttonTypeListOrient.UseUnderline = true;
			this.buttonTypeListOrient.Relief = ((global::Gtk.ReliefStyle)(2));
			global::Gtk.Image w17 = new global::Gtk.Image ();
			w17.Pixbuf = global::Gdk.Pixbuf.LoadFromResource ("CupboardDesigner.icons.emblem-synchronizing.png");
			this.buttonTypeListOrient.Image = w17;
			this.hbox5.Add (this.buttonTypeListOrient);
			global::Gtk.Box.BoxChild w18 = ((global::Gtk.Box.BoxChild)(this.hbox5 [this.buttonTypeListOrient]));
			w18.PackType = ((global::Gtk.PackType)(1));
			w18.Position = 0;
			w18.Expand = false;
			w18.Fill = false;
			this.vbox6.Add (this.hbox5);
			global::Gtk.Box.BoxChild w19 = ((global::Gtk.Box.BoxChild)(this.vbox6 [this.hbox5]));
			w19.PackType = ((global::Gtk.PackType)(1));
			w19.Position = 0;
			w19.Expand = false;
			w19.Fill = false;
			this.table3.Add (this.vbox6);
			global::Gtk.Table.TableChild w20 = ((global::Gtk.Table.TableChild)(this.table3 [this.vbox6]));
			w20.TopAttach = ((uint)(3));
			w20.BottomAttach = ((uint)(4));
			w20.LeftAttach = ((uint)(3));
			w20.RightAttach = ((uint)(4));
			w20.XOptions = ((global::Gtk.AttachOptions)(4));
			w20.YOptions = ((global::Gtk.AttachOptions)(4));
			this.vbox4.Add (this.table3);
			global::Gtk.Box.BoxChild w21 = ((global::Gtk.Box.BoxChild)(this.vbox4 [this.table3]));
			w21.Position = 0;
			w21.Expand = false;
			w21.Fill = false;
			// Container child vbox4.Gtk.Box+BoxChild
			this.scrolledTypesH = new global::Gtk.ScrolledWindow ();
			this.scrolledTypesH.CanFocus = true;
			this.scrolledTypesH.Name = "scrolledTypesH";
			this.scrolledTypesH.VscrollbarPolicy = ((global::Gtk.PolicyType)(2));
			this.scrolledTypesH.ShadowType = ((global::Gtk.ShadowType)(1));
			this.vbox4.Add (this.scrolledTypesH);
			global::Gtk.Box.BoxChild w22 = ((global::Gtk.Box.BoxChild)(this.vbox4 [this.scrolledTypesH]));
			w22.Position = 1;
			this.hbox1.Add (this.vbox4);
			global::Gtk.Box.BoxChild w23 = ((global::Gtk.Box.BoxChild)(this.hbox1 [this.vbox4]));
			w23.Position = 0;
			// Container child hbox1.Gtk.Box+BoxChild
			this.scrolledTypesV = new global::Gtk.ScrolledWindow ();
			this.scrolledTypesV.CanFocus = true;
			this.scrolledTypesV.Name = "scrolledTypesV";
			this.scrolledTypesV.HscrollbarPolicy = ((global::Gtk.PolicyType)(2));
			this.scrolledTypesV.ShadowType = ((global::Gtk.ShadowType)(1));
			this.hbox1.Add (this.scrolledTypesV);
			global::Gtk.Box.BoxChild w24 = ((global::Gtk.Box.BoxChild)(this.hbox1 [this.scrolledTypesV]));
			w24.Position = 1;
			w24.Expand = false;
			this.notebook1.Add (this.hbox1);
			// Notebook tab
			this.label7 = new global::Gtk.Label ();
			this.label7.Name = "label7";
			this.label7.LabelProp = global::Mono.Unix.Catalog.GetString ("1. Информация");
			this.notebook1.SetTabLabel (this.hbox1, this.label7);
			this.label7.ShowAll ();
			// Container child notebook1.Gtk.Notebook+NotebookChild
			this.vbox2 = new global::Gtk.VBox ();
			this.vbox2.Name = "vbox2";
			this.vbox2.Spacing = 6;
			// Container child vbox2.Gtk.Box+BoxChild
			this.hbox6 = new global::Gtk.HBox ();
			this.hbox6.Name = "hbox6";
			this.hbox6.Spacing = 6;
			// Container child hbox6.Gtk.Box+BoxChild
			this.labelInfo = new global::Gtk.Label ();
			this.labelInfo.Name = "labelInfo";
			this.labelInfo.Xalign = 0F;
			this.labelInfo.LabelProp = global::Mono.Unix.Catalog.GetString ("label1");
			this.hbox6.Add (this.labelInfo);
			global::Gtk.Box.BoxChild w26 = ((global::Gtk.Box.BoxChild)(this.hbox6 [this.labelInfo]));
			w26.Position = 0;
			w26.Expand = false;
			w26.Fill = false;
			// Container child hbox6.Gtk.Box+BoxChild
			this.buttonCubeListOrientation = new global::Gtk.Button ();
			this.buttonCubeListOrientation.TooltipMarkup = "Изменить положение листа";
			this.buttonCubeListOrientation.CanFocus = true;
			this.buttonCubeListOrientation.Name = "buttonCubeListOrientation";
			this.buttonCubeListOrientation.UseUnderline = true;
			this.buttonCubeListOrientation.Relief = ((global::Gtk.ReliefStyle)(2));
			global::Gtk.Image w27 = new global::Gtk.Image ();
			w27.Pixbuf = global::Gdk.Pixbuf.LoadFromResource ("CupboardDesigner.icons.emblem-synchronizing.png");
			this.buttonCubeListOrientation.Image = w27;
			this.hbox6.Add (this.buttonCubeListOrientation);
			global::Gtk.Box.BoxChild w28 = ((global::Gtk.Box.BoxChild)(this.hbox6 [this.buttonCubeListOrientation]));
			w28.PackType = ((global::Gtk.PackType)(1));
			w28.Position = 1;
			w28.Expand = false;
			w28.Fill = false;
			this.vbox2.Add (this.hbox6);
			global::Gtk.Box.BoxChild w29 = ((global::Gtk.Box.BoxChild)(this.vbox2 [this.hbox6]));
			w29.Position = 0;
			w29.Expand = false;
			w29.Fill = false;
			// Container child vbox2.Gtk.Box+BoxChild
			this.tableConstructor = new global::Gtk.Table (((uint)(2)), ((uint)(2)), false);
			this.tableConstructor.Name = "tableConstructor";
			this.tableConstructor.RowSpacing = ((uint)(6));
			this.tableConstructor.ColumnSpacing = ((uint)(6));
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
			global::Gtk.Table.TableChild w31 = ((global::Gtk.Table.TableChild)(this.tableConstructor [this.scrolledCubeListH]));
			w31.TopAttach = ((uint)(1));
			w31.BottomAttach = ((uint)(2));
			w31.YOptions = ((global::Gtk.AttachOptions)(6));
			// Container child tableConstructor.Gtk.Table+TableChild
			this.scrolledCubeListV = new global::Gtk.ScrolledWindow ();
			this.scrolledCubeListV.CanFocus = true;
			this.scrolledCubeListV.Name = "scrolledCubeListV";
			this.scrolledCubeListV.VscrollbarPolicy = ((global::Gtk.PolicyType)(0));
			this.scrolledCubeListV.HscrollbarPolicy = ((global::Gtk.PolicyType)(2));
			this.scrolledCubeListV.ShadowType = ((global::Gtk.ShadowType)(1));
			this.tableConstructor.Add (this.scrolledCubeListV);
			global::Gtk.Table.TableChild w32 = ((global::Gtk.Table.TableChild)(this.tableConstructor [this.scrolledCubeListV]));
			w32.LeftAttach = ((uint)(1));
			w32.RightAttach = ((uint)(2));
			w32.XOptions = ((global::Gtk.AttachOptions)(4));
			this.vbox2.Add (this.tableConstructor);
			global::Gtk.Box.BoxChild w33 = ((global::Gtk.Box.BoxChild)(this.vbox2 [this.tableConstructor]));
			w33.PackType = ((global::Gtk.PackType)(1));
			w33.Position = 1;
			this.notebook1.Add (this.vbox2);
			global::Gtk.Notebook.NotebookChild w34 = ((global::Gtk.Notebook.NotebookChild)(this.notebook1 [this.vbox2]));
			w34.Position = 1;
			// Notebook tab
			this.label8 = new global::Gtk.Label ();
			this.label8.Name = "label8";
			this.label8.LabelProp = global::Mono.Unix.Catalog.GetString ("2. Конструктор");
			this.notebook1.SetTabLabel (this.vbox2, this.label8);
			this.label8.ShowAll ();
			// Container child notebook1.Gtk.Notebook+NotebookChild
			this.vbox3 = new global::Gtk.VBox ();
			this.vbox3.Name = "vbox3";
			this.vbox3.Spacing = 6;
			// Container child vbox3.Gtk.Box+BoxChild
			this.label1 = new global::Gtk.Label ();
			this.label1.Name = "label1";
			this.label1.LabelProp = global::Mono.Unix.Catalog.GetString ("На этой странице для каждого элемента шкафа можно указать ма\nтериал и отделку.");
			this.vbox3.Add (this.label1);
			global::Gtk.Box.BoxChild w35 = ((global::Gtk.Box.BoxChild)(this.vbox3 [this.label1]));
			w35.Position = 0;
			w35.Expand = false;
			w35.Fill = false;
			// Container child vbox3.Gtk.Box+BoxChild
			this.GtkScrolledWindow1 = new global::Gtk.ScrolledWindow ();
			this.GtkScrolledWindow1.Name = "GtkScrolledWindow1";
			this.GtkScrolledWindow1.ShadowType = ((global::Gtk.ShadowType)(1));
			// Container child GtkScrolledWindow1.Gtk.Container+ContainerChild
			this.treeviewComponents = new global::Gtk.TreeView ();
			this.treeviewComponents.CanFocus = true;
			this.treeviewComponents.Name = "treeviewComponents";
			this.GtkScrolledWindow1.Add (this.treeviewComponents);
			this.vbox3.Add (this.GtkScrolledWindow1);
			global::Gtk.Box.BoxChild w37 = ((global::Gtk.Box.BoxChild)(this.vbox3 [this.GtkScrolledWindow1]));
			w37.Position = 1;
			// Container child vbox3.Gtk.Box+BoxChild
			this.labelTotalCount = new global::Gtk.Label ();
			this.labelTotalCount.Name = "labelTotalCount";
			this.labelTotalCount.Xalign = 1F;
			this.labelTotalCount.LabelProp = global::Mono.Unix.Catalog.GetString ("label2");
			this.vbox3.Add (this.labelTotalCount);
			global::Gtk.Box.BoxChild w38 = ((global::Gtk.Box.BoxChild)(this.vbox3 [this.labelTotalCount]));
			w38.Position = 2;
			w38.Expand = false;
			w38.Fill = false;
			this.notebook1.Add (this.vbox3);
			global::Gtk.Notebook.NotebookChild w39 = ((global::Gtk.Notebook.NotebookChild)(this.notebook1 [this.vbox3]));
			w39.Position = 2;
			// Notebook tab
			this.label5 = new global::Gtk.Label ();
			this.label5.Name = "label5";
			this.label5.LabelProp = global::Mono.Unix.Catalog.GetString ("3. Отделка");
			this.notebook1.SetTabLabel (this.vbox3, this.label5);
			this.label5.ShowAll ();
			w1.Add (this.notebook1);
			global::Gtk.Box.BoxChild w40 = ((global::Gtk.Box.BoxChild)(w1 [this.notebook1]));
			w40.Position = 0;
			// Internal child CupboardDesigner.Order.ActionArea
			global::Gtk.HButtonBox w41 = this.ActionArea;
			w41.Name = "dialog1_ActionArea";
			w41.Spacing = 10;
			w41.BorderWidth = ((uint)(5));
			w41.LayoutStyle = ((global::Gtk.ButtonBoxStyle)(4));
			// Container child dialog1_ActionArea.Gtk.ButtonBox+ButtonBoxChild
			this.buttonCancel = new global::Gtk.Button ();
			this.buttonCancel.CanDefault = true;
			this.buttonCancel.CanFocus = true;
			this.buttonCancel.Name = "buttonCancel";
			this.buttonCancel.UseStock = true;
			this.buttonCancel.UseUnderline = true;
			this.buttonCancel.Label = "gtk-cancel";
			this.AddActionWidget (this.buttonCancel, -6);
			global::Gtk.ButtonBox.ButtonBoxChild w42 = ((global::Gtk.ButtonBox.ButtonBoxChild)(w41 [this.buttonCancel]));
			w42.Expand = false;
			w42.Fill = false;
			// Container child dialog1_ActionArea.Gtk.ButtonBox+ButtonBoxChild
			this.buttonOk = new global::Gtk.Button ();
			this.buttonOk.CanDefault = true;
			this.buttonOk.CanFocus = true;
			this.buttonOk.Name = "buttonOk";
			this.buttonOk.UseStock = true;
			this.buttonOk.UseUnderline = true;
			this.buttonOk.Label = "gtk-ok";
			w41.Add (this.buttonOk);
			global::Gtk.ButtonBox.ButtonBoxChild w43 = ((global::Gtk.ButtonBox.ButtonBoxChild)(w41 [this.buttonOk]));
			w43.Position = 1;
			w43.Expand = false;
			w43.Fill = false;
			if ((this.Child != null)) {
				this.Child.ShowAll ();
			}
			this.DefaultWidth = 689;
			this.DefaultHeight = 511;
			this.Show ();
			this.notebook1.SwitchPage += new global::Gtk.SwitchPageHandler (this.OnNotebook1SwitchPage);
			this.buttonTypeListOrient.Clicked += new global::System.EventHandler (this.OnButtonTypeListOrientClicked);
			this.comboCubeV.Changed += new global::System.EventHandler (this.OnComboCubeVChanged);
			this.comboCubeH.Changed += new global::System.EventHandler (this.OnComboCubeHChanged);
			this.buttonCubeListOrientation.Clicked += new global::System.EventHandler (this.OnButtonCubeListOrientationClicked);
			this.drawCupboard.ExposeEvent += new global::Gtk.ExposeEventHandler (this.OnDrawCupboardExposeEvent);
			this.drawCupboard.SizeAllocated += new global::Gtk.SizeAllocatedHandler (this.OnDrawCupboardSizeAllocated);
			this.drawCupboard.DragMotion += new global::Gtk.DragMotionHandler (this.OnDrawCupboardDragMotion);
			this.drawCupboard.DragDrop += new global::Gtk.DragDropHandler (this.OnDrawCupboardDragDrop);
			this.drawCupboard.DragBegin += new global::Gtk.DragBeginHandler (this.OnDrawCupboardDragBegin);
			this.drawCupboard.DragDataDelete += new global::Gtk.DragDataDeleteHandler (this.OnDrawCupboardDragDataDelete);
			this.drawCupboard.MotionNotifyEvent += new global::Gtk.MotionNotifyEventHandler (this.OnDrawCupboardMotionNotifyEvent);
			this.drawCupboard.ButtonPressEvent += new global::Gtk.ButtonPressEventHandler (this.OnDrawCupboardButtonPressEvent);
			this.buttonOk.Clicked += new global::System.EventHandler (this.OnButtonOkClicked);
		}
	}
}