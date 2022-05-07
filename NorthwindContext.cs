using Microsoft.EntityFrameworkCore;

namespace FinalProject
{
    public class NorthwindContext : DbContext
    {
        public DbSet<Category2> Categories { get; set; }

        public void AddCategory(Category2 category)
        {
            this.Categories.Add(category);
            this.SaveChanges();
        }
    }
}