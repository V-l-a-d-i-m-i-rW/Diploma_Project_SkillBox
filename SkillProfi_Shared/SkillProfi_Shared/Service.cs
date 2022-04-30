namespace SkillProfi_Shared
{
    /// <summary>
    /// класс описания услуги 
    /// </summary>
    public class Service
    {
        /// <summary>
        /// идентификатор услуги
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// заголовок услуги
        /// </summary>
        public string Header { get; set; }

        /// <summary>
        /// детальное описание услуги
        /// </summary>
        public string Description { get; set; }
        ///// <summary>
        ///// детальное описание услуги без html
        ///// </summary>
        //[JsonIgnore]
        //[NotMapped]
        //public string DescriptionNotHtml { get => SkillProfiHelper.GetStringWithOutHtml(Description); }
    }
}
