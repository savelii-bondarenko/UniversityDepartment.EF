using AutoMapper;
using ElectronicDepartment.BLL.DTOs;
using ElectronicDepartment.DAL.Entities;

namespace ElectronicDepartment.BLL;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        // Мапінг для Teacher
        CreateMap<Teacher, TeacherDto>()
            .ForMember(dest => dest.Subjects, opt => opt.MapFrom(src => src.Subjects));
        CreateMap<CreateTeacherDto, Teacher>()
            .ForMember(dest => dest.Role, opt => opt.MapFrom(src => Common.Enums.UserRole.Teacher));
        CreateMap<UpdateTeacherDto, Teacher>();

        // Мапінг для Admin
        CreateMap<Admin, AdminDto>();
        CreateMap<CreateAdminDto, Admin>()
            .ForMember(dest => dest.Role, opt => opt.MapFrom(src => Common.Enums.UserRole.Admin));
        CreateMap<UpdateAdminDto, Admin>();

        // Мапінг для Manager
        CreateMap<Manager, ManagerDto>();
        CreateMap<CreateManagerDto, Manager>()
            .ForMember(dest => dest.Role, opt => opt.MapFrom(src => Common.Enums.UserRole.Manager));
        CreateMap<UpdateManagerDto, Manager>();

        // Мапінг для Student
        CreateMap<Student, StudentDto>()
            .ForMember(dest => dest.GroupName, opt => opt.MapFrom(src => src.Group.Name));
        CreateMap<CreateStudentDto, Student>()
            .ForMember(dest => dest.Role, opt => opt.MapFrom(src => Common.Enums.UserRole.Student));
        CreateMap<UpdateStudentDto, Student>();

        // Мапінг для Subject
        CreateMap<Subject, SubjectDto>()
            .ForMember(dest => dest.DepartmentName, opt => opt.MapFrom(src => src.Department.Name))
            .ForMember(dest => dest.TeacherName, opt => opt.MapFrom(src => $"{src.Teacher.FirstName} {src.Teacher.LastName}"))
            .ForMember(dest => dest.GroupName, opt => opt.MapFrom(src => src.Group.Name));

        CreateMap<CreateSubjectDto, Subject>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.Department, opt => opt.Ignore())
            .ForMember(dest => dest.Group, opt => opt.Ignore())
            .ForMember(dest => dest.Teacher, opt => opt.Ignore())
            .ForMember(dest => dest.StudentSubjects, opt => opt.Ignore());

        // Мапінг для Grade
        CreateMap<StudentSubject, GradeDto>()
            .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.StudentId + "_" + src.SubjectId))
            .ForMember(dest => dest.StudentId, opt => opt.MapFrom(src => src.StudentId))
            .ForMember(dest => dest.StudentName, opt => opt.MapFrom(src => $"{src.Student.FirstName} {src.Student.LastName}"))
            .ForMember(dest => dest.SubjectId, opt => opt.MapFrom(src => src.SubjectId))
            .ForMember(dest => dest.SubjectName, opt => opt.MapFrom(src => src.Subject.Name))
            .ForMember(dest => dest.TeacherName, opt => opt.MapFrom(src => $"{src.Subject.Teacher.FirstName} {src.Subject.Teacher.LastName}"))
            .ForMember(dest => dest.Grade, opt => opt.MapFrom(src => src.Grade));

        CreateMap<CreateGradeDto, StudentSubject>()
            .ForMember(dest => dest.StudentId, opt => opt.MapFrom(src => src.StudentId))
            .ForMember(dest => dest.SubjectId, opt => opt.MapFrom(src => src.SubjectId))
            .ForMember(dest => dest.Grade, opt => opt.MapFrom(src => src.Grade));

        CreateMap<Department, DepartmentDto>();
            //.ForMember(dest => dest.Groups, opt => opt.MapFrom(src => src.Groups.Select(g => g.Name).ToList()))
            //.ForMember(dest => dest.Subjects, opt => opt.MapFrom(src => src.Subjects.Select(s => s.Name).ToList()));

        CreateMap<CreateDepartmentDto, Department>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.Groups, opt => opt.Ignore())
            .ForMember(dest => dest.Subjects, opt => opt.Ignore());

        CreateMap<UpdateDepartmentDto, Department>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.Groups, opt => opt.Ignore())
            .ForMember(dest => dest.Subjects, opt => opt.Ignore());

        CreateMap<Group, GroupDto>()
            .ForMember(dest => dest.DepartmentName, opt => opt.MapFrom(src => src.Department.Name))
            .ForMember(dest => dest.Students, opt => opt.MapFrom(src => src.Students));
        CreateMap<CreateGroupDto, Group>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.Students, opt => opt.Ignore())
            .ForMember(dest => dest.Subjects, opt => opt.Ignore())
            .ForMember(dest => dest.Department, opt => opt.Ignore());
        CreateMap<UpdateGroupDto, Group>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.Students, opt => opt.Ignore())
            .ForMember(dest => dest.Subjects, opt => opt.Ignore())
            .ForMember(dest => dest.Department, opt => opt.Ignore());

        // Для авторизиції
        CreateMap<Student, UserDto>();
        CreateMap<Teacher, UserDto>();
        CreateMap<Manager, UserDto>();
        CreateMap<Admin, UserDto>();
    }
}
