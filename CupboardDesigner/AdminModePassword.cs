using System;

namespace CupboardDesigner
{
	public partial class AdminModePassword : Gtk.Dialog
	{
		public string Password;

		public AdminModePassword()
		{
			this.Build();
		}

		protected void OnButtonOkClicked(object sender, EventArgs e)
		{
			Password = entryPassword.Text;
		}

		protected void OnEntryPasswordActivated(object sender, EventArgs e)
		{
			buttonOk.Click();
		}
	}
}

