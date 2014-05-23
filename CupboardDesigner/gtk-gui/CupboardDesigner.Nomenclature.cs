
// This file has been generated by the GUI designer. Do not modify.
namespace CupboardDesigner
{
	public partial class Nomenclature
	{
		private global::Gtk.HBox hbox1;
		private global::Gtk.Table table1;
		private global::Gtk.ComboBox comboType;
		private global::Gtk.Entry entryArticle;
		private global::Gtk.Entry entryDescription;
		private global::Gtk.Entry entryName;
		private global::Gtk.HBox hbox2;
		private global::Gtk.SpinButton spinL;
		private global::Gtk.CheckButton checkPlusL;
		private global::Gtk.HBox hbox3;
		private global::Gtk.SpinButton spinH;
		private global::Gtk.CheckButton checkPlusH;
		private global::Gtk.Label label1;
		private global::Gtk.Label label2;
		private global::Gtk.Label label3;
		private global::Gtk.Label label4;
		private global::Gtk.Label label5;
		private global::Gtk.Label label6;
		private global::Gtk.Label label7;
		private global::Gtk.Label label8;
		private global::Gtk.Label labelId;
		private global::Gtk.SpinButton spinW;
		private global::Gtk.VBox vbox2;
		private global::Gtk.DrawingArea drawCube;
		private global::Gtk.Button buttonLoadImage;
		private global::Gtk.Button buttonCancel;
		private global::Gtk.Button buttonOk;

		protected virtual void Build ()
		{
			global::Stetic.Gui.Initialize (this);
			// Widget CupboardDesigner.Nomenclature
			this.Name = "CupboardDesigner.Nomenclature";
			this.Title = global::Mono.Unix.Catalog.GetString ("Номенклатура");
			this.WindowPosition = ((global::Gtk.WindowPosition)(4));
			// Internal child CupboardDesigner.Nomenclature.VBox
			global::Gtk.VBox w1 = this.VBox;
			w1.Name = "dialog1_VBox";
			w1.BorderWidth = ((uint)(2));
			// Container child dialog1_VBox.Gtk.Box+BoxChild
			this.hbox1 = new global::Gtk.HBox ();
			this.hbox1.Name = "hbox1";
			this.hbox1.Spacing = 6;
			// Container child hbox1.Gtk.Box+BoxChild
			this.table1 = new global::Gtk.Table (((uint)(8)), ((uint)(2)), false);
			this.table1.Name = "table1";
			this.table1.RowSpacing = ((uint)(6));
			this.table1.ColumnSpacing = ((uint)(6));
			// Container child table1.Gtk.Table+TableChild
			this.comboType = global::Gtk.ComboBox.NewText ();
			this.comboType.AppendText (global::Mono.Unix.Catalog.GetString ("Куб"));
			this.comboType.AppendText (global::Mono.Unix.Catalog.GetString ("Конструкция"));
			this.comboType.Name = "comboType";
			this.comboType.Active = 0;
			this.table1.Add (this.comboType);
			global::Gtk.Table.TableChild w2 = ((global::Gtk.Table.TableChild)(this.table1 [this.comboType]));
			w2.TopAttach = ((uint)(1));
			w2.BottomAttach = ((uint)(2));
			w2.LeftAttach = ((uint)(1));
			w2.RightAttach = ((uint)(2));
			w2.XOptions = ((global::Gtk.AttachOptions)(4));
			w2.YOptions = ((global::Gtk.AttachOptions)(4));
			// Container child table1.Gtk.Table+TableChild
			this.entryArticle = new global::Gtk.Entry ();
			this.entryArticle.TooltipMarkup = "Для вставки размеров шкафа, в тексте можно использовать {L} и {H}.";
			this.entryArticle.CanFocus = true;
			this.entryArticle.Name = "entryArticle";
			this.entryArticle.IsEditable = true;
			this.entryArticle.InvisibleChar = '●';
			this.table1.Add (this.entryArticle);
			global::Gtk.Table.TableChild w3 = ((global::Gtk.Table.TableChild)(this.table1 [this.entryArticle]));
			w3.TopAttach = ((uint)(2));
			w3.BottomAttach = ((uint)(3));
			w3.LeftAttach = ((uint)(1));
			w3.RightAttach = ((uint)(2));
			w3.XOptions = ((global::Gtk.AttachOptions)(4));
			w3.YOptions = ((global::Gtk.AttachOptions)(4));
			// Container child table1.Gtk.Table+TableChild
			this.entryDescription = new global::Gtk.Entry ();
			this.entryDescription.CanFocus = true;
			this.entryDescription.Name = "entryDescription";
			this.entryDescription.IsEditable = true;
			this.entryDescription.InvisibleChar = '●';
			this.table1.Add (this.entryDescription);
			global::Gtk.Table.TableChild w4 = ((global::Gtk.Table.TableChild)(this.table1 [this.entryDescription]));
			w4.TopAttach = ((uint)(4));
			w4.BottomAttach = ((uint)(5));
			w4.LeftAttach = ((uint)(1));
			w4.RightAttach = ((uint)(2));
			w4.XOptions = ((global::Gtk.AttachOptions)(4));
			w4.YOptions = ((global::Gtk.AttachOptions)(4));
			// Container child table1.Gtk.Table+TableChild
			this.entryName = new global::Gtk.Entry ();
			this.entryName.CanFocus = true;
			this.entryName.Name = "entryName";
			this.entryName.IsEditable = true;
			this.entryName.InvisibleChar = '●';
			this.table1.Add (this.entryName);
			global::Gtk.Table.TableChild w5 = ((global::Gtk.Table.TableChild)(this.table1 [this.entryName]));
			w5.TopAttach = ((uint)(3));
			w5.BottomAttach = ((uint)(4));
			w5.LeftAttach = ((uint)(1));
			w5.RightAttach = ((uint)(2));
			w5.XOptions = ((global::Gtk.AttachOptions)(4));
			w5.YOptions = ((global::Gtk.AttachOptions)(4));
			// Container child table1.Gtk.Table+TableChild
			this.hbox2 = new global::Gtk.HBox ();
			this.hbox2.Name = "hbox2";
			this.hbox2.Spacing = 6;
			// Container child hbox2.Gtk.Box+BoxChild
			this.spinL = new global::Gtk.SpinButton (0, 10000, 1);
			this.spinL.CanFocus = true;
			this.spinL.Name = "spinL";
			this.spinL.Adjustment.PageIncrement = 10;
			this.spinL.ClimbRate = 1;
			this.spinL.Numeric = true;
			this.hbox2.Add (this.spinL);
			global::Gtk.Box.BoxChild w6 = ((global::Gtk.Box.BoxChild)(this.hbox2 [this.spinL]));
			w6.Position = 0;
			// Container child hbox2.Gtk.Box+BoxChild
			this.checkPlusL = new global::Gtk.CheckButton ();
			this.checkPlusL.TooltipMarkup = "Прибавляем размеры всего шкафа";
			this.checkPlusL.CanFocus = true;
			this.checkPlusL.Name = "checkPlusL";
			this.checkPlusL.Label = global::Mono.Unix.Catalog.GetString ("+ база");
			this.checkPlusL.DrawIndicator = true;
			this.checkPlusL.UseUnderline = true;
			this.hbox2.Add (this.checkPlusL);
			global::Gtk.Box.BoxChild w7 = ((global::Gtk.Box.BoxChild)(this.hbox2 [this.checkPlusL]));
			w7.Position = 1;
			w7.Expand = false;
			this.table1.Add (this.hbox2);
			global::Gtk.Table.TableChild w8 = ((global::Gtk.Table.TableChild)(this.table1 [this.hbox2]));
			w8.TopAttach = ((uint)(5));
			w8.BottomAttach = ((uint)(6));
			w8.LeftAttach = ((uint)(1));
			w8.RightAttach = ((uint)(2));
			w8.XOptions = ((global::Gtk.AttachOptions)(4));
			w8.YOptions = ((global::Gtk.AttachOptions)(4));
			// Container child table1.Gtk.Table+TableChild
			this.hbox3 = new global::Gtk.HBox ();
			this.hbox3.Name = "hbox3";
			this.hbox3.Spacing = 6;
			// Container child hbox3.Gtk.Box+BoxChild
			this.spinH = new global::Gtk.SpinButton (0, 10000, 1);
			this.spinH.CanFocus = true;
			this.spinH.Name = "spinH";
			this.spinH.Adjustment.PageIncrement = 10;
			this.spinH.ClimbRate = 1;
			this.spinH.Numeric = true;
			this.hbox3.Add (this.spinH);
			global::Gtk.Box.BoxChild w9 = ((global::Gtk.Box.BoxChild)(this.hbox3 [this.spinH]));
			w9.Position = 0;
			// Container child hbox3.Gtk.Box+BoxChild
			this.checkPlusH = new global::Gtk.CheckButton ();
			this.checkPlusH.TooltipMarkup = "Прибавляем размеры всего шкафа";
			this.checkPlusH.CanFocus = true;
			this.checkPlusH.Name = "checkPlusH";
			this.checkPlusH.Label = global::Mono.Unix.Catalog.GetString ("+ база");
			this.checkPlusH.DrawIndicator = true;
			this.checkPlusH.UseUnderline = true;
			this.hbox3.Add (this.checkPlusH);
			global::Gtk.Box.BoxChild w10 = ((global::Gtk.Box.BoxChild)(this.hbox3 [this.checkPlusH]));
			w10.Position = 1;
			w10.Expand = false;
			this.table1.Add (this.hbox3);
			global::Gtk.Table.TableChild w11 = ((global::Gtk.Table.TableChild)(this.table1 [this.hbox3]));
			w11.TopAttach = ((uint)(7));
			w11.BottomAttach = ((uint)(8));
			w11.LeftAttach = ((uint)(1));
			w11.RightAttach = ((uint)(2));
			w11.XOptions = ((global::Gtk.AttachOptions)(4));
			w11.YOptions = ((global::Gtk.AttachOptions)(4));
			// Container child table1.Gtk.Table+TableChild
			this.label1 = new global::Gtk.Label ();
			this.label1.Name = "label1";
			this.label1.Xalign = 1F;
			this.label1.LabelProp = global::Mono.Unix.Catalog.GetString ("Код:");
			this.table1.Add (this.label1);
			global::Gtk.Table.TableChild w12 = ((global::Gtk.Table.TableChild)(this.table1 [this.label1]));
			w12.XOptions = ((global::Gtk.AttachOptions)(4));
			w12.YOptions = ((global::Gtk.AttachOptions)(4));
			// Container child table1.Gtk.Table+TableChild
			this.label2 = new global::Gtk.Label ();
			this.label2.Name = "label2";
			this.label2.Xalign = 1F;
			this.label2.LabelProp = global::Mono.Unix.Catalog.GetString ("Тип<span foreground=\"red\">*</span>:");
			this.label2.UseMarkup = true;
			this.table1.Add (this.label2);
			global::Gtk.Table.TableChild w13 = ((global::Gtk.Table.TableChild)(this.table1 [this.label2]));
			w13.TopAttach = ((uint)(1));
			w13.BottomAttach = ((uint)(2));
			w13.XOptions = ((global::Gtk.AttachOptions)(4));
			w13.YOptions = ((global::Gtk.AttachOptions)(4));
			// Container child table1.Gtk.Table+TableChild
			this.label3 = new global::Gtk.Label ();
			this.label3.Name = "label3";
			this.label3.Xalign = 1F;
			this.label3.LabelProp = global::Mono.Unix.Catalog.GetString ("Артикул:");
			this.table1.Add (this.label3);
			global::Gtk.Table.TableChild w14 = ((global::Gtk.Table.TableChild)(this.table1 [this.label3]));
			w14.TopAttach = ((uint)(2));
			w14.BottomAttach = ((uint)(3));
			w14.XOptions = ((global::Gtk.AttachOptions)(4));
			w14.YOptions = ((global::Gtk.AttachOptions)(4));
			// Container child table1.Gtk.Table+TableChild
			this.label4 = new global::Gtk.Label ();
			this.label4.Name = "label4";
			this.label4.Xalign = 1F;
			this.label4.LabelProp = global::Mono.Unix.Catalog.GetString ("Наименование<span foreground=\"red\">*</span>:");
			this.label4.UseMarkup = true;
			this.table1.Add (this.label4);
			global::Gtk.Table.TableChild w15 = ((global::Gtk.Table.TableChild)(this.table1 [this.label4]));
			w15.TopAttach = ((uint)(3));
			w15.BottomAttach = ((uint)(4));
			w15.XOptions = ((global::Gtk.AttachOptions)(4));
			w15.YOptions = ((global::Gtk.AttachOptions)(4));
			// Container child table1.Gtk.Table+TableChild
			this.label5 = new global::Gtk.Label ();
			this.label5.Name = "label5";
			this.label5.Xalign = 1F;
			this.label5.LabelProp = global::Mono.Unix.Catalog.GetString ("Описание:");
			this.table1.Add (this.label5);
			global::Gtk.Table.TableChild w16 = ((global::Gtk.Table.TableChild)(this.table1 [this.label5]));
			w16.TopAttach = ((uint)(4));
			w16.BottomAttach = ((uint)(5));
			w16.XOptions = ((global::Gtk.AttachOptions)(4));
			w16.YOptions = ((global::Gtk.AttachOptions)(4));
			// Container child table1.Gtk.Table+TableChild
			this.label6 = new global::Gtk.Label ();
			this.label6.Name = "label6";
			this.label6.Xalign = 1F;
			this.label6.LabelProp = global::Mono.Unix.Catalog.GetString ("Длинна(L):");
			this.table1.Add (this.label6);
			global::Gtk.Table.TableChild w17 = ((global::Gtk.Table.TableChild)(this.table1 [this.label6]));
			w17.TopAttach = ((uint)(5));
			w17.BottomAttach = ((uint)(6));
			w17.XOptions = ((global::Gtk.AttachOptions)(4));
			w17.YOptions = ((global::Gtk.AttachOptions)(4));
			// Container child table1.Gtk.Table+TableChild
			this.label7 = new global::Gtk.Label ();
			this.label7.Name = "label7";
			this.label7.Xalign = 1F;
			this.label7.LabelProp = global::Mono.Unix.Catalog.GetString ("Ширина(W):");
			this.table1.Add (this.label7);
			global::Gtk.Table.TableChild w18 = ((global::Gtk.Table.TableChild)(this.table1 [this.label7]));
			w18.TopAttach = ((uint)(6));
			w18.BottomAttach = ((uint)(7));
			w18.XOptions = ((global::Gtk.AttachOptions)(4));
			w18.YOptions = ((global::Gtk.AttachOptions)(4));
			// Container child table1.Gtk.Table+TableChild
			this.label8 = new global::Gtk.Label ();
			this.label8.Name = "label8";
			this.label8.Xalign = 1F;
			this.label8.LabelProp = global::Mono.Unix.Catalog.GetString ("Высота(H):");
			this.table1.Add (this.label8);
			global::Gtk.Table.TableChild w19 = ((global::Gtk.Table.TableChild)(this.table1 [this.label8]));
			w19.TopAttach = ((uint)(7));
			w19.BottomAttach = ((uint)(8));
			w19.XOptions = ((global::Gtk.AttachOptions)(4));
			w19.YOptions = ((global::Gtk.AttachOptions)(4));
			// Container child table1.Gtk.Table+TableChild
			this.labelId = new global::Gtk.Label ();
			this.labelId.Name = "labelId";
			this.labelId.LabelProp = global::Mono.Unix.Catalog.GetString ("не определен");
			this.table1.Add (this.labelId);
			global::Gtk.Table.TableChild w20 = ((global::Gtk.Table.TableChild)(this.table1 [this.labelId]));
			w20.LeftAttach = ((uint)(1));
			w20.RightAttach = ((uint)(2));
			w20.XOptions = ((global::Gtk.AttachOptions)(4));
			w20.YOptions = ((global::Gtk.AttachOptions)(4));
			// Container child table1.Gtk.Table+TableChild
			this.spinW = new global::Gtk.SpinButton (0, 10000, 1);
			this.spinW.CanFocus = true;
			this.spinW.Name = "spinW";
			this.spinW.Adjustment.PageIncrement = 10;
			this.spinW.ClimbRate = 1;
			this.spinW.Numeric = true;
			this.table1.Add (this.spinW);
			global::Gtk.Table.TableChild w21 = ((global::Gtk.Table.TableChild)(this.table1 [this.spinW]));
			w21.TopAttach = ((uint)(6));
			w21.BottomAttach = ((uint)(7));
			w21.LeftAttach = ((uint)(1));
			w21.RightAttach = ((uint)(2));
			w21.XOptions = ((global::Gtk.AttachOptions)(4));
			w21.YOptions = ((global::Gtk.AttachOptions)(4));
			this.hbox1.Add (this.table1);
			global::Gtk.Box.BoxChild w22 = ((global::Gtk.Box.BoxChild)(this.hbox1 [this.table1]));
			w22.Position = 0;
			w22.Expand = false;
			w22.Fill = false;
			// Container child hbox1.Gtk.Box+BoxChild
			this.vbox2 = new global::Gtk.VBox ();
			this.vbox2.Name = "vbox2";
			this.vbox2.Spacing = 6;
			// Container child vbox2.Gtk.Box+BoxChild
			this.drawCube = new global::Gtk.DrawingArea ();
			this.drawCube.Name = "drawCube";
			this.vbox2.Add (this.drawCube);
			global::Gtk.Box.BoxChild w23 = ((global::Gtk.Box.BoxChild)(this.vbox2 [this.drawCube]));
			w23.Position = 0;
			// Container child vbox2.Gtk.Box+BoxChild
			this.buttonLoadImage = new global::Gtk.Button ();
			this.buttonLoadImage.CanFocus = true;
			this.buttonLoadImage.Name = "buttonLoadImage";
			this.buttonLoadImage.UseUnderline = true;
			this.buttonLoadImage.Label = global::Mono.Unix.Catalog.GetString ("Загрузить изображение");
			global::Gtk.Image w24 = new global::Gtk.Image ();
			w24.Pixbuf = global::Stetic.IconLoader.LoadIcon (this, "gtk-harddisk", global::Gtk.IconSize.Menu);
			this.buttonLoadImage.Image = w24;
			this.vbox2.Add (this.buttonLoadImage);
			global::Gtk.Box.BoxChild w25 = ((global::Gtk.Box.BoxChild)(this.vbox2 [this.buttonLoadImage]));
			w25.Position = 1;
			w25.Expand = false;
			w25.Fill = false;
			this.hbox1.Add (this.vbox2);
			global::Gtk.Box.BoxChild w26 = ((global::Gtk.Box.BoxChild)(this.hbox1 [this.vbox2]));
			w26.Position = 1;
			w1.Add (this.hbox1);
			global::Gtk.Box.BoxChild w27 = ((global::Gtk.Box.BoxChild)(w1 [this.hbox1]));
			w27.Position = 0;
			w27.Expand = false;
			w27.Fill = false;
			// Internal child CupboardDesigner.Nomenclature.ActionArea
			global::Gtk.HButtonBox w28 = this.ActionArea;
			w28.Name = "dialog1_ActionArea";
			w28.Spacing = 10;
			w28.BorderWidth = ((uint)(5));
			w28.LayoutStyle = ((global::Gtk.ButtonBoxStyle)(4));
			// Container child dialog1_ActionArea.Gtk.ButtonBox+ButtonBoxChild
			this.buttonCancel = new global::Gtk.Button ();
			this.buttonCancel.CanDefault = true;
			this.buttonCancel.CanFocus = true;
			this.buttonCancel.Name = "buttonCancel";
			this.buttonCancel.UseStock = true;
			this.buttonCancel.UseUnderline = true;
			this.buttonCancel.Label = "gtk-cancel";
			this.AddActionWidget (this.buttonCancel, -6);
			global::Gtk.ButtonBox.ButtonBoxChild w29 = ((global::Gtk.ButtonBox.ButtonBoxChild)(w28 [this.buttonCancel]));
			w29.Expand = false;
			w29.Fill = false;
			// Container child dialog1_ActionArea.Gtk.ButtonBox+ButtonBoxChild
			this.buttonOk = new global::Gtk.Button ();
			this.buttonOk.CanDefault = true;
			this.buttonOk.CanFocus = true;
			this.buttonOk.Name = "buttonOk";
			this.buttonOk.UseStock = true;
			this.buttonOk.UseUnderline = true;
			this.buttonOk.Label = "gtk-ok";
			w28.Add (this.buttonOk);
			global::Gtk.ButtonBox.ButtonBoxChild w30 = ((global::Gtk.ButtonBox.ButtonBoxChild)(w28 [this.buttonOk]));
			w30.Position = 1;
			w30.Expand = false;
			w30.Fill = false;
			if ((this.Child != null)) {
				this.Child.ShowAll ();
			}
			this.DefaultWidth = 499;
			this.DefaultHeight = 369;
			this.Show ();
			this.comboType.Changed += new global::System.EventHandler (this.OnComboTypeChanged);
			this.drawCube.ExposeEvent += new global::Gtk.ExposeEventHandler (this.OnDrawCubeExposeEvent);
			this.buttonLoadImage.Clicked += new global::System.EventHandler (this.OnButtonLoadImageClicked);
			this.buttonOk.Clicked += new global::System.EventHandler (this.OnButtonOkClicked);
		}
	}
}
