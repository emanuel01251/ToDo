using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ToDo
{
    public class CalendarGenerator : ICalendarGenerator
    {
        public void GenerateCalendarLayout(TableLayoutPanel panel, DateTime startDate)
        {
            // Clear existing controls
            panel.Controls.Clear();
            panel.ColumnStyles.Clear();
            panel.RowStyles.Clear();

            // Add two rows: one for headers (day names) and one for items
            panel.RowCount = 2;
            panel.RowStyles.Add(new RowStyle(SizeType.Absolute, 30F)); // Row for headers
            panel.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));  // Row for items

            // Add columns for each day of the week
            for (int i = 0; i < 7; i++)
            {
                panel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 14.28F));

                // Calculate the date for the current column
                var currentDay = startDate.AddDays(i);

                // Create and add the day name header
                var headerLabel = new Label
                {
                    Text = currentDay.ToString("dddd"),
                    Font = new Font("Myanmar Text", 10, FontStyle.Bold),
                    Height = 20,
                    Dock = DockStyle.Fill,
                    TextAlign = ContentAlignment.MiddleCenter,
                    BackColor = Color.FromArgb(190, 216, 237) // Just an example, you can customize the appearance
                };
                panel.Controls.Add(headerLabel, i, 0);

                // Add a FlowLayoutPanel for each day that will contain the to-do items
                var dayPanel = new FlowLayoutPanel
                {
                    Dock = DockStyle.Fill,
                    FlowDirection = FlowDirection.TopDown,
                    AutoScroll = true,
                    Tag = currentDay // Store the date in the Tag property for reference
                };
                panel.Controls.Add(dayPanel, i, 1); // Add to the second row for content

            }
        }



        public void PopulateCalendar(TableLayoutPanel panel, DateTime startDate, DataTable todoList)
        {
            // Clear previous items from the panel, if necessary
            foreach (Control control in panel.Controls)
            {
                if (control is FlowLayoutPanel flowPanel)
                {
                    flowPanel.Controls.Clear();
                }
            }

            // Each column in the panel represents a day and each row represents a to-do item
            foreach (DataRow row in todoList.Rows)
            {
                bool isDone = row["Done"] is DBNull ? false : Convert.ToBoolean(row["Done"]);
                if (!isDone)
                {
                    var deadline = (DateTime)row["Deadline"];
                    if (deadline >= startDate && deadline < startDate.AddDays(7))
                    {
                        int columnIndex = (int)deadline.DayOfWeek;
                        var dayPanel = (FlowLayoutPanel)panel.GetControlFromPosition(columnIndex, 1);

                        if (dayPanel != null)
                        {
                            // Create a control to represent the to-do item
                            var taskLabel = new Label
                            {
                                Text = $"{row["Title"]} - {row["Description"]} - {deadline.ToString("MM/dd/yyyy")}",
                                Font = new Font("Myanmar Text", 10),
                                AutoSize = true,
                                Margin = new Padding(3),
                                Padding = new Padding(3)
                            };

                            // Add the control to the corresponding day's FlowLayoutPanel
                            dayPanel.Controls.Add(taskLabel);
                        }
                    }
                }
            }
        }

    }
}


