using SkillProfi_Shared;
using SkillProfi_WebAPI.Models.DBData;
using System.Linq;

namespace SkillProfi_WebAPI.DATA
{
    /// <summary>
    /// класс для заполнения БД начальными данными
    /// </summary>
    public static class SampleSkillProfiData
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
