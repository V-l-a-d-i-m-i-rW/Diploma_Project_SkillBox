using System.ComponentModel.DataAnnotations.Schema;

namespace SkillProfi_Shared
{
    /// <summary>
    /// описание для блока заголовка страницы
    /// </summary>
    public class HeaderDescription
    {
        /// <summary>
        /// идентификатор описания заголовка
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// детальное описания заголовка
        /// </summary>
        public string Description { get; set; }
        /// <summary>
        /// признак что ссылка уже сохранена 
        /// </summary>
        [NotMapped]
        public bool Saved { get; set; }

        public HeaderDescription()
        {
            Saved = false;
        }
    }
}
