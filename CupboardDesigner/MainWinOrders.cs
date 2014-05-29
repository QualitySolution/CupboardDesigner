using System;
using CupboardDesigner;
using QSProjectsLib;
using Mono.Data.Sqlite;
using Gtk;

public partial class MainWindow: Gtk.Window
{
	ListStore OrdersListStore;
	TreeModelFilter OrdersFilter;

	private enum OrdersCol{
		id,
		custom,
		phones,
		address,
		arrval,
		delivery
	}

	void PrerareOrders()
	{
		OrdersListStore = new ListStore (typeof (int), typeof (string), typeof (string), typeof (string), typeof (string), typeof (string));

		treeviewOrders.AppendColumn("Номер", new Gtk.CellRendererText (), "text", (int)OrdersCol.id);
		treeviewOrders.AppendColumn("Ф.И.О. заказчика", new Gtk.CellRendererText (), "text", (int)OrdersCol.custom);
		treeviewOrders.AppendColumn("Дата прихода", new Gtk.CellRendererText (), "text", (int)OrdersCol.arrval);
		treeviewOrders.AppendColumn("Дата сдачи", new Gtk.CellRendererText (), "text", (int)OrdersCol.delivery);

		OrdersFilter = new TreeModelFilter (OrdersListStore, null);
		OrdersFilter.VisibleFunc = new TreeModelFilterVisibleFunc (FilterTreeOrders);
		treeviewOrders.Model = OrdersFilter;
		treeviewOrders.Selection.Changed += OnTreeviewOrdersSelectionChanged;
		treeviewOrders.ShowAll();
		UpdateOrders();
	}

	public void UpdateOrders()
	{
		MainClass.StatusMessage("Получаем таблицу заказов...");

		string sql = "SELECT orders.id, orders.customer, orders.address, orders.phone1, orders.phone2, orders.arrval, orders.delivery FROM orders ";
		SqliteCommand cmd = new SqliteCommand(sql, (SqliteConnection) QSMain.ConnectionDB);

		using(SqliteDataReader rdr = cmd.ExecuteReader())
		{
			OrdersListStore.Clear();
			while (rdr.Read())
			{
				OrdersListStore.AppendValues(rdr.GetInt32(rdr.GetOrdinal("id")),
					rdr["customer"].ToString(),
					rdr["phone1"].ToString() + rdr["phone2"].ToString(),
					rdr["address"].ToString(),
					String.Format("{0:d}", rdr["arrval"]),
					String.Format("{0:d}", rdr["delivery"])
				);
			}

		}
		MainClass.StatusMessage("Ok");
	}

	private bool FilterTreeOrders (Gtk.TreeModel model, Gtk.TreeIter iter)
	{
		if (entrySearch.Text == "")
			return true;
		bool filterCustom = false;
		bool filterNumber = false;
		bool filterPhones = false;
		bool filterAddress = false;
		string cellvalue;

		if (model.GetValue (iter, (int)OrdersCol.custom) != null)
		{
			cellvalue  = model.GetValue (iter, (int)OrdersCol.custom).ToString();
			filterCustom = cellvalue.IndexOf (entrySearch.Text, StringComparison.CurrentCultureIgnoreCase) > -1;
		}
		if (model.GetValue (iter, (int)OrdersCol.id) != null)
		{
			cellvalue  = model.GetValue (iter, (int)OrdersCol.id).ToString();
			filterNumber = cellvalue.IndexOf (entrySearch.Text, StringComparison.CurrentCultureIgnoreCase) > -1;
		}
		if (model.GetValue (iter, (int)OrdersCol.address) != null)
		{
			cellvalue  = model.GetValue (iter, (int)OrdersCol.address).ToString();
			filterAddress = cellvalue.IndexOf (entrySearch.Text, StringComparison.CurrentCultureIgnoreCase) > -1;
		}
		if (model.GetValue (iter, (int)OrdersCol.phones) != null)
		{
			cellvalue  = model.GetValue (iter, (int)OrdersCol.phones).ToString();
			filterPhones = cellvalue.IndexOf (entrySearch.Text, StringComparison.CurrentCultureIgnoreCase) > -1;
		}
		return (filterCustom || filterNumber || filterPhones || filterAddress);
	}

	protected void OnButtonClearSearchClicked(object sender, EventArgs e)
	{
		entrySearch.Text = "";
	}

	protected void OnEntrySearchChanged(object sender, EventArgs e)
	{
		OrdersFilter.Refilter();
	}

	protected void OnTreeviewOrdersRowActivated(object o, RowActivatedArgs args)
	{
		buttonEdit.Click();
	}

	protected void OnTreeviewOrdersSelectionChanged(object sender, EventArgs e)
	{
		bool isSelect = treeviewOrders.Selection.CountSelectedRows() == 1;
		buttonEdit.Sensitive = isSelect;
		buttonDel.Sensitive = isSelect;
	}

	protected void OnButtonAddClicked(object sender, EventArgs e)
	{
		Order ItemOrder = new Order();
		ItemOrder.DeleteEvent += OnDeleteOrderEvent;
		ItemOrder.Show();
	}

	protected void OnDeleteOrderEvent( object s, DeleteEventArgs arg)
	{
		Order OrderWin = (Order)s;
		OrderWin.Destroy ();
		UpdateOrders();
	}
		
	protected void OnButtonEditClicked(object sender, EventArgs e)
	{
		TreeIter iter;
		treeviewOrders.Selection.GetSelected(out iter);
		int itemid = Convert.ToInt32(OrdersFilter.GetValue(iter,0));
		Order winOrder = new Order();
		winOrder.Fill(itemid);
		winOrder.DeleteEvent += OnDeleteOrderEvent;
		winOrder.Show();
	}

	protected void OnButtonDelClicked(object sender, EventArgs e)
	{
		TreeIter iter;
		Delete winDelete = new Delete();

		treeviewOrders.Selection.GetSelected(out iter);
		int itemid = (int) OrdersFilter.GetValue(iter, (int)OrdersCol.id);
		winDelete.RunDeletion("orders", itemid);
		UpdateOrders();
	}

}