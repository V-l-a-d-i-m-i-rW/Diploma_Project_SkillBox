using System;
/// <summary>
/// класс описания заявки
/// </summary>
namespace SkillProfi_Shared
{
    public class Bid
    {
        /// <summary>
        /// идентификатор заявки
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// дата добавления заявки
        /// </summary>
        public DateTime Date { get; set; }

        /// <summary>
        /// имя пользователя
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// emailGjkmpjdfntkz
        /// </summary>
        public string UserEmail { get; set; }

        /// <summary>
        /// текст заявки
        /// </summary>
        public string Text { get; set; }

        public BidStatusEnum BidStatus { get; set; }

        public Bid()
        {
            BidStatus = BidStatusEnum.received;
        }
    }
}
