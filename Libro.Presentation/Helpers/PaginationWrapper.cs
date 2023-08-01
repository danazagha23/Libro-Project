namespace Libro.Presentation.Helpers
{
    public class PaginationWrapper<T> : IPaginationWrapper<T> where T : class
    {
        public ICollection<T> GetPage(ICollection<T> data, int page, int pageSize)
        {
            if (page < 1)
                page = 1;

            return data.Skip((page - 1) * pageSize).Take(pageSize).ToList();
        }

        public int GetTotalPages(IEnumerable<T> data, int pageSize)
        {
            return (int)Math.Ceiling((double)data.Count() / pageSize);
        }
    }
}
