
// This file has been generated by the GUI designer. Do not modify.
namespace CupboardDesigner
{
	public partial class CubeListItem
	{
		private global::Gtk.Table table1;
		
		private global::Gtk.DrawingArea drawCube;
		
		private global::Gtk.Label labelName;

		protected virtual void Build ()
		{
			global::Stetic.Gui.Initialize (this);
			// Widget CupboardDesigner.CubeListItem
			global::Stetic.BinContainer.Attach (this);
			this.Name = "CupboardDesigner.CubeListItem";
			// Container child CupboardDesigner.CubeListItem.Gtk.Container+ContainerChild
			this.table1 = new global::Gtk.Table (((uint)(2)), ((uint)(1)), false);
			this.table1.Name = "table1";
			this.table1.RowSpacing = ((uint)(4));
			this.table1.ColumnSpacing = ((uint)(6));
			// Container child table1.Gtk.Table+TableChild
			this.drawCube = new global::Gtk.DrawingArea ();
			this.drawCube.Name = "drawCube";
			this.table1.Add (this.drawCube);
			global::Gtk.Table.TableChild w1 = ((global::Gtk.Table.TableChild)(this.table1 [this.drawCube]));
			w1.XOptions = ((global::Gtk.AttachOptions)(1));
			w1.YOptions = ((global::Gtk.AttachOptions)(1));
			// Container child table1.Gtk.Table+TableChild
			this.labelName = new global::Gtk.Label ();
			this.labelName.Name = "labelName";
			this.labelName.LabelProp = global::Mono.Unix.Catalog.GetString ("label1");
			this.table1.Add (this.labelName);
			global::Gtk.Table.TableChild w2 = ((global::Gtk.Table.TableChild)(this.table1 [this.labelName]));
			w2.TopAttach = ((uint)(1));
			w2.BottomAttach = ((uint)(2));
			w2.YOptions = ((global::Gtk.AttachOptions)(0));
			this.Add (this.table1);
			if ((this.Child != null)) {
				this.Child.ShowAll ();
			}
			this.Show ();
			this.drawCube.ExposeEvent += new global::Gtk.ExposeEventHandler (this.OnDrawCubeExposeEvent);
			this.drawCube.DragBegin += new global::Gtk.DragBeginHandler (this.OnDrawCubeDragBegin);
		}
	}
}
