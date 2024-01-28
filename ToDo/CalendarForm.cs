using System;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.Runtime.InteropServices.ComTypes;
using System.Windows.Forms;

namespace ToDo
{
    public partial class CalendarForm : Form
    {
        private DataTable _todoList;

        public CalendarForm(DataTable todoList)
        {
            InitializeComponent();
            _todoList = todoList;

            SetUpComboBoxes();

            this.cmbMonth.SelectedIndexChanged += new EventHandler(cmbMonthYear_SelectedIndexChanged);
            this.cmbYear.SelectedIndexChanged += new EventHandler(cmbMonthYear_SelectedIndexChanged);
            this.cmbWeeks.SelectedIndexChanged += new EventHandler(cmbWeeks_SelectedIndexChanged);

            // Populate the weeks ComboBox based on the current month and year
            PopulateWeeksComboBox(DateTime.Now.Month, DateTime.Now.Year);
        }


        private void SetUpComboBoxes()
        {
            for (int month = 1; month <= 12; month++)
            {
                cmbMonth.Items.Add(CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(month));
            }
            cmbMonth.SelectedIndex = DateTime.Now.Month - 1;

            int currentYear = DateTime.Now.Year;
            for (int year = currentYear - 10; year <= currentYear + 10; year++)
            {
                cmbYear.Items.Add(year.ToString());
            }
            cmbYear.SelectedItem = currentYear.ToString();
        }

        private void InitializeWeeklyCalendar(DateTime startDate)
        {
            // Clear existing labels
            foreach (FlowLayoutPanel panel in calendarLayoutPanel.Controls)
            {
                panel.Controls.Clear();
            }


            for (int i = 0; i < 7; i++)
            {
                DateTime currentDay = startDate.AddDays(i);
                int columnIndex = (int)currentDay.DayOfWeek;

                FlowLayoutPanel dayPanel = (FlowLayoutPanel)calendarLayoutPanel.GetControlFromPosition(columnIndex, 0);
                if (dayPanel != null)
                {
                    Label lblDay = new Label
                    {
                        Text = currentDay.ToString("dddd, MMMM d"),
                        Dock = DockStyle.Top,
                        TextAlign = ContentAlignment.MiddleCenter,
                        Font = new Font("Arial", 10, FontStyle.Bold),
                        Height = 20
                    };
                    dayPanel.Controls.Add(lblDay);
                }
            }
        }

        private void PopulateWeeklyCalendar(DateTime startDate)
        {
            // Clear existing items
            foreach (FlowLayoutPanel panel in calendarLayoutPanel.Controls)
            {
                // Skip the day header
                for (int i = panel.Controls.Count - 1; i > 0; i--)
                {
                    panel.Controls.RemoveAt(i);
                }
            }

            // Populate each day's panel with to-do items
            var endDate = startDate.AddDays(7);
            foreach (DataRow row in _todoList.Rows)
            {
                var date = (DateTime)row["Date"];
                if (date >= startDate && date < endDate)
                {
                    var title = row["Title"].ToString();
                    var panel = (FlowLayoutPanel)calendarLayoutPanel.GetControlFromPosition((int)date.DayOfWeek, 0);
                    panel.Controls.Add(new Label
                    {
                        Text = title,
                        AutoSize = true
                    });
                }
            }
        }

        private void cmbMonthYear_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cmbMonth.SelectedIndex >= 0 && cmbYear.SelectedIndex >= 0)
            {
                int month = cmbMonth.SelectedIndex + 1;
                int year = int.Parse(cmbYear.SelectedItem.ToString());
                PopulateWeeksComboBox(month, year);
            }
        }


        private DateTime GetStartDateOfWeek(int month, int year)
        {
            // Assuming the week starts on Sunday and the first week is the first of the month
            var firstDayOfMonth = new DateTime(year, month, 1);
            return firstDayOfMonth.AddDays(-(int)firstDayOfMonth.DayOfWeek);
        }

        private void CalendarForm_Load(object sender, EventArgs e)
        {
            // Generate the calendar for the current month and year when the form loads
            var startDate = GetStartDateOfWeek(DateTime.Now.Month, DateTime.Now.Year);
            PopulateWeeklyCalendar(startDate);
            PopulateWeeksComboBox(DateTime.Now.Month, DateTime.Now.Year);
        }

        private void PopulateWeeksComboBox(int month, int year)
        {
            cmbWeeks.Items.Clear(); // Clear existing items

            DateTime firstDayOfMonth = new DateTime(year, month, 1);
            // Find the first day of the week (e.g., Sunday)
            DateTime startOfWeek = firstDayOfMonth.AddDays(-((int)firstDayOfMonth.DayOfWeek));
            // Calculate the end date of the first week
            DateTime endOfWeek = startOfWeek.AddDays(6);

            // Loop through all weeks of the month
            while (startOfWeek.Month == month || endOfWeek.Month == month)
            {
                string weekRange = $"{startOfWeek:MM/dd/yyyy} - {endOfWeek:MM/dd/yyyy}";
                cmbWeeks.Items.Add(weekRange);

                // Move to the next week
                startOfWeek = endOfWeek.AddDays(1);
                endOfWeek = startOfWeek.AddDays(6);
            }

            if (cmbWeeks.Items.Count > 0)
                cmbWeeks.SelectedIndex = 0; // Select the first week by default
        }


        private void cmbWeeks_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cmbWeeks.SelectedItem != null)
            {
                string selectedWeek = cmbWeeks.SelectedItem.ToString();
                // Parse the startDate from the selectedWeek string
                DateTime startDate = DateTime.ParseExact(selectedWeek.Split(new string[] { " - " }, StringSplitOptions.None)[0], "MM/dd/yyyy", CultureInfo.InvariantCulture);

                InitializeWeeklyCalendar(startDate);
                PopulateWeeklyCalendar(startDate);
            }
        }

    }
}