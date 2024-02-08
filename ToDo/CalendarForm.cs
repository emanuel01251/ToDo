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
        private ICalendarGenerator _calendarGenerator;
        private DataTable _todoList;

        public DataTable TodoList { get; }

        public CalendarForm(DataTable todoList, ICalendarGenerator calendarGenerator)
        {
            InitializeComponent();
            _todoList = todoList;
            _calendarGenerator = calendarGenerator;

            SetUpComboBoxes();
            InitializeWeeklyCalendar(DateTime.Now);
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
            // Use the _calendarGenerator to create the layout
            _calendarGenerator.GenerateCalendarLayout(calendarLayoutPanel, startDate);
        }

        private void PopulateWeeklyCalendar(DateTime startDate)
        {
            // Use the _calendarGenerator to populate the calendar
            _calendarGenerator.PopulateCalendar(calendarLayoutPanel, startDate, _todoList);
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

        private void calendarLayoutPanel_Paint(object sender, PaintEventArgs e)
        {
            base.OnPaint(e);

            int cornerRadius = 1;

            System.Drawing.Drawing2D.GraphicsPath path = new System.Drawing.Drawing2D.GraphicsPath();
            path.AddArc(new Rectangle(0, 0, cornerRadius, cornerRadius), 180, 90);
            path.AddArc(new Rectangle(Width - cornerRadius, 0, cornerRadius, cornerRadius), -90, 90);
            path.AddArc(new Rectangle(Width - cornerRadius, Height - cornerRadius, cornerRadius, cornerRadius), 0, 90);
            path.AddArc(new Rectangle(0, Height - cornerRadius, cornerRadius, cornerRadius), 90, 90);
            path.CloseFigure();

            e.Graphics.SetClip(path);

            using (SolidBrush brush = new SolidBrush(BackColor))
            {
                e.Graphics.FillRectangle(brush, 0, 0, Width, Height);
            }

            using (Pen pen = new Pen(Color.Transparent))
            {
                e.Graphics.DrawPath(pen, path);
            }
        }

        private void flowLayoutPanel5_Paint(object sender, PaintEventArgs e)
        {

        }

        private void flowLayoutPanel1_Paint(object sender, PaintEventArgs e)
        {

        }
    }
}