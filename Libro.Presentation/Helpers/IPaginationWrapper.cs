namespace Libro.Presentation.Helpers
{
    public interface IPaginationWrapper<T>
    {
        ICollection<T> GetPage(ICollection<T> data, int page, int pageSize);
        int GetTotalPages(IEnumerable<T> data, int pageSize);
    }
}
