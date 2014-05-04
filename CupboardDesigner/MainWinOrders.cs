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
		basis_type
	}

	void PrerareOrders()
	{
		OrdersListStore = new ListStore (typeof (int), typeof (string), typeof (string));

		treeviewOrders.AppendColumn("Номер", new Gtk.CellRendererText (), "text", (int)OrdersCol.id);
		treeviewOrders.AppendColumn("Клиент", new Gtk.CellRendererText (), "text", (int)OrdersCol.custom);
		treeviewOrders.AppendColumn("Тип шкафа", new Gtk.CellRendererText (), "text", (int)OrdersCol.basis_type);

		OrdersFilter = new TreeModelFilter (OrdersListStore, null);
		OrdersFilter.VisibleFunc = new TreeModelFilterVisibleFunc (FilterTreeOrders);
		treeviewOrders.Model = OrdersFilter;
		treeviewOrders.Selection.Changed += OnTreeviewOrdersSelectionChanged;
		treeviewOrders.ShowAll();
		UpdateOrders();
	}

	void UpdateOrders()
	{
		MainClass.StatusMessage("Получаем таблицу заказов...");

		string sql = "SELECT orders.id, orders.customer, basis.name as basis FROM orders" +
			" LEFT JOIN basis ON basis.id = orders.basis_id ";
		SqliteCommand cmd = new SqliteCommand(sql, (SqliteConnection) QSMain.ConnectionDB);

		using(SqliteDataReader rdr = cmd.ExecuteReader())
		{
			OrdersListStore.Clear();
			while (rdr.Read())
			{
				OrdersListStore.AppendValues(rdr.GetInt32(rdr.GetOrdinal("id")),
					rdr["customer"].ToString(),
					rdr["basis"].ToString()
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
		return (filterCustom || filterNumber);
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
		ItemOrder.Show();
		if ((ResponseType)ItemOrder.Run() == ResponseType.Ok)
			UpdateOrders();
		ItemOrder.Destroy();
	}

	protected void OnButtonEditClicked(object sender, EventArgs e)
	{
		TreeIter iter;
		treeviewOrders.Selection.GetSelected(out iter);
		int itemid = Convert.ToInt32(OrdersFilter.GetValue(iter,0));
		Order winOrder = new Order();
		winOrder.Fill(itemid);
		winOrder.Show();
		ResponseType result = (ResponseType)winOrder.Run();
		winOrder.Destroy();
		if(result == ResponseType.Ok)
			UpdateOrders();
	}

	protected void OnButtonDelClicked(object sender, EventArgs e)
	{
		throw new NotImplementedException();
	}

}