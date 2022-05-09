using SkillProfi_Shared;
using System.Linq;

namespace SkillProfi_WebAPI.Classes
{
    /// <summary>
    /// класс для заполнения БД начальными данными
    /// </summary>
    public static class SampleSkillProfi
    {
        public static void Initialize(SkillProfiContext context)
        {
            bool isChange = false;
            if (!context?.MenuNamesAndPageHeaders?.Any() ?? false)
            {
                context?.MenuNamesAndPageHeaders.Add(new MenuNameAndPageHeader());
                isChange = true;
            }
            if (!context?.DataBidPage?.Any() ?? false)
            {
                context?.DataBidPage.Add(new BidPageData());
                isChange = true;
            }
            if (!context?.DataBidPage?.Any() ?? false)
            {
                context?.DataBidPage.Add(new BidPageData());
                isChange = true;
            }
            if (!context?.Contacts?.Any() ?? false)
            {
                context?.Contacts.Add(new Contact());
                isChange = true;
            }
            if (!context?.Contacts?.Any() ?? false)
            {
                context?.Contacts.Add(new Contact());
                isChange = true;
            }
            if (isChange)
                context.SaveChanges();
        }
    }
}
