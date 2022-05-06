using Microsoft.EntityFrameworkCore;

namespace FinalProject
{
    public class NorthwindContext : DbContext
    {
        public DbSet<Category> Categories { get; set; }

        public void AddCategory(Category category)
        {
            this.Categories.Add(category);
            this.SaveChanges();
        }
    }
}