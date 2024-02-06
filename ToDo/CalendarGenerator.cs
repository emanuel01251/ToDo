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
                    Font = new Font("Arial Black", 10),
                    Height = 20,
                    Dock = DockStyle.Fill,
                    TextAlign = ContentAlignment.MiddleCenter,
                    BackColor = Color.LightGray // Just an example, you can customize the appearance
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
            // each column in the panel represents a day and each row represents a to-do item
            foreach (DataRow row in todoList.Rows)
            {
                var date = (DateTime)row["Date"];
                if (date >= startDate && date < startDate.AddDays(7))
                {
                    int columnIndex = (int)date.DayOfWeek;
                    // Correct the row index to 1 to retrieve the FlowLayoutPanel from the second row.
                    var dayPanel = (FlowLayoutPanel)panel.GetControlFromPosition(columnIndex, 1);


                    // Create a control to represent the to-do item
                    var taskLabel = new Label
                    {
                        Text = row["Title"].ToString() + " - " + row["Description"].ToString() + " - " + row["Deadline"].ToString(),
                        Font = new Font("Arial", 10, FontStyle.Regular),
                        Height = 20,
                        AutoSize = true,
                        Margin = new Padding(3),
                        Padding = new Padding(3),
                        //BorderStyle = BorderStyle.FixedSingle
                    };

                    // Add the control to the corresponding day's FlowLayoutPanel
                    dayPanel.Controls.Add(taskLabel);
                }
            }
        }

    }
}


