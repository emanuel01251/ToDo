using System;
using System.Data;
using System.Windows.Forms;
using System.IO;
using System.Drawing;

namespace ToDo
{
    public partial class TodoList : Form
    {
        DataTable todoList = new DataTable();
        bool isEditing = false;
        private const string DataFilePath = "TodoData.xml";
        private DateTimePicker dateTimePickerDeadline;

        public TodoList()
        {
            InitializeComponent();
            // Set the table name
            todoList.TableName = "Todos";

            // Add a column to our datatable
            todoList.Columns.Add("Date", typeof(DateTime));
            todoList.Columns.Add("Title");
            todoList.Columns.Add("Description");
            todoList.Columns.Add("Deadline", typeof(DateTime));
            todoList.Columns.Add("Done", typeof(bool));
            AddDoneColumnToDataGridView();
            toDoListView.DataSource = todoList;

            // Set initial placeholder text
            textBoxTitle.Text = "Enter title...";
            textBoxTitle.ForeColor = Color.Gray;

            textBoxDescription.Text = "Enter description...";
            textBoxDescription.ForeColor = Color.Gray;

            // Subscribe to Enter and Leave events
            textBoxTitle.Enter += TextBoxTitle_Enter;
            textBoxTitle.Leave += TextBoxTitle_Leave;

            textBoxDescription.Enter += TextBoxDescription_Enter;
            textBoxDescription.Leave += TextBoxDescription_Leave;

            toDoListView.CellContentClick += toDoListView_CellContentClick;

            monthCalendar1.SetDate(DateTime.Today);
            toDoListView.CellFormatting += new DataGridViewCellFormattingEventHandler(toDoListView_CellFormatting);

            InitializeDateTimePickerDeadline();
        }

        private void InitializeDateTimePickerDeadline()
        {
            dateTimePickerDeadline = new DateTimePicker();
            dateTimePickerDeadline.Location = new Point(100, 130);
            dateTimePickerDeadline.Size = new Size(160, 20);

            // Set the format of the date.
            dateTimePickerDeadline.Format = DateTimePickerFormat.Custom;
            dateTimePickerDeadline.CustomFormat = "MM/dd/yyyy";

            // Add the DateTimePicker to the form's controls.
            this.Controls.Add(dateTimePickerDeadline);
        }

        // Start of Textbox Placeholder
        private void TextBoxTitle_Enter(object sender, EventArgs e)
        {
            if (textBoxTitle.Text == "Enter title...")
            {
                textBoxTitle.Text = "";
                textBoxTitle.ForeColor = Color.Black;
            }
        }

        private void TextBoxTitle_Leave(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(textBoxTitle.Text))
            {
                textBoxTitle.Text = "Enter title...";
                textBoxTitle.ForeColor = Color.Gray;
            }
        }

        private void TextBoxDescription_Enter(object sender, EventArgs e)
        {
            if (textBoxDescription.Text == "Enter description...")
            {
                textBoxDescription.Text = "";
                textBoxDescription.ForeColor = Color.Black;
            }
        }

        private void TextBoxDescription_Leave(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(textBoxDescription.Text))
            {
                textBoxDescription.Text = "Enter description...";
                textBoxDescription.ForeColor = Color.Gray;
            }
        }
        // End of Textbox Placeholder

        //Start of Done function
        private void AddDoneColumnToDataGridView()
        {
            var doneColumn = new DataGridViewCheckBoxColumn
            {
                Name = "DoneColumn",
                HeaderText = "Done",
                DataPropertyName = "Done", // This must match the name of the DataColumn.
                AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells,
                TrueValue = true,
                FalseValue = false
            };
            toDoListView.Columns.Add(doneColumn);
        }

        private void toDoListView_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            // Confirm this is a click in the "DoneColumn" column.
            if (toDoListView.Columns[e.ColumnIndex] is DataGridViewCheckBoxColumn && e.RowIndex >= 0)
            {
                // Safely toggle the "Done" status.
                object value = toDoListView.Rows[e.RowIndex].Cells["DoneColumn"].Value;
                bool currentValue = (value is DBNull) ? false : Convert.ToBoolean(value);
                toDoListView.Rows[e.RowIndex].Cells["DoneColumn"].Value = !currentValue;

                // Optionally, save the updated to-do list to your data file.
                todoList.WriteXml(DataFilePath);
                RefreshTodoList(monthCalendar1.SelectionRange.Start.Date);
            }
        }

        private void toDoListView_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            if (toDoListView.Columns["DoneColumn"] != null && e.ColumnIndex == toDoListView.Columns["DoneColumn"].Index && e.RowIndex >= 0)
            {
                DataGridViewRow row = toDoListView.Rows[e.RowIndex];
                var doneCellValue = row.Cells["DoneColumn"].Value;

                // Safely check if the value is not null and is a boolean before casting.
                if (doneCellValue != null && doneCellValue != DBNull.Value && doneCellValue is bool)
                {
                    bool done = (bool)doneCellValue;
                    if (done)
                    {
                        row.DefaultCellStyle.Font = new Font(toDoListView.Font, FontStyle.Strikeout);
                        row.DefaultCellStyle.ForeColor = Color.Gray;
                    }
                    else
                    {
                        row.DefaultCellStyle.Font = new Font(toDoListView.Font, FontStyle.Regular);
                        row.DefaultCellStyle.ForeColor = Color.Black;
                    }
                }
            }
        }

        //End of Done Function
        private void TodoList_Load(object sender, EventArgs e)
        {
            // Load existing to-dos if the file exists and is not empty
            if (File.Exists(DataFilePath) && new FileInfo(DataFilePath).Length > 0)
            {
                try
                {
                    todoList.ReadXml(DataFilePath);
                }
                catch (Exception ex)
                {
                    MessageBox.Show("There was an error reading the to-do list data. The file may be corrupted. " +
                                    "Please check the file or delete it to create a new to-do list.",
                                    "Error Reading Data", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            else
            {

            }

            ShowAllTodoItems();
            RefreshTodoList(monthCalendar1.SelectionRange.Start.Date);
        }

        private void monthCalendar1_DateChanged(object sender, DateRangeEventArgs e)
        {

            RefreshTodoList(e.Start);
        }

        private void FilterTodosByDate(DateTime date)
        {

            DataView view = new DataView(todoList);
            view.RowFilter = $"Date = '{date.ToString("yyyy-MM-dd")}'";
            toDoListView.DataSource = view;
        }

        private void saveButton_Click(object sender, EventArgs e)
        {
            if (isEditing)
            {
                DataRow row = todoList.Rows[toDoListView.CurrentCell.RowIndex];
                row["Title"] = textBoxTitle.Text;
                row["Description"] = textBoxDescription.Text;
                row["Date"] = monthCalendar1.SelectionRange.Start.Date;
                row["Deadline"] = dateTimePickerDeadline.Value.Date; // Set the deadline
            }
            else
            {
                todoList.Rows.Add(
                    monthCalendar1.SelectionRange.Start.Date,
                    textBoxTitle.Text,
                    textBoxDescription.Text,
                    dateTimePickerDeadline.Value.Date // Add the deadline
                );
            }

            // Clear the inputs
            textBoxTitle.Text = "Enter title...";
            textBoxDescription.Text = "Enter description...";
            isEditing = false;

            // Set placeholder text color
            textBoxTitle.ForeColor = Color.Gray;
            textBoxDescription.ForeColor = Color.Gray;

            // Filter and refresh the to-do list view
            FilterTodosByDate(monthCalendar1.SelectionRange.Start);
            RefreshTodoList(monthCalendar1.SelectionRange.Start.Date);

            // Save the updated list to the XML file
            todoList.WriteXml(DataFilePath);
        }

        private void editButton_Click(object sender, EventArgs e)
        {
            if (toDoListView.SelectedRows.Count > 0) // Ensure there is a selected row
            { 
                isEditing = true;
                DataRow row = todoList.Rows[toDoListView.CurrentCell.RowIndex];
                textBoxTitle.Text = row["Title"].ToString();
                textBoxDescription.Text = row["Description"].ToString();
                dateTimePickerDeadline.Value = (DateTime)row["Deadline"]; // Load the deadline
                RefreshTodoList(monthCalendar1.SelectionRange.Start.Date);
            }

        }

        private void deleteButton_Click(object sender, EventArgs e)
        {
            if (toDoListView.SelectedRows.Count > 0) // Ensure there is a selected row
            {
                int rowIndex = toDoListView.CurrentCell.RowIndex;
                todoList.Rows[rowIndex].Delete();
                RefreshTodoList(monthCalendar1.SelectionRange.Start.Date);
                // If you have a backing data source, remove the item from there as well
                todoList.WriteXml(DataFilePath); // Save the changes after deletion
                
            }
            else
            {
                MessageBox.Show("Please select an item to delete.");
            }
        }


        private void newButton_Click(object sender, EventArgs e)
        {
            textBoxTitle.Text = "";
            textBoxDescription.Text = "";
        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            // Check if the double-click is not on the header.
            if (e.RowIndex >= 0)
            {
                // Begin edit the current cell if it's not the date cell.
                if (e.ColumnIndex != toDoListView.Columns["Date"].Index)
                {
                    toDoListView.BeginEdit(true);
                }
                else
                {
                    
                }
            }
        }

        private void dataGridView1_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            // Save the updated data to your data source here
            // and refresh the data grid view if needed.
            todoList.WriteXml(DataFilePath);
        }

        private void RefreshTodoList(DateTime date)
        {
            DataView view = new DataView(todoList)
            {
                RowFilter = $"Date = '{date.ToString("MM/dd/yyyy")}'"
            };

            toDoListView.DataSource = view;

            toDoListView.Refresh();
        }

        private void btnOpenCalendar_Click(object sender, EventArgs e)
        {
            ICalendarGenerator calendarGenerator = new CalendarGenerator();
 

            CalendarForm calendarForm = new CalendarForm(todoList, calendarGenerator);

            calendarForm.Show();
        }

        private void ShowAllTodoItems()
        {
            // Remove any existing date filters
            DataView view = new DataView(todoList);
            view.RowFilter = ""; // An empty string filter will show all rows

            // Set the data source of the DataGridView to the DataView with no filter
            toDoListView.DataSource = view;

            // Refresh the DataGridView to update the UI
            toDoListView.Refresh();
        }

        private void btnShowAll_Click(object sender, EventArgs e)
        {
            ShowAllTodoItems();
        }

        private void ShowTodosForDate(DateTime date)
        {
            DataView view = new DataView(todoList)
            {
                RowFilter = $"Date = '{date:MM/dd/yyyy}'"
            };

            toDoListView.DataSource = view;
        }
        private void btnShowToday_Click(object sender, EventArgs e)
        {
            ShowTodosForDate(DateTime.Today);
        }
    }
}
