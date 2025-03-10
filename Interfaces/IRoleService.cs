using ExcelFilesCompiler.Models;
using ExcelToCsv.Models;
using Microsoft.EntityFrameworkCore.Storage;

namespace ExcelFilesCompiler.Interfaces
{
    public interface IRoleService
    {
        Task<List<ApplicationRole>> GetRolesByCategoryAsync(string category);
        //Task<ResponseDto> UpdateUserEventStaffRolesAsync(EventStaff eventStaff);
    }

}
