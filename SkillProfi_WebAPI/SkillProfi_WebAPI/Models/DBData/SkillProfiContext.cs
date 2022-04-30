using Microsoft.EntityFrameworkCore;
using SkillProfi_Shared;

namespace SkillProfi_WebAPI.Models.DBData
{
    public class SkillProfiContext : DbContext
    {
        /// <summary>
        /// список заявок
        /// </summary>
        public DbSet<Bid> Bids { get; set; }
        /// <summary>
        /// список услуг
        /// </summary>
        public DbSet<Service> Services { get; set; }

        /// <summary>
        /// список проектов
        /// </summary>
        public DbSet<Project> Projects { get; set; }

        /// <summary>
        /// список блогов
        /// </summary>
        public DbSet<Blog> Blogs { get; set; }

        /// <summary>
        /// список контактов
        /// </summary>
        public DbSet<Contact> Contacts { get; set; }

        /// <summary>
        /// список ссылок контактов
        /// </summary>
        public DbSet<ContactLink> Links{ get; set; }

        /// <summary>
        /// список описаний для блока заголовка страницы
        /// </summary>
        public DbSet<HeaderDescription> HeaderDescriptions { get; set; }

        /// <summary>
        /// список названий меню
        /// </summary>
        public DbSet<MenuNameAndPageHeader> MenuNamesAndPageHeaders { get; set; }


        /// <summary>
        /// список названий меню
        /// </summary>
        public DbSet<BidPageData> DataBidPage { get; set; }

        public SkillProfiContext(DbContextOptions<SkillProfiContext> options) : base(options)
        {
           Database.EnsureCreated();
        }
    }
}
