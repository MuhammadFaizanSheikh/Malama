using Malama.Models;

namespace ExcelFilesCompiler.Interfaces
{
    public interface IContainerMonitoringService
    {
        Task<Container?> GetContainerByIdAsync(long id);
        IQueryable<ContainerType> GetAllContainerTypes();
        Task<List<Container>> GetContainersByEventIdAsync(long eventId);
        Task<List<Container>> GetOnlyContainersByEventIdAsync(long eventId);
        Task<List<ContainerTemperatureReading>> GetReadingsForContainer(long containerId);
        Task<ResponseDto> AddContainerAsync(CreateContainerDto dto, string addedBy);
        Task<ResponseDto> AddReadingAsync(CreateReadingDto dto, string performedBy);
        Task AcknowledgeNotificationAsync(long notificationId);
    }
}
