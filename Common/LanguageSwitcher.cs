using System;
using System.Collections;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.Reflection;
using System.Resources;
using System.Threading;
using System.Windows.Forms;
using TD.Eyefinder;

namespace Common 
{
	public class LanguageSwitcher 
	{
		/// <summary>
		///     Static read-only instance.
		/// </summary>
		protected static readonly LanguageSwitcher m_instance = new LanguageSwitcher();

        /// <summary>
        ///     <c>CultureInfo</c> used by Resource Manager.
        /// </summary>
        private CultureInfo m_cultureInfo;
        
		/// <summary>
		///     Hidden constructor called during static member <c>m_instance</c> 
		///     initialization.
		/// </summary>
		protected LanguageSwitcher() {}

		/// <summary>
		///     Gets the only instance of the object.
		/// </summary>
		public static LanguageSwitcher Instance 
		{
			get { return m_instance; }
		}

        /// <summary>
        ///     Changes the language of the <c>Form</c> object provided and all 
        ///     its MDI children (in the case of MDI UI) to the currently 
        ///     selected language.
        /// </summary>
        /// <param name="form">
        ///     <c>Form</c> object to apply changes to.
        /// </param>
        /// <param name="cultureInfo">
        ///     <c>CultureInfo</c> to which language has to be changed.
        /// </param>
        public void ChangeLanguage(Form form, CultureInfo cultureInfo)
        {
            m_cultureInfo = cultureInfo;
            Thread.CurrentThread.CurrentUICulture = m_cultureInfo;
            ChangeFormLanguage(form);
            foreach (Form childForm in form.MdiChildren)
            {
                ChangeFormLanguage(childForm);
            }
        }

        /// <summary>
        ///     Changes <c>Text</c> properties associated with following 
        ///     controls: <c>AxHost</c>, <c>ButtonBase</c>, <c>GroupBox</c>, 
        ///     <c>Label</c>, <c>ScrollableControl</c>, <c>StatusBar</c>, 
        ///     <c>TabControl</c>, <c>ToolBar</c>. Method is made virtual so it 
        ///     can be overriden in derived class to redefine types.
        /// </summary>
        /// <param name="parent">
        ///     <c>Control</c> object.
        /// </param>
        /// <param name="resources">
        ///     <c>ResourceManager</c> object.
        /// </param>
        protected virtual void ReloadTextForSelectedControls(Control control, ResourceManager resources) 
		{
			if (control is AxHost || control is ButtonBase || control is GroupBox ||
                control is Label || control is ScrollableControl || control is StatusBar ||
                control is TabControl || control is ToolBar) 
			{
				control.Text = resources.GetString(control.Name + ".Text", m_cultureInfo);
			}
		}

		/// <summary>
		///     Reloads properties common to all controls (except the <c>Text</c>
		///     property).
		/// </summary>
		/// <param name="control">
		///     <c>Control</c> object for which resources should be reloaded.
		/// </param>
		/// <param name="resources">
		///     <c>ResourceManager</c> object.
		/// </param>
		protected virtual void ReloadControlCommonProperties(Control control, ResourceManager resources) 
		{
			SetProperty(control, "AccessibleDescription", resources);
			SetProperty(control, "AccessibleName", resources);
			SetProperty(control, "BackgroundImage", resources);
			SetProperty(control, "Font", resources);
			SetProperty(control, "ImeMode", resources);
			SetProperty(control, "RightToLeft", resources);
			SetProperty(control, "Size", resources);
			// following properties are not changed for the form
			if (!(control is Form)) 
			{
				SetProperty(control, "Anchor", resources);
				SetProperty(control, "Dock", resources);
				SetProperty(control, "Enabled", resources);
				SetProperty(control, "Location", resources);
				SetProperty(control, "TabIndex", resources);
				SetProperty(control, "Visible", resources);
			}
			if (control is ScrollableControl) 
			{
				ReloadScrollableControlProperties((ScrollableControl)control, resources);
				if (control is Form) 
				{
					ReloadFormProperties((Form)control, resources);
				}
			}
		}

		/// <summary>
		///     Reloads properties specific to some controls.
		/// </summary>
		/// <param name="control">
		///     <c>Control</c> object for which resources should be reloaded.
		/// </param>
		/// <param name="resources">
		///     <c>ResourceManager</c> object.
		/// </param>
		protected virtual void ReloadControlSpecificProperties(Control control, ResourceManager resources) 
		{
			// ImageIndex property for ButtonBase, Label, TabPage, ToolBarButton, TreeNode, TreeView
			SetProperty(control, "ImageIndex", resources);
			// ToolTipText property for StatusBar, TabPage, ToolBarButton
			SetProperty(control, "ToolTipText", resources);
			// IntegralHeight property for ComboBox, ListBox
			SetProperty(control, "IntegralHeight", resources);
			// ItemHeight property for ListBox, ComboBox, TreeView
			SetProperty(control, "ItemHeight", resources);
			// MaxDropDownItems property for ComboBox
			SetProperty(control, "MaxDropDownItems", resources);
			// MaxLength property for ComboBox, RichTextBox, TextBoxBase
			SetProperty(control, "MaxLength", resources);
			// Appearance property for CheckBox, RadioButton, TabControl, ToolBar
			SetProperty(control, "Appearance", resources);
			// CheckAlign property for CheckBox and RadioBox
			SetProperty(control, "CheckAlign", resources);
			// FlatStyle property for ButtonBase, GroupBox and Label
			SetProperty(control, "FlatStyle", resources);
			// ImageAlign property for ButtonBase, Image and Label
			SetProperty(control, "ImageAlign", resources);
			// Indent property for TreeView
			SetProperty(control, "Indent", resources);
			// Multiline property for RichTextBox, TabControl, TextBoxBase
			SetProperty(control, "Multiline", resources);
			// BulletIndent property for RichTextBox
			SetProperty(control, "BulletIndent", resources);
			// RightMargin property for RichTextBox
			SetProperty(control, "RightMargin", resources);
			// ScrollBars property for RichTextBox, TextBox
			SetProperty(control, "ScrollBars", resources);
			// WordWrap property for TextBoxBase
			SetProperty(control, "WordWrap", resources);
			// ZoomFactor property for RichTextBox
			SetProperty(control, "ZoomFactor", resources);
			// ButtonSize property for ToolBar
			SetProperty(control, "ButtonSize", resources);
			// ButtonSize property for ToolBar
			SetProperty(control, "DropDownArrows", resources);
			// ShowToolTips property for TabControl, ToolBar
			SetProperty(control, "ShowToolTips", resources);
			// Wrappable property for ToolBar
			SetProperty(control, "Wrappable", resources);
			// AutoSize property for Label, RichTextBox, ToolBar, TrackBar
			SetProperty(control, "AutoSize", resources);
		}

		/// <summary>
		///     Scans controls that are not contained by <c>Controls</c> 
		///     collection, like <c>MenuItem</c>s, <c>StatusBarPanel</c>s
		///     and <c>ColumnHeader</c>s.
		/// </summary>
		/// <param name="form">
		///     <c>ContainerControl</c> object to scan.
		/// </param>
		/// <param name="resources">
		///     <c>ResourceManager</c> used to get localized resources.
		/// </param>
		protected virtual void ScanNonControls(ContainerControl containerControl, ResourceManager resources) 
		{
			FieldInfo[] fieldInfo = containerControl.GetType().GetFields(BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Public);
			for (int i = 0; i < fieldInfo.Length; i++) 
			{
				object obj = fieldInfo[i].GetValue(containerControl);
				string fieldName = fieldInfo[i].Name;
				
                //if (obj is TD.SandBar.MenuBarItem) 
                //{
                //    TD.SandBar.MenuBarItem menuItem = (TD.SandBar.MenuBarItem)obj;
                //    menuItem.Text = resources.GetString(fieldName + ".Text", _cultureInfo);
                //}
                //if (obj is TD.SandBar.MenuButtonItem) 
                //{
                //    TD.SandBar.MenuButtonItem menuItem = (TD.SandBar.MenuButtonItem)obj;
                //    menuItem.Text = resources.GetString(fieldName + ".Text", _cultureInfo);
                //}
                //if (obj is TD.SandBar.ButtonItem) 
                //{
                //    TD.SandBar.ButtonItem buttonItem = (TD.SandBar.ButtonItem)obj;
                //    buttonItem.Text = resources.GetString(fieldName + ".Text", _cultureInfo);
                //    buttonItem.ToolTipText = resources.GetString(fieldName + ".ToolTipText", _cultureInfo);
                //}
                //if (obj is TD.SandBar.DropDownMenuItem)
                //{
                //    TD.SandBar.DropDownMenuItem menuItem = (TD.SandBar.DropDownMenuItem)obj;
                //    menuItem.Text = resources.GetString(fieldName + ".Text", m_cultureInfo);
                //    menuItem.ToolTipText = resources.GetString(fieldName + ".ToolTipText", _cultureInfo);
                //}
                if (obj is ToolStripButton)
                {
                    ToolStripButton toolStripItem = (ToolStripButton)obj;
                    toolStripItem.Text = resources.GetString(fieldName + ".Text", m_cultureInfo);
                    toolStripItem.ToolTipText = resources.GetString(fieldName + ".ToolTipText", m_cultureInfo);
                }
                if (obj is ToolStripMenuItem)
                {
                    ToolStripMenuItem toolStripMenuItem = (ToolStripMenuItem)obj;
                    toolStripMenuItem.Text = resources.GetString(fieldName + ".Text", m_cultureInfo);
                    toolStripMenuItem.ToolTipText = resources.GetString(fieldName + ".ToolTipText", m_cultureInfo);
                }
                if (obj is ToolStripSplitButton)
                {
                    ToolStripSplitButton toolStripSplitButton = (ToolStripSplitButton)obj;
                    toolStripSplitButton.Text = resources.GetString(fieldName + ".Text", m_cultureInfo);
                    toolStripSplitButton.ToolTipText = resources.GetString(fieldName + ".ToolTipText", m_cultureInfo);
                }
                if (obj is MenuItem) 
				{
					MenuItem menuItem = (MenuItem)obj;
					menuItem.Enabled = (bool)(resources.GetObject(fieldName + ".Enabled", m_cultureInfo));
					menuItem.Shortcut = (Shortcut)(resources.GetObject(fieldName + ".Shortcut", m_cultureInfo));
					menuItem.ShowShortcut = (bool)(resources.GetObject(fieldName + ".ShowShortcut", m_cultureInfo));
					menuItem.Text = resources.GetString(fieldName + ".Text", m_cultureInfo);
					menuItem.Visible = (bool)(resources.GetObject(fieldName + ".Visible", m_cultureInfo));
				}
				if (obj is StatusBarPanel) 
				{
					StatusBarPanel panel = (StatusBarPanel)obj;
					panel.Icon = (Icon)(resources.GetObject(fieldName + ".Icon", m_cultureInfo));
					panel.Text = resources.GetString(fieldName + ".Text", m_cultureInfo);
				}
				if (obj is ColumnHeader) 
				{
					ColumnHeader header = (ColumnHeader)obj;
					header.Text = resources.GetString(fieldName + ".Text", m_cultureInfo);
					header.TextAlign = (HorizontalAlignment)(resources.GetObject(fieldName + ".TextAlign", m_cultureInfo));
					header.Width = (int)(resources.GetObject(fieldName + ".Width", m_cultureInfo));
				}
				if (obj is ToolBarButton) 
				{
					ToolBarButton button = (ToolBarButton)obj;
					button.Enabled = (bool)(resources.GetObject(fieldName + ".Enabled", m_cultureInfo));
					button.ImageIndex = (int)(resources.GetObject(fieldName + ".ImageIndex", m_cultureInfo));
					button.Text = resources.GetString(fieldName + ".Text", m_cultureInfo);
					button.ToolTipText = resources.GetString(fieldName + ".ToolTipText", m_cultureInfo);
					button.Visible = (bool)(resources.GetObject(fieldName + ".Visible", m_cultureInfo));
				}
				if (obj is NavigationBar)
				{
					NavigationBar naviBar = (NavigationBar)obj;
					naviBar.Text = resources.GetString(fieldName + ".Text", m_cultureInfo);
				}
				if (obj is NavigationPane)
				{
					NavigationPane naviPane = (NavigationPane)obj;
					naviPane.Text = resources.GetString(fieldName + ".Text", m_cultureInfo);
				}
				if (obj is TabPage || obj is GroupBox)
					RecurControls((Control)obj, resources, null);
			}
		}

		/// <summary>
		///     Reloads items in following controls: <c>ComboBox</c>, 
		///     <c>ListBox</c>, <c>DomainUpDown</c>. Method is made virtual so 
		///     it can be overriden in derived class to redefine types.
		/// </summary>
		/// <param name="parent">
		///     <c>Control</c> object.
		/// </param>
		/// <param name="resources">
		///     <c>ResourceManager</c> object.
		/// </param>
		protected virtual void ReloadListItems(Control control, ResourceManager resources) 
		{
			if (control is ComboBox) 
				ReloadComboBoxItems((ComboBox)control, resources);
			else if (control is ListBox) 
				ReloadListBoxItems((ListBox)control, resources);
			else if (control is DomainUpDown)
				ReloadUpDownItems((DomainUpDown)control, resources);
		}

		/// <summary>
		///     Changes the language of the form.
		/// </summary>
		/// <param name="form">
		///     <c>Form</c> object to apply changes to. 
		/// </param>
		private void ChangeFormLanguage(Form form) 
		{
			form.SuspendLayout();
			Cursor.Current = Cursors.WaitCursor;
			ResourceManager resources = new ResourceManager(form.GetType());
			// change main form resources
			form.Text = resources.GetString("$this.Text", m_cultureInfo);
			ReloadControlCommonProperties(form, resources);
			ToolTip toolTip = GetToolTip(form);
			// change text of all containing controls
			RecurControls(form, resources, toolTip);
			// change the text of menus
			ScanNonControls(form, resources);
			form.ResumeLayout();
		}

		/// <summary>
		///     Gets the <c>ToolTip</c> member of the control (<c>Form</c> or 
		///     <c>UserControl</c>).
		/// </summary>
		/// <param name="control">
		///     <c>Control</c> for which tooltip is requested.
		/// </param>
		/// <returns>
		///     <c>ToolTip</c> of the control or <c>null</c> if not defined.
		/// </returns>
		private ToolTip GetToolTip(System.Windows.Forms.Control control) 
		{
			Debug.Assert(control is System.Windows.Forms.Form || control is UserControl);
			FieldInfo[] fieldInfo = control.GetType().GetFields(BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Public | BindingFlags.Default);
			for (int i = 0; i < fieldInfo.Length; i++) 
			{
				object obj = fieldInfo[i].GetValue(control);
				if (obj is ToolTip) 
					return (ToolTip)obj;
			}
			return null;
		}

		/// <summary>
		///     Recurs <c>Controls</c> members of the control to change 
		///     corresponding texts.
		/// </summary>
		/// <param name="parent">
		///     Parent <c>Control</c> object.
		/// </param>
		/// <param name="resources">
		///     <c>ResourceManager</c> object.
		/// </param>
		private void RecurControls(Control parent, ResourceManager resources, ToolTip toolTip) 
		{
			foreach (Control control in parent.Controls) 
			{
				control.SuspendLayout();
				ReloadControlCommonProperties(control, resources);
				ReloadControlSpecificProperties(control, resources);
				if (toolTip != null)
					toolTip.SetToolTip(control, resources.GetString(control.Name + ".ToolTip", m_cultureInfo));
				if (control is UserControl)
					RecurUserControl((UserControl)control);
				else 
				{
					ReloadTextForSelectedControls(control, resources);
					ReloadListItems(control, resources);
					if (control is TreeView)
						if (control.Controls.Count > 0)
							RecurControls(control, resources, toolTip);
				}
				control.ResumeLayout();
			}
		}

		/// <summary>
		///     Reloads resources specific to the <c>Form</c> type.
		/// </summary>
		/// <param name="form">
		///     <c>Form</c> object to apply changes to.
		/// </param>
		/// <param name="resources">
		///     <c>ResourceManager</c> object.
		/// </param>
		private void ReloadFormProperties(Form form, ResourceManager resources) 
		{
			SetProperty(form, "AutoScaleBaseSize", resources);
			SetProperty(form, "Icon", resources);
			SetProperty(form, "MaximumSize", resources);
			SetProperty(form, "MinimumSize", resources);
		}

		/// <summary>
		///     Reloads resources specific to the <c>ScrollableControl</c> type.
		/// </summary>
		/// <param name="control">
		///     <c>Control</c> object for which resources should be reloaded.
		/// </param>
		/// <param name="resources">
		///     <c>ResourceManager</c> object.
		/// </param>
		private void ReloadScrollableControlProperties(ScrollableControl control, ResourceManager resources) 
		{
			SetProperty(control, "AutoScroll", resources);
			SetProperty(control, "AutoScrollMargin", resources);
			SetProperty(control, "AutoScrollMinSize", resources);
		}

		/// <summary>
		///     Reloads resources for a property.
		/// </summary>
		/// <param name="control">
		///     <c>Control</c> object for which resources should be reloaded.
		/// </param>
		/// <param name="propertyName">
		///     Name of the property to reload.
		/// </param>
		/// <param name="resources">
		///     <c>ResourceManager</c> object.
		/// </param>
		private void SetProperty(Control control, string propertyName, ResourceManager resources) 
		{
			PropertyInfo propertyInfo = control.GetType().GetProperty(propertyName);
			if (propertyInfo != null) 
			{
				string controlName = control.Name;
				if (control is Form)
					controlName = "$this";
				object resObject = resources.GetObject(controlName + "." + propertyName, m_cultureInfo);
				if (resObject != null) 
					propertyInfo.SetValue(control, Convert.ChangeType(resObject, propertyInfo.PropertyType), null);
			}
		}
        
		/// <summary>
		///     Recurs <c>UserControl</c> to change.
		/// </summary>
		/// <param name="parent">
		///     <c>UserControl</c> object to scan.
		/// </param>
		private void RecurUserControl(UserControl userControl) 
		{
			ResourceManager resources = new ResourceManager(userControl.GetType());
			ToolTip toolTip = GetToolTip(userControl);
			RecurControls(userControl, resources, toolTip);
			// addition suggested by Piotr Sielski:
			ScanNonControls(userControl, resources);
		}

		/// <summary>
		///     Reloads items in the <c>ListBox</c>. If items are not sorted, 
		///     selections are kept.
		/// </summary>
		/// <param name="listBox">
		///     <c>ListBox</c> to localize.
		/// </param>
		/// <param name="resources">
		///     <c>ResourceManager</c> object.
		/// </param>
		private void ReloadListBoxItems(ListBox listBox, ResourceManager resources) 
		{
			if (listBox.Items.Count > 0) 
			{
				int[] selectedItems = new int[listBox.SelectedIndices.Count];
				listBox.SelectedIndices.CopyTo(selectedItems, 0);
				ReloadItems(listBox.Name, listBox.Items, listBox.Items.Count, resources);
				if (!listBox.Sorted) 
				{
					for (int i = 0; i < selectedItems.Length; i++) 
					{
						listBox.SetSelected(selectedItems[i], true);
					}
				}
			}
		}

		/// <summary>
		///     Reloads items in the <c>ComboBox</c>. If items are not sorted, 
		///     selection is kept.
		/// </summary>
		/// <param name="listBox">
		///     <c>ComboBox</c> to localize.
		/// </param>
		/// <param name="resources">
		///     <c>ResourceManager</c> object.
		/// </param>
		private void ReloadComboBoxItems(ComboBox comboBox, ResourceManager resources) 
		{
			if (comboBox.Items.Count > 0) 
			{
				int selectedIndex = comboBox.SelectedIndex;
				ReloadItems(comboBox.Name, comboBox.Items, comboBox.Items.Count, resources);
				if (!comboBox.Sorted)
					comboBox.SelectedIndex = selectedIndex;
			}
		}

		/// <summary>
		///     Reloads items in the <c>DomainUpDown</c> control. If items are 
		///     not sorted, selection is kept.
		/// </summary>
		/// <param name="listBox">
		///     <c>DomainUpDown</c> to localize.
		/// </param>
		/// <param name="resources">
		///     <c>ResourceManager</c> object.
		/// </param>
		private void ReloadUpDownItems(DomainUpDown domainUpDown, ResourceManager resources) 
		{
			if (domainUpDown.Items.Count > 0) 
			{
				int selectedIndex = domainUpDown.SelectedIndex;
				ReloadItems(domainUpDown.Name, domainUpDown.Items, domainUpDown.Items.Count, resources);
				if (!domainUpDown.Sorted)
					domainUpDown.SelectedIndex = selectedIndex;
			}
		}

		/// <summary>
		///     Reloads content of a <c>TreeView</c>.
		/// </summary>
		/// <param name="treeView">
		///     <c>TreeView</c> control to reload.
		/// </param>
		/// <param name="resources">
		///     <c>ResourceManager</c> object.
		/// </param>
		private void ReloadTreeViewNodes(TreeView treeView, ResourceManager resources) 
		{
			if (treeView.Nodes.Count > 0) 
			{
				string resourceName = treeView.Name + ".Nodes";
				TreeNode[] newNodes = new TreeNode[treeView.Nodes.Count];
				newNodes[0] = (TreeNode)resources.GetObject(resourceName, m_cultureInfo);
				// VS2002 generates node resource names with additional ".Nodes" string
				if (newNodes[0] == null) 
				{
					resourceName += ".Nodes";
					newNodes[0] = (TreeNode)resources.GetObject(resourceName, m_cultureInfo);
				}
				Debug.Assert(newNodes[0] != null);
				for (int i = 1; i < treeView.Nodes.Count; i++) 
				{
					newNodes[i] = (TreeNode)resources.GetObject(resourceName + i.ToString(), m_cultureInfo);
				}
				treeView.Nodes.Clear();
				treeView.Nodes.AddRange(newNodes);
			}
		}

		/// <summary>
		///     Clears all items in the <c>IList</c> and reloads the list with
		///     items according to language settings.
		/// </summary>
		/// <param name="controlName">
		///     Name of the control.
		/// </param>
		/// <param name="list">
		///     <c>IList</c> with items to change.
		/// </param>
		/// <param name="itemsNumber">
		///     Number of items.
		/// </param>
		/// <param name="resources">
		///     <c>ResourceManager</c> object.
		/// </param>
		private void ReloadItems(string controlName, IList list, int itemsNumber, ResourceManager resources) 
		{
			string resourceName = controlName + ".Items";
			list.Clear();
			object obj = resources.GetString(resourceName, m_cultureInfo);
			// VS2002 generates item resource name with additional ".Items" string
			if (obj == null) 
			{
				resourceName += ".Items";
				obj = resources.GetString(resourceName, m_cultureInfo);
			}
			Debug.Assert(obj != null);
			list.Add(obj);
			for (int i = 1; i < itemsNumber; i++) 
				list.Add(resources.GetString(resourceName + i, m_cultureInfo));
		}
	}
}