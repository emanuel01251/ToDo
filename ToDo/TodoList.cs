using System;
using System.Data;
using System.Windows.Forms;
using System.IO;

namespace ToDo
{
    public partial class TodoList : Form
    {
        DataTable todoList = new DataTable();
        bool isEditing = false;
        private const string DataFilePath = "TodoData.xml";

        public TodoList()
        {
            InitializeComponent();

            // Set the table name
            todoList.TableName = "Todos";

            // Add a date column to our datatable
            todoList.Columns.Add("Date", typeof(DateTime));
            todoList.Columns.Add("Title");
            todoList.Columns.Add("Description");

            toDoListView.DataSource = todoList;

            monthCalendar1.SetDate(DateTime.Today);
        }

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
            }
            else
            {
                todoList.Rows.Add(monthCalendar1.SelectionRange.Start.Date, textBoxTitle.Text, textBoxDescription.Text);
            }

            textBoxTitle.Text = "";
            textBoxDescription.Text = "";
            isEditing = false;
            FilterTodosByDate(monthCalendar1.SelectionRange.Start);

            RefreshTodoList(monthCalendar1.SelectionRange.Start.Date);
            todoList.WriteXml(DataFilePath);
        }

        private void editButton_Click(object sender, EventArgs e)
        {
            isEditing = true;

            textBoxTitle.Text = todoList.Rows[toDoListView.CurrentCell.RowIndex].ItemArray[1].ToString();
            textBoxDescription.Text = todoList.Rows[toDoListView.CurrentCell.RowIndex].ItemArray[2].ToString();
        }

        private void deleteButton_Click(object sender, EventArgs e)
        {
            if (toDoListView.SelectedRows.Count > 0) // Ensure there is a selected row
            {
                // Get the current selected row
                DataGridViewRow selectedRow = toDoListView.SelectedRows[0];

                if (selectedRow != null) // Check if the selected row is not null
                {
                    toDoListView.Rows.Remove(selectedRow);
                    // If you have a backing data source, remove the item from there as well
                }
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
            RefreshTodoList(monthCalendar1.SelectionRange.Start.Date);
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



    }
}
