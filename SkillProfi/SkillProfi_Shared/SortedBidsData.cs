using System.Collections.Generic;

namespace SkillProfi_Shared
{
    public class SortedBidsData
    {
        /// <summary>
        /// общее количество заявок в БД
        /// </summary>
        public int AllBidsCount { get; set; }
        /// <summary>
        /// cписок отсортированных заявок
        /// </summary>
        public List<Bid> Bids { get; set; }
    }
}
