using PaginationTest.Shared;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Cosmonaut;
using Cosmonaut.Extensions;

namespace PaginationTest.Server.Controllers
{
    [Route("api")]
    public class StudentsController : Controller
    {
        private readonly ICosmosStore<Student> _studentsStore;

        public StudentsController(ICosmosStore<Student> studentsStore)
        {
            _studentsStore = studentsStore;
        }

        [HttpGet("students/skiptake")]
        public async Task<NumSizePagedData<Student>> GetStudentsNumSize([FromQuery]int page = 1, [FromQuery]int pageSize = 10)
        {
            var students = await _studentsStore.Query().WithPagination(page, pageSize).ToListAsync();
            var totalCount = await _studentsStore.Query().CountAsync();
            var pager = new NumSizePager(totalCount, page, pageSize);

            return new NumSizePagedData<Student>
            {
                Items = students,
                NumSizePager = pager
            };
        }

        [HttpGet("students/nextprev")]
        public async Task<NextPrevPagedData<Student>> GetStudentsNextPrev([FromQuery]string continuationToken = "", [FromQuery]int pageSize = 10)
        {
            var students = await _studentsStore.Query().WithPagination(continuationToken, pageSize).ToPagedListAsync();

            return new NextPrevPagedData<Student>
            {
                Items = students.Results,
                Pager = new NextPrevPager {ContinuationToken = students.NextPageToken, HasNextPage = students.HasNextPage}
            };
        }
    }
}
