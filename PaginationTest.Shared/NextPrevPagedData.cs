using System.Collections.Generic;

namespace PaginationTest.Shared
{
    public class NextPrevPagedData<T>
    {
        public IEnumerable<T> Items { get; set; }

        public NextPrevPager Pager { get; set; }
    }
}