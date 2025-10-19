using Malama.Models;

namespace ExcelFilesCompiler.Interfaces
{
    public interface IContainerMonitoringService
    {
        Task<Container?> GetContainerByIdAsync(long id);
        Task<IEnumerable<ContainerType>> GetAllContainerTypesAsync();
        Task<List<Container>> GetContainersByEventIdAsync(string eventId);
        Task<List<Container>> GetOnlyContainersByEventIdAsync(string eventId);
        Task<List<ContainerTemperatureReading>> GetReadingsForContainer(long containerId);
        Task<ResponseDto> AddContainerAsync(CreateContainerDto dto, string addedBy);
        Task<ResponseDto> AddReadingAsync(CreateReadingDto dto, string performedBy);
    }
}
