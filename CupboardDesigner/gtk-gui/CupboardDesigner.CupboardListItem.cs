
// This file has been generated by the GUI designer. Do not modify.
namespace CupboardDesigner
{
	public partial class CupboardListItem
	{
		private global::Gtk.Table table1;
		private global::Gtk.DrawingArea drawImage;
		private global::Gtk.RadioButton radioType;

		protected virtual void Build ()
		{
			global::Stetic.Gui.Initialize (this);
			// Widget CupboardDesigner.CupboardListItem
			global::Stetic.BinContainer.Attach (this);
			this.Name = "CupboardDesigner.CupboardListItem";
			// Container child CupboardDesigner.CupboardListItem.Gtk.Container+ContainerChild
			this.table1 = new global::Gtk.Table (((uint)(2)), ((uint)(1)), false);
			this.table1.Name = "table1";
			this.table1.ColumnSpacing = ((uint)(6));
			this.table1.BorderWidth = ((uint)(2));
			// Container child table1.Gtk.Table+TableChild
			this.drawImage = new global::Gtk.DrawingArea ();
			this.drawImage.Name = "drawImage";
			this.table1.Add (this.drawImage);
			global::Gtk.Table.TableChild w1 = ((global::Gtk.Table.TableChild)(this.table1 [this.drawImage]));
			w1.XOptions = ((global::Gtk.AttachOptions)(0));
			w1.YOptions = ((global::Gtk.AttachOptions)(1));
			// Container child table1.Gtk.Table+TableChild
			this.radioType = new global::Gtk.RadioButton (global::Mono.Unix.Catalog.GetString ("radiobutton1"));
			this.radioType.CanFocus = true;
			this.radioType.Name = "radioType";
			this.radioType.Active = true;
			this.radioType.DrawIndicator = true;
			this.radioType.UseUnderline = true;
			this.radioType.Group = new global::GLib.SList (global::System.IntPtr.Zero);
			this.table1.Add (this.radioType);
			global::Gtk.Table.TableChild w2 = ((global::Gtk.Table.TableChild)(this.table1 [this.radioType]));
			w2.TopAttach = ((uint)(1));
			w2.BottomAttach = ((uint)(2));
			w2.XOptions = ((global::Gtk.AttachOptions)(4));
			w2.YOptions = ((global::Gtk.AttachOptions)(4));
			this.Add (this.table1);
			if ((this.Child != null)) {
				this.Child.ShowAll ();
			}
			this.Hide ();
			this.drawImage.ExposeEvent += new global::Gtk.ExposeEventHandler (this.OnDrawImageExposeEvent);
			this.drawImage.ButtonPressEvent += new global::Gtk.ButtonPressEventHandler (this.OnDrawImageButtonPressEvent);
		}
	}
}
