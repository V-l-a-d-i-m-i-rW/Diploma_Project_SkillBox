using Microsoft.AspNetCore.Mvc;
using SkillProfi_Shared;
using SkillProfi_WebAPI.Models.DBData;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SkillProfi_WebAPI.Controllers
{
    /// <summary>
    /// данные страницы Проекты
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class ProjectDataController : ControllerBase
    {
        #region поля
        /// <summary>
        /// ссылка на контекст БД
        /// </summary>
        private readonly SkillProfiContext db;
        #endregion
        #region коннструкторы
        public ProjectDataController(SkillProfiContext context)
        {
            db = context;
        }
        #endregion

        /// <summary>
        /// получить все проекты (GET api/ProjectData)
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<IEnumerable<Project>> GetProjectsAsync()
        {
            return await Task.Run(() => db?.Projects?.ToList());
        }

        /// <summary>
        /// получить проект по идентификатору (GET api/ProjectData/Id)
        /// </summary>
        /// <param name="Id">идентификатор проекта</param>
        /// <returns></returns>
        [HttpGet("{Id}")]
        public async Task<Project> GetProjectByIdAsync(int? Id)
        {
            return await Task.Run(() => db?.Projects?.FirstOrDefault(p => p.Id == Id));
        }

        /// <summary>
        /// редактировать проект (PUT api/ProjectData)
        /// </summary>
        /// <param name="project">отредактированный проект</param>
        /// <returns></returns>
        [HttpPut]
        public async Task EditProjectAsync([FromBody]Project project)
        {
            if (project != null)
            {
                //project.Image = SkillProfiHelper.GetImageToImageByte(project.ImageFormFile);
                Project cur_project = await GetProjectByIdAsync(project.Id);
                if (cur_project != null)
                {
                    cur_project.Header = project.Header;
                    cur_project.Description = project.Description;
                    if (project.Image?.Length > 0)
                        cur_project.Image = project.Image;
                    await db.SaveChangesAsync();
                }                
            }
        }

        /// <summary>
        ///  добавить проект (POST api/ProjectData)
        /// </summary>
        /// <param name="project">добавляемый проект</param>
        /// <returns></returns>
        [HttpPost]
        public async Task CreateProjectAsync([FromBody]Project project)
        {
            if (project?.Id == 0)
            {
                //project.Image = SkillProfiHelper.GetImageToImageByte(project.ImageFormFile);
                await db.Projects.AddAsync(project);
                await db.SaveChangesAsync();               
            }
        }
        /// <summary>
        /// удалить проект по идентификатору (DELETE api/ServiceData/Id)
        /// </summary>
        /// <param name="Id">идентификатор проекта</param>
        /// <returns></returns>
        [HttpDelete("{Id}")] 
        public async Task RemoveProjectByIdAsync(int? Id)
        {
            Project removeProject = await GetProjectByIdAsync(Id);
            if (removeProject != null)
            {
                db.Projects.Remove(removeProject);
                await db.SaveChangesAsync();
            }
        }
    }
}
