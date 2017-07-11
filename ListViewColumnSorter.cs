using System;
using System.Collections;
using System.Windows.Forms;

namespace directoryWatcher
{
    public sealed class ListViewColumnSorter : IComparer
    {
        private int column_to_sort;
        private SortOrder order_of_sort;
        public ListViewColumnSorter()
        {
            column_to_sort = 0;
            order_of_sort = SortOrder.Ascending;
        }
        public int Compare(object x, object y)
        {
            int compare_result = 0;
            ListViewItem lv_x, lv_y;
            lv_x = x as ListViewItem;
            lv_y = y as ListViewItem;

            if (lv_x != lv_y)
            {
                int int_lv_x, int_lv_y;
                Int32.TryParse(lv_x.SubItems[column_to_sort].Text, out int_lv_x);
                Int32.TryParse(lv_y.SubItems[column_to_sort].Text, out int_lv_y);

                if (int_lv_x > 0 && int_lv_y > 0)
                    compare_result = Decimal.Compare(int_lv_x, int_lv_y);
                else
                    compare_result = String.CompareOrdinal(lv_x.SubItems[column_to_sort].Text, lv_y.SubItems[column_to_sort].Text);
            }

            if (order_of_sort == SortOrder.Ascending)
                return compare_result;

            return -compare_result;
        }

        public int SortColumn
        {
            set
            {
                column_to_sort = value;
            }
            get
            {
                return column_to_sort;
            }
        }
        public SortOrder Order
        {
            set
            {
                order_of_sort = value;
            }
            get
            {
                return order_of_sort;
            }
        }
    }
}
