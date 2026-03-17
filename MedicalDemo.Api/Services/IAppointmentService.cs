using MedicalDemo.Api.DTOs;

namespace MedicalDemo.Api.Services;

public interface IAppointmentService
{
    IReadOnlyCollection<AppointmentResponseDTO> GetAll();

    AppointmentResponseDTO Create(CreateAppointmentDTO request);

    AppointmentResponseDTO UpdateEligibility(int id, UpdateAppointmentEligibilityDTO request);
}
