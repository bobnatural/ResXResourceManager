﻿namespace tomenglertde.ResXManager.View.Behaviors
{
    using System;
    using System.Diagnostics.Contracts;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Data;
    using System.Windows.Interactivity;
    using System.Windows.Markup;

    using tomenglertde.ResXManager.View.ColumnHeaders;

    public class SynchronizeTextBoxWithDataGridCellBehavior : Behavior<TextBox>
    {
        public DataGrid DataGrid
        {
            get { return (DataGrid)GetValue(DataGridProperty); }
            set { SetValue(DataGridProperty, value); }
        }
        /// <summary>
        /// Identifies the DataGrid dependency property
        /// </summary>
        public static readonly DependencyProperty DataGridProperty =
            DependencyProperty.Register("DataGrid", typeof(DataGrid), typeof(SynchronizeTextBoxWithDataGridCellBehavior), new FrameworkPropertyMetadata(null, (sender, e) => ((SynchronizeTextBoxWithDataGridCellBehavior)sender).DataGrid_Changed((DataGrid)e.OldValue, (DataGrid)e.NewValue)));

        private void DataGrid_Changed(DataGrid oldValue, DataGrid newValue)
        {
            if (oldValue != null)
            {
                oldValue.CurrentCellChanged -= DataGrid_CurrentCellChanged;
            }

            if (newValue != null)
            {
                newValue.CurrentCellChanged += DataGrid_CurrentCellChanged;
                DataGrid_CurrentCellChanged(newValue, EventArgs.Empty);
            }
        }

        private TextBox TextBox
        {
            get
            {
                Contract.Ensures((AssociatedObject == null) || (Contract.Result<TextBox>() != null));

                return AssociatedObject;
            }
        }

        private void DataGrid_CurrentCellChanged(object sender, EventArgs e)
        {
            Contract.Requires(sender != null);

            var textBox = TextBox;
            if (textBox == null)
                return;

            var dataGrid = (DataGrid)sender;
            var currentCell = dataGrid.CurrentCell;

            var column = currentCell.Column as DataGridBoundColumn;
            if (column == null)
                return;

            var header = column.Header as ILanguageColumnHeader;
            if (header != null)
            {
                textBox.IsHitTestVisible = true;
                textBox.DataContext = currentCell.Item;

                var ieftLanguageTag = header.EffectiveCulture.IetfLanguageTag;
                textBox.Language = XmlLanguage.GetLanguage(ieftLanguageTag);

                BindingOperations.SetBinding(textBox, TextBox.TextProperty, column.Binding);
            }
            else
            {
                textBox.IsHitTestVisible = false;
                textBox.DataContext = null;
                BindingOperations.ClearBinding(textBox, TextBox.TextProperty);
            }
        }
    }
}
