using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ToDo
{
    public interface ICalendarGenerator
    {
        void GenerateCalendarLayout(TableLayoutPanel panel, DateTime startDate);
        void PopulateCalendar(TableLayoutPanel panel, DateTime startDate, DataTable todoList);
    }
}

