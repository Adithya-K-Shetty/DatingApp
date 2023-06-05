namespace API.Helpers
{
    public class PaginationParams
    {
         private const int MaxPageSize = 50;
        public int PageNumber { get; set; } = 1; //always return first page unless they specify

        //underscore for variable
        //is convention for private access specifier
        private int _pageSize = 10;

        public int PageSize
        {
            get => _pageSize;
            set => _pageSize = (value > MaxPageSize) ? MaxPageSize : value;
        }
    }
}