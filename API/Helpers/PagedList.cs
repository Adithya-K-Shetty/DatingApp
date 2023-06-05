using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace API.Helpers
{
    //T represent a generic type
    //it suites different types
    public class PagedList<T> : List<T>
    {
        public PagedList(IEnumerable<T> items, int count, int pageNumber, int pageSize)
        {
            CurrentPage = pageNumber;
            TotalPages = (int)Math.Ceiling(count/(double)pageSize);
            PageSize = pageSize;
            TotalCount = count;
            AddRange(items); //adds the items to the ienumerable list
        }

        public int CurrentPage {get;set;} //Represents the current page number.
        public int TotalPages { get;set;} //Represents the total number of pages in the paged list
        public int PageSize {get;set;} //Represents the number of items per page.

        public int TotalCount {get;set;} //Represents the total number of items in the source collection


        //CreateAsync method calculates the total count of items in the source collection, retrieves the appropriate subset of items for the specified page,
        //and returns a new instance of PagedList<T>


        //source represent :-  source data to be paged
        public static async Task<PagedList<T>> CreateAsync(IQueryable<T>source,int pageNumber,int pageSize)
        {
            //source collection of items used for querying and filtering
            var count = await source.CountAsync(); //count of items from query
            // skips the appropriate number of items based 
            //on the current pageNumber and pageSize values
            var items = await source.Skip((pageNumber-1) *pageSize).Take(pageSize).ToListAsync();
            return new PagedList<T>(items,count,pageNumber,pageSize);
        }
    }
}