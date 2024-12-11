using Microsoft.EntityFrameworkCore;

namespace ContosoUniversity.Views{
    public class PaginatedList<T> : List<T> {

        public int PageIndex {get; set;}

        public int TotalPages {get; set;}

        public PaginatedList(IEnumerable<T> items, int pageIndex, int pageSize, int count){

            this.TotalPages =(int) Math.Ceiling( count / (double) pageSize);
            this.PageIndex = pageIndex;
            this.AddRange(items);
        }

        public bool HasPreviousPage => PageIndex > 1;
        public bool HasNextPage => PageIndex < TotalPages;
        public static async Task<PaginatedList<T>> CreateAsync(IQueryable<T> source, int pageSize, int pageIndex){
            int count = await source.CountAsync();
            var items = await source.Skip((pageIndex - 1)* pageSize).Take(pageSize).ToListAsync();

            return new PaginatedList<T>(items, pageIndex, pageSize, count);
        }


    }
}