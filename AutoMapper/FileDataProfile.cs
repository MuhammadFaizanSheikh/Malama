using AutoMapper;
using ExcelFilesCompiler;
using ExcelToCsv.Models;

namespace Malama.AutoMapper
{
    public class FileDataProfile : Profile
    {
        public FileDataProfile()
        {
            CreateMap<FileDataDto, FileDataDto>()
                .ForMember(dest => dest.isDeleted, opt => opt.MapFrom(src => false))
                .ForMember(dest => dest.AddedBy, opt => opt.Ignore())
                .ForMember(dest => dest.AddedOn, opt => opt.Ignore())
                .ForMember(dest => dest.CheckInTime, opt => opt.Ignore())
                .ForMember(dest => dest.CheckOutTime, opt => opt.Ignore());
        }
    }
}
